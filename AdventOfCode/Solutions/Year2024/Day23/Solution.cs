using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    class Day23 : ASolution
    {
        readonly List<HashSet<string>> connected = [];

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
            Dictionary<string, HashSet<string>> connections = [];

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
            return string.Empty;
        }
    }
}

