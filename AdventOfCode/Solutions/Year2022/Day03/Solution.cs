using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Diagnostics;

namespace AdventOfCode.Solutions.Year2022
{

    class Day03 : ASolution
    {

        public Day03() : base(03, 2022, "Rucksack Reorganization")
        {
            var example = PrioritySum(@"vJrwpWtwJgWrhcsFMMfFFhFp
                jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL
                PmmdzqPrVvPwwTWBwg
                wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn
                ttgJtRGJQctTZtZT
                CrZsJsPPZsGzwwsLwLmpwMDw");

            Debug.Assert(Debug.Equals(example, 157), $"Expected: 157\nActual: {example}");
        }

        private int PrioritySum(string input)
        {
            int endSum = 0;

            // a-z => 1 to 26
            // A-Z => 27 to 52
            foreach (var sacks in
                input.SplitByNewline(true)
                .Select(line => line
                    .ToCharArray()
                    .Select(c => (int)c)
                    .Select(c => c > 97 ? c - 96 : c - 38)
                    .ToArray()
                )
            )
            {
                // Split each line in half
                var sackA = sacks[..(sacks.Length / 2)];
                var sackB = sacks[(sacks.Length / 2)..];

                // Find the common (but distinctify) and sum it
                var common = sackA.Intersect(sackB).Distinct();
                endSum += common.Sum();
            }

            return endSum;
        }

        protected override string? SolvePartOne()
        {
            return PrioritySum(Input).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

