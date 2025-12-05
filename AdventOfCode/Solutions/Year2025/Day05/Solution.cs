using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2025
{

    class Day05 : ASolution
    {
        private readonly List<(ulong a, ulong b)> ranges = [];
        private ulong[] ingredients;

        public Day05() : base(05, 2025, "Cafeteria")
        {
            // DebugInput = @"3-5
            //     10-14
            //     16-20
            //     12-18

            //     1
            //     5
            //     8
            //     11
            //     17
            //     32";

            var split = Input.SplitByBlankLine(true);

            (ulong a, ulong b)[] tRanges = [.. split[0].Select(line => { var t = line.Split('-'); return (a: ulong.Parse(t[0]), b: ulong.Parse(t[1])); }).OrderBy(t => t.a)];
            ingredients = [.. split[1].Select(ulong.Parse)];

            // For Part 2, we explicitly need to reduce the ranges
            // This has no impact on Part 1 so we can do it here
            (var rangeStart, var rangeEnd) = tRanges[0];

            for (int i = 1; i < tRanges.Length; i++)
            {
                (var tRangeStart, var tRangeEnd) = tRanges[i];

                // If we are extending the range, identify the new end
                if (rangeStart <= tRangeStart && tRangeStart <= rangeEnd)
                {
                    rangeEnd = Math.Max(rangeEnd, tRangeEnd);

                    // Only skip if this is not the last one
                    if (i < tRanges.Length - 1)
                        continue;
                }

                // If this is out of range, then we save the old one and start new
                ranges.Add((rangeStart, rangeEnd));

                rangeStart = tRangeStart;
                rangeEnd = tRangeEnd;
            }
        }

        private bool IsFresh(ulong ingredient)
        {
            return ranges.Any(range => range.a <= ingredient && ingredient <= range.b);
        }

        protected override string? SolvePartOne()
        {
            // Time  : 00:00:00.0041733
            return ingredients.Count(IsFresh).ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Time  : 00:00:00.0124796
            return ranges.SumBigInteger(itm => itm.b - itm.a + 1).ToString();
        }
    }
}

