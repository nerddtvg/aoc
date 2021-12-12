using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{

    class Day22 : ASolution
    {
        private class GridStorage
        {
            public int size = 0;
            public int used = 0;
            public int avail = 0;
            public int usePer = 0;
        }

        private Dictionary<(int x, int y), GridStorage> storage = new Dictionary<(int x, int y), GridStorage>();

        public Day22() : base(22, 2016, "Grid Computing")
        {
            Reset();
        }

        private void Reset()
        {
            this.storage.Clear();

            foreach(var line in Input.Trim().SplitByNewline().Skip(2))
            {
                var matches = Regex.Match(line, @"node-x([0-9]+)-y([0-9]+)\s+([0-9]+)T\s+([0-9]+)T\s+([0-9]+)T\s+([0-9]+)%");

                if (!matches.Success)
                    throw new Exception($"Invalid line: {line}");

                this.storage.Add((Int32.Parse(matches.Groups[1].Value), Int32.Parse(matches.Groups[2].Value)), new GridStorage()
                {
                    size = Int32.Parse(matches.Groups[3].Value),
                    used = Int32.Parse(matches.Groups[4].Value),
                    avail = Int32.Parse(matches.Groups[5].Value),
                    usePer = Int32.Parse(matches.Groups[6].Value)
                });
            }
        }

        protected override string SolvePartOne()
        {
            var viable = 0;

            for (int node1X = 0; node1X <= 34; node1X++)
            {
                for (int node1Y = 0; node1Y <= 24; node1Y++)
                {
                    for (int node2X = 0; node2X <= 34; node2X++)
                    {
                        for (int node2Y = 0; node2Y <= 24; node2Y++)
                        {
                            if (node1X == node2X && node1Y == node2Y)
                                continue;

                            var node1 = this.storage[(node1X, node1Y)];
                            var node2 = this.storage[(node2X, node2Y)];

                            if (node1.used == 0)
                                continue;

                            if (node1.used <= node2.avail)
                                viable++;
                        }
                    }

                }
            }

            return viable.ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
