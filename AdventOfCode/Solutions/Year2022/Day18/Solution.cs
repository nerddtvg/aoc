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
            DebugInput = @"2,2,2
                1,2,2
                3,2,2
                2,1,2
                2,3,2
                2,2,1
                2,2,3
                2,2,4
                2,2,6
                1,2,5
                3,2,5
                2,1,5
                2,3,5";

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
            // Coordinates:
            var coords = points.Select(p => p.coordinate).ToArray();

            return points.Sum(point =>
            {
                if (point.coordinate[0] == 2 && point.coordinate[1] == 2 && point.coordinate[2] == 4)
                    System.Diagnostics.Debugger.Break();
                // If any of the exterior X, Y, or Z coords are true, then we have an exterior point
                // Assumption: We should be able to go a large range in at least one direction to be an exterior point
                if (
                    (
                        point.exteriorNeighborsX1.All(n => !coords.Contains(n))
                        ^
                        point.exteriorNeighborsX2.All(n => !coords.Contains(n))
                    )
                    ||
                    (
                        point.exteriorNeighborsY1.All(n => !coords.Contains(n))
                        ^
                        point.exteriorNeighborsY2.All(n => !coords.Contains(n))
                    )
                    ||
                    (
                        point.exteriorNeighborsZ1.All(n => !coords.Contains(n))
                        ^
                        point.exteriorNeighborsZ2.All(n => !coords.Contains(n))
                    )
                )
                    // On this point, count how many of the neighbors don't exist in our list
                    return point.neighbors.Count(n => !coords.Contains(n));
                else
                    return 0;
            }).ToString();
        }

        public struct DropletEdge
        {
            public Point<int> coordinate;

            public Point<int>[] neighbors;

            public Point<int>[] exteriorNeighborsX1;
            public Point<int>[] exteriorNeighborsX2;
            public Point<int>[] exteriorNeighborsY1;
            public Point<int>[] exteriorNeighborsY2;
            public Point<int>[] exteriorNeighborsZ1;
            public Point<int>[] exteriorNeighborsZ2;

            public DropletEdge(Point<int> coordinate)
            {
                this.coordinate = coordinate;

                var shifts = new Point<int>[]
                {
                    new(-1, 0, 0),
                    new(1, 0, 0),
                    new(0, -1, 0),
                    new(0, 1, 0),
                    new(0, 0, -1),
                    new(0, 0, 1)
                };

                this.neighbors = shifts.Select(shift => coordinate + shift).ToArray();

                var exteriorRange = 20;

                this.exteriorNeighborsX1 = Enumerable.Range(1, exteriorRange).SelectMany(extOffset => new Point<int>[]
                {
                    new(-1, 0, 0)
                }.Select(s => s * extOffset)).Select(shift => coordinate + shift).ToArray();

                this.exteriorNeighborsX2 = Enumerable.Range(1, exteriorRange).SelectMany(extOffset => new Point<int>[]
                {
                    new(1, 0, 0)
                }.Select(s => s * extOffset)).Select(shift => coordinate + shift).ToArray();

                this.exteriorNeighborsY1 = Enumerable.Range(1, exteriorRange).SelectMany(extOffset => new Point<int>[]
                {
                    new(0, -1, 0)
                }.Select(s => s * extOffset)).Select(shift => coordinate + shift).ToArray();

                this.exteriorNeighborsY2 = Enumerable.Range(1, exteriorRange).SelectMany(extOffset => new Point<int>[]
                {
                    new(0, 1, 0)
                }.Select(s => s * extOffset)).Select(shift => coordinate + shift).ToArray();

                this.exteriorNeighborsZ1 = Enumerable.Range(1, exteriorRange).SelectMany(extOffset => new Point<int>[]
                {
                    new(0, 0, -1)
                }.Select(s => s * extOffset)).Select(shift => coordinate + shift).ToArray();

                this.exteriorNeighborsZ2 = Enumerable.Range(1, exteriorRange).SelectMany(extOffset => new Point<int>[]
                {
                    new(0, 0, 1)
                }.Select(s => s * extOffset)).Select(shift => coordinate + shift).ToArray();
            }
        }
    }
}

