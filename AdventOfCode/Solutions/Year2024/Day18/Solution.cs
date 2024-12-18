using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    class Day18 : ASolution
    {
        public Point<int>[] points = [];
        public HashSet<Point<int>> part1Points = [];
        public int width = 70;
        public int height = 70;

        public Day18() : base(18, 2024, "RAM Run")
        {
            // DebugInput = @"5,4
            // 4,2
            // 4,5
            // 3,0
            // 2,1
            // 6,3
            // 2,4
            // 1,5
            // 0,6
            // 3,3
            // 2,6
            // 5,1
            // 1,2
            // 5,5
            // 2,5
            // 6,5
            // 1,4
            // 0,4
            // 6,4
            // 1,1
            // 6,1
            // 1,0
            // 0,5
            // 1,6
            // 2,0";

            points = Input.SplitByNewline(true).Select(line =>
            {
                var s = line.Split(',');
                return new Point<int>(int.Parse(s[0]), int.Parse(s[1]));
            }).ToArray();

            // If debugging...
            if (points.Length < 100)
            {
                part1Points = [.. points[..12]];

                width = 6;
                height = 6;
            }
            else
                part1Points = [.. points[..1024]];
        }

        protected override string? SolvePartOne()
        {
            var start = new Point<int>(0, 0);
            var end = new Point<int>(width, height);

            var visited = new Dictionary<string, int>();
            var queue = new PriorityQueue<Point<int>, int>();

            queue.Enqueue(start, 0);

            var possibleMoves = new Point<int>[] { Point2D.MoveUp, Point2D.MoveRight, Point2D.MoveDown, Point2D.MoveLeft };

            while (queue.TryDequeue(out Point<int> point, out int length))
            {
                if (point == end)
                    return length.ToString();

                // If we have been here with a shorter or same length, skip
                if (visited.TryGetValue(point.ToString(), out int visitedLength))
                    if (visitedLength <= length)
                        continue;

                // Otherwise save this
                visited[point.ToString()] = length;

                // Find each possible move
                foreach(var move in possibleMoves)
                {
                    var newPoint = point + move;

                    // It is inside the grid, add it to the queue
                    if (0 <= newPoint.x && newPoint.x <= width && 0 <= newPoint.y && newPoint.y <= height)
                    {
                        // If this is in the point list, we have hit a corrupt block
                        if (part1Points.Contains(newPoint))
                            continue;

                        queue.Enqueue(newPoint, length + 1);
                    }
                }
            }

            return string.Empty;
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

