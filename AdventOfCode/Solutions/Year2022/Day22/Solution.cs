using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2022
{

    class Day22 : ASolution
    {
        private char[][] grid;
        private (int x, int y) start;

        public Day22() : base(22, 2022, "Monkey Map")
        {
            grid = ReadGrid(Input);

            start = (FindRowStart(0), 0);
        }

        private char[][] ReadGrid(string input)
        {
            // Read the input into [y][x] char array
            var arr = input.SplitByNewline()
                .Select(line => line.ToCharArray())
                .ToArray();

            return arr;
        }

        /// <summary>
        /// Search the grid by row, find the first non-empty character
        /// </summary>
        private int FindRowStart(int y)
        {
            for (int x = 0; x < grid[y].Length; x++)
            {
                if (grid[y][x] != ' ')
                    return x;
            }

            throw new Exception();
        }

        /// <summary>
        /// Search the grid by row, find the last non-empty character
        /// </summary>
        private int FindRowEnd(int y)
        {
            for (int x = grid[y].Length - 1; x >= 0; x--)
            {
                if (grid[y][x] != ' ')
                    return x;
            }

            throw new Exception();
        }

        /// <summary>
        /// Search the grid by column, find the first non-empty character
        /// </summary>
        private int FindColStart(int x)
        {
            for (int y = 0; y < grid.Length; y++)
            {
                if (grid[y][x] != ' ')
                    return y;
            }

            throw new Exception();
        }

        /// <summary>
        /// Search the grid by column, find the last non-empty character
        /// </summary>
        private int FindColEnd(int x)
        {
            // This may require searching different length arrays
            for (int y = grid.Length - 1; y >= 0; y--)
            {
                if (x >= grid[y].Length) continue;

                if (grid[y][x] != ' ')
                    return x;
            }

            throw new Exception();
        }

        protected override string? SolvePartOne()
        {
            return string.Empty;
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

