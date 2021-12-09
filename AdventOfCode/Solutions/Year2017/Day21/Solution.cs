using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2017
{

    class Day21 : ASolution
    {
        private static string StartingGrid = ".#...####";
        private string grid = string.Empty;
        private Dictionary<string, string> inPatterns = new Dictionary<string, string>();

        public Day21() : base(21, 2017, "Fractal Art")
        {
            this.grid = Day21.StartingGrid;

//             DebugInput = @"../.# => ##./#../...
// .#./..#/### => #..#/..../..../#..#";

            this.inPatterns = Input.SplitByNewline()
                .ToDictionary(line => line.Split("=>", StringSplitOptions.TrimEntries)[0], line => line.Split("=>", StringSplitOptions.TrimEntries)[1]);
        }

        private void Run()
        {
            // For each turn:
            // Determine the grid size sideLength % 3 or sideLength % 2
            // Break up the patterns into their strings with '/' in between
            // Determine each replacement and position in the grid
            // Reconstitute the grid string
            var side = (int) Math.Sqrt(this.grid.Length);
            var size = (side % 2) == 0 ? 2 : 3;

            var patterns = new string[(side / size)][];

            // This converts our string into "rows"
            var splitGrid = this.grid.SplitBySize(side).ToArray();

            // We can now grab each of the patterns out of string
            for (int y = 0; y < (side/size); y++)
            {
                // Initiate our new row
                patterns[y] = new string[(side / size)];

                for (int x = 0; x < (side/size); x++)
                {
                    var t = new string[8];

                    for (int dy = 0; dy < size; dy++)
                    {
                        for (int dx = 0; dx < size; dx++)
                        {
                            // No rotation
                            t[0] += splitGrid[((y * size) + dy)][(x * size) + dx];
                            //t[0] += this.grid[((y * size * side) + (dy * size)) + (x * size * side) + dx];

                            // 90 rotation
                            t[1] += splitGrid[((y * size) + dx)][(x * size) + (size - 1 - dy)];
                            // t[1] += this.grid[((x * size * side) + (dx * size)) + (y * size * side) + (size - 1 - dy)];

                            // 180 rotation
                            t[2] += splitGrid[((y * size) + (size - 1 - dy))][(x * size) + (size - 1 - dx)];
                            // t[2] += this.grid[((y * size * side) + ((size - 1 - dy) * size)) + (x * size * side) + (size - 1 - dx)];

                            // 270 rotation
                            t[3] += splitGrid[(y * size) + (size - 1 - dx)][((x * size) + dy)];
                            // t[3] += this.grid[((x * size * side) + ((size - 1 - dx) * size)) + (y * size * side) + dy];
                        }

                        t[0] += ' ';
                        t[1] += ' ';
                        t[2] += ' ';
                        t[3] += ' ';
                    }

                    // Now we need to flip each pattern as well
                    for (int i = 0; i < 4; i++)
                    {
                        t[i + 4] = string.Join("/", t[i].Trim().Split(' ').Select(line => line.Reverse()));
                    }

                    // Get the pattern
                    t = t.Select(p => p.Trim().Replace(' ', '/')).ToArray();

                    // Find the match
                    var match = t.First(p => this.inPatterns.ContainsKey(p));
                    patterns[y][x] = this.inPatterns[match];
                }
            }

            // Now we have our final patterns for this round, let's reconstitute it
            var newGrid = string.Empty;

            for (int y = 0; y < (side/size); y++)
            {
                // This is an array of each of the lines in this row-set
                var lines = new string[patterns[y][0].Split('/')[0].Length];

                for (int x = 0; x < (side/size); x++)
                {
                    var tLines = patterns[y][0].Split('/').ToArray();

                    for (int i = 0; i < tLines.Length; i++)
                    {
                        lines[i] += tLines[i];
                    }
                }

                // Append each of these new rows into the new grid
                newGrid += lines.JoinAsString();
            }

            // Save it
            this.grid = newGrid;
        }

        protected override string? SolvePartOne()
        {
            Utilities.Repeat(() => Run(), 5);

            Console.WriteLine(string.Join("\r\n", this.grid.SplitBySize((int)Math.Sqrt(this.grid.Length))));

            return this.grid.Count(ch => ch == '#').ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
