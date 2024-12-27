using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using QuikGraph;
using QuikGraph.Algorithms.Cliques;


namespace AdventOfCode.Solutions.Year2024
{

    class Day23 : ASolution
    {
        readonly Dictionary<string, HashSet<string>> connections = [];

        readonly List<HashSet<string>> cliques = [];

        public Day23() : base(23, 2024, "LAN Party")
        {
            // DebugInput = @"kh-tc
            // qp-kh
            // de-cg
            // ka-co
            // yn-aq
            // qp-ub
            // cg-tb
            // vc-aq
            // tb-ka
            // wh-tc
            // yn-cg
            // kh-ub
            // ta-co
            // de-co
            // tc-td
            // tb-wq
            // wh-td
            // ta-ka
            // td-qp
            // aq-cg
            // wq-ub
            // ub-vc
            // de-ta
            // wq-aq
            // wq-vc
            // wh-yn
            // ka-de
            // kh-ta
            // co-tc
            // wh-qp
            // tb-vc
            // td-yn";
        }

        protected override string? SolvePartOne()
        {
            foreach (var line in Input.SplitByNewline(shouldTrim: true))
            {
                var split = line.Split('-');

                if (connections.TryGetValue(split[0], out var c))
                    c.Add(split[1]);
                else
                    connections[split[0]] = [split[1]];

                if (connections.TryGetValue(split[1], out var c2))
                    c2.Add(split[0]);
                else
                    connections[split[1]] = [split[0]];
            }

            // For each computer that starts with t, look for two connected computers
            var sets = new HashSet<string>();
            foreach (var tComputerKey in connections.Keys.Where(k => k[0] == 't'))
            {
                var tComputer = connections[tComputerKey];

                foreach (var tA in tComputer)
                {
                    foreach (var tB in tComputer)
                    {
                        if (tA == tB) continue;

                        // If tA appears in tB's list, we have a cluster
                        if (connections[tA].Contains(tB))
                        {
                            sets.Add(string.Join(",", new string[] { tComputerKey, tA, tB }.OrderBy(v => v)));
                        }
                    }
                }
            }

            // Time: 00:00:00.0120076
            return sets.Count.ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Hint from the solutions thread: Bron-Kerbosch (maximal clique)
            // https://en.wikipedia.org/wiki/Bron%E2%80%93Kerbosch_algorithm
            // https://www.geeksforgeeks.org/maximal-clique-problem-recursive-solution/
            // https://iq.opengenus.org/bron-kerbosch-algorithm/

            // QuikGraph does not have an implementation for this:
            // https://github.com/KeRNeLith/QuikGraph/issues/2

            var graph = new UndirectedGraph<string, Edge<string>>();
            connections.ForEach(kvp => kvp.Value.ForEach(end => graph.AddVerticesAndEdge(new(kvp.Key, end))));

            // Made a graph for easy access
            BK([], [.. graph.Vertices], [], graph);

            // Time: 00:00:02.0925665
            return string.Join(",", cliques.OrderByDescending(set => set.Count).FirstOrDefault()?.Select(item => item).OrderBy(item => item).ToArray() ?? []);
        }

        void BK(HashSet<string> R, HashSet<string> P, HashSet<string> X, UndirectedGraph<string, Edge<string>> graph)
        {
            if (P.Count == 0 && X.Count == 0)
            {
                cliques.Add(R);
                return;
            }

            while(P.Count > 0)
            {
                var v = P.First();

                HashSet<string> newR = [.. R, v];
                HashSet<string> newP = [.. P.Where(p => graph.TryGetEdge(v, p, out var edge))];
                HashSet<string> newX = [.. X.Where(x => graph.TryGetEdge(v, x, out var edge))];

                BK(newR, newP, newX, graph);

                P.Remove(v);
                X.Add(v);
            }
        }
    }
}

