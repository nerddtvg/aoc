using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2021
{

    class Day08 : ASolution
    {

        public Day08() : base(08, 2021, "Seven Segment Search")
        {

        }

        protected override string? SolvePartOne()
        {
            // Digits 1, 4, 7, 8 all use different length sequences
            // These are the sequence lengths
            var uniqueLengths = new int[] { 2, 4, 3, 7 };

            return Input.SplitByNewline()
                // For every line...
                .Sum(line =>
                    // Get the output side
                    line.Split('|', 2, StringSplitOptions.TrimEntries)[1]
                    // Get all output groups
                    .Split(' ', StringSplitOptions.TrimEntries)
                    // Group them by length
                    .GroupBy(grp => grp.Length)
                    // If they're in the unique list, count them
                    .Where(grp => uniqueLengths.Contains(grp.Key))
                    .Sum(grp => grp.Count())
                ).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
