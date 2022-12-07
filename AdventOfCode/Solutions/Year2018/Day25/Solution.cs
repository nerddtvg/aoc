using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Diagnostics;

using QuikGraph;

namespace AdventOfCode.Solutions.Year2018
{
    class Day25 : ASolution
    {

        public Day25() : base(25, 2018, "Four-Dimensional Adventure")
        {
            var examples = new Dictionary<string, int>()
            {
                {
                    @" 0,0,0,0
                    3,0,0,0
                    0,3,0,0
                    0,0,3,0
                    0,0,0,3
                    0,0,0,6
                    9,0,0,0
                    12,0,0,0",
                    2
                },
                {
                    @"-1,2,2,0
                    0,0,2,-2
                    0,0,0,-2
                    -1,2,0,0
                    -2,-2,-2,2
                    3,0,2,-1
                    -1,3,2,2
                    -1,0,-1,0
                    0,2,1,-2
                    3,0,0,0",
                    4
                },
                {
                    @"1,-1,0,1
                    2,0,-1,0
                    3,2,-1,0
                    0,0,3,1
                    0,0,-1,-1
                    2,3,-2,0
                    -2,2,0,0
                    2,-2,0,-1
                    1,-1,0,-1
                    3,2,0,2",
                    3
                },
                {
                    @"1,-1,-1,-2
                    -2,-2,0,1
                    0,2,1,3
                    -2,3,-2,1
                    0,2,3,-2
                    -1,-1,1,-2
                    0,-2,-1,0
                    -2,2,3,-1
                    1,2,2,0
                    -1,-2,0,-2",
                    8
                },
            };

            // Check each example against the logic
            foreach (var example in examples)
            {
                var count = CountConstellations(example.Key);
                Debug.Assert(Debug.Equals(count, example.Value), $"Expected: {example.Value}\nActual: {count}");
            }
        }

        private int CountConstellations(string input)
        {
            // Each line is a star coord in (x, y, z, t)
            var stars = input.SplitByNewline(true)
                .Select(line => line.Split(",", options: StringSplitOptions.TrimEntries).Select(pt => Int32.Parse(pt)).ToArray())
                .Select(pt => (pt[0], pt[1], pt[2], pt[3]))
                .ToList();

            // Doing this with QuikGraph instead
            var edges = new List<Edge<(int x, int y, int z, int t)>>();
            foreach(var star in stars)
                foreach(var star2 in stars)
                    if (star.ManhattanDistance(star2) <= 3)
                        edges.Add(new Edge<(int x, int y, int z, int t)>(star, star2));

            // Convert this to a graph
            var graph = edges.ToUndirectedGraph<(int x, int y, int z, int t), Edge<(int x, int y, int z, int t)>>();

            // Calculate the clusters
            var connections = new QuikGraph.Algorithms.ConnectedComponents.ConnectedComponentsAlgorithm<(int x, int y, int z, int t), Edge<(int x, int y, int z, int t)>>(graph);
            connections.Compute();
            return connections.ComponentCount;
        }

        protected override string? SolvePartOne()
        {
            return CountConstellations(Input).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

