using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Numerics;


namespace AdventOfCode.Solutions.Year2025
{

    class Day09 : ASolution
    {
        private readonly List<(int a, int b)> points;
        private readonly HashSet<(int a, int b)> insidePoints = [];
        private readonly HashSet<(int a, int b)> outsidePoints = [];

        public Day09() : base(09, 2025, "Movie Theater")
        {
            // DebugInput = @"7,1
            //     11,1
            //     11,7
            //     9,7
            //     9,5
            //     2,5
            //     2,3
            //     7,3";

            points = [.. Input.SplitByNewline(true, true)
                .Select(line =>
                {
                    var t = line.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();
                    return (t[0], t[1]);
                })];
        }

        protected override string? SolvePartOne()
        {
            // Time  : 00:00:00.1020996
            return points
                .GetAllCombos(2)
                .Select(pair => (BigInteger.Abs(pair[0].a - pair[1].a) + 1) * (BigInteger.Abs(pair[0].b - pair[1].b) + 1))
                .Max()
                .ToString();
        }
        
        private bool IsInside(IList<(int x, int y)> rectangle)
        {
            var sx = Math.Min(rectangle[0].x, rectangle[1].x);
            var sy = Math.Min(rectangle[0].y, rectangle[1].y);

            var ex = Math.Max(rectangle[0].x, rectangle[1].x);
            var ey = Math.Max(rectangle[0].y, rectangle[1].y);

            // We must travel clockwise (order of the list) to ensure the math lines up
            for (int i = 0; i < points.Count; i++)
            {
                (var x1, var y1) = points[i];
                (var x2, var y2) = points[(i + 1) % points.Count];

                //  +-----------+
                //  |           |
                //  |           |
                // ---------------
                //  |           |
                //  |           |
                //  +-----------+

                //  +-----|-----+
                //  |     |     |
                //  |     |     |
                //  |     |     |
                //  |     |     |
                //  |     |     |
                //  +-----|-----+

                if (x1 == x2)
                {
                    // Vertical
                    if (sx < Math.Min(x1, x2) && Math.Max(x1, x2) < ex && !(Math.Max(y1, y2) <= sy || ey <= Math.Min(y1, y2)))
                        return false;
                }
                else if (y1 == y2)
                {
                    // Horizontal
                    if (sy < Math.Min(y1, y2) && Math.Max(y1, y2) < ey && !(Math.Max(x1, x2) <= sx || ex <= Math.Min(x1, x2)))
                        return false;
                }
            }

            return true;
        }

        protected override string? SolvePartTwo()
        {
            // This is a classic line segment counting puzzle from AoC
            // We will look at each rectangle and pick the alternating corners
            // Determining if that is "inside" the polygon or not
            // https://en.wikipedia.org/wiki/Even%E2%80%93odd_rule
            // https://en.wikipedia.org/wiki/Nonzero-rule

            // After failing at that math, I went to intersecting rectangles

            // Time  : 00:00:00.5435687
            return points
                .GetAllCombos(2)
                // Take this rectangle and see if it intersects any of the edges of the map
                .Where(IsInside)
                // Then calculate the area
                .Select(pair => (BigInteger.Abs(pair[0].a - pair[1].a) + 1) * (BigInteger.Abs(pair[0].b - pair[1].b) + 1))
                .Max()
                .ToString();
        }
    }
}

