using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Diagnostics;
using System.Numerics;
using System.Collections;


namespace AdventOfCode.Solutions.Year2025
{

    class Day03 : ASolution
    {

        public Day03() : base(03, 2025, "Lobby")
        {
            var tests = new[]
            {
                ("987654321111111", 98, 987654321111),
                ("811111111111119", 89, 811111111119),
                ("234234234234278", 78, 434234234278),
                ("818181911112111", 92, 888911112111)
            };

            foreach ((var line, var expected1, BigInteger expected2) in tests)
            {
                var result1 = MaxJoltage(line, 2);
                Debug.Assert(result1 == expected1, $"[Part 1] Line '{line}' returned '{result1}', expected '{expected1}'");

                var result2 = MaxJoltage(line, 12);
                Debug.Assert(result2 == expected2, $"[Part 2] Line '{line}' returned '{result2}', expected '{expected2}'");
            }
        }

        protected override string? SolvePartOne()
        {
            // Time  : 00:00:00.0048054
            return Input.SplitByNewline().SumBigInteger(line => MaxJoltage(line, 2)).ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Time  : 00:00:00.0038272
            return Input.SplitByNewline().SumBigInteger(line => MaxJoltage(line, 12)).ToString();
        }

        private BigInteger? MaxJoltage(string line, int desiredLength = 12)
        {
            // I went to the subreddit for a hint because each attempt I had kept failing
            // Hint from: https://old.reddit.com/r/adventofcode/comments/1pcyjfs/2025_day_03_part_2/

            // Work with a sliding window
            // 0 to length-desiredLength
            // Find maximum value, slide window, try again
            var ret = string.Empty;
            for (int left = 0; left < line.Length && ret.Length < desiredLength; left++)
            {
                int right = line.Length - desiredLength + ret.Length;

                // Get the substring to check
                var part = line[left..(right + 1)];

                // Find the maximum character and its first position
                var max = part.Max();
                var idx = part.IndexOf(max);

                left += idx;
                ret += max;
            }

            return BigInteger.Parse(ret);
        }
    }
}
