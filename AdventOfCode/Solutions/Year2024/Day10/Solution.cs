using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    class Day10 : ASolution
    {
        public int[][] grid;
        public HashSet<Point<int>> trailheads = new();

        public Day10() : base(10, 2024, "Hoof It")
        {
            // DebugInput = @"89010123
            // 78121874
            // 87430965
            // 96549874
            // 45678903
            // 32019012
            // 01329801
            // 10456732";

            grid = Input.ToIntGrid();

            grid.ForEach((line, y) => line.ForEach((c, x) =>
            {
                if (c == 0)
                    trailheads.Add(new(x, y));
            }));
        }

        /// <summary>
        /// Quickly determine if a point is valid inside grid
        /// </summary>
        public bool InGrid(Point<int> pt) => 0 <= pt.x && pt.x < grid[0].Length && 0 <= pt.y && pt.y < grid.Length;

        /// <summary>
        /// Return the valid moves from an existing point, must be uphill with a diff of 1
        /// </summary>
        public IEnumerable<Point<int>> GetMoves(Point<int> pt) {
            foreach (var moves in new Point<int>[] { Point2D.MoveUp, Point2D.MoveRight, Point2D.MoveDown, Point2D.MoveLeft })
            {
                var newPt = pt + moves;
                if (InGrid(newPt) && grid[newPt.y][newPt.x] - grid[pt.y][pt.x] == 1)
                    yield return newPt;
            }
        }

        /// <summary>
        /// Find valid walking path counts from <paramref name="pt"/>
        /// </summary>
        public int GetWalkingPaths(Point<int> pt) {
            // Start at pt, get all possible paths
            // When a path ends in 9, it is valid
            HashSet<Point<int>> validEnds = new();
            Stack<Point<int>> stack = new([pt]);

            while(stack.Count > 0) {
                var pos = stack.Pop();

                // Try each valid move
                foreach(var move in GetMoves(pos))
                {
                    // If the move ends in 9, we are done
                    if (grid[move.y][move.x] == 9)
                        validEnds.Add(move);
                    else
                        // Otherwise add it to the stack
                        stack.Push(move);
                }
            }

            return validEnds.Count;
        }

        protected override string? SolvePartOne()
        {
            // Time: 00:00:00.0236671
            return trailheads.Sum(GetWalkingPaths).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

