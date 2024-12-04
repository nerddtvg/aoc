using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    class Day03 : ASolution
    {
        public Regex regex = new("mul\\(([0-9]{1,3}),([0-9]{1,3})\\)");

        public Day03() : base(03, 2024, "Mull It Over")
        {
            // DebugInput = "xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))";
        }

        protected override string? SolvePartOne()
        {
            // Time: 00:00:00.0034292
            return regex.Matches(Input)
                .Sum(match => ulong.Parse(match.Groups[1].Value) * ulong.Parse(match.Groups[2].Value))
                .ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

