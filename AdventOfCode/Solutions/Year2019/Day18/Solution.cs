using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;
using System.Diagnostics;
using QuikGraph;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.Search;

namespace AdventOfCode.Solutions.Year2019
{
    enum DoorKeyType
    {
        Default,
        Wall,
        Passage,
        Door,
        Key,
        Start
    }

    /// <summary>
    /// Struct to enable passing by value
    /// </summary>
    struct DoorLockPos
    {
        public int x { get; set; }
        public int y { get; set; }
        public DoorKeyType type { get; set; }
        /// <summary>
        /// What character is this door or key
        /// </summary>
        /// <value></value>
        public char? value { get; set; }
        /// <summary>
        /// Notes if the door is unlocked and/or key collected
        /// </summary>
        /// <value></value>
        public bool collected { get; set; }
    }

    class DoorLockPosEdge : Edge<DoorLockPos>
    {
        public int cost;

        public DoorLockPosEdge(DoorLockPos source, DoorLockPos target, int cost) : base(source, target)
        {
            this.cost = cost;
        }
    }

    class Day18 : ASolution
    {
        char[][] map = Array.Empty<char[]>();

        public (int x, int y) start = (0, 0);

        public int minDistance = int.MaxValue;

        public int keyCount = 0;

        PriorityQueue<State, int> queue = new();

        public struct State
        {
            public (int x, int y) pos;
            public char[] keys;
            public (int x, int y)[] path;
            public int depth;
        }

        public Day18() : base(18, 2019, "Many-Worlds Interpretation")
        {
            var part1Example = new Dictionary<string, int>()
            {
                {
                    @"#########
                    #b.A.@.a#
                    #########",
                    8
                },
                {
                    @"########################
                    #f.D.E.e.C.b.A.@.a.B.c.#
                    ######################.#
                    #d.....................#
                    ########################",
                    86
                },
                {
                    @"########################
                    #...............b.C.D.f#
                    #.######################
                    #.....@.a.B.c.d.A.e.F.g#
                    ########################",
                    132
                },
                {
                    @"#################
                    #i.G..c...e..H.p#
                    ########.########
                    #j.A..b...f..D.o#
                    ########@########
                    #k.E..a...g..B.n#
                    ########.########
                    #l.F..d...h..C.m#
                    #################",
                    136
                },
                {
                    @"########################
                    #@..............ac.GI.b#
                    ###d#e#f################
                    ###A#B#C################
                    ###g#h#i################
                    ########################",
                    81
                }
            };

            // foreach(var exKvp in part1Example)
            // {
            //     ResetGrid(exKvp.Key);

            //     var paths = GetShortestPath(start, Array.Empty<char>(), new (int x, int y)[] { start })
            //         .Select(p => p.ToList())
            //         .OrderBy(p => p.Count)
            //         .FirstOrDefault();

            //     Debug.Assert(Debug.Equals(minDistance, exKvp.Value), $"Expected: {exKvp.Value}\nActual: {minDistance}");
            // }
        }

        private void ResetGrid(string input)
        {
            map = input.SplitByNewline(true)
                .Select(line => line.ToCharArray())
                .ToArray();

            minDistance = Int32.MaxValue;

            // Find the start
            start = (-1, -1);
            for (int y = 0; start.x == -1 && y < map.Length; y++)
                for (int x = 0; start.x == -1 && x < map[y].Length; x++)
                    if (map[y][x] == '@')
                        start = (x, y);

            keyCount = map.Sum(line => line.Count(c => 97 <= c && c <= 122));

            queue.Clear();
        }

        public char? GetGrid((int x, int y) pos)
        {
            if (pos.x < 0 || pos.x >= map[0].Length)
                return null;

            if (pos.y < 0 || pos.y >= map.Length)
                return null;

            return map[pos.y][pos.x];
        }

        private IEnumerable<(int x, int y)> GetMoves((int x, int y) pos, char[] keys)
        {
            foreach(var move in new (int x, int y)[] { (-1, 0), (1, 0), (0, -1), (0, 1) })
            {
                var newPos = pos.Add(move);
                var newChar = GetGrid(newPos);

                if (newChar != null)
                {
                    // Found a possible opening, door, or key
                    if (newChar != '#')
                    {
                        // If this is a door, make sure we have the corresponding key
                        if (65 <= newChar && newChar <= 90)
                        {
                            if (keys.Any(c => c == newChar + 32))
                                yield return newPos;
                            continue;
                        }

                        // Otherwise it's a key or an opening
                        yield return newPos;
                    }
                }
            }
        }

        public List<List<char>> GetPaths()
        {
            // Loop through the queue from an initial state
            queue.Enqueue(new()
            {
                pos = start,
                keys = Array.Empty<char>(),
                path = new (int x, int y)[] { start },
                depth = 0
            }, keyCount);

            var retList = new List<List<char>>();

            while (queue.Count > 0)
            {
                var state = queue.Dequeue();
                foreach(var path in GetShortestPath(state))
                {
                    retList.Add(path.ToList());
                }
            }

            return retList;
        }

        /// <summary>
        /// Provide the shortest path from pos to the rest of the keys
        /// </summary>
        /// <param name="pos">Current position</param>
        /// <param name="keys">Currently collected keys</param>
        /// <param name="path">Array of current path</param>
        public IEnumerable<IEnumerable<char>> GetShortestPath(State state)
        {
            (int x, int y) pos = state.pos;
            char[] keys = state.keys;
            (int x, int y)[] path = state.path;
            int depth = state.depth;

            var moves = GetMoves(pos, keys)
                // Exclude visited locations
                .Where(m => !path.Contains(m))
                .ToArray();

            // Maybe we're too far in
            if (++depth > minDistance)
                yield break;

            // Each move has to be valid
            // Either it's another key, opening, or door that is unlocked
            foreach(var move in moves)
            {
                // Duplicate keys and paths
                var newKeys = (char[])keys.Clone();
                var newPath = ((int x, int y)[])path.Clone();

                // Adding the move
                newPath = newPath.Append(move).ToArray();

                // If this move is a key and we don't have it, pick it up and start fresh!
                var newChar = GetGrid(move);
                if (97 <= newChar && newChar <= 122 && !keys.Any(c => c == newChar.Value))
                {
                    newKeys = newKeys.Union(new char[] { newChar.Value }).ToArray();
                    newPath = Array.Empty<(int x, int y)>();

                    // If this is all of the keys, return our result instead
                    if (newKeys.Length == keyCount)
                    {
                        // Found a new length
                        minDistance = Math.Min(minDistance, depth);

                        yield return newKeys;
                        continue;
                    }
                }

                queue.Enqueue(new()
                {
                    pos = move,
                    keys = newKeys,
                    path = newPath,
                    depth = depth
                }, keyCount - keys.Length);
            }
        }

        protected override string? SolvePartOne()
        {
            ResetGrid(Input);

            var paths = GetPaths();

            return paths.FirstOrDefault()?.JoinAsString() ?? string.Empty;
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}
