using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;
using System.Diagnostics;
using QuikGraph;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.Search;
using QuikGraph.Algorithms.ShortestPath;

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
        public DoorKeyType type { get; set; }
        /// <summary>
        /// What character is this door or key
        /// </summary>
        /// <value></value>
        public char value { get; set; }

        public int id { get; set; }

        public DoorLockPos() {
            id = new Random().Next();
        }
        public DoorLockPos(char v)
        {
            id = new Random().Next();
            type = 65 <= v && v <= 90 ? DoorKeyType.Door : (97 <= v && v <= 122 ? DoorKeyType.Key : (v == '.' ? DoorKeyType.Passage : (v == '@' ? DoorKeyType.Start : DoorKeyType.Wall)));
            value = v;
        }
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
        DoorLockPos[][] map = Array.Empty<DoorLockPos[]>();

        public (int x, int y) start = (0, 0);

        public int minDistance = int.MaxValue;

        public int keyCount = 0;

        PriorityQueue<State, int> queue = new();

        public struct State
        {
            public DoorLockPos pos;
            public char[] keys;
            // public DoorLockPos[] path;
            public int depth;
        }

        public UndirectedGraph<DoorLockPos, DoorLockPosEdge> graph = default!;

        public Day18() : base(18, 2019, "Many-Worlds Interpretation")
        {
            var part1Example = new Dictionary<string, int>()
            {
                // {
                //     @"#########
                //     #b.A.@.a#
                //     #########",
                //     8
                // },
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

            var sw = new Stopwatch();
            foreach (var exKvp in part1Example)
            {
                Debug.WriteLine($"Starting Test: {exKvp.Value}");

                sw.Restart();
                ResetGrid(exKvp.Key);
                sw.Stop();
                Debug.WriteLine($"Reset Grid: {sw.Elapsed}");

                sw.Restart();
                (var minDistance, var keys) = GetShortestPath();
                sw.Stop();

                Debug.Assert(Debug.Equals(minDistance, exKvp.Value), $"Expected: {exKvp.Value}\nActual: {minDistance}");
                Debug.WriteLine($"Test Results: Expected: {exKvp.Value}, Actual: {minDistance}");
                Debug.WriteLine($"Shortest Path: {sw.Elapsed}");

                Debug.WriteLine(string.Empty);
            }

            ResetGrid(Input);

            sw.Restart();
            ResetGrid(Input);
            sw.Stop();
            Debug.WriteLine($"Reset Grid Input: {sw.Elapsed}");
        }

        private void ResetGrid(string input)
        {
            map = input.SplitByNewline(true)
                .Select(line => line.ToCharArray().Select(c => new DoorLockPos(c)).ToArray())
                .ToArray();

            minDistance = Int32.MaxValue;

            // Find the start
            start = (-1, -1);
            for (int y = 0; start.x == -1 && y < map.Length; y++)
                for (int x = 0; start.x == -1 && x < map[y].Length; x++)
                    if (map[y][x].type == DoorKeyType.Start)
                        start = (x, y);

            keyCount = map.Sum(line => line.Count(c => c.type == DoorKeyType.Key));

            queue.Clear();

            // Build the primary graph of all passages, start, keys, and doors
            var edges = new List<DoorLockPosEdge>();

            for (int y = 0; y < map.Length; y++)
            {
                for (int x = 0; x < map[0].Length; x++)
                {
                    if (map[y][x].type == DoorKeyType.Wall || map[y][x].type == DoorKeyType.Default)
                        continue;

                    if (y > 0)
                    {
                        var up = map[y - 1][x];

                        if (up.type != DoorKeyType.Wall && up.type != DoorKeyType.Default)
                            edges.Add(new(up, map[y][x], 1));
                    }
                    
                    if (x > 0)
                    {
                        var left = map[y][x - 1];

                        if (left.type != DoorKeyType.Wall && left.type != DoorKeyType.Default)
                            edges.Add(new(left, map[y][x], 1));
                    }
                }
            }

            // Build a full graph of walkable objects
            graph = edges.ToUndirectedGraph<DoorLockPos, DoorLockPosEdge>();

            // We will now reduce the graph down to remove passages and replace the edge costs with counts
            // Go through every permutation of these combos
            // var groups = graph.Vertices
            //     .Where(v => new DoorKeyType[] { DoorKeyType.Door, DoorKeyType.Key, DoorKeyType.Start }.Contains(v.type))
            //     .GetAllCombos(2)
            //     // Pre-process the combos
            //     .Select(combo => combo.ToArray())
            //     .GroupBy(combo => combo[0])
            //     .ToList();

            // Can we do a quick reduction of edges that are cost == 1 (passage to passage)
            // and that each side only has one more connection? (pass through, not a multi-direction intersection)
            // Method:
            // 1. Find all verices that are _not_ passages
            // 2. Go out one and find all connected vertices
            //    * Track: Start point, edges between start and current
            // 3. If the count is more than 2, reduce the path to that point
            //    * Remove tracked edges / vertices
            var vertices = graph.Vertices
                .Where(v => new DoorKeyType[] { DoorKeyType.Door, DoorKeyType.Key, DoorKeyType.Start }.Contains(v.type))
                .ToArray();

            Debug.WriteLine($"Before Vertex Count: {graph.VertexCount}");
            Debug.WriteLine($"Before Edge Count: {graph.EdgeCount}");

            foreach (var startVertex in vertices)
            {
                // Get the next vertices to this one
                var adjVertices = graph.AdjacentVertices(startVertex).ToArray();

                foreach(var tAdjVertex in adjVertices)
                {
                    var adjVertex = tAdjVertex;

                    // Don't go back
                    if (adjVertex.id == startVertex.id)
                        continue;

                    // Immediately skip any non-passages
                    if (adjVertex.type != DoorKeyType.Passage)
                        continue;

                    // Start with a fresh path
                    var foundPath = new List<DoorLockPos>() { startVertex };

                    // if (startVertex.value == 'd')
                    //     System.Diagnostics.Debugger.Break();

                    do
                    {
                        var adjEdges = graph
                            .AdjacentEdges(adjVertex)
                            .Where(edge =>
                                // This may have already been processed
                                edge.cost == 1
                                &&
                                (
                                    // Kick out where we came from
                                    (edge.Source.id != foundPath[^1].id && edge.Target.id == adjVertex.id)
                                    ||
                                    (edge.Target.id != foundPath[^1].id && edge.Source.id == adjVertex.id)
                                )
                            )
                            .ToArray();

                        // If we have found an intersection, stop
                        if (adjEdges.Length != 1)
                            break;

                        // Track this location
                        foundPath.Add(adjVertex);

                        // If we have found a non-passage, step out
                        if (adjVertex.type != DoorKeyType.Passage)
                            break;

                        // Find our next step out
                        adjVertex = (adjEdges[0].Source.id != adjVertex.id) ? adjEdges[0].Source : adjEdges[0].Target;
                    } while (true);

                    if (foundPath.Count > 2)
                    {
                        // If we found a path to remove, let's remove it and our startVertex
                        var lastVertex = foundPath[^1];
                        foundPath.RemoveAt(0);
                        foundPath.Remove(lastVertex);

                        graph.AddEdge(new DoorLockPosEdge(startVertex, lastVertex, foundPath.Count + 1));

                        foundPath.ForEach(removeVertex => graph.RemoveVertex(removeVertex));
                    }
                }
            }

            Debug.WriteLine($"After Vertex Count: {graph.VertexCount}");
            Debug.WriteLine($"After Edge Count: {graph.EdgeCount}");

            var groups = graph.Vertices
                .Where(v => new DoorKeyType[] { DoorKeyType.Door, DoorKeyType.Key, DoorKeyType.Start }.Contains(v.type))
                .ToArray();

            // New edges
            edges = new List<DoorLockPosEdge>();

            // Go through each group (Key is the start, then a list of destinations)
            for(int iGroup = 0; iGroup<groups.Length-1; iGroup++)
            {
                var groupStart = groups[iGroup];

                for(int qGroup = iGroup+1; qGroup<groups.Length; qGroup++)
                {
                    var groupDestination = groups[qGroup];

                    // We can't simply rely on the base algorithm class because it doesn't return
                    // the path, only the distance, and we need to filter it
                    // We manipulate the edge cost such that if the target node is a door or key, increase the cost significantly
                    // This way any additional door or key is seen as too expensive
                    // foreach (var destination in group.Select(grp => grp[1]))
                    // {
                    // Weighted to PositiveInfinity should be rejected from shortest paths
                    Func<DoorLockPosEdge, double> edgeTest = edge =>
                        (
                            edge.Source.type != DoorKeyType.Passage
                            &&
                            edge.Source.id != groupStart.id
                            &&
                            edge.Source.id != groupDestination.id
                        )
                        ||
                        (
                            edge.Target.type != DoorKeyType.Passage
                            &&
                            edge.Target.id != groupStart.id
                            &&
                            edge.Target.id != groupDestination.id
                        ) ? double.PositiveInfinity : edge.cost;

                    var tryGetPath = graph.ShortestPathsDijkstra(edgeTest, groupStart);

                    if (tryGetPath(groupDestination, out IEnumerable<DoorLockPosEdge> path))
                    {
                        // Found a path!
                        // Make sure we do not include another key or door
                        var pathList = path.ToList();
                        if (pathList.Any(
                            edge =>
                                (
                                    edge.Source.type != DoorKeyType.Passage
                                    &&
                                    edge.Source.id != groupStart.id
                                    &&
                                    edge.Source.id != groupDestination.id
                                )
                                ||
                                (
                                    edge.Target.type != DoorKeyType.Passage
                                    &&
                                    edge.Target.id != groupStart.id
                                    &&
                                    edge.Target.id != groupDestination.id
                                )
                            )
                        )
                            continue;

                        // Make sure all of the 
                        edges.Add(new(groupStart, groupDestination, pathList.Sum(edge => edge.cost)));
                    }
                }
            }

            // New Graph
            graph = edges.ToUndirectedGraph<DoorLockPos, DoorLockPosEdge>();
        }

        private IEnumerable<DoorLockPos> GetMoves(DoorLockPos pos, char[] keys)
        {
            // Found a possible opening, door, or key
            foreach(var move in graph.AdjacentVertices(pos))
            {
                if (move.type == DoorKeyType.Door)
                {
                    // If this is a door, make sure we have the corresponding key
                    if (keys.Any(c => c == move.value + 32))
                        yield return move;

                    continue;
                }

                // Otherwise it's a key or an opening
                yield return move;
            }
        }

        public (int minDistance, string keys) GetShortestPath()
        {
            // Loop through the queue from an initial state
            var start = graph.Vertices.First(v => v.type == DoorKeyType.Start);

            var startState = new State()
            {
                pos = start,
                keys = Array.Empty<char>(),
                // path = new DoorLockPos[] { start },
                depth = 0
            };

            // Track if we have seen this state before and what depth
            // Dictionary<string, int> seenKeys = new();
            Dictionary<(int vertexId, string keys), int> seenState = new();

            var queue = new PriorityQueue<State, ulong>();
            queue.Enqueue(startState, 0);

            var minKeys = string.Empty;

            while(queue.Count > 0)
            {
                var state = queue.Dequeue();

                var pos = state.pos;
                var keys = state.keys;
                // var path = state.path;
                var depth = state.depth;

                // Check if we have seen this state before
                // If we have gotten to the same position with the same keys
                // in a lower depth, skip this branch
                var stateHash = (pos.id, keys.OrderBy(c => c).JoinAsString());
                if (seenState.ContainsKey(stateHash) && seenState[stateHash] < depth)
                {
                    continue;
                }
                else
                    seenState[stateHash] = depth;

                var moves = GetMoves(pos, keys)
                    // Exclude visited locations
                    // .Where(m => !path.Select(p => p.id).Contains(m.id))
                    .ToArray();

                // Maybe we're too far in
                if (depth >= minDistance)
                    continue;

                // Each move has to be valid
                // Either it's another key, opening, or door that is unlocked
                foreach (var move in moves)
                {
                    // Duplicate keys and paths
                    var newKeys = (char[])keys.Clone();
                    // var newPath = (DoorLockPos[])path.Clone();

                    // Adding the move
                    // newPath = newPath.Append(move).ToArray();

                    // Need our edge cost
                    var success = graph.TryGetEdge(pos, move, out DoorLockPosEdge edge);

                    if (!success)
                        throw new Exception();

                    var newDepth = depth + edge.cost;

                    // Maybe we're too far in (check again due to >1 weights)
                    if (newDepth >= minDistance)
                        continue;

                    // If this move is a key and we don't have it, pick it up and start fresh!
                    if (move.type == DoorKeyType.Key && !keys.Any(c => c == move.value))
                    {
                        newKeys = newKeys.Union(new char[] { move.value }).ToArray();
                        // newPath = Array.Empty<DoorLockPos>();

                        // If this is all of the keys, return our result instead
                        if (newKeys.Length == keyCount)
                        {
                            // Found a new length
                            minDistance = Math.Min(minDistance, newDepth);
                            if (minDistance == newDepth)
                                minKeys = newKeys.JoinAsString();
                            continue;
                        }

                        // Avoid this if we have seen this key set before
                        // a,b,c,d and b,d,c,a are identical, we only care about
                        // the lowest depth to get to that last key
                        // NOTE: Moved this to before Enqueue because we should only
                        // check this when adding a new key
                        // var sortedKeys = newKeys.OrderBy(c => c).JoinAsString();
                        // if (seenKeys.ContainsKey(sortedKeys) && seenKeys[sortedKeys] < newDepth)
                        // {
                        //     continue;
                        // }
                        // else
                        // {
                        //     seenKeys[sortedKeys] = newDepth;
                        // }
                    }

                    queue.Enqueue(new()
                    {
                        pos = move,
                        keys = newKeys,
                        // path = newPath,
                        depth = newDepth
                    }, (ulong)(keyCount - newKeys.Length) * (ulong)newDepth);
                }
            }

            return (minDistance, minKeys);
        }

        protected override string? SolvePartOne()
        {
            return string.Empty;

            return GetShortestPath().minDistance.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}
