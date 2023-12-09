using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Globalization;
using System.Threading;


namespace AdventOfCode.Solutions.Year2023
{

    class Day09 : ASolution
    {

        public Day09() : base(09, 2023, "Mirage Maintenance")
        {

        }

        private long FindExtrapolatedValue(string line, int part = 1)
        {
            var digits = line.Split(' ').Select(long.Parse).ToList();

            if (digits.Count == 0) return 0;

            // Start our rows of differences
            var rows = new List<List<long>>() { digits };

            // If the row is not all zeros, keep going
            while (rows[^1].Any(d => d != 0))
            {
                var thisRow = new List<long>();
                var lastRow = rows[^1];

                for(int i=0; i<lastRow.Count-1; i++)
                    thisRow.Add(lastRow[i + 1] - lastRow[i]);

                if (thisRow.Count == 0)
                    thisRow.Add(0);

                rows.Add(thisRow);
            }

            // Our last row is all zeros now
            // Go through and add a new value to each line
            // Part 1: Add to the end of the line
            // Part 2: Add to the beginning of the line
            for (int q = rows.Count - 2; 0 <= q; q--)
            {
                if (part == 2)
                {
                    rows[q].Insert(0, rows[q][0] - rows[q + 1][0]);
                }
                else
                {
                    rows[q].Add(rows[q][^1] + rows[q + 1][^1]);
                }
            }

            // return the last value in row[0]
            if (part == 2)
                return rows[0][0];

            return rows[0][^1];
        }

        protected override string? SolvePartOne()
        {
            return Input.SplitByNewline().Sum(line => FindExtrapolatedValue(line)).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return Input.SplitByNewline().Sum(line => FindExtrapolatedValue(line, 2)).ToString();
        }
    }
}

