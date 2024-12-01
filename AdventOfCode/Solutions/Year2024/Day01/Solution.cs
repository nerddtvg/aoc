using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    class Day01 : ASolution
    {

        public int[] List1;
        public int[] List2;

        public Day01() : base(01, 2024, "Historian Hysteria")
        {
//             DebugInput = @"3   4
// 4   3
// 2   5
// 1   3
// 3   9
// 3   3";

            var inputs = Input
                .SplitByNewline()
                .Select(line => line.Split("   "))
                .Select(item => new int[] { int.Parse(item[0]), int.Parse(item[1]) })
                .ToArray();

            List1 = inputs.Select(row => row[0]).ToArray();
            List2 = inputs.Select(row => row[1]).ToArray();
        }

        protected override string? SolvePartOne()
        {
            List1 = List1.Order().ToArray();
            List2 = List2.Order().ToArray();

            return Enumerable.Range(0, List1.Length)
                .Sum(i => Math.Abs(List1[i] - List2[i]))
                .ToString();
        }

        protected override string? SolvePartTwo()
        {
            return List1
                .Sum(l1item => (uint)l1item * List2.Count(l2item => l2item == l1item))
                .ToString();
        }
    }
}

