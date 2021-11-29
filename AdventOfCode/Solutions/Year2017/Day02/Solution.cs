using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2017
{

    class Day02 : ASolution
    {

        public Day02() : base(02, 2017, "Corruption Checksum")
        {

        }

        private int DiffRow(IEnumerable<int> values) =>
            values.Max() - values.Min();

        private IEnumerable<int> GetRow(string row) =>
            row.Split('\t').Select(i => Int32.Parse(i)).ToArray();

        private int GetDivisors(IEnumerable<int> values)
        {
            var vals = values.OrderByDescending(v => v).ToList();

            for (int i = 0; i < vals.Count; i++)
            {
                for (int q = i + 1; q < vals.Count; q++)
                {
                    if (vals[i] % vals[q] == 0)
                        return vals[i] / vals[q];
                }
            }

            // Backup:
            return 0;
        }

        protected override string? SolvePartOne()
        {
            return Input.SplitByNewline().Sum(row => DiffRow(GetRow(row))).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return Input.SplitByNewline().Sum(row => GetDivisors(GetRow(row))).ToString();
        }
    }
}

#nullable restore
