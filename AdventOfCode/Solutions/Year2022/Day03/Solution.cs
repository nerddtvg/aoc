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

            example = GroupSum(@"vJrwpWtwJgWrhcsFMMfFFhFp
                jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL
                PmmdzqPrVvPwwTWBwg");

            // r = 18
            Debug.Assert(Debug.Equals(example, 18), $"Expected: 18\nActual: {example}");

            example = GroupSum(@"wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn
                ttgJtRGJQctTZtZT
                CrZsJsPPZsGzwwsLwLmpwMDw");

            // Z = 52
            Debug.Assert(Debug.Equals(example, 52), $"Expected: 52\nActual: {example}");

            example = GroupSum(@"vJrwpWtwJgWrhcsFMMfFFhFp
                jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL
                PmmdzqPrVvPwwTWBwg
                wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn
                ttgJtRGJQctTZtZT
                CrZsJsPPZsGzwwsLwLmpwMDw");

            Debug.Assert(Debug.Equals(example, 70), $"Expected: 70\nActual: {example}");
        }

        private int PrioritySum(string input)
        {
            int endSum = 0;

            foreach (var sacks in
                input.SplitByNewline(true)
                .Select(line => line
                    .ToCharArray()
                    .Select(c => CharacterPriority(c))
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

        /// <summary>
        /// a-z => 1 to 26
        /// A-Z => 27 to 52
        /// </summary>
        private int CharacterPriority(char c) => c > 97 ? c - 96 : c - 38;

        private int GroupSum(string input)
        {
            // For each group of 3, find the intersecting character
            // Convert that to a Priority
            // Sum
            var inputLines = input
                .SplitByNewline(true)
                .Select(line => line
                    .ToCharArray()
                    .Select(c => CharacterPriority(c))
                    .ToArray()
                )
                .ToArray();

            if (inputLines.Length % 3 != 0)
                throw new Exception();

            int endSum = 0;

            for (int i = 0; i < inputLines.Length; i += 3)
            {
                endSum += inputLines[i].Intersect(inputLines[i + 1]).Intersect(inputLines[i + 2]).Sum();
            }

            return endSum;
        }

        protected override string? SolvePartOne()
        {
            return PrioritySum(Input).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return GroupSum(Input).ToString();
        }
    }
}

