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
            // Coordinates:
            var coords = points.Select(p => p.coordinate).ToArray();
            var exteriors = new HashSet<Point<int>>();

            // Look from top, left, right, and bottom
            // What are the first coordinates we get to in each direction?
            // Those are exterior edges
            // var minX = coords.Min(c => c[0])-1;
            // var maxX = coords.Max(c => c[0])+1;
            // var minY = coords.Min(c => c[1])-1;
            // var maxY = coords.Max(c => c[1])+1;
            // var minZ = coords.Min(c => c[2])-1;
            // var maxZ = coords.Max(c => c[2])+1;

            // for (int z = minZ; z <= maxZ; z++)
            // {
            //     Console.WriteLine($"z = {z}");
            //     for (int y = minY; y <= maxY; y++)
            //     {
            //         for (int x = minX; x <= maxX; x++)
            //         {
            //             if (coords.Contains(new Point<int>(x, y, z)))
            //                 Console.Write('#');
            //             else
            //                 Console.Write('.');
            //         }
            //         Console.WriteLine();
            //     }
            //     Console.WriteLine();
            // }

            // ASSUMPTION: No exterior cube has an interior edge exposed

            // Top coords:
            coords
                // Group by x,y
                .GroupBy(coor => (x: coor[0], y: coor[1]))
                // Find the lowest value z in each x,y column
                .Select(grp => (grp.Key, grp.Select(coor => coor[2]).Min()))
                // Convert to Point<int>
                .Select(grp => new Point<int>(grp.Key.x, grp.Key.y, grp.Item2))
                .ToList()
                .ForEach(pt => exteriors.Add(pt));

            // Bottom coords:
            coords
                // Group by x,y
                .GroupBy(coor => (x: coor[0], y: coor[1]))
                // Find the lowest value z in each x,y column
                .Select(grp => (grp.Key, grp.Select(coor => coor[2]).Max()))
                // Convert to Point<int>
                .Select(grp => new Point<int>(grp.Key.x, grp.Key.y, grp.Item2))
                .ToList()
                .ForEach(pt => exteriors.Add(pt));

            // Left coords:
            coords
                // Group by z,y
                .GroupBy(coor => (z: coor[2], y: coor[1]))
                // Find the lowest value z in each x,y column
                .Select(grp => (grp.Key, grp.Select(coor => coor[0]).Min()))
                // Convert to Point<int>
                .Select(grp => new Point<int>(grp.Key.Item2, grp.Key.y, grp.Key.z))
                .ToList()
                .ForEach(pt => exteriors.Add(pt));

            // Right coords:
            coords
                // Group by z,y
                .GroupBy(coor => (z: coor[2], y: coor[1]))
                // Find the lowest value z in each x,y column
                .Select(grp => (grp.Key, grp.Select(coor => coor[0]).Max()))
                // Convert to Point<int>
                .Select(grp => new Point<int>(grp.Key.Item2, grp.Key.y, grp.Key.z))
                .ToList()
                .ForEach(pt => exteriors.Add(pt));

            // Count only exteriors
            return points
                .Where(point => exteriors.Contains(point.coordinate))
                .Sum(point =>
                {
                    // On this point, count how many of the neighbors don't exist in our list
                    return point.neighbors.Count(n => !coords.Contains(n));
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

                var exteriorRange = 5;

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

