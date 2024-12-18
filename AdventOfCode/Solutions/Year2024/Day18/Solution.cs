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
        public Point<int>[] path = [];

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

        // Will run each path on a set of points
        Point<int>[] FindPath(HashSet<Point<int>> points)
        {
            var start = new Point<int>(0, 0);
            var end = new Point<int>(width, height);

            var visited = new Dictionary<string, int>();
            var queue = new PriorityQueue<(Point<int> point, Point<int>[] path), int>();

            queue.Enqueue((start, [start]), 0);

            var possibleMoves = new Point<int>[] { Point2D.MoveUp, Point2D.MoveRight, Point2D.MoveDown, Point2D.MoveLeft };

            while (queue.TryDequeue(out (Point<int> point, Point<int>[] path) item, out int length))
            {
                if (item.point == end)
                    return item.path;

                // If we have been here with a shorter or same length, skip
                if (visited.TryGetValue(item.point.ToString(), out int visitedLength))
                    if (visitedLength <= length)
                        continue;

                // Otherwise save this
                visited[item.point.ToString()] = length;

                // Find each possible move
                foreach (var move in possibleMoves)
                {
                    var newPoint = item.point + move;

                    // It is inside the grid, add it to the queue
                    if (0 <= newPoint.x && newPoint.x <= width && 0 <= newPoint.y && newPoint.y <= height)
                    {
                        // If this is in the point list, we have hit a corrupt block
                        if (points.Contains(newPoint))
                            continue;

                        queue.Enqueue((newPoint, [.. item.path, newPoint]), length + 1);
                    }
                }
            }

            return [];
        }

        protected override string? SolvePartOne()
        {
            // Time: 00:00:00.0428068
            // Time with Part 2 rewrite: 00:00:00.0539815
            path = FindPath(part1Points);
            return (path.Length - 1).ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Time: 00:00:00.6073102

            // Start with the Part 1 path
            // and for each new point in the list, see if that is in the path
            // If it is, re-run the path
            var pathHashSet = new HashSet<Point<int>>(path);

            for(int index = part1Points.Count; index<points.Length; index++)
            {
                var point = points[index];

                // New point, see if it is in the path
                if (pathHashSet.Contains(point))
                {
                    // Found a blocker, re-run the path
                    path = FindPath(new HashSet<Point<int>>(points[..(index + 1)]));

                    // Found our limit
                    if (path.Length == 0)
                        return $"{point.x},{point.y}";

                    // We got a new path, it works, try again
                    pathHashSet = [.. path];
                }
            }

            return string.Empty;
        }
    }
}

