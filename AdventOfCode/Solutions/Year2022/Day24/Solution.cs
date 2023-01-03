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

        /// <summary>
        /// Our input list of blizzards
        /// </summary>
        public List<Blizzard> blizzards = new();

        /// <summary>
        /// The map's start location
        /// </summary>
        public Point<int> start;

        /// <summary>
        /// The map's end location
        /// </summary>
        public Point<int> end;

        /// <summary>
        /// Tracking any discovered path to shortcut out of it
        /// </summary>
        public static int minDistance = Int32.MaxValue;

        /// <summary>
        /// The map's height
        /// </summary>
        public int height;

        /// <summary>
        /// The map's width
        /// </summary>
        public int width;

        /// <summary>
        /// Our queue to work through steps
        /// </summary>
        public PriorityQueue<(Point<int> current, int minute, Point<int>[] path), long> queue = new();

        /// <summary>
        /// Track our visited location (coordinate + minute)
        /// </summary>
        public HashSet<(Point<int> current, int minute)> visited = new();

        /// <summary>
        /// Precalculated blizzard locations
        /// </summary>
        public Dictionary<int, HashSet<Point<int>>> blizzardMinutes = new();

        /// <summary>
        /// Our loop modulus (LCM of width and height)
        /// </summary>
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

            minDistance = int.MaxValue;
        }

        public HashSet<Point<int>> GetBlizzardsAtMinute(int minute) =>
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
                .ToHashSet();

        /// <summary>
        /// Get all possible moves from <paramref name="current" /> in the provided list of <paramref name="blizzards" />
        /// </summary>
        public IEnumerable<Point<int>> GetPossibleMoves(HashSet<Point<int>> blizzards, Point<int> current, Point<int> end)
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

                // Always valid to move to end
                if (newPos == end)
                    yield return newPos;
                else
                {

                    // Make sure this is a valid move
                    // If the move is 0,0, it's valid out of bounds for start/end
                    if (move != moves[^1] && (newPos.x < 0 || width <= newPos.x))
                        continue;

                    if (move != moves[^1] && (newPos.y < 0 || height <= newPos.y))
                        continue;

                    if (!blizzards.Contains(newPos))
                        yield return newPos;
                }
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
            // This accounts for the same minute within the loop
            if (AddHasVisited(current, minute))
                return default;

            // Get our possible moves
            var newMinute = minute + 1;
            foreach(var newPos in GetPossibleMoves(blizzardMinutes[newMinute % loop], current, end))
            {
                // Priority queue is dequeued in order of lowest priority
                // By taking the distance to end, we have preferences towards closer answers
                // Add in the current minute so that the same position but at a lower minute is preferred
                queue.Enqueue((newPos, newMinute, path.Append(newPos).ToArray()), ((long)(newPos % end) * 1000) + newMinute);
            }

            return default;
        }

        /// <summary>
        /// Determines if we have visited this location + time or not
        /// </summary>
        private bool HasVisited(Point<int> current, int minute)
        {
            return visited.Contains((current, minute % loop));
        }

        /// <summary>
        /// Determines if we have visited this location + time or not, inserts it otherwise
        /// </summary>
        private bool AddHasVisited(Point<int> current, int minute)
        {
            if (HasVisited(current, minute % loop))
                return true;

            visited.Add((current, minute % loop));

            return false;
        }

        /// <summary>
        /// The primary loop that handles queue processing to find paths
        /// </summary>
        private void FindPath(Point<int> start, Point<int> end, int startMinute)
        {
            queue.Enqueue((start, startMinute, Array.Empty<Point<int>>()), 0);
            visited.Clear();

            while(queue.Count > 0)
            {
                var state = queue.Dequeue();

                var path = FindPath(state.current, end, state.minute, state.path);
            }
        }

        protected override string? SolvePartOne()
        {
            FindPath(start, end, 0);

            return minDistance == int.MaxValue ? string.Empty : minDistance.ToString();
        }

        protected override string? SolvePartTwo()
        {
            // We have the answer from part 1
            int totalMinutes = minDistance;

            // Reset so we can find end => start, then start => end again
            ReadMap();

            // Go from end to start
            FindPath(end, start, totalMinutes);

            // Get the new total (this includes the original minutes)
            totalMinutes = minDistance;

            // Now let's go back to end!
            ReadMap();

            FindPath(start, end, totalMinutes);

            return minDistance.ToString();
        }
    }
}

