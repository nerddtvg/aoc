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
        private List<(int x, int y)> lowestPoints = new List<(int x, int y)>();

        public Day09() : base(09, 2021, "")
        {
            grid = Input.SplitByNewline().Select(line => line.ToIntArray().ToArray()).ToArray();

            this.lowestPoints = Enumerable.Range(0, this.grid.Length).SelectMany(y =>
            {
                return Enumerable.Range(0, this.grid[y].Length).Where(x => IsLowest(x, y)).Select(x => (x, y));
            }).ToList();
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
            return this.lowestPoints.Sum(pt => GetValue(pt.x, pt.y) + 1).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
