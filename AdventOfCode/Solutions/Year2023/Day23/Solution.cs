using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2023
{
    using Point = (int x, int y);
    using QueueItem = ((int x, int y) pos, int depth, (int x, int y)[] path);

    class Day23 : ASolution
    {
        private char[][] grid;
        private Point start;
        private Point end;

        public Day23() : base(23, 2023, "A Long Walk")
        {
            // DebugInput = @"#.#####################
            //                #.......#########...###
            //                #######.#########.#.###
            //                ###.....#.>.>.###.#.###
            //                ###v#####.#v#.###.#.###
            //                ###.>...#.#.#.....#...#
            //                ###v###.#.#.#########.#
            //                ###...#.#.#.......#...#
            //                #####.#.#.#######.#.###
            //                #.....#.#.#.......#...#
            //                #.#####.#.#.#########v#
            //                #.#...#...#...###...>.#
            //                #.#.#v#######v###.###v#
            //                #...#.>.#...>.>.#.###.#
            //                #####v#.#.###v#.#.###.#
            //                #.....#...#...#.#.#...#
            //                #.#########.###.#.#.###
            //                #...###...#...#...#.###
            //                ###.###.#.###v#####v###
            //                #...#...#.#.>.>.#.>.###
            //                #.###.###.#.###.#.#v###
            //                #.....###...###...#...#
            //                #####################.#";

            grid = Input.SplitByNewline(shouldTrim: true).Select(line => line.ToCharArray()).ToArray();

            // Get the start
            start = (grid[0].JoinAsString().IndexOf('.'), 0);
            end = (grid[^1].JoinAsString().IndexOf('.'), grid.Length - 1);
        }

        // Get all possible moves up, down, left, and right
        private Point[] GetMoves(Point point) => [point.Add((-1, 0)), point.Add((1, 0)), point.Add((0, -1)), point.Add((0, 1))];

        // Get the grid character in play
        // This will return '#' as a wall for anything out of bounds
        private char GetChar(Point point) => 0 <= point.y && point.y < grid.Length ? (0 <= point.x && point.x < grid[point.y].Length ? grid[point.y][point.x] : '#') : '#';

        protected override string? SolvePartOne()
        {
            // Track our max depth
            var maxDepth = 0;

            // Finding the longest path is a different take
            var queue = new Queue<QueueItem>();

            // Start the queue
            queue.Enqueue((start, 0, Array.Empty<Point>()));

            while(queue.TryDequeue(out QueueItem item))
            {
                (var pos, var depth, var path) = item;

                if (pos == end)
                {
                    // Console.WriteLine($"Path found: {depth}");

                    maxDepth = Math.Max(depth, maxDepth);
                    continue;
                }

                // Get the current char
                var c = GetChar(pos);

                path = path.Append(pos).ToArray();

                if (c == '.')
                {
                    // We've moved to an open spot
                    var moves = GetMoves(pos);

                    foreach(var move in moves)
                    {
                        if (!path.Contains(move) && GetChar(move) != '#')
                        {
                            queue.Enqueue((move, depth + 1, path));
                        }
                    }
                }
                else if (c == '>')
                {
                    // Move one more right
                    pos = pos.Add((1, 0));

                    if (!path.Contains(pos))
                        queue.Enqueue((pos, depth + 1, path));
                }
                else if (c == 'v')
                {
                    // Move one more down
                    pos = pos.Add((0, 1));

                    if (!path.Contains(pos))
                        queue.Enqueue((pos, depth + 1, path));
                }
                else if (c == '<')
                {
                    // Move one more left
                    pos = pos.Add((-1, 0));

                    if (!path.Contains(pos))
                        queue.Enqueue((pos, depth + 1, path));
                }
                else if (c == '^')
                {
                    // Move one more up
                    pos = pos.Add((0, -1));

                    if (!path.Contains(pos))
                        queue.Enqueue((pos, depth + 1, path));
                }
            }

            return maxDepth.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

