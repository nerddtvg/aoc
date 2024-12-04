using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    class Day03 : ASolution
    {
        public Regex regex = new("(mul\\((?<d1>[0-9]{1,3}),(?<d2>[0-9]{1,3})\\)|do\\(\\)|don't\\(\\))");

        public Day03() : base(03, 2024, "Mull It Over")
        {
            // DebugInput = "xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))";
            // DebugInput = "xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))";
        }

        protected override string? SolvePartOne()
        {
            // Time: 00:00:00.0034292
            return regex.Matches(Input)
                // All matches are >3 chars, so this is safe
                .Where(match => match.Value[0..3] == "mul")
                .Sum(match => ulong.Parse(match.Groups["d1"].Value) * ulong.Parse(match.Groups["d2"].Value))
                .ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Time: 00:00:00.0014282
            ulong sum = 0;
            bool skip = false;

            foreach (Match match in regex.Matches(Input))
            {
                if (match.Value[0..3] == "do(")
                {
                    skip = false;
                }
                else if (match.Value[0..3] == "don")
                {
                    skip = true;
                }
                else if (!skip)
                {
                    sum += ulong.Parse(match.Groups["d1"].Value) * ulong.Parse(match.Groups["d2"].Value);
                }
            }

            return sum.ToString();
        }
    }
}

