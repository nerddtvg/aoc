using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    class Day06 : ASolution
    {
        private char[][] grid;
        private Dictionary<(int x, int y), HashSet<Direction>> visited = new();
        private (int x, int y) start;
        private (int x, int y) pos;
        private Direction dir = Direction.North;

        public Day06() : base(06, 2024, "Guard Gallivant")
        {
//             DebugInput = @"....#.....
// .........#
// ..........
// ..#.......
// .......#..
// ..........
// .#..^.....
// ........#.
// #.........
// ......#...";

            grid = Input.SplitByNewline().Select(line => line.ToCharArray()).ToArray();

            bool found = false;

            for (int y = 0; y < grid.Length && !found; y++)
                for (int x = 0; x < grid[y].Length && !found; x++)
                    if (grid[y][x] == '^')
                    {
                        start = (x, y);
                        found = true;
                        break;
                    }
        }

        private void Reset()
        {
            pos = start;
            dir = Direction.North;

            // Clear out the visited array each time
            visited = new() { [start] = [dir] };
        }

        private bool Move((int x, int y)? fakeObstacle = null)
        {
            (int x, int y) newPos = pos.Add(Directions.directionTuple[dir]);

            if (newPos.x < 0 || newPos.y < 0 || grid[0].Length <= newPos.x || grid.Length <= newPos.y)
            {
                return false;
            }

            switch (fakeObstacle?.x == newPos.x && fakeObstacle?.y == newPos.y ? '#' : grid[newPos.y][newPos.x])
            {
                case '.':
                case (char)DirectionChar.North:
                    pos = newPos;
                    // This tracks what we have visited
                    if (!visited.ContainsKey(pos))
                        visited[pos] = [];
                    break;

                case '#':
                    // CHANGE DIRECTION!
                    dir = dir.Rotate();
                    break;
            }

            return true;
        }

        protected override string? SolvePartOne()
        {
            // Time: 00:00:00.0058982
            // Rewrite: 00:00:00.0143739
            Reset();
            while (Move())
            {
                // Keep moving
            }
            return visited.Keys.Count.ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Time: 00:00:05.8077128
            // Rewrite: 00:00:06.1962430
            // We will review all visited locations and attempt to place an object there
            var visitedKeys = visited.Keys.Where(key => key.x != start.x || key.y != start.y).ToArray();

            int validObstructions = 0;

            foreach (var key in visitedKeys)
            {
                // if (key == (5, 1)) System.Diagnostics.Debugger.Break();
                Reset();
                while (Move(key))
                {
                    // Check if we have visited this location before
                    // Key is valid because it is added in Move()
                    if (visited[pos].Contains(dir)) {
                        validObstructions++;
                        break;
                    }

                    // Otherwise add this
                    visited[pos].Add(dir);
                }
            }

            return validObstructions.ToString();
        }
    }
}

