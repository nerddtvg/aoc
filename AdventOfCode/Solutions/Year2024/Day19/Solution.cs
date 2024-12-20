using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    class Day19 : ASolution
    {
        public string[] available;
        public string[] desired;

        public Regex patterns;

        public Day19() : base(19, 2024, "Linen Layout")
        {
            // DebugInput = @"r, wr, b, g, bwu, rb, gb, br

            // brwrr
            // bggr
            // gbbr
            // rrbgbr
            // ubwu
            // bwurrg
            // brgr
            // bbrgwb";

            List<string> tempPatterns = [.. Input
                .SplitByBlankLine(shouldTrim: true)[0][0]
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .OrderByDescending(str => str.Length)
                .ThenBy(str => str)
            ];
            List<string> validPatterns = [];

            desired = Input.SplitByBlankLine(shouldTrim: true)[1];

            // We need to reduce the patterns down
            // A lot of these can be combined ot make the longer ones such as 'r' and 'b' can make 'rb' but also 'br' making 'rb' redundant
            while(tempPatterns.Count > 1)
            {
                // Combine validPatterns with the other patterns
                patterns = new($"^({string.Join('|', [.. validPatterns, .. tempPatterns[1..]])})+$");

                // If we can match tempPatterns[0], then it is a duplicate
                if (!patterns.IsMatch(tempPatterns[0]))
                {
                    // Found a valid option
                    validPatterns.Add(tempPatterns[0]);
                }

                tempPatterns.RemoveAt(0);
            }

            // Add the last one on
            validPatterns.AddRange(tempPatterns);
            available = [.. validPatterns];

            // Regenerate
            patterns = new($"^({string.Join('|', available)})+$");
        }

        protected override string? SolvePartOne()
        {
            return desired.Count(patterns.IsMatch).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

