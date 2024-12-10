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

        public int validEnds = 0;
        public int validPaths = 0;

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
        public (int validEnds, int validPaths) GetWalkingPaths(Point<int> pt) {
            // Start at pt, get all possible paths
            // When a path ends in 9, it is valid

            // Part 1 is the count of valid endings (9)
            HashSet<Point<int>> validEnds = new();

            // Part 2 is the count of all possible paths
            // including duplicate endings
            int validPaths = 0;

            Stack<Point<int>> stack = new([pt]);

            while(stack.Count > 0) {
                var pos = stack.Pop();

                // Try each valid move
                foreach(var move in GetMoves(pos))
                {
                    // If the move ends in 9, we are done
                    if (grid[move.y][move.x] == 9)
                    {
                        validEnds.Add(move);
                        validPaths++;
                    }
                    else
                        // Otherwise add it to the stack
                        stack.Push(move);
                }
            }

            return (validEnds.Count, validPaths);
        }

        protected override string? SolvePartOne()
        {
            // Time: 00:00:00.0236671
            // Time with P2: 00:00:00.0222023
            trailheads.ForEach(pt =>
            {
                var (tValidEnds, tValidPaths) = GetWalkingPaths(pt);

                validEnds += tValidEnds;
                validPaths += tValidPaths;
            });

            return validEnds.ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Time: No additional time
            return validPaths.ToString();
        }
    }
}

