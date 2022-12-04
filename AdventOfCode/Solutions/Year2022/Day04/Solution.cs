using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2022
{

    class Day04 : ASolution
    {

        public Day04() : base(04, 2022, "")
        {

        }

        protected override string? SolvePartOne()
        {
            var pairs = Input.SplitByNewline()
                .Select(line => line.Split(',').ToArray())
                .Select(pair => new int[] { Int32.Parse(pair[0].Split('-')[0]), Int32.Parse(pair[0].Split('-')[1]), Int32.Parse(pair[1].Split('-')[0]), Int32.Parse(pair[1].Split('-')[1]) })
                .ToArray();

            return pairs.Count(pair => (pair[0] >= pair[2] && pair[1] <= pair[3]) || (pair[2] >= pair[0] && pair[3] <= pair[1])).ToString();
        }

        protected override string? SolvePartTwo()
        {
            var pairs = Input.SplitByNewline()
                .Select(line => line.Split(',').ToArray())
                .Select(pair => new int[] { Int32.Parse(pair[0].Split('-')[0]), Int32.Parse(pair[0].Split('-')[1]), Int32.Parse(pair[1].Split('-')[0]), Int32.Parse(pair[1].Split('-')[1]) })
                .ToArray();

            // Any overlap at all
            // start1, end1 => start1 <= end2 && end1 >= end2
            // start2, end2 => start2 <= end1 && end2 >= end1
            return pairs.Count(pair => (pair[0] <= pair[3] && pair[1] >= pair[3]) || (pair[2] <= pair[1] && pair[3] >= pair[1])).ToString();
        }
    }
}

