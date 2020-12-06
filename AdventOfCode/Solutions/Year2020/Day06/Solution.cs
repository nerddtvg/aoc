using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{

    class Day06 : ASolution
    {
        List<string> groups = new List<string>();

        public Day06() : base(06, 2020, "")
        {
            // Go through each group and combine them
            List<string> group = new List<string>();

            foreach(string line in Input.SplitByNewline(true, false)) {
                // If the line is blank, process the group
                if (string.IsNullOrWhiteSpace(line)) {
                    if (group.Count == 0) continue;

                    processGroup(group);
                    group = new List<string>();;
                }

                group.Add(line);
            }

            if (group.Count > 0) processGroup(group);
        }

        private void processGroup(List<string> group) {
            groups.Add(string.Join("", group.SelectMany(b => b.ToCharArray()).OrderBy(a => a).Distinct()));
        }

        protected override string SolvePartOne()
        {
            return groups.Sum(a => a.Length).ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
