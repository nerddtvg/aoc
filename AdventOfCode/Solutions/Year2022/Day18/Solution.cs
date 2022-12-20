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

            // No shortcuts about this one, just need to graph it out I guess
            // Based on to assist with the graph search: https://old.reddit.com/r/adventofcode/comments/zoqhvy/2022_day_18_solutions/j0vh5gd/
            var minX = coords.Min(c => c[0])-1;
            var maxX = coords.Max(c => c[0])+1;
            var minY = coords.Min(c => c[1])-1;
            var maxY = coords.Max(c => c[1])+1;
            var minZ = coords.Min(c => c[2])-1;
            var maxZ = coords.Max(c => c[2])+1;

            var queue = new Queue<Point<int>>();
            var exteriors = new HashSet<Point<int>>();

            // Start from a known point on the outside
            queue.Enqueue(new Point<int>(minX, minY, minZ));

            // For each point on the outside, count how many edges we bump up against
            int count = 0;
            while(queue.Count > 0)
            {
                var point = queue.Dequeue();
                if (!exteriors.Contains(point))
                {
                    // Haven't been here yet, check it out
                    exteriors.Add(point);

                    foreach(var neighbor in GetNeighbors(point).ToArray())
                    {
                        // If this is out of bounds, skip
                        if (neighbor[0] < minX || maxX < neighbor[0] || neighbor[1] < minY || maxY < neighbor[1] || neighbor[2] < minZ || maxZ < neighbor[2])
                            continue;

                        // If we have been here (perhaps an earlier iteration, skip it)
                        if (exteriors.Contains(neighbor))
                            continue;

                        // Check against our known droplet datapoints
                        if (coords.Contains(neighbor))
                        {
                            // Found an edge to a neighbor on the outside of the droplet
                            count++;
                        }
                        else
                        {
                            // Still outside, add to the queue
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }

            return count.ToString();
        }

        public static IEnumerable<Point<int>> GetNeighbors(Point<int> point)
        {
            return new Point<int>[]
                {
                    new(-1, 0, 0),
                    new(1, 0, 0),
                    new(0, -1, 0),
                    new(0, 1, 0),
                    new(0, 0, -1),
                    new(0, 0, 1)
                }.Select(shift => point + shift);
        }

        public struct DropletEdge
        {
            public Point<int> coordinate;

            public Point<int>[] neighbors;

            public DropletEdge(Point<int> coordinate)
            {
                this.coordinate = coordinate;

                this.neighbors = Day18.GetNeighbors(coordinate).ToArray();
            }
        }
    }
}

