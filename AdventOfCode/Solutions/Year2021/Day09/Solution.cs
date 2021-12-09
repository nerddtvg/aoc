using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2021
{

    class Day09 : ASolution
    {
        private int[][] grid;

        public Day09() : base(09, 2021, "")
        {
            grid = Input.SplitByNewline().Select(line => line.ToIntArray().ToArray()).ToArray();
        }

        private int GetValue(int x, int y)
        {
            if (x < 0 || y < 0)
                return Int32.MaxValue;

            if (x >= this.grid[0].Length || y >= this.grid.Length)
                return Int32.MaxValue;

            return this.grid[y][x];
        }

        private bool IsLowest(int x, int y)
        {
            // Get the locations nearby
            return GetValue(x, y) < Math.Min(GetValue(x - 1, y), Math.Min(GetValue(x + 1, y), Math.Min(GetValue(x, y - 1), GetValue(x, y + 1))));
        }

        protected override string? SolvePartOne()
        {
            int sum = Enumerable.Range(0, this.grid.Length).Sum(y =>
            {
                return Enumerable.Range(0, this.grid[y].Length).Where(x => IsLowest(x, y)).Sum(x => GetValue(x, y) + 1);
            });

            return sum.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
