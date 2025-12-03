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
                ("987654321111111", 98),
                ("811111111111119", 89),
                ("234234234234278", 78),
                ("818181911112111", 92)
            };

            foreach ((var line, var expected) in tests)
            {
                var result = MaxJoltage(line);
                Debug.Assert(result == expected, $"Line '{line}' returned '{result}', expected '{expected}'");
            }

            var tests2 = new[]
            {
                ("987654321111111", 987654321111),
                ("811111111111119", 811111111119),
                ("234234234234278", 434234234278),
                ("818181911112111", 888911112111)
            };

            foreach ((var line, BigInteger expected) in tests2)
            {
                var result = MaxJoltage2(line);
                Debug.Assert(result == expected, $"Line '{line}' returned '{result}', expected '{expected}'");
            }
        }

        protected override string? SolvePartOne()
        {
            // Time  : 00:00:00.0048054
            return Input.SplitByNewline().Sum(MaxJoltage).ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Time  : 00:00:00.0051798
            return Input.SplitByNewline().SumBigInteger(MaxJoltage2).ToString();
        }

        private int MaxJoltage(string line)
        {
            // Search LTR and find the highest value + index
            // Then search LTR index+1 to find the next highest value
            var ret = 0;
            var max = line[0];
            var maxI = 0;

            for (int i = 1; i < line.Length - 1; i++)
            {
                if (line[i] > max)
                {
                    max = line[i];
                    maxI = i;
                }
            }

            // Convert char int to int without parse
            ret += (max - 48) * 10;

            max = line[maxI + 1];
            for (int i = maxI + 2; i < line.Length; i++)
            {
                if (line[i] > max)
                {
                    max = line[i];
                }
            }

            ret += max - 48;

            return ret;
        }

        private BigInteger? MaxJoltage2(string line)
        {
            int desiredLength = 12;

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
