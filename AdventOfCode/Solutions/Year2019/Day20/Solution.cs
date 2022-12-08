using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;
using System.Diagnostics;
using QuikGraph;
using QuikGraph.Algorithms;

namespace AdventOfCode.Solutions.Year2019
{

    class Day20 : ASolution
    {
        public Day20() : base(20, 2019, "Donut Maze")
        {
            
        }

        private BidirectionalGraph<GraphNode, Edge<GraphNode>> ResetGraph(string input, int depth = 0)
        {
            // Get a direct [y][x] grid
            var grid = input.SplitByNewline()
                .Select(line => line.ToCharArray())
                .ToArray();

            int maxY = grid.Length - 1;
            int maxX = grid[0].Length - 1;

            var vertexes = new List<GraphNode>();
            var edges = new List<Edge<GraphNode>>();

            for (int y = 2; y <= maxY - 2; y++)
            {
                for (int x = 2; x <= maxX - 2; x++)
                {
                    // If this is a passage, we add it to our graph
                    var c = grid[y][x];

                    if (c == '.')
                    {
                        var vertex = new GraphNode()
                        {
                            Type = NodeType.Open,
                            x = x,
                            y = y,
                            depth = depth
                        };

                        // First, identify if this is a portal or not
                        // Look to neighbors for characters
                        var portalValue = string.Empty;
                        if (65 <= grid[y - 1][x] && grid[y - 1][x] <= 90)
                        {
                            portalValue += grid[y - 2][x];
                            portalValue += grid[y - 1][x];
                        }
                        else if (65 <= grid[y][x - 1] && grid[y][x - 1] <= 90)
                        {
                            portalValue += grid[y][x - 2];
                            portalValue += grid[y][x - 1];
                        }
                        else if (65 <= grid[y + 1][x] && grid[y + 1][x] <= 90)
                        {
                            portalValue += grid[y + 1][x];
                            portalValue += grid[y + 2][x];
                        }
                        else if (65 <= grid[y][x + 1] && grid[y][x + 1] <= 90)
                        {
                            portalValue += grid[y][x + 1];
                            portalValue += grid[y][x + 2];
                        }

                        // We found a portal!
                        // If depth > 1, then AA and ZZ do not exist
                        if (!string.IsNullOrEmpty(portalValue) && (depth <= 1 || (portalValue != "AA" && portalValue != "ZZ")))
                        {
                            vertex.Type = NodeType.Portal;
                            vertex.Value = portalValue;
                            vertex.Index = y == 2 || y == maxY - 2 || x == 2 || x == maxX - 2 ? 1 : 2;
                        }

                        // Add to the list
                        vertexes.Add(vertex);

                        // Look up and left to find neighbor nodes
                        // Subsequent searches will find down and right
                        if (grid[y - 1][x] == '.')
                        {
                            edges.Add(new Edge<GraphNode>(vertexes.First(v => v.x == x && v.y == y - 1), vertex));
                            edges.Add(new(edges.Last().Target, edges.Last().Source));
                        }

                        if (grid[y][x - 1] == '.')
                        {
                            edges.Add(new Edge<GraphNode>(vertexes.First(v => v.x == x - 1 && v.y == y), vertex));
                            edges.Add(new(edges.Last().Target, edges.Last().Source));
                        }
                    }
                }
            }

            // Add edges between portals for part 1 (depth == 0)
            if (depth == 0)
                foreach(var portal2 in vertexes.Where(v => v.Type == NodeType.Portal && v.Index == 2))
                {
                    edges.Add(new Edge<GraphNode>(portal2, vertexes.First(v => v.Index == 1 && v.Value == portal2.Value)));
                    edges.Add(new(edges.Last().Target, edges.Last().Source));
                }

            // We have a list of edges and vertexes
            return edges.ToBidirectionalGraph<GraphNode, Edge<GraphNode>>();
        }

        protected override string SolvePartOne()
        {
            var graph = ResetGraph(Input);

            // Find our starting and ending points
            var start = graph.Vertices.First(v => v.Value == "AA");
            var end = graph.Vertices.First(v => v.Value == "ZZ");

            // Initialize Dijkstra
            var tryPaths = graph.ShortestPathsDijkstra(edge => 1, start);

            if (tryPaths(end, out IEnumerable<Edge<GraphNode>> path))
            {
                return path.Count().ToString();
            }

            return string.Empty;
        }

        protected override string SolvePartTwo()
        {
            // To simply cheat by using QuikGraph's search functions instead of writing our own,
            // we will layer many graphs on top of each other.
            var graph = ResetGraph(Input, 1);

            // Remove outside portals in top-level graph
            graph.Vertices
                .Where(v => v.Type == NodeType.Portal && v.Index == 1 && v.depth == 1 && v.Value != "AA" && v.Value != "ZZ")
                .ToList()
                .ForEach(v =>
                {
                    v.Value = string.Empty;
                    v.Type = NodeType.Open;
                });

            int maxDepth = 40;

            for (int i = 2; i <= maxDepth; i++)
            {
                // Get a fresh sub-map with the given depth
                var tGraph = ResetGraph(Input, i);

                // Add to the overall graph
                graph.AddVerticesAndEdgeRange(tGraph.Edges);

                // Link the two layers together
                // For every tGraph vertex with index == 1, find the corresponding index == 2
                tGraph.Vertices
                    .Where(tV => tV.Index == 1)
                    .ToList()
                    .ForEach(tV =>
                    {
                        var v = graph.Vertices.FirstOrDefault(v1 => v1.Type == NodeType.Portal && v1.depth == i - 1 && v1.Index == 2 && v1.Value == tV.Value);

                        if (v != default)
                        {
                            graph.AddEdge(new Edge<GraphNode>(tV, v));
                            graph.AddEdge(new Edge<GraphNode>(v, tV));
                        }
                    });
            }

            // Remove inside portals in lowest-level graph
            graph.Vertices
                .Where(v => v.Type == NodeType.Portal && v.Index == 2 && v.depth == maxDepth)
                .ToList()
                .ForEach(v =>
                {
                    v.Value = string.Empty;
                    v.Type = NodeType.Open;
                });

            // Find our starting and ending points
            var start = graph.Vertices.First(v => v.Value == "AA");
            var end = graph.Vertices.First(v => v.Value == "ZZ");

            // Initialize Dijkstra
            var tryPaths = graph.ShortestPathsDijkstra(edge => 1, start);

            if (tryPaths(end, out IEnumerable<Edge<GraphNode>> path))
            {
                return path.Count().ToString();
            }

            return string.Empty;
        }

        public enum NodeType
        {
            Default,
            Open,
            Portal
        }

        public class GraphNode
        {
            public NodeType Type { get; set; }

            public string Value { get; set; } = string.Empty;

            public int Index { get; set; } = 0;

            public int x { get; set; }
            public int y { get; set; }
            public int depth { get; set; } = 0;

            public override string ToString()
            {
                return string.Format("[{0,3},{1,3}] {2} {3}", x, y, Type, (Type == NodeType.Open ? string.Empty : $"{Value}-{Index}")).Trim();
            }
        }
    }
}
