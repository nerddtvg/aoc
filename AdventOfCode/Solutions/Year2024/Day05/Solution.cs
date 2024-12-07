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
        public int[][] instructions;
        public List<int[]> invalid = new();

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

            instructions = inputs[1].Select(line => line.ToIntArray(",")).ToArray();
        }

        public (bool valid, int i, int q) IsValid(int[] pages)
        {
            for (int i = 0; i < pages.Length; i++)
            {
                for (int q = i + 1; q < pages.Length; q++)
                {
                    if (!(
                        // Make sure either pages[i] doesn't exist OR it includes pages[q] for AFTER
                        (!after.ContainsKey(pages[i]) || after[pages[i]].Contains(pages[q]))
                        &&
                        // Make sure either pages[q] doesn't exist OR it does NOT include pages[i] for BEFORE
                        (!after.ContainsKey(pages[q]) || !after[pages[q]].Contains(pages[i]))
                    ))
                        return (false, i, q);
                }
            }

            return (true, 0, 0);
        }

        protected override string? SolvePartOne()
        {
            // Time: 00:00:00.0182829
            return instructions.Select(pages =>
            {
                if (pages.Length % 2 == 0) return 0;

                var (valid, i, q) = IsValid(pages);

                if (valid)
                    return pages[pages.Length / 2];
                else
                {
                    // Swap the i and q positions
                    Swap(pages, (i, q));
                    invalid.Add(pages);
                }

                return 0;
            }).Sum().ToString();
        }

        private int[] Swap(int[] pages, (int i, int q) swap) {
            (pages[swap.i], pages[swap.q]) = (pages[swap.q], pages[swap.i]);

            return pages;
        }

        // We could check all permutations, but that is brute force and slow
        // This will swap two invalid entries until it finds a valid record
        private int[] FindValid(int[] pages) {
            var (valid, i, q) = IsValid(pages);

            if (valid)
                return pages;

            Swap(pages, (i, q));
            return FindValid(pages);
        }

        protected override string? SolvePartTwo()
        {
            return invalid.Select(pages =>
            {
                if (pages.Length % 2 == 0) return 0;

                var valid = FindValid(pages);

                return pages[pages.Length / 2];
            }).Sum().ToString();
        }
    }
}

