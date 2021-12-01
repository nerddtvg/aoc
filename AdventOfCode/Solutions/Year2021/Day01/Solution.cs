using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2021
{

    class Day01 : ASolution
    {

        public Day01() : base(01, 2021, "Sonar Sweep")
        {

        }

        protected override string? SolvePartOne()
        {
            int count = 0;

            var lines = Input.SplitByNewline().ToList();
            for (int i = 1; i < lines.Count; i++)
            {
                if (Int32.Parse(lines[i]) > Int32.Parse(lines[i-1]))
                    count++;
            }

            return count.ToString();
        }

        protected override string? SolvePartTwo()
        {
            int count = 0;

            var lines = Input.SplitByNewline().ToList();
            for (int i = 3; i < lines.Count; i++)
            {
                if (Int32.Parse(lines[i]) + Int32.Parse(lines[i-1]) + Int32.Parse(lines[i-2]) > Int32.Parse(lines[i-1]) + Int32.Parse(lines[i-2]) + Int32.Parse(lines[i-3]))
                    count++;
            }

            return count.ToString();
        }
    }
}

#nullable restore
