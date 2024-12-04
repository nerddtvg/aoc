using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    class Day04 : ASolution
    {
        public char[][] grid;
        public int maxX;
        public int maxY;

        public Day04() : base(04, 2024, "Ceres Search")
        {
//             DebugInput = @"MMMSXXMASM
// MSAMXMSMSA
// AMXSXMAAMM
// MSAMASMSMX
// XMASAMXAMM
// XXAMMXXAMA
// SMSMSASXSS
// SAXAMASAAA
// MAMMMXMMMM
// MXMXAXMASX";

            grid = Input.SplitByNewline().Select(line => line.ToCharArray()).ToArray();

            maxX = grid[0].Length - 1;
            maxY = grid.Length - 1;
        }

        public string[] GetStrings(int x, int y)
        {
            var ret = new List<string>();

            // Get all possible directions
            // We only need to look up and left, then reverse the findings
            if (x >= 3)
            {
                if (y >= 3)
                {
                    // Look left and up
                    ret.Add($"{grid[y][x]}{grid[y - 1][x - 1]}{grid[y - 2][x - 2]}{grid[y - 3][x - 3]}");
                }

                if (y <= maxY - 3)
                {
                    // Look left and down
                    ret.Add($"{grid[y][x]}{grid[y + 1][x - 1]}{grid[y + 2][x - 2]}{grid[y + 3][x - 3]}");
                }

                // Look straight left
                ret.Add($"{grid[y][x]}{grid[y][x - 1]}{grid[y][x - 2]}{grid[y][x - 3]}");
            }

            if (y >= 3)
            {
                // Look straight up
                ret.Add($"{grid[y][x]}{grid[y - 1][x]}{grid[y - 2][x]}{grid[y - 3][x]}");
            }

            return ret.ToArray();
        }

        public string[] GetDiagonals(int x, int y)
        {
            // Only valid for 3 character strings where (x,y) is A
            if (x >= 1 && x <= maxX - 1)
            {
                if (y >= 1 && y <= maxY - 1)
                {
                    if (grid[y][x] == 'A')
                    {
                        return [
                            $"{grid[y - 1][x - 1]}A{grid[y + 1][x + 1]}",
                            $"{grid[y - 1][x + 1]}A{grid[y + 1][x - 1]}"
                            ];
                    }
                }
            }

            return ["invalid"];
        }

        protected override string? SolvePartOne()
        {
            // Time: 00:00:00.0124730
            int count = 0;

            for (int y = 0; y <= maxY; y++)
            {
                for (int x = 0; x <= maxX; x++)
                {
                    count += GetStrings(x, y).Count(str => str == "XMAS" || str == "SAMX");
                }
            }

            return count.ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Time: 00:00:00.0035736
            int count = 0;

            for (int y = 0; y <= maxY; y++)
            {
                for (int x = 0; x <= maxX; x++)
                {
                    // Normally Any would be faster for long arrays, but for two strings All is fine
                    if (GetDiagonals(x, y).All(str => str == "MAS" || str == "SAM"))
                        count++;
                }
            }

            return count.ToString();
        }
    }
}

