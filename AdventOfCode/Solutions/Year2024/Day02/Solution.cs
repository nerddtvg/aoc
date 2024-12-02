using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    class Day02 : ASolution
    {
        public int[][] reports;

        public Day02() : base(02, 2024, "Red-Nosed Reports")
        {
//             DebugInput = @"7 6 4 2 1
// 1 2 7 8 9
// 9 7 6 2 1
// 1 3 2 4 5
// 8 6 4 4 1
// 1 3 6 7 9";

            reports = Input
                .SplitByNewline()
                .Select(line => line.Split(" ").Select(item => int.Parse(item)).ToArray())
                .ToArray();
        }

        public bool IsSafe(int[] report) {
            var diffs = Enumerable.Range(1, report.Length - 1).Select(i => report[i] - report[i - 1]).ToArray();

            // Must be all increasing or decreasing, any combo is invalid
            if (diffs.Any(d => d > 0) && diffs.Any(d => d < 0)) return false;

            // Can only be an increase/decrease of up to +/- 3, cannot be zero
            return !diffs.Any(d => d == 0 || d < -3 || d > 3);
        }

        protected override string? SolvePartOne()
        {
            return reports.Count(IsSafe).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

