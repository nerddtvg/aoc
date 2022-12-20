using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2022
{

    class Day18 : ASolution
    {
        private DropletEdge[] points;

        public Day18() : base(18, 2022, "Boiling Boulders")
        {
            // DebugInput = @"2,2,2
            //     1,2,2
            //     3,2,2
            //     2,1,2
            //     2,3,2
            //     2,2,1
            //     2,2,3
            //     2,2,4
            //     2,2,6
            //     1,2,5
            //     3,2,5
            //     2,1,5
            //     2,3,5";

            points = Input.SplitByNewline(true)
                .Select(line => new DropletEdge(new Point<int>(line.Split(",").Select(xyz => Int32.Parse(xyz)).ToArray())))
                .ToArray();
        }

        protected override string? SolvePartOne()
        {
            // Coordinates:
            var coords = points.Select(p => p.coordinate).ToArray();

            return points.Sum(point =>
            {
                // On this point, count how many of the neighbors don't exist in our list
                return point.neighbors.Count(n => !coords.Contains(n));
            }).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }

        public struct DropletEdge
        {
            public Point<int> coordinate;

            public Point<int>[] neighbors;

            public DropletEdge(Point<int> coordinate)
            {
                this.coordinate = coordinate;

                this.neighbors = new Point<int>[]
                {
                    new(-1, 0, 0),
                    new(1, 0, 0),
                    new(0, -1, 0),
                    new(0, 1, 0),
                    new(0, 0, -1),
                    new(0, 0, 1),
                }.Select(shift => coordinate + shift).ToArray();
            }
        }
    }
}

