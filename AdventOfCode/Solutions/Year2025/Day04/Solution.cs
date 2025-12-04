using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2025
{

    class Day04 : ASolution
    {
        private int[][] counted = [];

        private readonly (int x, int y)[] directions =
        [
            (1, 0),
            (-1, 1),
            (0, 1),
            (1, 1)
        ];

        public Day04() : base(04, 2025, "Printing Department")
        {
            // DebugInput = @"..@@.@@@@.
            //     @@@.@.@.@@
            //     @@@@@.@.@@
            //     @.@@@@..@.
            //     @@.@@@@.@@
            //     .@@@@@@@.@
            //     .@.@.@.@@@
            //     @.@@@.@@@@
            //     .@@@@@@@@.
            //     @.@.@@@.@.";

            CountGrid([.. Input.SplitByNewline(true).Select(line => line.ToCharArray())]);
        }

        private void CountGrid(char[][] grid)
        {
            // Load grid with negative one (we will know these are non-bales)
            // Starting at zero means we mix non-bales and those with no neighbors
            counted = [.. grid.Select(line => line.Select(c => -1).ToArray())];

            int maxY = grid.Length;
            int maxX = grid[0].Length;

            // For each spot: we check half of the directions: right, down-right, down, and down-left
            // We add the count to each bale (source and destination)
            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    // if (y > 0 && x > 13) System.Diagnostics.Debugger.Break();
                    // Found a bale
                    if (grid[y][x] == '@')
                    {
                        // Offset the -1 start
                        counted[y][x]++;

                        directions.ForEach(dir =>
                        {
                            // Add to a bale count if that other square is a bale
                            if (0 <= y + dir.y && y + dir.y < maxY && 0 <= x + dir.x && x + dir.x < maxX && grid[y + dir.y][x + dir.x] == '@')
                            {
                                counted[y][x]++;
                                counted[y + dir.y][x + dir.x]++;
                            }
                        });
                    }
                }
            }
        }

        private (int x, int y)[] ValidMoves(int limit = 4)
        {
            // Need to ensure we pass the x value through Where (.Where().Select() breaks the index)
            return [.. counted.SelectMany((line, y) => line.Select((c, x) => (x, y, c)).Where(itm => 0 <= itm.c && itm.c < limit).Select(itm => (itm.x, itm.y)))];
        }

        protected override string? SolvePartOne()
        {
            // Time  : 00:00:00.0138389
            return ValidMoves().Length.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

