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
            groups = Input.SplitByBlankLine(true);
        }

        private int GetMirrorPosition(string[] rows)
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
                if (rows[i] == rows[q])
                {
                    // Keep track if things have matched
                    bool matched = true;

                    for (int i2 = i - 1, q2 = q + 1; matched && 0 <= i2 && q2 < rows.Length; i2--, q2++)
                        if (rows[i2] != rows[q2])
                            matched = false;

                    // Return the 1-indexed position
                    // Not starting from zero
                    if (matched) return i + 1;
                }
            }

            return ret;
        }

        private string[] RowsToColums(string[] group)
        {
            return Enumerable.Range(0, group[0].Length)
                .Select(idx => group.Select(line => line[idx]).JoinAsString())
                .ToArray();
        }

        private int FindMirror(string[] group)
        {
            // Provided a group of rows, find the mirror position
            var cols = RowsToColums(group);

            var mirror = GetMirrorPosition(cols);

            if (mirror > 0)
                return mirror;

            mirror = GetMirrorPosition(group);

            if (mirror == 0)
                throw new Exception();

            return 100 * mirror;
        }

        protected override string? SolvePartOne()
        {
            return groups.Sum(FindMirror).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

