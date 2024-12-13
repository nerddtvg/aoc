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
            public int corners;
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

        public bool IsInRegion(Point<int> pt, char regionId) => InGrid(pt) && regionId == grid[pt.y][pt.x];

        public void FindRegion(Point<int> start)
        {
            if (visited.Contains(start)) return;

            var regionId = grid[start.y][start.x];
            var area = 0;
            var borders = 0;
            var corners = 0;
            var stack = new Stack<Point<int>>([start]);

            while (stack.Count > 0)
            {
                var pos = stack.Pop();

                // Don't double-count
                if (visited.Contains(pos)) continue;

                // Add it now
                visited.Add(pos);
                area++;

                // Get all regions surrounding us
                var moves = new Point<int>[]
                {
                    Point2D.MoveUp,
                    Point2D.MoveUpRight,
                    Point2D.MoveRight,
                    Point2D.MoveDownRight,
                    Point2D.MoveDown,
                    Point2D.MoveDownLeft,
                    Point2D.MoveLeft,
                    Point2D.MoveUpLeft,
                }.ToDictionary(dir => dir, dir => (move: pos + dir, inRegion: IsInRegion(pos + dir, regionId)));

                // Helpers:
                // *#*  O*O
                // ###  *#*
                // *#*  O*O  O == doesn't matter the value

                // Counting borders and corners together
                if (moves[Point2D.MoveLeft].inRegion)
                {
                    // Add this to the stack
                    stack.Push(moves[Point2D.MoveLeft].move);

                    // If up is in region, but up left is not => corner
                    if (moves[Point2D.MoveUp].inRegion && !moves[Point2D.MoveUpLeft].inRegion)
                        corners++;

                    // If down is in region, but down left is not => corner
                    if (moves[Point2D.MoveDown].inRegion && !moves[Point2D.MoveDownLeft].inRegion)
                        corners++;
                }
                else
                {
                    // We have a border
                    borders++;

                    // If both up or down are out of region, that's a corner because no diagonals
                    if (!moves[Point2D.MoveUp].inRegion)
                        corners++;

                    if (!moves[Point2D.MoveDown].inRegion)
                        corners++;
                }

                if (moves[Point2D.MoveRight].inRegion)
                {
                    // Add this to the stack
                    stack.Push(moves[Point2D.MoveRight].move);

                    // If up is in region, but up right is not => corner
                    if (moves[Point2D.MoveUp].inRegion && !moves[Point2D.MoveUpRight].inRegion)
                        corners++;

                    // If down is in region, but down right is not => corner
                    if (moves[Point2D.MoveDown].inRegion && !moves[Point2D.MoveDownRight].inRegion)
                        corners++;
                }
                else
                {
                    // We have a border
                    borders++;

                    // If both up or down are out of region, that's a corner because no diagonals
                    if (!moves[Point2D.MoveUp].inRegion)
                        corners++;

                    if (!moves[Point2D.MoveDown].inRegion)
                        corners++;
                }

                // Up/Down is easier
                if (moves[Point2D.MoveUp].inRegion)
                    // Add this to the stack
                    stack.Push(moves[Point2D.MoveUp].move);
                else
                    borders++;

                if (moves[Point2D.MoveDown].inRegion)
                    // Add this to the stack
                    stack.Push(moves[Point2D.MoveDown].move);
                else
                    borders++;
            }

            // At the end of a region, we will add it to or list
            regions.Add(new()
            {
                area = area,
                borders = borders,
                Id = regionId,
                corners = corners
            });
        }

        protected override string? SolvePartOne()
        {
            // Find all regions
            grid.ForEach((line, y) => line.ForEach((c, x) => FindRegion(new(x, y))));

            // Time: 00:00:00.1347395
            // Time with P2: 00:00:00.3729900
            return regions.Sum(r => r.area * r.borders).ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Find the number of sides
            // Which is also the number of "corners"
            // Time: 00:00:00.0003435
            return regions.Sum(r => r.area * r.corners).ToString();
        }
    }
}

