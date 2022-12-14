using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2022
{

    class Day14 : ASolution
    {
        char[][] grid = Array.Empty<char[]>();
        (int x, int y) start = (500, 0);

        public Day14() : base(14, 2022, "Regolith Reservoir")
        {
            // Initialize a blank grid of dots
            grid = Enumerable.Range(0, 1000)
                .Select(y => Enumerable.Range(0, 1000).Select(x => '.').ToArray())
                .ToArray();

            // The start is at 500, 0
            grid[start.y][start.x] = '+';

            // Draw the grid
            foreach(var line in Input.SplitByNewline())
            {
                // Gather all of our line segments
                var points = line
                    .Split(" -> ", StringSplitOptions.TrimEntries)
                    .Select(pt =>
                    {
                        var s = pt.Split(',');
                        return (Int32.Parse(s[0]), Int32.Parse(s[1]));
                    })
                    .ToArray();

                for (int i = 0; i < points.Length - 2; i++)
                {
                    // Draw from points[i] to points[i+1]
                    points[i].GetPointsBetweenInclusive(points[i + 1]).ToList()
                        .ForEach(pt => grid[pt.Item2][pt.Item1] = '#');
                }
            }
        }

        protected override string? SolvePartOne()
        {
            return string.Empty;
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

