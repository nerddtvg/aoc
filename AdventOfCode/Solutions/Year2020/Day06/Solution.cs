using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{

    class Day06 : ASolution
    {
        List<string> groups = new List<string>();
        List<int> groupsCount = new List<int>();

        public Day06() : base(06, 2020, "")
        {
            // Go through each group and combine them
            List<string> group = new List<string>();

            foreach(string line in Input.SplitByNewline(true, false)) {
                // If the line is blank, process the group
                if (string.IsNullOrWhiteSpace(line)) {
                    if (group.Count == 0) continue;

                    processGroup(group);
                    group = new List<string>();
                    continue;
                }

                group.Add(line);
            }

            if (group.Count > 0) processGroup(group);
        }

        private void processGroup(List<string> group) {
            string g = string.Join("", group.SelectMany(b => b.ToCharArray()).OrderBy(a => a).Distinct());
            groups.Add(g);

            // We need to search everything in List<> to see how many of g are included
            int gCount = 0;

            foreach(string c in g.ToCharArray().Select(a => a.ToString())) {
                bool found = true;

                foreach(string person in group) {
                    if (!person.Contains(c)) {
                        // Someone didn't have it, don't count it
                        found = false;
                        break;
                    }
                }

                if (found) gCount++;
            }

            groupsCount.Add(gCount);
        }

        protected override string SolvePartOne()
        {
            return groups.Sum(a => a.Length).ToString();
        }

        protected override string SolvePartTwo()
        {
            return groupsCount.Sum(a => a).ToString();
        }
    }
}
