using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

// /u/Ok_System_5724 had a really cool solution: https://old.reddit.com/r/adventofcode/comments/rnejv5/2021_day_24_solutions/hqgl5hj/
// While I simplified the code into C#, the math was escaping me while trying to read it.
// It all comes down to just three lines in each program block

// I'm going to reimplement what they have provided and interpret it myself as a learning experience

namespace AdventOfCode.Solutions.Year2021
{

    class Day24 : ASolution
    {
        (int a, int b, int c)[] programs;

        long[] valid = new long[] { };

        public Day24() : base(24, 2021, "Arithmetic Logic Unit")
        {
            programs = Input
                // Each program is 18 lines long
                .SplitByNewline().Chunk(18)
                // Select lines 4, 5, and 15
                // Line 4: div z 1 or div z 26 (always one or the other)
                // Line 5: add x -8 (sample)
                // Line 15: add y 12 (sample)
                .SelectMany(c => new[] { c[4], c[5], c[15] })
                // Get the integer values of each of these
                .Select(c => Convert.ToInt32(c.Substring(6))).Chunk(3)
                .Select(c => (a: c[0], b: c[1], c: c[2]))
                .ToArray();
        }

        protected override string? SolvePartOne()
        {
            // Only use the digits 1 through 9
            var nums = Enumerable.Range(1, 9);

            IEnumerable<long> check(int[] digits, int i, int z)
            {
                if (digits.Length == programs.Length) return z == 0 
                    ? new[] { long.Parse(String.Join("", digits)) } : new long[0];

                IEnumerable<long> sub(IEnumerable<int> range, Func<int, int> z) =>
                    range.SelectMany(w => check(digits.Append(w).ToArray(), i + 1, z(w)));

                return programs[i].b < 0
                    ? sub(nums.Where(n => z % 26 + programs[i].b == n), (w) => z / 26)
                    : sub(nums, (w) => z * 26 + w + programs[i].c);
            }

            // Get all possible answers
            valid = check(new int[0], 0, 0).ToArray();

            return valid.Max().ToString();
        }

        protected override string? SolvePartTwo()
        {
            return valid.Min().ToString();
        }
    }
}

#nullable restore
