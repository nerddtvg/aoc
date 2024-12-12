using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    class Day12 : ASolution
    {
        public struct Region
        {
            public char Id;
            public Point<int>[] points;
            public int borders;
            public int area;
        }

        public HashSet<Point<int>> visited = [];
        public char[][] grid;
        public List<Region> regions = [];

        public Day12() : base(12, 2024, "Garden Groups")
        {
            // DebugInput = @"RRRRIICCFF
            // RRRRIICCCF
            // VVRRRCCFFF
            // VVRCCCJFFF
            // VVVVCJJCFE
            // VVIVCCJJEE
            // VVIIICJJEE
            // MIIIIIJJEE
            // MIIISIJEEE
            // MMMISSJEEE";

            grid = Input.ToCharGrid();
        }

        /// <summary>
        /// Quickly determine if a point is valid inside grid
        /// </summary>
        public bool InGrid(Point<int> pt) => 0 <= pt.x && pt.x < grid[0].Length && 0 <= pt.y && pt.y < grid.Length;

        public void FindRegion(Point<int> start)
        {
            if (visited.Contains(start)) return;

            var regionId = grid[start.y][start.x];
            var region = new List<Point<int>>();
            var borders = 0;
            var stack = new Stack<Point<int>>([start]);

            while (stack.Count > 0)
            {
                var pos = stack.Pop();

                // Don't double-count
                if (visited.Contains(pos)) continue;

                // Add it now
                visited.Add(pos);
                region.Add(pos);

                // Count the borders and add to the list
                foreach (var move in new Point<int>[] { Point2D.MoveUp, Point2D.MoveRight, Point2D.MoveDown, Point2D.MoveLeft })
                {
                    var newPos = pos + move;

                    // Outside the grid? That's a border
                    if (!InGrid(newPos))
                    {
                        borders++;
                        continue;
                    }

                    // Not the same ID? That's a border
                    if (grid[newPos.y][newPos.x] != regionId)
                    {
                        borders++;
                        continue;
                    }

                    // Otherwise add this to the stack and let things recalculate
                    stack.Push(newPos);
                }
            }

            // At the end of a region, we will add it to or list
            regions.Add(new()
            {
                area = region.Count,
                borders = borders,
                Id = regionId,
                points = region.ToArray()
            });
        }

        protected override string? SolvePartOne()
        {
            // Find all regions
            grid.ForEach((line, y) => line.ForEach((c, x) => FindRegion(new(x, y))));

            return regions.Sum(r => r.area * r.borders).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

