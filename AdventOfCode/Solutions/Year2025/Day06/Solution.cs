using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Numerics;


namespace AdventOfCode.Solutions.Year2025
{

    class Day06 : ASolution
    {
        private readonly string[][] lines;

        public Day06() : base(06, 2025, "Trash Compactor")
        {
            // DebugInput = @"123 328  51 64 
            //     45 64  387 23 
            //     6 98  215 314
            //     *   +   *   +  ";

            lines = [..Input.SplitByNewline().Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToArray())] ;
        }

        protected override string? SolvePartOne()
        {
            var idx = -1;

            return lines[^1].SumBigInteger(op =>
            {
                idx++;

                var enumerable = lines.SkipLast(1);

                if (op == "+")
                {
                    return enumerable.SumBigInteger(line => BigInteger.Parse(line[idx]));
                } else
                {
                    return enumerable.Aggregate((BigInteger)1, (agg, line) => agg * BigInteger.Parse(line[idx]));
                }
            }).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

