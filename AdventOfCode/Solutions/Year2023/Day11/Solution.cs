using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using AdventOfCode.Solutions.Year2019;


namespace AdventOfCode.Solutions.Year2023
{
    using Point = (int x, int y);

    class Day11 : ASolution
    {
        public string[] Grid;
        public int[] EmptyCols;
        public int[] EmptyRows;
        public Point[] Galaxies;

        public Day11() : base(11, 2023, "Cosmic Expansion")
        {
            // DebugInput = @"...#......
            //                .......#..
            //                #.........
            //                ..........
            //                ......#...
            //                .#........
            //                .........#
            //                ..........
            //                .......#..
            //                #...#.....";

            // Empty rows/columns are double in size
            // Find the manhattan distance between two '#' galaxies
            // Then count how many rows/colums it traversed to
            // get there and increase the count by one for each
            Grid = Input.SplitByNewline(true).ToArray();

            EmptyCols = Enumerable.Range(0, Grid.Length).Select(idx =>
                {
                    if (!Grid.Select(row => row[idx]).All(c => c == '.'))
                        return -1;

                    return idx;
                })
                .Where(idx => idx >= 0)
                .ToArray();

            EmptyRows = Grid.Select((row, idx) =>
                {
                    if (!row.All(c => c == '.'))
                        return -1;

                    return idx;
                })
                .Where(idx => idx >= 0)
                .ToArray();

            Galaxies = Enumerable.Range(0, Grid.Length)
                .SelectMany(y => Grid[y].Select((c, x) => c == '.' ? (-1, -1) : (x, y)))
                .Where(point => point != (-1, -1))
                .ToArray();
        }

        public uint ExpandDistance(Point a, Point b)
        {
            uint ret = a.ManhattanDistance(b);

            // For each col/row that is empty, increase by 1
            int x1 = Math.Min(a.x, b.x);
            int x2 = Math.Max(a.x, b.x);
            int y1 = Math.Min(a.y, b.y);
            int y2 = Math.Max(a.y, b.y);

            ret += (uint)EmptyCols.Count(x => x1 < x && x < x2);
            ret += (uint)EmptyRows.Count(y => y1 < y && y < y2);

            return ret;
        }

        protected override string? SolvePartOne()
        {
            return Galaxies
                .GetAllCombos()
                .Sum(combo => ExpandDistance(combo[0], combo[1]))
                .ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

