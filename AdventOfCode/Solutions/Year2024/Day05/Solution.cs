using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    class Day05 : ASolution
    {
        // Tracks pages printed after Key page
        public Dictionary<int, HashSet<int>> after = new();
        public string[] instructions;

        public Day05() : base(05, 2024, "Print Queue")
        {
//             DebugInput = @"47|53
// 97|13
// 97|61
// 97|47
// 75|29
// 61|13
// 75|53
// 29|13
// 97|29
// 53|29
// 61|53
// 97|53
// 61|29
// 47|13
// 75|47
// 97|75
// 47|61
// 75|61
// 47|29
// 75|13
// 53|13

// 75,47,61,53,29
// 97,61,53,29,13
// 75,29,13
// 75,97,47,61,53
// 61,13,29
// 97,13,75,29,47";

            var inputs = Input.SplitByBlankLine();

            inputs[0].ForEach(line =>
            {
                var vals = line.Split('|').Select(v => Int32.Parse(v)).ToArray();

                if (!after.ContainsKey(vals[0]))
                    after[vals[0]] = new();

                after[vals[0]].Add(vals[1]);
            });

            instructions = inputs[1];
        }

        public bool IsValid(int[] pages)
        {
            return Enumerable.Range(0, pages.Length - 1).All(i =>
            {
                return Enumerable
                .Range(i + 1, pages.Length - i - 1)
                .All(q =>
                // Make sure either pages[i] doesn't exist OR it includes pages[q] for AFTER
                (!after.ContainsKey(pages[i]) || after[pages[i]].Contains(pages[q]))
                &&
                // Make sure either pages[q] doesn't exist OR it does NOT include pages[i] for BEFORE
                (!after.ContainsKey(pages[q]) || !after[pages[q]].Contains(pages[i]))
                );
            });
        }

        protected override string? SolvePartOne()
        {
            // Time: 00:00:00.0182829
            return instructions.Select(line =>
            {
                var pages = line.ToIntArray(",");

                if (pages.Length % 2 == 1 && IsValid(pages))
                    return pages[pages.Length / 2];

                return 0;
            }).Sum().ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

