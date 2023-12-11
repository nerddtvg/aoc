using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using AdventOfCode.Solutions.Year2019;
using System.Numerics;


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

        public BigInteger ExpandDistance(Point a, Point b, BigInteger muli)
        {
            var ret = new BigInteger(a.ManhattanDistance(b));

            // For each col/row that is empty, increase by muli times
            // Part 2 makes this 1,000,000
            int x1 = Math.Min(a.x, b.x);
            int x2 = Math.Max(a.x, b.x);
            int y1 = Math.Min(a.y, b.y);
            int y2 = Math.Max(a.y, b.y);

            ret += muli * new BigInteger(EmptyCols.Count(x => x1 < x && x < x2));
            ret += muli * new BigInteger(EmptyRows.Count(y => y1 < y && y < y2));

            return ret;
        }

        protected override string? SolvePartOne()
        {
            BigInteger ret = 0;

            Galaxies
                .GetAllCombos()
                .ForEach(combo => ret += ExpandDistance(combo[0], combo[1], BigInteger.One));

            return ret.ToString();
        }

        protected override string? SolvePartTwo()
        {
            BigInteger ret = 0;

            Galaxies
                .GetAllCombos()
                .ForEach(combo => ret += ExpandDistance(combo[0], combo[1], new BigInteger(999999)));

            return ret.ToString();
        }
    }
}

