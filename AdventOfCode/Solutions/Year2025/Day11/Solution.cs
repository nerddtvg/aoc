using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using QuikGraph;
using QuikGraph.Algorithms;


namespace AdventOfCode.Solutions.Year2025
{

    class Day11 : ASolution
    {
        public readonly BidirectionalGraph<string, Edge<string>> graph = new();
        public readonly string[] sorted = [];

        public Day11() : base(11, 2025, "Reactor")
        {
            // DebugInput = @"aaa: you hhh
            //     you: bbb ccc
            //     bbb: ddd eee
            //     ccc: ddd eee fff
            //     ddd: ggg
            //     eee: out
            //     fff: out
            //     ggg: out
            //     hhh: ccc fff iii
            //     iii: out";

            Input.SplitByNewline(true, true)
                .ForEach(line =>
                {
                    // Each node is 3 letters making this easier
                    var split = line.Split(' ');
                    var dests = new HashSet<string>(split[1..]);

                    dests.ForEach(dest =>
                    {
                        graph.AddVerticesAndEdge(new Edge<string>(split[0][0..3], dest));
                    });
                });

            // Quikgraph makes this part easy
            sorted = graph.TopologicalSort().ToArray();
        }

        private ulong PathCount(string from, string to)
        {
            // I've chosen to do a topological sort and then count paths from "you" to "out"
            // This is easier than trying to do a DFS or BFS and tracking paths
            // This solution assumes there are no cycles in the graph
            // https://www.geeksforgeeks.org/dsa/count-possible-paths-two-vertices/

            // The first index of the sorted array is a node with no incoming edges
            // There can be more than one, but we remove this node until we reach "you"
            // because we can never reach these nodes from "you"
            // PART 2: Removed because we cannot trim out these nodes
            // while (sorted[0] != "you")
            // {
            //     graph.RemoveVertex(sorted[0]);
            //     sorted = graph.TopologicalSort().ToArray();
            // }

            // Now that we have a sorted order of nodes starting from "you"
            // we can count the number of paths to each node
            var pathCounts = sorted.ToDictionary(node => node, node => (ulong)0);
            pathCounts[from] = 1;

            foreach (var node in sorted)
            {
                foreach (var edge in graph.OutEdges(node))
                {
                    pathCounts[edge.Target] += pathCounts[node];
                }
            }

            return pathCounts[to];
        }

        protected override string? SolvePartOne()
        {
            // Time  : 00:00:00.3097396
            // Time  : 00:00:00.0157675 (after removing the sorted pruning)
            return PathCount("you", "out").ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Since this is directional, svr -> fft -> dac -> out and svr -> dac -> fft -> out cannot have the same path
            // So we can identify the counts between those and combine them
            // Time  : 00:00:00.0042417
            return (
                (PathCount("svr", "fft") * PathCount("fft", "dac") * PathCount("dac", "out")) +
                (PathCount("svr", "dac") * PathCount("dac", "fft") * PathCount("fft", "out"))
                ).ToString();
        }
    }
}

