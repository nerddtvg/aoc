using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2021
{

    class Day05 : ASolution
    {
        public List<(int x, int y)[]> straightLines = new List<(int x, int y)[]>();
        public List<(int x, int y)[]> diagonalLines = new List<(int x, int y)[]>();

        public Day05() : base(05, 2021, "")
        {
            foreach(var line in Input.SplitByNewline())
            {
                var points = line
                    // Get each endpoint
                    .Split(" -> ", StringSplitOptions.TrimEntries)
                    .Select(point => 
                    {
                        // Split them out
                        var pts = point.Split(',', StringSplitOptions.TrimEntries);
                        // Return them as (x, y)
                        return (Int32.Parse(pts[0]), Int32.Parse(pts[1]));
                    })
                    .ToArray();

                if (points.Length != 2)
                    throw new InvalidOperationException();

                // No diagonal lines (yet?)
                if (!(points[0].Item1 != points[1].Item1 && points[0].Item2 != points[1].Item2))
                    this.straightLines.Add(points[0].GetPointsBetweenInclusive(points[1], true));
                else
                    this.diagonalLines.Add(points[0].GetPointsBetweenInclusive(points[1], true));
            }
        }

        protected override string? SolvePartOne()
        {
            return this.straightLines
                // Select all of the points of these lines together
                .SelectMany(line => line)
                // Group by the number of points we have together
                .GroupBy(point => point)
                // Only keep the points that overlap
                .Where(grp => grp.Count() > 1)
                // Count and return
                .Count().ToString();
        }

        protected override string? SolvePartTwo()
        {
            // All the lines!
            return this.diagonalLines.Union(this.straightLines)
                // Select all of the points of these lines together
                .SelectMany(line => line)
                // Group by the number of points we have together
                .GroupBy(point => point)
                // Only keep the points that overlap
                .Where(grp => grp.Count() > 1)
                // Count and return
                .Count().ToString();
        }
    }
}

#nullable restore
