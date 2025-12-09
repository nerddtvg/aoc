using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Numerics;


namespace AdventOfCode.Solutions.Year2025
{

    class Day09 : ASolution
    {
        private readonly HashSet<(int a, int b)> points;

        public Day09() : base(09, 2025, "Movie Theater")
        {
            // DebugInput = @"7,1
            //     11,1
            //     11,7
            //     9,7
            //     9,5
            //     2,5
            //     2,3
            //     7,3";

            points = [.. Input.SplitByNewline(true, true)
                .Select(line =>
                {
                    var t = line.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();
                    return (t[0], t[1]);
                })];
        }

        protected override string? SolvePartOne()
        {
            return points
                .GetAllCombos(2)
                .Select(combo => {
                    var pair = combo.ToArray();
                    return (pair, area: (BigInteger.Abs(pair[0].a - pair[1].a) + 1) * (BigInteger.Abs(pair[0].b - pair[1].b) + 1));
                })
                .Max(itm => itm.area)
                .ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

