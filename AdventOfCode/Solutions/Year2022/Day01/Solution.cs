using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2022
{

    class Day01 : ASolution
    {

        public Day01() : base(01, 2022, "Calorie Counting")
        {

        }

        protected override string? SolvePartOne()
        {
            return Input.SplitByBlankLine().Max(lines => lines.Select(line => Int32.Parse(line)).Sum()).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return Input.SplitByBlankLine()
                .Select(lines => lines.Select(line => Int32.Parse(line)).Sum())
                .OrderByDescending(x => x)
                .Take(3)
                .Sum()
                .ToString();
        }
    }
}

