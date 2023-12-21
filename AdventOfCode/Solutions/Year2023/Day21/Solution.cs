using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using QuikGraph.Algorithms;


namespace AdventOfCode.Solutions.Year2023
{
    using Point = (int x, int y);

    class Day21 : ASolution
    {
        public required char[][] grid;
        public Point start;

        public Day21() : base(21, 2023, "Step Counter")
        {
            // DebugInput = @"...........
            //                .....###.#.
            //                .###.##..#.
            //                ..#.#...#..
            //                ....#.#....
            //                .##..S####.
            //                .##..#...#.
            //                .......##..
            //                .##.#.####.
            //                .##..##.##.
            //                ...........";

            grid = Input.SplitByNewline(shouldTrim: true).Select(line => line.ToCharArray()).ToArray();

            start = (-1, -1);

            // Find the start
            for (int y = 0; y < grid.Length && start.x == -1; y++)
                for (int x = 0; x < grid[0].Length && start.x == -1; x++)
                    if (grid[y][x] == 'S')
                        start = (x, y);
        }

        private List<Point> GetPoints(int maxDistance)
        {
            // Work from start through the points and get what we can
            var queue = new Queue<(Point pos, int distance)>();
            var seen = new Dictionary<Point, int>();

            // Our start
            queue.Enqueue((start, 0));

            while(queue.Count > 0)
            {
                (var pos, int distance) = queue.Dequeue();

                // Check if we have been here before
                if (seen.TryGetValue(pos, out int seenDistance))
                {
                    if (seenDistance <= distance)
                        continue;
                }

                // Save our distance
                seen[pos] = distance;

                // If we have hit our maxDistance, stop here
                if (distance >= maxDistance)
                    continue;

                // Get the possible moves
                var moves = GetMoves(pos);

                foreach(var move in moves)
                {
                    // Check that we can move here
                    var c = GetChar(move);

                    if (c == '.')
                    {
                        queue.Enqueue((move, distance + 1));
                    }
                }
            }

            // DrawGrid(seen, maxDistance);

            // This will generate a "seen" list of the distances to get to those positions
            // Return the list
            return seen.Where(kvp => IsValidDistance(kvp.Value, maxDistance)).Select(kvp => kvp.Key).ToList();
        }

        private bool IsValidDistance(int distance, int desiredDistance)
        {
            // The trick of being "exactly" desiredDistance is that you can double back
            // at any point but you will always lose an extra step so:
            // #S.. can get back to S in 2, 4, or any even number of moves
            return distance == desiredDistance || (distance < desiredDistance && (distance % 2) == 0);
        }

        private void DrawGrid(Dictionary<Point, int> distances, int showDistance)
        {
            Console.WriteLine();

            for (int y = 0; y < grid.Length; y++)
            {
                for (int x = 0; x < grid[0].Length; x++)
                {
                    var pos = (x, y);
                    var c = GetChar(pos);
                    var found = distances.TryGetValue(pos, out int distance);

                    if (pos == start)
                        Console.Write('S');
                    else if (found && IsValidDistance(distance, showDistance))
                        Console.Write('*');
                    else if (found)
                        Console.Write(distance % 10);
                    else
                        Console.Write(c);
                }

                Console.WriteLine();
            }
        }

        private char GetChar(Point pt) => 0 <= pt.y && pt.y < grid.Length ? (0 <= pt.x && pt.x < grid[pt.y].Length ? grid[pt.y][pt.x] : '#') : '#';

        private List<Point> GetMoves(Point a) => new() { a.Add((-1, 0)), a.Add((1, 0)), a.Add((0, -1)), a.Add((0, 1)) };

        protected override string? SolvePartOne()
        {
            return GetPoints(64).Count.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

