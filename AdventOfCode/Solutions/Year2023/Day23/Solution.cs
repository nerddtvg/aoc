using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using QuikGraph;

using System.Linq;
using QuikGraph.Algorithms;


namespace AdventOfCode.Solutions.Year2023
{
    using Point = (int x, int y);
    using QueueItem = ((int x, int y) pos, int depth, (int x, int y)[] path);

    class Day23 : ASolution
    {
        private class Vertex
        {
            public required int x { get; set; }
            public required int y { get; set; }
        }

        private class GraphEdge : Edge<Vertex>
        {
            public int cost { get; set; } = 1;

            public GraphEdge(Vertex source, Vertex target) : base(source, target)
            {

            }
        }

        private char[][] grid;
        private Point start;
        private Point end;

        public Day23() : base(23, 2023, "A Long Walk")
        {
            // DebugInput = @"#.#####################
            //                #.......#########...###
            //                #######.#########.#.###
            //                ###.....#.>.>.###.#.###
            //                ###v#####.#v#.###.#.###
            //                ###.>...#.#.#.....#...#
            //                ###v###.#.#.#########.#
            //                ###...#.#.#.......#...#
            //                #####.#.#.#######.#.###
            //                #.....#.#.#.......#...#
            //                #.#####.#.#.#########v#
            //                #.#...#...#...###...>.#
            //                #.#.#v#######v###.###v#
            //                #...#.>.#...>.>.#.###.#
            //                #####v#.#.###v#.#.###.#
            //                #.....#...#...#.#.#...#
            //                #.#########.###.#.#.###
            //                #...###...#...#...#.###
            //                ###.###.#.###v#####v###
            //                #...#...#.#.>.>.#.>.###
            //                #.###.###.#.###.#.#v###
            //                #.....###...###...#...#
            //                #####################.#";

            grid = Input.SplitByNewline(shouldTrim: true).Select(line => line.ToCharArray()).ToArray();

            // Get the start
            start = (grid[0].JoinAsString().IndexOf('.'), 0);
            end = (grid[^1].JoinAsString().IndexOf('.'), grid.Length - 1);
        }

        // Get all possible moves up, down, left, and right
        private Point[] GetMoves(Point point) => [point.Add((-1, 0)), point.Add((1, 0)), point.Add((0, -1)), point.Add((0, 1))];

        // Get the grid character in play
        // This will return '#' as a wall for anything out of bounds
        private char GetChar(Point point) => 0 <= point.y && point.y < grid.Length ? (0 <= point.x && point.x < grid[point.y].Length ? grid[point.y][point.x] : '#') : '#';

