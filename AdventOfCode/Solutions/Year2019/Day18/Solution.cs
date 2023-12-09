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

        public override string ToString()
        {
            return $"{type} {value}";
        }
    }

    class DoorLockPosEdge : Edge<DoorLockPos>
    {
        public int cost;

        public DoorLockPosEdge(DoorLockPos source, DoorLockPos target, int cost) : base(source, target)
        {
            this.cost = cost;
        }

        public override string ToString()
        {
            return $"[{cost}] {Source} -> {Target}";
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
            public char[] keys;
            public int depth;
            public DoorLockPos[] pos;
        }

        public UndirectedGraph<DoorLockPos, DoorLockPosEdge> graph = default!;

        public Day18() : base(18, 2019, "Many-Worlds Interpretation")
        {
            var doExamples = false;

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

            if (doExamples)
            {
                var sw = new Stopwatch();
                foreach (var exKvp in part1Example)
                {
                    Debug.WriteLine($"Starting Test: {exKvp.Value}");

                    sw.Restart();
                    ResetGrid(exKvp.Key);
                    sw.Stop();
                    Debug.WriteLine($"Reset Grid: {sw.Elapsed}");

                    sw.Restart();
                    var minDistance = GetShortestPath();
                    sw.Stop();

                    Debug.Assert(Debug.Equals(minDistance, exKvp.Value), $"Expected: {exKvp.Value}\nActual: {minDistance}");
                    Debug.WriteLine($"Test Results: Expected: {exKvp.Value}, Actual: {minDistance}");
                    Debug.WriteLine($"Shortest Path: {sw.Elapsed}");

                    Debug.WriteLine(string.Empty);
                }
            }
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
                .Where(v =>
                    new DoorKeyType[] { DoorKeyType.Door, DoorKeyType.Key, DoorKeyType.Start }.Contains(v.type)
                    ||
                    // Also start at intersections
                    (v.type == DoorKeyType.Passage && graph.AdjacentEdges(v).Count() > 2)
                )
                .ToArray();

            Debug.WriteLine($"Before Vertex Count: {graph.VertexCount}");
            Debug.WriteLine($"Before Edge Count: {graph.EdgeCount}");

            ReduceGraph(graph, vertices);

            Debug.WriteLine($"Pass 1 Vertex Count: {graph.VertexCount}");
            Debug.WriteLine($"Pass 1 Edge Count: {graph.EdgeCount}");

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
                        // Make sure all of the 
                        edges.Add(new(groupStart, groupDestination, path.Sum(edge => edge.cost)));
                    }
                }
            }

            // New Graph
            graph = edges.ToUndirectedGraph<DoorLockPos, DoorLockPosEdge>();

            Debug.WriteLine($"Final Vertex Count: {graph.VertexCount}");
            Debug.WriteLine($"Final Edge Count: {graph.EdgeCount}");
        }

        private void ReduceGraph(UndirectedGraph<DoorLockPos, DoorLockPosEdge> graph, DoorLockPos[] vertices)
        {
            foreach (var startVertex in vertices)
            {
                // Get the next vertices to this one
                var adjVertices = graph.AdjacentVertices(startVertex).ToArray();

                foreach (var tAdjVertex in adjVertices)
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
                    var cost = graph.AdjacentEdges(adjVertex).First(e => e.Source.id == startVertex.id || e.Target.id == startVertex.id).cost;

                    // if (startVertex.value == 'F')
                    //     System.Diagnostics.Debugger.Break();

                    do
                    {
                        var adjEdges = graph
                            .AdjacentEdges(adjVertex)
                            .Where(edge =>
                                // This may have already been processed
                                // edge.cost == 1
                                // &&
                                (
                                    // Kick out where we came from
                                    edge.GetOtherVertex(adjVertex).id != foundPath[^1].id
                                )
                            )
                            .ToArray();

                        // Track this location
                        foundPath.Add(adjVertex);

                        // If we have found an intersection, stop
                        if (adjEdges.Length != 1)
                            break;

                        // Second check here, we could have found a dead end
                        // if (adjEdges.Length == 0)
                        //     break;

                        // If we have found a non-passage, step out
                        if (adjVertex.type != DoorKeyType.Passage)
                            break;

                        // Find our next step out
                        adjVertex = adjEdges[0].GetOtherVertex(adjVertex);
                        cost += adjEdges[0].cost;
                    } while (true);

                    if (foundPath.Count > 2)
                    {
                        // If we found a path to remove, let's remove it and our startVertex
                        var lastVertex = foundPath[^1];
                        foundPath.RemoveAt(0);
                        foundPath.Remove(lastVertex);

                        graph.AddEdge(new DoorLockPosEdge(startVertex, lastVertex, cost));

                        foundPath.ForEach(removeVertex => graph.RemoveVertex(removeVertex));
                    }
                }
            }
        }

        private IEnumerable<(DoorLockPos currentBot, DoorLockPos[] bots, DoorLockPos move)> GetMoves(DoorLockPos[] pos, char[] keys)
        {
            // Found a possible opening, door, or key
            foreach (var thisPos in pos)
            {
                var otherBots = pos.Where(pos => pos.id != thisPos.id).ToArray();

                foreach (var move in graph.AdjacentVertices(thisPos))
                {
                    if (move.type == DoorKeyType.Door)
                    {
                        // If this is a door, make sure we have the corresponding key
                        if (keys.Any(c => c == move.value + 32))
                            yield return (thisPos, otherBots, move);

                        continue;
                    }

                    // Otherwise it's a key or an opening
                    yield return (thisPos, otherBots, move);
                }
            }
        }

        public int GetShortestPath()
        {
            var queue = new PriorityQueue<State, ulong>();

            // Loop through the queue from an initial state
            var start = graph.Vertices
                .Where(v => v.type == DoorKeyType.Start)
                .ToArray();

            queue.Enqueue(new State()
            {
                pos = start,
                keys = Array.Empty<char>(),
                depth = 0
            }, (ulong)0);

            // Track if we have seen this state before and what depth
            // Dictionary<string, int> seenKeys = new();
            Dictionary<(string vertexId, string keys), int> seenState = new();

            while(queue.Count > 0)
            {
                var state = queue.Dequeue();

                var pos = state.pos;
                var keys = state.keys;
                // var path = state.path;
                var depth = state.depth;

                // Maybe we're too far in
                if (depth >= minDistance)
                    continue;

                // Check if we have seen this state before
                // If we have gotten to the same position with the same keys
                // in a lower depth, skip this branch
                var stateHash = (string.Join("-",pos.OrderBy(p => p.id).Select(p => p.id)), keys.OrderBy(c => c).JoinAsString());
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

                // Each move has to be valid
                // Either it's another key, opening, or door that is unlocked
                foreach ((var currentBot, var bots, var move) in moves)
                {
                    // Duplicate keys and paths
                    var newKeys = (char[])keys.Clone();
                    // var newPath = (DoorLockPos[])path.Clone();

                    // Adding the move
                    // newPath = newPath.Append(move).ToArray();

                    // Need our edge cost
                    var success = graph.TryGetEdge(currentBot, move, out DoorLockPosEdge edge);

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
                        pos = bots.Append(move).ToArray(),
                        keys = newKeys,
                        // path = newPath,
                        depth = newDepth
                    }, (ulong)(keyCount - newKeys.Length) * (ulong)newDepth);
                }
            }

            return minDistance;
        }

        protected override string? SolvePartOne()
        {
            var sw = new Stopwatch();
            sw.Restart();
            ResetGrid(Input);
            sw.Stop();
            Debug.WriteLine($"Reset Grid Input: {sw.Elapsed}");

            return GetShortestPath().ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
            var input = Input.SplitByNewline(true)
                .Select(line => line.ToCharArray())
                .ToArray();
            
            // Replace the center of the grid
            input[39][39] = '@';
            input[39][40] = '#';
            input[39][41] = '@';

            input[40][39] = '#';
            input[40][40] = '#';
            input[40][41] = '#';

            input[41][39] = '@';
            input[41][40] = '#';
            input[41][41] = '@';

            var sw = new Stopwatch();
            sw.Restart();
            ResetGrid(string.Join('\n', input.Select(line => line.JoinAsString())));
            sw.Stop();
            Debug.WriteLine($"Reset Grid Input: {sw.Elapsed}");

            return GetShortestPath().ToString();
        }
    }
}
