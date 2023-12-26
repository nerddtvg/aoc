using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using QuikGraph;

namespace AdventOfCode.Solutions.Year2023
{
    class Day25 : ASolution
    {
        UndirectedGraph<Node, Edge<Node>> graph;

        private class Node
        {
            public string node;
        }

        public Day25() : base(25, 2023, "Snowverload")
        {
            // DebugInput = @"jqt: rhn xhk nvd
            //                rsh: frs pzl lsr
            //                xhk: hfx
            //                cmg: qnr nvd lhk bvb
            //                rhn: xhk bvb hfx
            //                bvb: xhk hfx
            //                pzl: lsr hfx nvd
            //                qnr: nvd
            //                ntq: jqt hfx bvb xhk
            //                nvd: lhk
            //                lsr: lhk
            //                rzs: qnr cmg lsr rsh
            //                frs: qnr lhk lsr";

            graph = new();

            foreach(var line in Input.SplitByNewline(shouldTrim: true))
            {
                var split = line.Split(':', StringSplitOptions.TrimEntries);
                var children = split[1].Split(' ').Select(child => graph.Vertices.FirstOrDefault(v => v.node == child) ?? new Node() { node = child }).ToList();

                var sNode = graph.Vertices.FirstOrDefault(v => v.node == split[0]) ?? new Node() { node = split[0] };

                graph.AddVerticesAndEdgeRange(children.Select(child => new Edge<Node>(sNode, child)));
            }
        }

        protected override string? SolvePartOne()
        {
            // Tried to implement mincut as shown from Thomas Jungblut
            // This is a very slow algorithm given our data structures
            // https://blog.thomasjungblut.com/graph/mincut/mincut/

            // I wanted to look into Krager's algorithm earlier, but didn't understand the pseudo codes
            // so I tried the earlier Jungblunt copy of mincut.
            // I decided to use the base graph and use /u/noonan1487's implementation
            // https://old.reddit.com/r/adventofcode/comments/18qbsxs/2023_day_25_solutions/keu4oci/

            // Node names
            List<List<string>> subsets = new List<List<string>>();

            do
            {
                subsets = new List<List<string>>();

                foreach (var vertex in graph.Vertices)
                {
                    subsets.Add(new List<string>() { vertex.node });
                }

                int i;
                List<string> subset1, subset2;

                while (subsets.Count > 2)
                {
                    i = new Random().Next() % graph.Edges.Count();
                    var edge = graph.Edges.ElementAt(i);

                    subset1 = subsets.Where(s => s.Contains(edge.Source.node)).First();
                    subset2 = subsets.Where(s => s.Contains(edge.Target.node)).First();

                    if (subset1 == subset2) continue;

                    subsets.Remove(subset2);
                    subset1.AddRange(subset2);
                }

            } while (CountCuts(subsets) != 3);

            return (subsets[0].Count * subsets[1].Count).ToString();
        }

        private int CountCuts(List<List<string>> subsets)
        {
            var edges = graph.Edges.ToList();

            int cuts = 0;
            for (int i = 0; i < edges.Count; ++i)
            {
                var subset1 = subsets.Where(s => s.Contains(edges[i].Source.node)).First();
                var subset2 = subsets.Where(s => s.Contains(edges[i].Target.node)).First();
                if (subset1 != subset2) ++cuts;
            }

            return cuts;
        }

        protected override string? SolvePartTwo()
        {
            return "Finished!";
        }
    }
}

