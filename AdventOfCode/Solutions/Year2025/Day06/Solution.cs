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

            // Pad the last line with spaces
            var maxLength = lines.Max(line => line.Length);
            lines[^1] = lines[^1].PadRight(maxLength, ' ');

            // Go through the bottom line to figure out each problem's "width"
            var widths = new List<int>();
            int w = 1;

            for (int i = 1; i < lines[^1].Length; i++)
            {
                if (lines[^1][i] != ' ')
                {
                    widths.Add(w - 1);
                    w = 1;
                }
                else
                {
                    w++;
                }
            }

            // Add the last one (this does not include an extra space)
            widths.Add(w);

            // Track offset so we know where to start our string selection
            var finalResult = BigInteger.Zero;
            var offset = 0;

            for(int idx=0; idx<widths.Count; idx++)
            {
                var op = lines[^1][offset];
                BigInteger result = op == '+' ? BigInteger.Zero : BigInteger.One;

                // For each operation:
                // Build the numbers
                // Do the operation
                // Add to the finalResult
                for (int i = widths[idx]-1; i >= 0; i--)
                {
                    var num = BigInteger.Parse(lines.SkipLast(1).Select(line => line[offset + i]).JoinAsString().Trim());

                    if (op == '+')
                        result += num;
                    else
                        result *= num;
                }

                // Move the offset
                offset += widths[idx] + 1;

                finalResult += result;
            }

            return finalResult.ToString();
        }
    }
}

