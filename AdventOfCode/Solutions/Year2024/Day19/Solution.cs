using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    class Day19 : ASolution
    {
        string[] desired = [];
        readonly HashSet<string> allPatterns = [];

        Regex patterns = new("^$");
        int maxLength;

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
        }

        protected override string? SolvePartOne()
        {
            List<string> validPatterns = [];

            List<string> tempPatterns = [.. Input
                .SplitByBlankLine(shouldTrim: true)[0][0]
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .OrderByDescending(str => str.Length)
                .ThenBy(str => str)
            ];

            desired = Input.SplitByBlankLine(shouldTrim: true)[1];

            // We need to reduce the patterns down
            // A lot of these can be combined ot make the longer ones such as 'r' and 'b' can make 'rb' but also 'br' making 'rb' redundant
            while (tempPatterns.Count > 1)
            {
                // Combine validPatterns with the other patterns
                patterns = new($"^({string.Join('|', tempPatterns[1..])})+$");

                allPatterns.Add(tempPatterns[0]);

                // If we can match tempPatterns[0], then it is a duplicate
                if (!patterns.IsMatch(tempPatterns[0]))
                {
                    // Found a valid option
                    // Only one way to make this
                    validPatterns.Add(tempPatterns[0]);
                }

                tempPatterns.RemoveAt(0);
            }

            // Add the last one on
            allPatterns.Add(tempPatterns[0]);
            validPatterns.Add(tempPatterns[0]);

            // For Part 2, we need the maxLength
            maxLength = allPatterns.Max(key => key.Length);

            // Regenerate
            // Include the duplicates in here (change from original Part 1 code)
            patterns = new($"^({string.Join('|', validPatterns)})+$");

            // Time: 00:00:00.1188499
            // Time with P2, moving procesing code locally: 00:00:00.2104896
            return desired.Count(patterns.IsMatch).ToString();
        }

        ulong CountCombinations(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return 0;
            }

            // Need to go through the permutations
            // Took a hint from one of the subreddit solutions
            // which was similar to my second attempt to subdivide the strings
            var counts = new Dictionary<int, ulong>() { [0] = 1 };

            // i goes from 1 to str.Length because it is our ending offset, not an index
            for (int i = 1; i <= str.Length; i++)
            {
                if (!counts.ContainsKey(i))
                    counts[i] = 0;

                var start = Math.Max(0, i - maxLength);
                var end = start + i;

                // Go from i-maxLength to maxLength offset
                for (int q = start; q < end && q < i; q++)
                {
                    if (!counts.ContainsKey(q))
                        counts[q] = 0;

                    // If this combination is a valid pattern, then count it
                    // We add count[q] because we're multiplying each step
                    if (allPatterns.Contains(str[q..i]))
                    {
                        counts[i] += counts[q];
                    }
                }
            }

            return counts[counts.Count - 1];
        }

        protected override string? SolvePartTwo()
        {
            // Time: 00:00:00.1384304
            return desired
                .Where(pattern => patterns.IsMatch(pattern))
                .Sum(pattern => CountCombinations(pattern))
                .ToString();
        }
    }
}

