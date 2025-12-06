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
        private string[][] lines = [];

        public Day06() : base(06, 2025, "Trash Compactor")
        {
//             DebugInput = @"123 328  51 64 
//  45 64  387 23 
//   6 98  215 314
// *   +   *   +  ";
        }

        protected override string? SolvePartOne()
        {
            lines = [.. Input.SplitByNewline().Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToArray())];

            var idx = -1;

            // Time  : 00:00:00.0165900
            return lines[^1].SumBigInteger(op =>
            {
                idx++;

                var enumerable = lines.SkipLast(1);

                if (op == "+")
                {
                    return enumerable.SumBigInteger(line => BigInteger.Parse(line[idx]));
                }
                else
                {
                    return enumerable.Aggregate((BigInteger)1, (agg, line) => agg * BigInteger.Parse(line[idx]));
                }
            }).ToString();
        }

        protected override string? SolvePartTwo()
        {
            string[] lines = [.. Input.SplitByNewline()];

            // Ensure all lines are the same length
            var maxLength = lines.Max(line => line.Length);
            lines.ForEach((line, idx) => lines[idx] = lines[idx].PadRight(maxLength));

            // Keep a rolling result
            var finalResult = BigInteger.Zero;
            var tempResult = BigInteger.Zero;

            // Get our starting operation
            var op = lines[^1][0];

            for (int i = 0; i < lines[0].Length; i++)
            {
                // Check if we have moved on
                if (lines[^1][i] != ' ')
                {
                    op = lines[^1][i];

                    if (op == '+')
                        tempResult = BigInteger.Zero;
                    else
                        tempResult = BigInteger.One;
                }

                // Ending of current operation
                if (lines.All(line => line[i] == ' '))
                {
                    finalResult += tempResult;
                    continue;
                }

                var num = BigInteger.Parse(lines.SkipLast(1).Select(line => line[i]).JoinAsString().Trim());

                if (op == '+')
                    tempResult += num;
                else
                    tempResult *= num;
            }

            // Add the last item
            finalResult += tempResult;

            // Time  : 00:00:00.0093177
            return finalResult.ToString();
        }
    }
}

