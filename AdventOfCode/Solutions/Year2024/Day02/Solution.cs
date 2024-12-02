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

        public bool IsSafeP2(int[] report)
        {
            // Go through each report and:
            // 1. If IsSafe as-is, return true
            // 2. Remove a single instance, try again
            // 3. If IsSafe, count it
            // 4. Repeat until all variances are tried
            // 5. Check that safeCount == 1

            // There is probably a faster way to analyze this
            // But at 0.0064284 seconds, I didn't care

            if (IsSafe(report)) return true;

            for (int i = 0; i < report.Length; i++)
            {
                // Take + Skip to avoid the item in this index
                if (IsSafe(report.Take(i).Concat(report.Skip(i + 1)).ToArray()))
                    return true;
            }

            return false;
        }

        protected override string? SolvePartOne()
        {
            return reports.Count(IsSafe).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return reports.Count(IsSafeP2).ToString();
        }
    }
}

