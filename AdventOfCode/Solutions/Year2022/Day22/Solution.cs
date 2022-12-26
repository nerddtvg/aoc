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
        private Point<int> start;
        private Point<int> current;
        private int deltaIndex = 0;
        private List<string> instructions;

        /// <summary>
        /// Used to change our <see cref="direction" /> value
        /// </summary>
        private Point<int>[] deltas = new Point<int>[]
        {
            // Right
            new(1, 0),
            // Down
            new(0, 1),
            // Left
            new(-1, 0),
            // Up
            new(0, -1)
        };

        public Day22() : base(22, 2022, "Monkey Map")
        {
            DebugInput = @"        ...#
        .#..
        #...
        ....
...#.......#
........#...
..#....#....
..........#.
        ...#....
        .....#..
        .#......
        ......#.

10R5L5R10L4R5L5";

            var map = string.Join('\n', Input.SplitByBlankLine()[0]);
            var steps = Input.SplitByBlankLine()[1][0];

            grid = ReadGrid(map);

            // Start in the first spot in the top row
            start = new(FindRowStart(0), 0);

            // Start by heading right
            deltaIndex = 0;

            // Make sure we're not in a wall
            while(GetGrid(start) == '#')
            {
                start = FindNext(start, deltas[deltaIndex]);
            }

            current = start;

            // This matches all but a lone distance at the end
            var pattern = new Regex(@"((?<distance>[0-9]+)(?<direction>[LR]))");
            var patternEnd = new Regex(@"[LR](?<distance>[0-9]+)$");

            var matches = pattern.Matches(steps);
            var matchEnd = patternEnd.Match(steps);

            instructions = new();

            foreach (Match match in matches)
            {
                // These are matches in <distance><direction>
                instructions.Add(match.Groups["distance"].Value);
                instructions.Add(match.Groups["direction"].Value);
            }

            //  Then add the final
            if (matchEnd.Success)
                instructions.Add(matchEnd.Groups["distance"].Value);
        }

        private Point<int> FindNext(Point<int> pos, Point<int> direction)
        {
            pos += direction;

            // See if we're in the grid
            if (IsInGrid(pos))
            {
                // If this is not a space...
                if (GetGrid(pos) != ' ')
                    return pos;

                // Found a space, keep going...
                do
                {
                    pos += direction;

                    if (GetGrid(pos) != ' ')
                        return pos;
                } while (IsInGrid(pos));
            }

            do
            {
                // We have rolled off the grid somewhere, loop us back
                // Under zero, bring us back to the bottom
                while (pos.y < 0)
                {
                    pos.y += FindColEnd(pos.x) + 1;
                }

                // Over length, bring us back to the top
                while (pos.y >= grid.Length)
                {
                    pos.y %= grid.Length;
                }

                // Under zero, bring us back to the right
                while (pos.x < 0)
                {
                    pos.x += FindRowEnd(pos.y);
                }

                // Over length, bring us back to the left
                while (pos.x >= grid[pos.y].Length)
                {
                    pos.x %= grid[pos.y].Length;
                }

                if (GetGrid(pos) != ' ')
                    return pos;

                // Might have found another space
                pos += direction;
            } while (true);
        }

        private bool IsInGrid(Point<int> pos)
        {
            return pos.y >= 0 && pos.y < grid.Length && pos.x >= 0 && pos.x < grid[pos.y].Length;
        }

        private char GetGrid(Point<int> pos)
        {
            if (IsInGrid(pos))
                return grid[pos.y][pos.x];

            return ' ';
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
                    return y;
            }

            throw new Exception();
        }

        protected override string? SolvePartOne()
        {
            PrintGrid();

            var steps = new Queue<string>(instructions);
            while(steps.Count > 0)
            {
                var step = steps.Dequeue();

                if (step == "R" || step == "L")
                {
                    // Change direction now
                    if (step == "R")
                        deltaIndex++;
                    else
                        deltaIndex--;

                    if (deltaIndex < 0)
                        deltaIndex += deltas.Length;

                    deltaIndex %= deltas.Length;

                    UpdateArrow();

                    continue;
                }

                var distance = Int32.Parse(step);

                for (int i = 0; i < distance; i++)
                {
                    var newPos = FindNext(current, deltas[deltaIndex]);

                    // If this is a wall, end out
                    if (GetGrid(newPos) == '#')
                        break;

                    // Otherwise, it's a valid step
                    current = newPos;

                    UpdateArrow();
                }

                PrintGrid();
            }

            return (((current.y + 1) * 1000) + ((current.x + 1) * 4) + deltaIndex).ToString();
        }

        private void UpdateArrow()
        {
            switch (deltaIndex)
            {
                case 0:
                    grid[current.y][current.x] = '>';
                    break;

                case 1:
                    grid[current.y][current.x] = 'v';
                    break;

                case 2:
                    grid[current.y][current.x] = '<';
                    break;

                case 3:
                    grid[current.y][current.x] = '^';
                    break;
            }
        }

        private void PrintGrid()
        {
            Console.WriteLine(grid.Select(line => $"{line.JoinAsString()}\n").JoinAsString());
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

