using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Collections;
using System.Runtime.InteropServices;


namespace AdventOfCode.Solutions.Year2023
{
    using Point = (int x, int y);

    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    class Day17 : ASolution
    {
        private int[][] grid;
        private int maxX;
        private int maxY;

        private Dictionary<Direction, Point> deltas = new()
        {
            { Direction.Up, (0, -1) },
            { Direction.Right, (1, 0) },
            { Direction.Down, (0, 1) },
            { Direction.Left, (-1, 0) }
        };

        public Day17() : base(17, 2023, "Clumsy Crucible")
        {
            // DebugInput = @"2413432311323
            //                3215453535623
            //                3255245654254
            //                3446585845452
            //                4546657867536
            //                1438598798454
            //                4457876987766
            //                3637877979653
            //                4654967986887
            //                4564679986453
            //                1224686865563
            //                2546548887735
            //                4322674655533";

            // Load the grid
            grid = Input.SplitByNewline(shouldTrim: true).Select(line => line.Select(c => int.Parse(c.ToString())).ToArray()).ToArray();
            maxY = grid.Length - 1;
            maxX = grid[0].Length - 1;
        }

        private int RunGrid(Point start, Point end)
        {
            // Using a priority queue based on heatLoss ensures
            // our first end will be the lowest hestLoss possible
            // Our seen only needs to know if this has been visited
            // ever in the past
            var seen = new HashSet<Point>();

            // Track our queue of work
            // Priority is heatLoss so the lowest heatLoss is always tried next
            var queue = new PriorityQueue<(Point pos, Direction dir, int straightCount), int>();

            queue.Enqueue((start, Direction.Right, 0), 0);
            queue.Enqueue((start, Direction.Down, 0), 0);

            while (queue.TryDequeue(out var item, out var thisHeatLoss))
            {
                (var thisPos, var thisDir, var thisStraight) = item;

                // If we are at the end, skip out
                if (thisPos == end)
                {
                    return thisHeatLoss;
                }

                // Get our next steps
                // Add heat loss in the queue because the start
                // does not incur loss
                var left = (Direction)(((int)thisDir + 3) % 4);
                var right = (Direction)(((int)thisDir + 1) % 4);

                var newPoints = new (Point newPos, Direction newDir)[] {
                    (thisPos.Add(deltas[left]), left),
                    (thisPos.Add(deltas[thisDir]), thisDir),
                    (thisPos.Add(deltas[right]), right)
                };

                foreach((var newPos, var newDir) in newPoints)
                {
                    // Make sure it is in the grid
                    // Make sure the move is valid
                    // Make sure we add it to the HashSet (this is false if already visited)
                    if (IsInGrid(newPos) && (newDir != thisDir || thisStraight < 3) && seen.Add(newPos))
                    {
                        var newHeatLoss = thisHeatLoss + grid[newPos.y][newPos.x];

                        queue.Enqueue((newPos, newDir, newDir == thisDir ? (thisStraight + 1) : 0), newHeatLoss);
                    }
                }
            }

            return int.MaxValue;
        }

        private bool IsInGrid(Point point) => 0 <= point.y && point.y <= maxY && 0 <= point.x && point.x <= maxX;

        protected override string? SolvePartOne()
        {
            return RunGrid((0, 0), (maxX, maxY)).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

