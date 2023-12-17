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

            // BFS: Never backtrack, know if we have been somewhere
            // But we need to account for the heatLoss quantity and
            // if we have been to the same point but have less heatLoss
            // then we should continue
            var seen = new Dictionary<Point, int>();

            // Track our queue of work
            var queue = new PriorityQueue<(Point pos, Direction dir, int straightCount, int heatLoss), int>();

            queue.Enqueue((start, Direction.Right, 0, 0), 0);

            while (queue.Count > 0)
            {
                (var thisPos, var thisDir, var thisStraight, var thisHeatLoss) = queue.Dequeue();

                // If we are at the end, skip out
                if (thisPos == end)
                {
                    heatLoss = Math.Min(thisHeatLoss, heatLoss);
                    continue;
                }

                // If we're already over the minimum, get out of here
                // Not using <= here because we already checked for the end
                // position which means we are always going to increase
                // on the next iteration in this loop
                if (heatLoss < thisHeatLoss)
                    continue;

                // Making sure if we have been here before we have a reason to continue
                if (seen.TryGetValue(thisPos, out int seenHeatLoss) && seenHeatLoss < thisHeatLoss)
                    continue;

                seen[thisPos] = thisHeatLoss;

                // Get our next steps
                // Add heat loss in the queue because the start
                // does not incur loss
                var left = (Direction)(((int)thisDir + 3) % 4);
                var right = (Direction)(((int)thisDir + 1) % 4);

                Point posLeft = thisPos.Add(deltas[left]);
                Point posStraight = thisPos.Add(deltas[thisDir]);
                Point posRight = thisPos.Add(deltas[right]);

                var newPoints = new (Point newPos, Direction newDir)[] {
                    (posLeft, left),
                    (posStraight, thisDir),
                    (posRight, right)
                };

                foreach((var newPos, var newDir) in newPoints)
                {
                    if (IsInGrid(newPos) && (newDir != thisDir || thisStraight < 3))
                    {
                        var newHeatLoss = thisHeatLoss + grid[newPos.y][newPos.x];

                        // Weight the priority so that we are always moving down and to the right if possible
                        var priority = (int)newPos.ManhattanDistance(end) + (newDir == Direction.Up || newDir == Direction.Left ? 5 : 0);

                        // Skip ahead just to not pollute the queue
                        if (newHeatLoss > heatLoss)
                            continue;

                        queue.Enqueue((newPos, newDir, newDir == thisDir ? (thisStraight + 1) : 0, newHeatLoss), priority);
                    }
                }
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

