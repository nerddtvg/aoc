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

        private Dictionary<Direction, char> chars = new()
        {
            { Direction.Up, '^' },
            { Direction.Right, '>' },
            { Direction.Down, 'v' },
            { Direction.Left, '<' }
        };

        public Day17() : base(17, 2023, "Clumsy Crucible")
        {
            // Load the grid
            grid = Input.SplitByNewline(shouldTrim: true).Select(line => line.Select(c => int.Parse(c.ToString())).ToArray()).ToArray();
            maxY = grid.Length - 1;
            maxX = grid[0].Length - 1;
        }

        private int RunGrid(int minStraightSteps, int maxStraightSteps)
        {
            // Start and end locations
            Point start = (0, 0);
            Point end = (maxX, maxY);

            // Using a priority queue based on heatLoss ensures
            // our first end will be the lowest hestLoss possible
            // Our seen only needs to know if this has been visited
            // plus direction and number of straight steps
            var seen = new HashSet<(Point pos, Direction dir, int straight)>();

            // Track our queue of work
            // Priority is heatLoss so the lowest heatLoss is always tried next
            var queue = new PriorityQueue<(Point pos, Direction dir, int straightCount, List<(Point pos, Direction dir)> path), int>();

            queue.Enqueue((start, Direction.Right, 0, new List<(Point pos, Direction dir)>()), 0);
            queue.Enqueue((start, Direction.Down, 0, new List<(Point pos, Direction dir)>()), 0);

            while (queue.TryDequeue(out var item, out var thisHeatLoss))
            {
                (var thisPos, var thisDir, var thisStraight, var thisPath) = item;

                thisPath.Add((thisPos, thisDir));

                // If we are at the end, skip out
                if (thisPos == end)
                    return thisHeatLoss;

                // Get our next steps
                // Add heat loss in the queue because the start
                // does not incur loss
                (Point newPos, Direction newDir, int newSteps)[] newPoints = [];

                // If we have no straight steps, we may need to go a minimum distance first
                // This will occur after every turn
                if (thisStraight == 0 && minStraightSteps > 0)
                {
                    var tmpPos = thisPos.Clone();
                    var isInGrid = true;
                    for (int i = 0; i < minStraightSteps - 1 && isInGrid; i++)
                    {
                        tmpPos = tmpPos.Add(deltas[thisDir]);

                        // If it is outside the grid, it will be ignored later
                        isInGrid = IsInGrid(tmpPos);

                        // Increase our heatloss
                        // We don't include the last position because it is added later
                        if (i < minStraightSteps - 2 && isInGrid)
                            thisHeatLoss += grid[tmpPos.y][tmpPos.x];
                    }

                    if (isInGrid)
                        newPoints = [(tmpPos, thisDir, minStraightSteps - 1)];
                }
                else
                {
                    var left = (Direction)(((int)thisDir + 3) % 4);
                    var right = (Direction)(((int)thisDir + 1) % 4);

                    newPoints = [
                        (thisPos.Add(deltas[left]), left, 0),
                        (thisPos.Add(deltas[thisDir]), thisDir, thisStraight+1),
                        (thisPos.Add(deltas[right]), right, 0)
                    ];
                }

                foreach ((var newPos, var newDir, var newStraight) in newPoints)
                {
                    // Make sure it is in the grid
                    // Make sure the move is valid
                    // Make sure we add it to the HashSet (this is false if already visited)
                    if (IsInGrid(newPos) && newStraight < maxStraightSteps && seen.Add((newPos, newDir, newStraight)))
                    {
                        var newHeatLoss = thisHeatLoss + grid[newPos.y][newPos.x];

                        queue.Enqueue((newPos, newDir, newStraight, new List<(Point pos, Direction dir)>(thisPath)), newHeatLoss);
                    }
                }
            }

            return 0;
        }

        private bool IsInGrid(Point point) => 0 <= point.y && point.y <= maxY && 0 <= point.x && point.x <= maxX;

        protected override string? SolvePartOne()
        {
            return RunGrid(0, 3).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return RunGrid(4, 10).ToString();
        }
    }
}

