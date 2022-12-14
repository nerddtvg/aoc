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
        Dictionary<(int x, int y), DoorLockPos> map = new();

        public (int x, int y) start = (0, 0);

        public static int minDistance = int.MaxValue;

        UndirectedGraph<DoorLockPos, Edge<DoorLockPos>> graph = default!;
        UndirectedGraph<DoorLockPos, DoorLockPosEdge> subGraph = default!;

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
                // ResetGrid(exKvp.Key);
                // var paths = GetPath(start, default, map.Select(kvp => kvp.Value).ToArray(), Array.Empty<DoorLockPos>()).ToList();
                // var min = paths.Min(path => path.Count());
                // Debug.Assert(Debug.Equals(min, exKvp.Value), $"Expected: {exKvp.Value}\nActual: {min}");
            }
        }

        private void ResetGrid(string input)
        {
            int x = 0;
            int y = 0;
            minDistance = Int32.MaxValue;
            start = (0, 0);

            map = new();
            var edges = new List<Edge<DoorLockPos>>();

            foreach (string line in input.SplitByNewline(true))
            {
                foreach (char loc in line.ToCharArray())
                {
                    if (loc == '@')
                        start = (x, y);

                    if (loc != '#')
                    {
                        map.Add((x, y), new DoorLockPos()
                        {
                            value = loc.ToString().ToUpperInvariant()[0],
                            type = loc switch
                            {
                                '#' => DoorKeyType.Wall,
                                '.' => DoorKeyType.Passage,
                                '@' => DoorKeyType.Start,
                                _ => (65 <= (int)loc && (int)loc <= 90) ? DoorKeyType.Door : DoorKeyType.Key
                            },
                            collected = false,
                            x = x,
                            y = y
                        });

                        // Look up and left for edges
                        if (map.ContainsKey((x, y - 1)))
                        {
                            edges.Add(new(map[(x, y - 1)], map[(x, y)]));
                        }

                        if (map.ContainsKey((x - 1, y)))
                        {
                            edges.Add(new(map[(x - 1, y)], map[(x, y)]));
                        }
                    }

                    x++;
                }

                y++;
                x = 0;
            }

            graph = edges.ToUndirectedGraph<DoorLockPos, Edge<DoorLockPos>>();

            // We're going to create a new graph that is connecting only keys, doors, and the start
            // Go through every permutation of these combos
            var groups = graph.Vertices
                .Where(v => new DoorKeyType[] { DoorKeyType.Door, DoorKeyType.Key, DoorKeyType.Start }.Contains(v.type))
                .GetAllCombos(2)
                // Pre-process the combos
                .Select(combo => combo.ToArray())
                .GroupBy(combo => combo[0])
                .ToList();

            // New edges
            var edges2 = new List<DoorLockPosEdge>();

            var tryGetPath = new QuikGraph.Algorithms.ShortestPath.UndirectedDijkstraShortestPathAlgorithm<DoorLockPos, Edge<DoorLockPos>>(graph, edge => 1);
            tryGetPath.Compute();

            // Go through each group (Key is the start, then a list of destinations)
            foreach (var group in groups)
            {
                tryGetPath.SetRootVertex(group.Key);

                foreach (var destination in group.Select(grp => grp[1]))
                {
                    if (tryGetPath.TryGetDistance(destination, out double distance))
                    {
                        // Found a path!
                        edges2.Add(new(group.Key, destination, (int)distance));
                    }
                }
            }

            // New Graph
            subGraph = edges2.ToUndirectedGraph<DoorLockPos, DoorLockPosEdge>();
        }

        protected override string? SolvePartOne()
        {
            ResetGrid(Input);

            // Our own BFS using the prebuilt graph
            // Start at "start" and collect keys into new states as we go
            minDistance = int.MaxValue;

            return null;
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}
