using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Reflection;


namespace AdventOfCode.Solutions.Year2023
{

    class Day13 : ASolution
    {
        private string[][] groups;

        public Day13() : base(13, 2023, "Point of Incidence")
        {
            // For Part 2, make these 0/1 strings so we can convert to numbers quickly
            groups = Input.SplitByBlankLine(true)
                .Select(group => group.Select(line => line.Replace('.', '0').Replace('#', '1')).ToArray())
                .ToArray();
        }

        private int GetMirrorPosition(uint[] rows, int part)
        {
            int ret = 0;

            // Work down each row provided
            // Look at the current row and next row
            // If they match:
            // * Work backwards and check if those rows match
            // * Until reaching the end of the rows list in either direction
            // * If the end is reached and rows matched, return the original row index
            for (int i = 0, q = 1; i < rows.Length-1; i++, q++)
            {
                // If rows[i] and rows[q] match, then we need to work backwards
                if (rows[i] == rows[q] || (part == 2 && Part2Match(rows[i], rows[q])))
                {
                    // Keep track if things have matched
                    bool matched = true;

                    // Need to track if we have used the "smudge"
                    // For part 1, we keep this true to prevent issues later
                    var usedPart2 = part == 1 || rows[i] != rows[q];

                    for (int i2 = i - 1, q2 = q + 1; matched && 0 <= i2 && q2 < rows.Length; i2--, q2++)
                    {
                        if (rows[i2] != rows[q2])
                        {
                            if (!usedPart2 && Part2Match(rows[i2], rows[q2]))
                            {
                                usedPart2 = true;
                                continue;
                            }

                            matched = false;
                        }
                    }

                    // Return the 1-indexed position
                    // Not starting from zero
                    if (matched) return i + 1;
                }
            }

            return ret;
        }

        private bool Part2Match(uint a, uint b)
        {
            // Allow for one bit to be different
            return System.Numerics.BitOperations.PopCount(a ^ b) == 1;
        }

        private string[] RowsToColums(string[] group)
        {
            return Enumerable.Range(0, group[0].Length)
                .Select(idx => group.Select(line => line[idx]).JoinAsString())
                .ToArray();
        }

        private uint[] StringsToInts(string[] group)
        {
            // Convert from base2 to decimal integers
            return group.Select(line => Convert.ToUInt32(line, 2)).ToArray();
        }

        private int FindMirror(string[] group, int part)
        {
            // Provided a group of rows, find the mirror position
            var cols = StringsToInts(RowsToColums(group));

            var mirror = GetMirrorPosition(cols, part);

            if (mirror > 0)
                return mirror;

            mirror = GetMirrorPosition(StringsToInts(group), part);

            if (mirror == 0)
                throw new Exception();

            return 100 * mirror;
        }

        protected override string? SolvePartOne()
        {
            return groups.Sum(group => FindMirror(group, 1)).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return groups.Sum(group => FindMirror(group, 2)).ToString();
        }
    }
}