        protected override string? SolvePartOne()
        {
            // Track our max depth
            var maxDepth = 0;

            // Finding the longest path is a different take
            var queue = new Queue<QueueItem>();

            // Start the queue
            queue.Enqueue((start, 0, Array.Empty<Point>()));

            while(queue.TryDequeue(out QueueItem item))
            {
                (var pos, var depth, var path) = item;

                if (pos == end)
                {
                    maxDepth = Math.Max(depth, maxDepth);
                    continue;
                }

                // Get the current char
                var c = GetChar(pos);

                path = path.Append(pos).ToArray();

                if (c == '.')
                {
                    // We've moved to an open spot
                    var moves = GetMoves(pos);

                    foreach(var move in moves)
                    {
                        if (!path.Contains(move) && GetChar(move) != '#')
                        {
                            queue.Enqueue((move, depth + 1, path));
                        }
                    }
                }
                else if (c == '>')
                {
                    // Move one more right
                    pos = pos.Add((1, 0));

                    if (!path.Contains(pos))
                        queue.Enqueue((pos, depth + 1, path));
                }
                else if (c == 'v')
                {
                    // Move one more down
                    pos = pos.Add((0, 1));

                    if (!path.Contains(pos))
                        queue.Enqueue((pos, depth + 1, path));
                }
                else if (c == '<')
                {
                    // Move one more left
                    pos = pos.Add((-1, 0));

                    if (!path.Contains(pos))
                        queue.Enqueue((pos, depth + 1, path));
                }
                else if (c == '^')
                {
                    // Move one more up
                    pos = pos.Add((0, -1));

                    if (!path.Contains(pos))
                        queue.Enqueue((pos, depth + 1, path));
                }
            }

            return maxDepth.ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Track our max depth
            var maxDepth = 0;

            // For this, it may be easier to create a real graph and reduce it
            // The fewer vertices we need to actually scan, the better
            var points = grid
                .SelectMany((line, y) => line.Select((c, x) => (x, y, c)))
                .Where(xyc => xyc.c != '#')
                .ToDictionary(xyc => (xyc.x, xyc.y), xyc => new Vertex { x = xyc.x, y = xyc.y });

            var edges = new List<GraphEdge>();

            // Generate a graph by going left to right, up to down
            // Look to the right and down for any connections
            for (int y=0; y<grid.Length; y++)
                for(int x=0; x<grid[y].Length; x++)
                {
                    if (points.TryGetValue((x, y), out var vertex))
                    {
                        // We have a vertex, now try down and right
                        if (points.TryGetValue((x+1, y), out var vertexRight))
                            edges.Add(new(vertex, vertexRight));

                        // We have a vertex, now try down and right
                        if (points.TryGetValue((x, y + 1), out var vertexDown))
                            edges.Add(new(vertex, vertexDown));
                    }
                }

            var graph = edges.ToUndirectedGraph<Vertex, GraphEdge>();
            var startVertex = points[start];
            var endVertex = points[end];

            // Let's reduce the graph
            // Basically we remove hallways and make it intersection -(dist)-> intersection
            ReduceGraph(graph, points.Values.Where(v => graph.AdjacentEdges(v).Count() > 2).ToArray());

            // Using a priority queue where the priority is negative distance
            // means the longest path will always be checked first
            var queue = new PriorityQueue<(Vertex pos, int depth, HashSet<Vertex> path), int>();

            // To find the deepest path without the directional arrows
            // we need a way to trim down the search tree
            // Keep track of where we have been and the depth at that point
            // If we have been in that point deeper, then skip it
            var seen = new Dictionary<Point, int>();

            // Start the queue
            queue.Enqueue((startVertex, 0, new()), 0);

            while (queue.TryDequeue(out var item, out int negativeDepth))
            {
                (var pos, var depth, var path) = item;

                if (pos == endVertex)
                {
                    maxDepth = Math.Max(depth, maxDepth);
                    continue;
                }

                // This is now a list of intersections to intersections
                // We do not need to check characters
                path = new HashSet<Vertex>(path)
                {
                    pos
                };

                // Make sure we don't go back
                var moves = graph
                    .AdjacentVertices(pos)
                    .Where(v => !path.Contains(v))
                    .ToArray();

                foreach (var move in moves)
                    // This will always be true but it's the safest way to get the edge from the graph
                    if (graph.TryGetEdge(pos, move, out var edge))
                        queue.Enqueue((move, depth + edge.cost, path), (depth + edge.cost) * -1);
            }

            // Not the most efficient thing
            // Probably could have looked into the acyclic directed graph
            // and finding the longest path, but 25 seconds is fine enough for me
            // Time  : 00:00:25.0577341
            return maxDepth.ToString();
        }

        private bool VerticesEqual(Vertex a, Vertex b) => a.x == b.x && a.y == b.y;

        private void ReduceGraph(UndirectedGraph<Vertex, GraphEdge> graph, Vertex[] vertices)
        {
            foreach (var startVertex in vertices)
            {
                // Get the next vertices to this one
                var adjVertices = graph.AdjacentVertices(startVertex).ToArray();

                foreach (var tAdjVertex in adjVertices)
                {
                    var adjVertex = tAdjVertex;

                    // Don't go back
                    if (VerticesEqual(adjVertex, startVertex))
                        continue;

                    // Start with a fresh path
                    var foundPath = new List<Vertex>() { startVertex };
                    graph.TryGetEdge(startVertex, adjVertex, out GraphEdge startEdge);
                    var cost = startEdge.cost;

                    do
                    {
                        var adjEdges = graph
                            .AdjacentEdges(adjVertex)
                            .Where(edge =>
                                // Kick out where we came from
                                !VerticesEqual(edge.GetOtherVertex(adjVertex), foundPath[^1])
                            )
                            .ToArray();

                        // Track this location
                        foundPath.Add(adjVertex);

                        // If we have found an intersection, stop
                        if (adjEdges.Length != 1)
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

                        graph.AddEdge(new GraphEdge(startVertex, lastVertex) { cost = cost });

                        foundPath.ForEach(removeVertex => graph.RemoveVertex(removeVertex));
                    }
                }
            }
        }
    }
}

