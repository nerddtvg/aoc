using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2022
{

    class Day24 : ASolution
    {

        public struct Blizzard
        {
            public Point<int> origin;
            public Point<int> direction;
            public int mod;
        }

        public readonly Dictionary<char, Point<int>> directions = new()
        {
            { '<', new(-1, 0) },
            { '>', new(1, 0) },
            { '^', new(0, -1) },
            { 'v', new(0, 1) }
        };

        public List<Blizzard> blizzards = new();

        public Point<int> start;
        public Point<int> end;
        public static int minDistance = Int32.MaxValue;
        public int height;
        public int width;
        public PriorityQueue<(Point<int> current, int minute, Point<int>[] path), ulong> queue = new();
        public HashSet<(Point<int> current, int minute)> visited = new();
        public Dictionary<int, Point<int>[]> blizzardMinutes = new();
        public int loop;

        public Day24() : base(24, 2022, "Blizzard Basin")
        {
            // DebugInput = @"#.######
            // #>>.<^<#
            // #.<..<<#
            // #>v.><>#
            // #<^v^^>#
            // ######.#";

            ReadMap();
        }

        public void ReadMap()
        {
            var lines = Input.SplitByNewline(true).ToArray();

            width = lines[0].Length - 2;
            height = lines.Length - 2;

            for (int i = 1; i < lines.Length - 1; i++)
            {
                var line = lines[i];

                for (int q = 1; q < line.Length - 1; q++)
                {
                    if (directions.ContainsKey(line[q]))
                    {
                        blizzards.Add(new()
                        {
                            // Change so the top left of the map is 0,0
                            // Helps with math later
                            origin = new(q - 1, i - 1),
                            direction = directions[line[q]],
                            // Figure out our wrap around value
                            mod = directions[line[q]].x == 0 ? height : width
                        });
                    }
                }
            }

            start = new(lines[0].IndexOf('.') - 1, -1);
            end = new(lines[^1].IndexOf('.') - 1, lines.Length - 2);

            // Blizzards repeat, so we can precalculate maps
            blizzardMinutes.Clear();
            loop = (int)Utilities.FindLCM(width, height);
            for (int i = 0; i < loop; i++)
            {
                blizzardMinutes.Add(i, GetBlizzardsAtMinute(i));
            }
        }

        public Point<int>[] GetBlizzardsAtMinute(int minute) =>
            blizzards
                .Select(b =>
                {
                    // if (b.origin.x == 2 && b.origin.y == 3)
                    //     System.Diagnostics.Debugger.Break();
                    var newX = b.direction.x == 0 ? b.origin.x : ((b.origin.x + (b.direction.x * minute)) % b.mod);
                    var newY = b.direction.y == 0 ? b.origin.y : ((b.origin.y + (b.direction.y * minute)) % b.mod);

                    while(newX < 0)
                        newX += b.mod;

                    while (newY < 0)
                        newY += b.mod;

                    return new Point<int>(newX, newY);
                })
                .ToArray();

        /// <summary>
        /// Get all possible moves from <paramref name="current" />
        /// </summary>
        public IEnumerable<Point<int>> GetPossibleMoves(Point<int>[] blizzards, Point<int> current, Point<int> end)
        {
            var moves = new Point<int>[]
            {
                new(-1, 0),
                new(1, 0),
                new(0, -1),
                new(0, 1),
                new(0, 0)
            };

            foreach(var move in moves)
            {
                var newPos = current + move;

                // Make sure this is a valid move
                // If the move is 0,0, it's valid out of bounds for start/end
                if (move != moves.Last() && move != end && (newPos.x < 0 || width <= newPos.x))
                    continue;

                if (move != moves.Last() && move != end && (newPos.y < 0 || height <= newPos.y))
                    continue;

                if (!blizzards.Contains(newPos))
                    yield return newPos;
            }
        }

        /// <summary>
        /// Finds paths from <paramref name="current" /> to <paramref name="end" />
        /// </summary>
        /// <param name="current">Our current position</param>
        /// <param name="end">Our desired destination</param>
        /// <param name="path">Current path</param>
        /// <returns>A full path</returns>
        private Point<int>[]? FindPath(Point<int> current, Point<int> end, int minute, Point<int>[] path)
        {
            if (minute > Day24.minDistance)
                return default;

            if (current == end)
            {
                minDistance = Math.Min(minDistance, minute);

                // Return our results
                return path;
            }

            // If we've been here, skip it
            if (visited.Contains((current, minute)))
                return default;

            visited.Add((current, minute));

            // Get our possible moves
            foreach(var newPos in GetPossibleMoves(blizzardMinutes[(minute + 1) % loop], current, end))
            {
                queue.Enqueue((newPos, minute + 1, path.Append(newPos).ToArray()), ((newPos % end) * 1000) + (ulong)minute);
            }

            return default;
        }

        private Point<int>[] FindPath()
        {
            queue.Enqueue((start, 0, Array.Empty<Point<int>>()), 0);
            visited.Clear();

            while(queue.Count > 0)
            {
                var state = queue.Dequeue();

                var path = FindPath(state.current, end, state.minute, state.path);

                if (path != default)
                {
                    return path;
                }
            }

            throw new Exception();
        }

        protected override string? SolvePartOne()
        {
            var path = FindPath();

            foreach(var move in path)
                Console.WriteLine($"({move.x}, {move.y})");

            return path.Length.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

