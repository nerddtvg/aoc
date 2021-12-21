using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2021
{

    class Day20 : ASolution
    {
        private string enhancement = string.Empty;
        private Dictionary<(int x, int y), bool> grid = new Dictionary<(int x, int y), bool>();

        public Day20() : base(20, 2021, "Trench Map")
        {
//             DebugInput = @"..#.#..#####.#.#.#.###.##.....###.##.#..###.####..#####..#....#..#..##..###..######.###...####..#..#####..##..#.#####...##.#.#..#.##..#.#......#.###.######.###.####...#.##.##..#..#..#####.....#.#....###..#.##......#.....#..#..#..##..#...##.######.####.####.#.#...#.......#..#.#.#...####.##.#......#..#...##.#.##..#...##.#.##..###.#......#.#.......#.#.#.####.###.##...#.....####.#..#..#.##.#....##..#.####....##...##..#...#......#.#.......#.......##..####..#...#.#.#...##..#.#..###..#####........#..####......#..#

// #..#.
// #....
// ##..#
// ..#..
// ..###";

            Reset();
        }

        private void Reset()
        {
            var split = Input.SplitByBlankLine();
            this.enhancement = split[0].JoinAsString();

            // Read in the grid
            this.grid.Clear();
            for (int y = 0; y < split[1].Length; y++)
            {
                int x = 0;
                foreach(var c in split[1][y])
                {
                    this.grid[(x++, y)] = c == '#';
                }
            }
        }

        private bool GetValue((int x, int y) pos)
        {
            if (this.grid.ContainsKey(pos))
                return this.grid[pos];

            return false;
        }

        private int GetIndex((int x, int y) pos)
        {
            // Look at everything from x-1 to x+1 and y-1 to y+1
            var str = "";

            str += GetValue((pos.x - 1, pos.y - 1)) ? "1" : "0";
            str += GetValue((pos.x, pos.y - 1)) ? "1" : "0";
            str += GetValue((pos.x + 1, pos.y - 1)) ? "1" : "0";
            str += GetValue((pos.x - 1, pos.y)) ? "1" : "0";
            str += GetValue((pos.x, pos.y)) ? "1" : "0";
            str += GetValue((pos.x + 1, pos.y)) ? "1" : "0";
            str += GetValue((pos.x - 1, pos.y + 1)) ? "1" : "0";
            str += GetValue((pos.x, pos.y + 1)) ? "1" : "0";
            str += GetValue((pos.x + 1, pos.y + 1)) ? "1" : "0";

            return Convert.ToInt32(str, fromBase: 2);
        }

        private bool GetNewValue((int x, int y) pos) =>
            this.enhancement[GetIndex(pos)] == '#';

        private void RunAlgorithm(int index)
        {
            // We always extend the image by one in each direction (anything further than +/-2 outside will always return 0 because of the "algorithm")
            // We're comparing x-1,y-1 to x+1,y+1
            var startX = this.grid.Min(kvp => kvp.Key.x) - 1;
            var startY = this.grid.Min(kvp => kvp.Key.y) - 1;
            var endX = this.grid.Max(kvp => kvp.Key.x) + 1;
            var endY = this.grid.Max(kvp => kvp.Key.y) + 1;

            // We need to prefill in some stuff
            // This is a trick that didn't apply to the example
            if (this.enhancement[0] == '#' && this.enhancement[this.enhancement.Length - 1] == '.')
            {
                // Add a border
                // This causes the outside border to "flicker"
                // Used this hint from the megathread:
                // "The trick was to check 0th bit of algo and if it is '#', and 511th bit is '.', then pixel outside the algo are going to toggle on every iteration."
                // https://old.reddit.com/r/adventofcode/comments/rkf5ek/2021_day_20_solutions/hpbty2g/

                // I kept doing this == 0, but that was incorrect
                var lit = index % 2 == 1;

                for (int x = startX - 1; x <= endX + 1; x++)
                {
                    // Add the top and bottom 2 rows
                    this.grid[(x, startY - 1)] = lit;
                    this.grid[(x, startY)] = lit;
                    this.grid[(x, endY)] = lit;
                    this.grid[(x, endY + 1)] = lit;
                }

                for (int y = startY - 1; y <= endY + 1; y++)
                {
                    // Add the vertical borders
                    this.grid[(startX - 1, y)] = lit;
                    this.grid[(startX, y)] = lit;
                    this.grid[(endX, y)] = lit;
                    this.grid[(endX + 1, y)] = lit;
                }
            }

            var newGrid = new Dictionary<(int x, int y), bool>();
            for (int y = startY; y <= endY; y++)
            {
                for (int x = startX; x <= endX; x++)
                {
                    newGrid[(x, y)] = GetNewValue((x, y));
                }
            }

            // Save the grid
            this.grid = newGrid;
        }

        protected override string? SolvePartOne()
        {
            Utilities.Repeat((i) => RunAlgorithm(i), 2);
            return this.grid.Count(kvp => kvp.Value).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
