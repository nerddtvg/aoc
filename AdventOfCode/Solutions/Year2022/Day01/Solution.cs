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
            return Input.SplitByBlankLine()
                // For each elf, split by new lines...
                .Max(lines =>
                    lines
                    // Convert to integers
                    .Select(line => Int32.Parse(line))
                    // Sum the total
                    .Sum()
                )
                .ToString();
        }

        protected override string? SolvePartTwo()
        {
            return Input.SplitByBlankLine()
                // For each elf, split by new lines, sum the total
                .Select(lines => lines.Select(line => Int32.Parse(line)).Sum())
                // Order it from highest to lowest
                .OrderByDescending(x => x)
                // Get the top 3
                .Take(3)
                // Sum the totals
                .Sum()
                .ToString();
        }
    }
}

