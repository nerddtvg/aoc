using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Diagnostics;
using QuikGraph;
using QuikGraph.Algorithms;

// I'm going to be lazy and load this into QuikGraph instead of rolling
// my own shortest-path algoritms

namespace AdventOfCode.Solutions.Year2022
{

    class Day12 : ASolution
    {

        public Day12() : base(12, 2022, "Hill Climbing Algorithm")
        {
            var example = @"Sabqponm
                abcryxxl
                accszExk
                acctuvwj
                abdefghi";

            var graph = ReadGraph(example);
            var path = GetShortestPath(graph);
            Debug.Assert(Debug.Equals(path.Count, 31), $"Expected: 31\nActual: {path.Count}");
        }

        private BidirectionalGraph<MapPoint, Edge<MapPoint>> ReadGraph(string input)
        {
            List<MapPoint> map = new();
            List<Edge<MapPoint>> edges = new();
            int y = 0;

            foreach(var line in input.SplitByNewline(true))
            {
                int x = 0;

                foreach(var c in line)
                {
                    // Generate this point
                    var point = new MapPoint()
                    {
                        x = x,
                        y = y,
                        value = c == 'S' ? 'a' : (c == 'E' ? 'z' : c),
                        start = c == 'S',
                        finish = c == 'E'
                    };

                    map.Add(point);

                    // Check for edges against left and up directions
                    // Can't go more than 1 higher, but can go lower
                    var up = map.FirstOrDefault(pt => pt.x == x && pt.y == y - 1);
                    var left = map.FirstOrDefault(pt => pt.x == x - 1 && pt.y == y);

                    if (up != default)
                    {
                        // If we are higher or equal to the neighbor, we can go there
                        // Or if the neighbor is only 1 higher
                        if (point.value >= up.value - 1)
                            edges.Add(new Edge<MapPoint>(point, up));

                        // If the neighbor is higher instead, go there
                        if (up.value >= point.value - 1)
                            edges.Add(new Edge<MapPoint>(up, point));
                    }

                    if (left != default)
                    {
                        // If we are higher or equal to the neighbor, we can go there
                        // Or if the neighbor is only 1 higher
                        if (point.value >= left.value - 1)
                            edges.Add(new Edge<MapPoint>(point, left));

                        // If the neighbor is higher instead, go there
                        if (left.value >= point.value - 1)
                            edges.Add(new Edge<MapPoint>(left, point));
                    }

                    x++;
                }

                y++;
            }

            // We could only do edges.ToUndirectedGraph()
            // but this also includes any unconnected vertices
            var graph = edges.ToBidirectionalGraph<MapPoint, Edge<MapPoint>>();
            graph.AddVertexRange(map);

            return graph;
        }

        private List<Edge<MapPoint>> GetShortestPath(BidirectionalGraph<MapPoint, Edge<MapPoint>> graph)
        {
            // Use one of the glorious shortest-path algorithms
            // Our algorithm starting from Start
            // All edges have a weight of one
            var getPaths = graph.ShortestPathsDijkstra(edge => 1, graph.Vertices.First(pt => pt.start));

            if (getPaths(graph.Vertices.First(pt => pt.finish), out IEnumerable<Edge<MapPoint>> path))
            {
                return path.ToList();
            }

            return new();
        }

        protected override string? SolvePartOne()
        {
            var graph = ReadGraph(Input);
            var path = GetShortestPath(graph);
            return path.Count.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }

        /// <summary>
        /// Simple class to provide vertices of the grid
        /// </summary>
        public class MapPoint
        {
            public int x;
            public int y;
            public char value;
            public bool start;
            public bool finish;

            public override string ToString()
            {
                return $"[{x},{y}] {(start ? "Start" : (finish ? "End" : value))}";
            }
        }
    }
}

