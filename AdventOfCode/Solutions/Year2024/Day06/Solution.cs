using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    class Day06 : ASolution
    {
        public enum Dir
        {
            North,
            East,
            South,
            West
        }

        private char[][] grid;
        private HashSet<(int x, int y)> visited = new();
        private (int x, int y) pos;
        private Dir dir = Dir.North;
        private Dictionary<Dir, (int x, int y)> move = new() {
            { Dir.North, (0, -1) },
            { Dir.East, (1, 0) },
            { Dir.South, (0, 1) },
            { Dir.West, (-1, 0) }
        };

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
                        pos = (x, y);
                        found = true;
                        break;
                    }

            visited.Add(pos);
        }

        private bool Move() {
            (int x, int y) newPos = pos.Add(move[dir]);

            if (newPos.x < 0 || newPos.y < 0 || grid[0].Length <= newPos.x || grid.Length <= newPos.y) {
                return false;
            }
            
            switch(grid[newPos.y][newPos.x]) {
                case '.':
                case '^':
                    pos = newPos;
                    visited.Add(pos);
                    break;

                case '#':
                    // CHANGE DIRECTION!
                    dir = (Dir)(((int)dir + 1) % 4);
                    break;
            }

            return true;
        }

        protected override string? SolvePartOne()
        {
            while(Move()) {
                // Keep moving
            }
            return visited.Count.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

