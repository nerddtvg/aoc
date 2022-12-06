using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;
using System.Diagnostics;

namespace AdventOfCode.Solutions.Year2019
{
    enum DoorKeyType
    {
        Default,
        Wall,
        Passage,
        Door,
        Key
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

    class Day18 : ASolution
    {
        Dictionary<(int x, int y), DoorLockPos> map = new();

        public (int x, int y) start = (0, 0);

        public static int minDistance = int.MaxValue;

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

            foreach(var exKvp in part1Example)
            {
                ResetGrid(exKvp.Key);
                var paths = GetPath(start, default, map.Select(kvp => kvp.Value).ToArray(), Array.Empty<DoorLockPos>()).ToList();
                var min = paths.Min(path => path.Count());
                Debug.Assert(Debug.Equals(min, exKvp.Value), $"Expected: {exKvp.Value}\nActual: {min}");
            }
        }

        private void ResetGrid(string input)
        {
            int x = 0;
            int y = 0;
            minDistance = Int32.MaxValue;
            start = (0, 0);

            map = new();

            foreach (string line in input.SplitByNewline(true))
            {
                foreach (char loc in line.ToCharArray())
                {
                    if (loc == '@')
                        start = (x, y);

                    map.Add((x, y), new DoorLockPos()
                    {
                        value = loc.ToString().ToUpperInvariant()[0],
                        type = loc switch
                        {
                            '#' => DoorKeyType.Wall,
                            '.' => DoorKeyType.Passage,
                            '@' => DoorKeyType.Passage,
                            _ => (65 <= (int)loc && (int)loc <= 90) ? DoorKeyType.Door : DoorKeyType.Key
                        },
                        collected = false,
                        x = x,
                        y = y
                    });
                    x++;
                }

                y++;
                x = 0;
            }
        }

        /// <summary>
        /// Get open neighbors from our given position (open passage, a key, or a door that is unlocked)
        /// </summary>
        private IEnumerable<(int x, int y)> GetOpenNeighbors(DoorLockPos[] grid, (int x, int y) pos)
        {
            // Can only move up, right, down, and left
            // Check if the position is open or not, return if so
            foreach(var move in new (int x, int y)[] { (0, -1), (1, 0), (0, 1), (-1, 0) })
            {
                var obj = grid.FirstOrDefault(sq => sq.x == pos.x + move.x && sq.y == pos.y + move.y && (sq.type == DoorKeyType.Passage || sq.type == DoorKeyType.Key || (sq.type == DoorKeyType.Door && sq.collected)));

                // Struct will default to type == 0 is nullable
                if (obj.type != DoorKeyType.Default)
                    yield return (obj.x, obj.y);
            }
        }

        /// <summary>
        /// Returns a given path within <paramref name="grid" /> in order from <paramref name="start" /> until all of the doors are unlocked
        /// </summary>
        /// <param name="x"></param>
        /// <param name="start"></param>
        /// <param name="seen"></param>
        /// <returns></returns>
        private IEnumerable<IEnumerable<DoorLockPos>> GetPath((int x, int y) start, IEnumerable<DoorLockPos>? currentPath, DoorLockPos[] grid, DoorLockPos[] seen, int depth = 0)
        {
            // If we have exceeded an already found distance, drop out
            if (depth > minDistance)
                yield break;

            // If we have entered this square already, skip it
            if (seen.Any(sq => sq.x == start.x && sq.y == start.y))
                yield break;

            // If this is not a valid position (somehow), break out
            if (!grid.Any(sq => sq.x == start.x && sq.y == start.y))
                yield break;

            // Get our spot
            var loc = grid.First(sq => sq.x == start.x && sq.y == start.y);

            // Add ourselves to the seen list
            seen = seen.Append(loc).ToArray();

            if (currentPath == default)
                currentPath = new List<DoorLockPos>();

            // // Check to see if we are the last door. If so, we've found a path
            // // Found doors:
            // var passedDoors = currentPath.Where(sq => sq.type == DoorKeyType.Door && sq.collected).ToList();
            // // All doors:
            // var allDoors = grid.Count(sq => sq.type == DoorKeyType.Door);
            // if (
            //     loc.type == DoorKeyType.Door
            //     &&
            //     loc.collected
            //     &&
            //     passedDoors.Count == allDoors - 1
            //     &&
            //     passedDoors.Any(door => door.value != loc.value)
            // )
            // {
            //     // We have a vaild path to the final, unlocked door
            //     minDistance = depth + 1;
            //     yield return currentPath.Append(loc);
            //     yield break;
            // }

            // Recurse into a new GetPath if:
            // * We are a key
            // * We are an unlocked door
            // * We are an open passage
            if (
                loc.type == DoorKeyType.Key
                ||
                (loc.type == DoorKeyType.Door && loc.collected)
                ||
                loc.type == DoorKeyType.Passage
            )
            {
                // Clone the grid
                var subGrid = grid.Select(sq => sq).ToArray();

                // Find our door and list it unlocked
                if (loc.type == DoorKeyType.Key && !loc.collected)
                {
                    // Collect this key and rebuild the grid
                    loc.collected = true;
                    subGrid = grid
                        .Where(sq => !(sq.x == loc.x && sq.y == loc.y))
                        .Append(loc)
                        .ToArray();

                    var matchingDoor = subGrid.FirstOrDefault(sq => sq.type == DoorKeyType.Door && sq.value == loc.value);

                    // Reset seen
                    seen = new DoorLockPos[] { loc };

                    // Ignore if we have already collected this key
                    // Otherwise we go into an infinite loop of resetting seen
                    if (matchingDoor.type != DoorKeyType.Default)
                    {

                        // Rebuild the grid again
                        subGrid = subGrid
                            .Where(sq => !(sq.x == matchingDoor.x && sq.y == matchingDoor.y))
                            .Append(matchingDoor with { collected = true })
                            .ToArray();
                    }

                    // If this is the last key, we have solved part 1
                    if (subGrid.Count(sq => sq.type == DoorKeyType.Key && sq.collected) == subGrid.Count(sq => sq.type == DoorKeyType.Key))
                    {
                        minDistance = Math.Min(minDistance, depth + 1);
                        yield return currentPath;
                        yield break;
                    }
                }

                foreach(var neighbor in GetOpenNeighbors(subGrid, start))
                {
                    // Search from here
                    foreach (var path in GetPath(neighbor, currentPath.Append(loc), subGrid, seen, depth + 1))
                        // Return this new sub-path prepended with our position
                        yield return path;
                }
            }
        }

        protected override string SolvePartOne()
        {
            ResetGrid(Input);
            var paths = GetPath(start, default, map.Select(kvp => kvp.Value).ToArray(), Array.Empty<DoorLockPos>()).ToList();
            return paths.Min(path => path.Count()).ToString();
        }

        protected override string SolvePartTwo()
        {
            return string.Empty;
        }
    }
}
