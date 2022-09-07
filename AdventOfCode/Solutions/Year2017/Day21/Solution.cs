using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


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

            // DebugInput = "../.# => ##./#../...\n.#./..#/### => #..#/..../..../#..#";

            var inputPatterns = Input.SplitByNewline()
                .SelectMany(line =>
                {
                    var ret = new List<(string key, string val)>();

                    var split = line.Split("=>", 2, StringSplitOptions.TrimEntries);

                    ret.Add((split[0], split[1]));
                    ret.Add((transpose(ret[ret.Count - 1].key), split[1]));
                    ret.Add((flip(ret[ret.Count - 1].key), split[1]));
                    ret.Add((transpose(ret[ret.Count - 1].key), split[1]));
                    ret.Add((flip(ret[ret.Count - 1].key), split[1]));
                    ret.Add((transpose(ret[ret.Count - 1].key), split[1]));
                    ret.Add((flip(ret[ret.Count - 1].key), split[1]));
                    ret.Add((transpose(ret[ret.Count - 1].key), split[1]));

                    return ret;
                })
                .ToList();

            foreach (var item in inputPatterns)
            {
                if (!this.inPatterns.ContainsKey(item.key))
                    this.inPatterns.Add(item.key, item.val);
            }

            // Let's try flipping the keys here instead of the math later

        }

        // Based off /u/willkill07
        // https://old.reddit.com/r/adventofcode/comments/7l78eb/2017_day_21_solutions/drk4ohr/
        private string transpose(string input)
        {
            var lines = input.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var len = lines[0].Length;
            var maxIndex = lines[0].Length - 1;

            var outLines = new string[lines[0].Length];

            for (int y = 0; y <= maxIndex; y++)
            {

                for (int x = 0; x <= maxIndex; x++)
                {
                    outLines[y] += lines[maxIndex - x][maxIndex - y];
                }
            }

            return string.Join("/", outLines);
        }

        private string flip(string input)
        {
            return string.Join("/", input
                .Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Reverse());
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
                    var t = string.Empty;

                    for (int dy = 0; dy < size; dy++)
                    {
                        for (int dx = 0; dx < size; dx++)
                        {
                            // No rotation
                            t += splitGrid[((y * size) + dy)][(x * size) + dx];
                        }

                        if (dy != size-1)
                            t += '/';
                    }

                    // Find the match
                    patterns[y][x] = this.inPatterns.First(item => item.Key == t).Value;
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
                    var tLines = patterns[y][x].Split('/').ToArray();

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

        private void PrintGrid() => Console.WriteLine(string.Join("\r\n", this.grid.SplitBySize((int)Math.Sqrt(this.grid.Length))) + "\r\n");

        protected override string? SolvePartOne()
        {
            Utilities.Repeat(() =>
            {
                Run();
            }, 5);

            return this.grid.Count(ch => ch == '#').ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Run 18 times, already ran 5
            // This takes about 8 seconds

            Utilities.Repeat(() =>
            {
                Run();
            }, 13);

            return this.grid.Count(ch => ch == '#').ToString();
        }
    }
}

