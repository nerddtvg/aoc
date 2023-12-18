using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Reflection;
using System.Diagnostics;


namespace AdventOfCode.Solutions.Year2023
{
    using Point = (int x, int y);

    class Day18 : ASolution
    {
        private List<(char dir, int length, string rgb, byte r, byte g, byte b)> instructions;

        private readonly Dictionary<char, Point> deltas = new()
        {
            { 'U', (0, -1) },
            { 'R', (1, 0) },
            { 'D', (0, 1) },
            { 'L', (-1, 0) },
        };

        private HashSet<Point> pit = new();

        public Day18() : base(18, 2023, "Lavaduct Lagoon")
        {
            // DebugInput = @"R 6 (#70c710)
            //                D 5 (#0dc571)
            //                L 2 (#5713f0)
            //                D 2 (#d2c081)
            //                R 2 (#59c680)
            //                D 2 (#411b91)
            //                L 5 (#8ceee2)
            //                U 2 (#caa173)
            //                L 1 (#1b58a2)
            //                U 2 (#caa171)
            //                R 2 (#7807d2)
            //                U 3 (#a77fa3)
            //                L 2 (#015232)
            //                U 2 (#7a21e3)";

            instructions = Input.SplitByNewline(shouldTrim: true)
                .Select(line =>
                {
                    var split = line.Split(' ', 3);

                    return (split[0][0], int.Parse(split[1]), split[2][2..^1], Convert.ToByte(split[2][2..4], 16), Convert.ToByte(split[2][4..6], 16), Convert.ToByte(split[2][6..8], 16));
                })
                .ToList();
        }

        protected override string? SolvePartOne()
        {
            pit.Clear();

            Point pos = (0, 0);
            pit.Add(pos);

            // Follow the instructions
            foreach(var instruction in instructions)
            {
                var move = deltas[instruction.dir];
                Utilities.Repeat(() =>
                {
                    pos = pos.Add(move);
                    pit.Add(pos);
                }, instruction.length);
            }

            FillPit();

            return pit.Count.ToString();
        }

        private void FillPit()
        {
            // Fill method will be a basic search method
            // Start with the first known "in pit" point
            // This will be the corner inside the top points
            Point topLeftCorner = pit.OrderBy(itm => itm.y).ThenBy(itm => itm.x).First();
            topLeftCorner = (topLeftCorner.x + 1, topLeftCorner.y + 1);

            // Make sure that we have a true corner
            Debug.Assert(pit.Contains((topLeftCorner.x - 1, topLeftCorner.y)));
            Debug.Assert(pit.Contains((topLeftCorner.x, topLeftCorner.y - 1)));
            Debug.Assert(!pit.Contains((topLeftCorner.x, topLeftCorner.y)));

            // For each point up, down, left, right
            // If the point is in the pit: skip
            // If not, add to the pit and keep going
            var queue = new Queue<Point>();
            queue.Enqueue(topLeftCorner);

            while(queue.TryDequeue(out Point point))
            {
                if (pit.Contains(point))
                    continue;

                pit.Add(point);

                queue.Enqueue(point.Add(deltas['U']));
                queue.Enqueue(point.Add(deltas['R']));
                queue.Enqueue(point.Add(deltas['D']));
                queue.Enqueue(point.Add(deltas['L']));
            }
        }

        private void DrawPit()
        {
            var minX = pit.Min(itm => itm.x);
            var minY = pit.Min(itm => itm.y);
            var maxX = pit.Max(itm => itm.x);
            var maxY = pit.Max(itm => itm.y);

            Console.WriteLine("Pit:");

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                    Console.Write(pit.Contains((x, y)) ? "#" : ".");

                Console.WriteLine();
            }
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

