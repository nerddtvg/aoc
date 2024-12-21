using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    class Day20 : ASolution
    {
        Dictionary<Point<int>, int> distance = [];
        Dictionary<int, Point<int>> reversedDistance = [];
        Dictionary<(Point<int> a, Point<int> b), int> saved = [];
        readonly char[][] grid;
        Point<int> start;
        Point<int> end;

        Point<int>[] moves = [Point2D.MoveUp, Point2D.MoveRight, Point2D.MoveDown, Point2D.MoveLeft];
        public bool InGrid(Point<int> pt) => 0 <= pt.x && pt.x < grid[0].Length && 0 <= pt.y && pt.y < grid.Length;

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
                foreach(var moveDelta in moves)
                {
                    var pt = pos + moveDelta;

                    if (!InGrid(pt) || (grid[pt.y][pt.x] != '.' && grid[pt.y][pt.x] != 'E')) continue;
                    if (distance.ContainsKey(pt)) continue;

                    distance[pt] = ++currentDistance;
                    pos = pt;
                    break;
                }
            }

            reversedDistance = distance.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        }

        protected override string? SolvePartOne()
        {
            foreach(var pos in distance.Keys)
            {
                // Get the next move
                foreach (var moveDelta in moves)
                {
                    var pt = pos + moveDelta;

                    // We want WALLS
                    if (!InGrid(pt) || grid[pt.y][pt.x] != '#') continue;

                    var pt2 = pt + moveDelta;

                    // We want open spaces
                    if (!InGrid(pt2) || (grid[pt2.y][pt2.x] != '.' && grid[pt2.y][pt2.x] != 'E')) continue;

                    // Don't go backwards
                    if (distance[pt2] < distance[pos]) continue;

                    // Remove 2 because of the 2 steps it takes to "cheat"
                    saved[(pos, pt2)] = distance[pt2] - distance[pos] - 2;
                }
            }

            return saved.Count(kvp => kvp.Value >= 100).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

