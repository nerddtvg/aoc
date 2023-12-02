using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2023
{

    class Day01 : ASolution
    {
        private List<int> calibrationNumbers = new();

        public Day01() : base(01, 2023, "Trebuchet?!")
        {
            var reg = new Regex("[0-9]");

            calibrationNumbers = Input.SplitByNewline()
                .Select(line => reg.Matches(line))
                .Select(allNumbers => int.Parse($"{allNumbers[0].Value}{allNumbers[^1].Value}"))
                .ToList();
        }

        protected override string? SolvePartOne()
        {
            return calibrationNumbers.Sum().ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

