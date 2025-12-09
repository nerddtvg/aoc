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

            DebugInput = @"3,2
17,2
17,13
13,13
13,11
15,11
15,8
11,8
11,15
18,15
18,17
4,17
4,12
6,12
6,5
3,5";

            points = [.. Input.SplitByNewline(true, true)
                .Select(line =>
                {
                    var t = line.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();
                    return (t[0], t[1]);
                })];
        }

        protected override string? SolvePartOne()
        {
            // Time  : 00:00:00.5179150
            return points
                .GetAllCombos(2)
                .Select(pair => (BigInteger.Abs(pair[0].a - pair[1].a) + 1) * (BigInteger.Abs(pair[0].b - pair[1].b) + 1))
                .Max()
                .ToString();
        }
        
        private bool IsInside((int x, int y) pt)
        {
            var upScore = 0;
            var leftScore = 0;

            // Added caching, no idea if this helps
            if (insidePoints.Contains(pt))
                return true;

            if (outsidePoints.Contains(pt))
                return false;

            (var x, var y) = pt;

            if (pt == (11, 15))
                System.Diagnostics.Debugger.Break();

            // We must travel clockwise (order of the list) to ensure the math lines up
            for (int i = 0; i < points.Count; i++)
            {
                (var x1, var y1) = points[i];
                (var x2, var y2) = points[(i + 1) % points.Count];

                // We are dealing with horizontal or vertical lines so this makes the math easier

                // By traveling "up" to zero y or "left" to zero x
                // If we count the number of lines we pass, we can tell if we are inside or not
                // We *MUST* only cross one line to be fully inside
                // Crossing zero lines: Outside
                // Crossing more than one line: Outside or inside but non-contiguous

                if (x1 == x2)
                {
                    // We have a vertical line
                    // We check that we "pass" through this line by bounding to the Y-coords
                    // And we do not check <= right Y coord because if two vertical lines
                    // start and stop on that same Y, it will count twice
                    if (y1 < y2 && y1 <= y && y < y2 && x1 < x)
                        // LTR
                        leftScore++;

                    if (y2 < y1 && y2 <= y && y < y1 && x1 < x)
                        // RTL
                        leftScore--;
                }
                else
                {
                    // We have a horizontal line
                    // We check that we "pass" through this line by bounding to the X-coords
                    // And we do not check <= right X coord because if two horizontal lines
                    // start and stop on that same X, it will count twice
                    if (x1 < x2 && x1 <= x && x < x2 && y1 < y)
                        // LTR
                        upScore++;

                    if (x2 < x1 && x2 <= x && x < x1 && y1 < y)
                        // RTL
                        upScore--;
                }
            }

            if (leftScore == 1 && upScore == 1)
            {
                insidePoints.Add(pt);
                return true;
            }

            outsidePoints.Add(pt);
            return false;
        }

        protected override string? SolvePartTwo()
        {
            // This is a classic line segment counting puzzle from AoC
            // We will look at each rectangle and pick the alternating corners
            // Determining if that is "inside" the polygon or not
            // https://en.wikipedia.org/wiki/Even%E2%80%93odd_rule
            // https://en.wikipedia.org/wiki/Nonzero-rule

            return points
                .GetAllCombos(2)
                // Pick the midpoint and see if that is inside or not
                .Where(pair => {
                    // Get the bounding boxes by swapping X/Y in each
                    return IsInside(pair[0]) && IsInside(pair[1]) && IsInside((pair[0].a, pair[1].b)) && IsInside((pair[1].a, pair[0].b));
                })
                // Then calculate the area
                .Select(pair => {
                    var a = (BigInteger.Abs(pair[0].a - pair[1].a) + 1) * (BigInteger.Abs(pair[0].b - pair[1].b) + 1);

                    if (a == 4628638200)
                    {
                        Console.WriteLine($"({pair[0].a}, {pair[0].b}) - ({pair[1].a}, {pair[1].b})");
                    }

                    Console.WriteLine($"({pair[0].a}, {pair[0].b}) - ({pair[1].a}, {pair[1].b}) - {a}");

                    return a;
                })
                .Max()
                .ToString();

            return string.Empty;
        }
    }
}

