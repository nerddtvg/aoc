using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Runtime.InteropServices;


namespace AdventOfCode.Solutions.Year2024
{

    class Day20 : ASolution
    {
        Dictionary<Point<int>, int> distance = [];
        int saved = 0;
        readonly char[][] grid;
        Point<int> start;
        Point<int> end;
        int saveLimit = 100;

        Point<int>[] moves = [Point2D.MoveUp, Point2D.MoveRight, Point2D.MoveDown, Point2D.MoveLeft];

        // Changing to remove access to the border around the grid
        // Important for P2
        public bool InGrid(Point<int> pt) => 0 < pt.x && pt.x < grid[0].Length - 1 && 0 < pt.y && pt.y < grid.Length - 1;

        public Day20() : base(20, 2024, "Race Condition")
        {
            // DebugInput = @"###############
            // #...#...#.....#
            // #.#.#.#.#.###.#
            // #S#...#.#.#...#
            // #######.#.#.###
            // #######.#.#...#
            // #######.#.###.#
            // ###..E#...#...#
            // ###.#######.###
            // #...###...#...#
            // #.#####.#.###.#
            // #.#...#.#.#...#
            // #.#.#.#.#.#.###
            // #...#...#...###
            // ###############";
            // saveLimit = 50;

            grid = Input.ToCharGrid();

            // We need to run the maze to calculate distances
            grid.ForEach((line, y) => line.ForEach((c, x) =>
            {
                if (c == 'S')
                    start = new(x, y);
                if (c == 'E')
                    end = new(x, y);
            }));

            var pos = start;
            distance[start] = 0;
            var currentDistance = 0;

            while (pos != end)
            {
                // Get the next move
                foreach (var moveDelta in moves)
                {
                    var pt = pos + moveDelta;

                    if (!InGrid(pt) || (grid[pt.y][pt.x] != '.' && grid[pt.y][pt.x] != 'E')) continue;
                    if (distance.ContainsKey(pt)) continue;

                    distance[pt] = ++currentDistance;
                    pos = pt;
                    break;
                }
            }
        }

        protected override string? SolvePartOne()
        {
            saved = 0;

            foreach (var pos in distance.Keys)
            {
                FindSaved(pos);
            }

            // Time: 00:00:07.4463386
            // Time with Part 2 rewrite: 00:00:00.1131662
            return saved.ToString();
        }

        void FindSaved(Point<int> pos, int cheatDistance = 2)
        {
            // A cheat that starts and ends in the same spot is the same cheat
            HashSet<Point<int>> savedCount = [];
            var startDist = distance[pos];

            for (int x = Math.Max(-cheatDistance, -pos.x); x <= cheatDistance && (pos.x + x) < grid[0].Length - 1; x++)
            {
                if (pos.x + x <= 0) continue;

                var yMax = cheatDistance - Math.Abs(x);

                for (int y = yMax; -yMax <= y && (pos.y + y) > 0; y--)
                {
                    if (x == 0 && y == 0) continue;

                    // We're going to check positive and negative values here
                    var pt = new Point<int>(pos.x + x, pos.y + y);

                    if (!InGrid(pt)) continue;

                    // If this is a valid endpoint, find if we have a good cheat or not
                    // distance only has racetrack points in it so if the key exists, it is valid
                    if (distance.TryGetValue(pt, out int dist))
                    {
                        // For Part 1, this is always 2
                        // For Part 2, it is variable
                        var cheatDist = (pt.x, pt.y).ManhattanDistance((pos.x, pos.y));

                        if (dist - startDist - cheatDist >= saveLimit)
                            savedCount.Add(pt);
                    }
                }
            }

            saved += savedCount.Count;
        }

        protected override string? SolvePartTwo()
        {
            saved = 0;

            foreach (var pos in distance.Keys)
            {
                FindSaved(pos, 20);
            }

            // Time: 00:00:04.8144069
            return saved.ToString();
        }
    }
}

