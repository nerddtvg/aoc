using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Numerics;


namespace AdventOfCode.Solutions.Year2023
{
    using Point = (long x, long y);

    class Day18 : ASolution
    {
        private List<(char dir, int length)> instructions;
        private List<(char dir, int length)> instructions2;

        private readonly Dictionary<char, Point> deltas = new()
        {
            { 'U', (0, 1) },
            { 'R', (1, 0) },
            { 'D', (0, -1) },
            { 'L', (-1, 0) },
        };

        public Day18() : base(18, 2023, "Lavaduct Lagoon")
        {
            // DebugInput = @"R 6 (#70c710)
            //                D 5 (#0dc571)
            //                L 2 (#5713f0)
            //                D 2 (#d2c081)
            //                R 2 (#59c680)
            //                D 2 (#411b91)
            //                L 5 (#8ceee2)
            //                U 2 (#caa173)
            //                L 1 (#1b58a2)
            //                U 2 (#caa171)
            //                R 2 (#7807d2)
            //                U 3 (#a77fa3)
            //                L 2 (#015232)
            //                U 2 (#7a21e3)";

            instructions = Input.SplitByNewline(shouldTrim: true)
                .Select(line =>
                {
                    var split = line.Split(' ', 3);

                    return (split[0][0], int.Parse(split[1]));
                })
                .ToList();

            // Part 2 is a different parse
            instructions2 = Input.SplitByNewline(shouldTrim: true)
                .Select(line =>
                {
                    var split = line.Split(' ', 3);
                    var rgb = split[2][2..^1];
                    var dir = 'U';

                    switch (rgb[^1])
                    {
                        case '0':
                            dir = 'R';
                            break;

                        case '1':
                            dir = 'D';
                            break;

                        case '2':
                            dir = 'L';
                            break;

                        case '3':
                            dir = 'U';
                            break;
                    }

                    return (dir, Convert.ToInt32(rgb[..^1], 16));
                })
                .ToList();
        }

        protected override string? SolvePartOne()
        {
            // https://www.mathopenref.com/coordpolygonarea2.html
            // 2D Polygon math
            // How does it work?
            // Magic.

            // The points are going clockwise so this works as-is

            var points = GetPoints(instructions);

            return GetArea(points).ToString();
        }

        private List<Point> GetPoints(List<(char dir, int length)> instructions)
        {
            var points = new List<Point>() { (0, 0) };

            foreach (var instruction in instructions)
            {
                var lastPoint = points[points.Count - 1];
                var delta = deltas[instruction.dir];

                points.Add(lastPoint.Add(deltas[instruction.dir], instruction.length));
            }

            return points;
        }

        private BigInteger GetArea(List<Point> points)
        {
            // Requires: Y positive is up
            // https://jeremytammik.github.io/tbc/a/0053_2d_polygon_area_outer_loop.htm
            // https://www.mathopenref.com/coordpolygonarea2.html
            // 2D Polygon math
            // How does it work?
            // Magic.

            // Remove the final point, if it is a duplicate
            if (points[0] == points[^1])
                points.RemoveAt(points.Count - 1);

            // We now have a list of points
            BigInteger area = 0;
            BigInteger lineLength = 0;
            for (int i = 0, j = points.Count - 1; i < points.Count; i++)
            {
                area += (points[j].x + points[i].x) * (points[j].y - points[i].y);

                lineLength += points[j].ManhattanDistance(points[i]);

                j = i;
            }

            // If the points were counter-clockwise, the answer is negative
            area = BigInteger.Abs(area);

            area /= 2;

            // That was just the internal area, we need to count our line lengths as well
            // This also needs to be divided by 2
            // We'll be off by one at the end because we didn't count the starting point twice
            // which will be chopped off in integer division
            area += (lineLength / 2) + 1;

            return area;
        }

        protected override string? SolvePartTwo()
        {
            // Part 2 required:
            // * Implementing polygon math
            // * Changing from int to long
            // * Removing old fill/draw methods

            var points = GetPoints(instructions2);

            return GetArea(points).ToString();
        }
    }
}

