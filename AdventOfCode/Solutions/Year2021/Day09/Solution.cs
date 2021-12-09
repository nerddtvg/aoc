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
            // DebugInput = "2199943210\n3987894921\n9856789892\n8767896789\n9899965678";

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

        private void FindBasin((int x, int y) pt, ref List<(int x, int y)> found)
        {
            if (found.Contains(pt))
                return;

            // If this point is a 9 or out of bounds, skip it
            if (GetValue(pt.x, pt.y) >= 9)
                return;

            // Add this point
            found.Add(pt);

            // Look around...
            FindBasin((pt.x - 1, pt.y), ref found);
            FindBasin((pt.x + 1, pt.y), ref found);
            FindBasin((pt.x, pt.y - 1), ref found);
            FindBasin((pt.x, pt.y + 1), ref found);
        }

        protected override string? SolvePartTwo()
        {
            // Basins
            var basins = this.lowestPoints.Select(pt =>
            {
                var lst = new List<(int x, int y)>();
                FindBasin(pt, ref lst);
                return lst;
            }).ToList();

            return basins.Select(lst => lst.Count).OrderByDescending(lst => lst).Take(3).Aggregate((x, y) => x * y).ToString();
        }
    }
}

#nullable restore
