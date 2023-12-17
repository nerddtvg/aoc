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
        private int heatLoss;

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

        private void RunGrid(Point start, Point end)
        {
            heatLoss = int.MaxValue;

            // Track if we've been in this position and not direction and heat loss
            // If we have and have more heat loss, then skip it
            // notDir == The only direction we can't go
            // This is because we could get to this position from 3 ways 
            var seen = new Dictionary<Point, int>();

            // Track our queue of work
            var queue = new Queue<(Point pos, Direction dir, int straightCount, int heatLoss)>();

            queue.Enqueue((start, Direction.Right, 0, 0));

            while (queue.Count > 0)
            {
                (var pos, var dir, var tempStraight, var tempHeatLoss) = queue.Dequeue();

                // If we are at the end, skip out
                if (pos == end)
                {
                    heatLoss = Math.Min(tempHeatLoss, heatLoss);
                    continue;
                }

                // This is BFS so we only care that we don't backtrack ever
                if (seen.TryGetValue(pos, out int seenLoss) && seenLoss < tempHeatLoss)
                    continue;

                seen[pos] = tempHeatLoss;

                // Get our next steps
                // Add heat loss in the queue because the start
                // does not incur loss
                var left = (Direction)(((int)dir + 3) % 4);
                var right = (Direction)(((int)dir + 1) % 4);

                var posLeft = pos.Add(deltas[left]);
                var posStraight = pos.Add(deltas[dir]);
                var posRight = pos.Add(deltas[right]);

                if (IsInGrid(posLeft))
                    queue.Enqueue((posLeft, left, 0, tempHeatLoss + grid[pos.y][pos.x]));

                if (IsInGrid(posStraight) && tempStraight < 3)
                    queue.Enqueue((posStraight, dir, tempStraight + 1, tempHeatLoss + grid[pos.y][pos.x]));

                if (IsInGrid(posRight))
                    queue.Enqueue((posRight, right, 0, tempHeatLoss + grid[pos.y][pos.x]));
            }
        }

        private bool IsInGrid(Point point) => 0 <= point.y && point.y <= maxY && 0 <= point.x && point.x <= maxX;

        protected override string? SolvePartOne()
        {
            RunGrid((0, 0), (maxX, maxY));

            return heatLoss.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

