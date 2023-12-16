using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Formats.Tar;
using System.IO;


namespace AdventOfCode.Solutions.Year2023
{
    using Point = (int x, int y);

    class Day16 : ASolution
    {
        private HashSet<Point> engergized = new();
        private HashSet<(Point pt, Point dir)> seen = new();
        private Queue<(Point pt, Point dir)> queue = new();
        private char[][] grid;

        private Point right = (1, 0);
        private Point up = (0, -1);
        private Point left = (-1, 0);
        private Point down = (0, 1);


        public Day16() : base(16, 2023, "The Floor Will Be Lava")
        {
            // DebugInput = @".|...\....
            //                |.-.\.....
            //                .....|-...
            //                ........|.
            //                ..........
            //                .........\
            //                ..../.\\..
            //                .-.-/..|..
            //                .|....-|.\
            //                ..//.|....";

            grid = Input.SplitByNewline(shouldTrim: true).Select(line => line.ToCharArray()).ToArray();
        }

        private void RunGrid(Point start, Point dir)
        {
            // Set our starting point
            queue.Enqueue((start, dir));

            engergized.Clear();
            seen.Clear();

            while(queue.Count > 0)
            {
                (var pos, dir) = queue.Dequeue();

                // Check that we're not in a loop
                // This matters if we've been in the same position and direction
                if (seen.Contains((pos, dir)))
                    continue;

                seen.Add((pos, dir));

                // Energize the position
                engergized.Add(pos);

                var c = grid[pos.y][pos.x];

                if (c == '.')
                {
                    // Just passing through
                    pos = pos.Add(dir);

                    if (IsInGrid(pos))
                        queue.Enqueue((pos, dir));
                }
                else if (c == '/')
                {
                    // Changing direction
                    if (dir == left)
                        dir = down;
                    else if (dir == up)
                        dir = right;
                    else if (dir == right)
                        dir = up;
                    else
                        dir = left;

                    pos = pos.Add(dir);

                    if (IsInGrid(pos))
                        queue.Enqueue((pos, dir));
                }
                else if (c == '\\')
                {
                    // Changing direction
                    if (dir == left)
                        dir = up;
                    else if (dir == up)
                        dir = left;
                    else if (dir == right)
                        dir = down;
                    else
                        dir = right;

                    pos = pos.Add(dir);

                    if (IsInGrid(pos))
                        queue.Enqueue((pos, dir));
                }
                else if (c == '|')
                {
                    if (dir == up || dir == down)
                    {
                        pos = pos.Add(dir);
                        if (IsInGrid(pos))
                            queue.Enqueue((pos, dir));
                    }
                    else
                    {
                        var posUp = pos.Add(up);
                        var posDown = pos.Add(down);

                        if (IsInGrid(posUp))
                            queue.Enqueue((posUp, up));

                        if (IsInGrid(posDown))
                            queue.Enqueue((posDown, down));
                    }
                }
                else if (c == '-')
                {
                    if (dir == left || dir == right)
                    {
                        pos = pos.Add(dir);
                        if (IsInGrid(pos))
                            queue.Enqueue((pos, dir));
                    }
                    else
                    {
                        var posLeft = pos.Add(left);
                        var posRight = pos.Add(right);

                        if (IsInGrid(posLeft))
                            queue.Enqueue((posLeft, left));

                        if (IsInGrid(posRight))
                            queue.Enqueue((posRight, right));
                    }
                }
            }
        }

        private bool IsInGrid(Point point) => 0 <= point.y && point.y < grid.Length && 0 <= point.x && point.x < grid[point.y].Length;

        protected override string? SolvePartOne()
        {
            RunGrid((0, 0), right);

            return engergized.Count.ToString();
        }

        protected override string? SolvePartTwo()
        {
            int max = 0;

            // For every starting point, find the largest number of tiles possible
            for (int y=0; y<grid.Length; y++)
            {
                if (y == 0 || y == grid.Length-1)
                {
                    for (int x = 0; x < grid[0].Length; x++)
                    {
                        // Top and bottoms
                        RunGrid((x, y), y == 0 ? down : up);

                        max = Math.Max(max, engergized.Count);
                    }
                }

                // Left and right
                RunGrid((0, y), right);
                max = Math.Max(max, engergized.Count);

                RunGrid((grid[0].Length-1, y), left);
                max = Math.Max(max, engergized.Count);
            }

            return max.ToString();
        }
    }
}

