using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2015
{

    class Day02 : ASolution
    {

        public Day02() : base(02, 2015, "")
        {

        }

        protected override string SolvePartOne()
        {
            uint totalSq = 0;

            // Get present dimensions
            foreach(var line in Input.SplitByNewline())
            {
                if (string.IsNullOrEmpty(line.Trim())) continue;

                var size = line.Split('x', 3, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                if (size.Length != 3) continue;

                var sizes = size.Select(a => UInt32.Parse(a)).ToArray();

                var sides = new uint[] { sizes[0] * sizes[1], sizes[1] * sizes[2], sizes[0] * sizes[2] };

                totalSq += (2 * sides[0]) + (2 * sides[1]) + (2 * sides[2]) + Math.Min(sides[0], Math.Min(sides[1], sides[2]));
            }

            return totalSq.ToString();
        }

        protected override string SolvePartTwo()
        {
            uint totalLen = 0;

            // Get present dimensions
            foreach(var line in Input.SplitByNewline())
            {
                if (string.IsNullOrEmpty(line.Trim())) continue;

                var size = line.Split('x', 3, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                if (size.Length != 3) continue;

                var sizes = size.Select(a => UInt32.Parse(a)).ToList();

                // Get the two smallest
                var max = sizes.Max();
                sizes.Remove(max);

                // Calculate the shortest perimeter and volume
                totalLen += (sizes[0] + sizes[0] + sizes[1] + sizes[1]) + (sizes[0] * sizes[1] * max);
            }

            return totalLen.ToString();
        }
    }
}
