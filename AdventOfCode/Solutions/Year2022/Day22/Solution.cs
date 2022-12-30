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

        public enum Direction
        {
            Right,
            Down,
            Left,
            Up
        }

        public Day22() : base(22, 2022, "Monkey Map")
        {
//             DebugInput = @"        ...#
//         .#..
//         #...
//         ....
// ...#.......#
// ........#...
// ..#....#....
// ..........#.
//         ...#....
//         .....#..
//         .#......
//         ......#.

// 10R5L5R10L4R5L5";

            var steps = Input.SplitByBlankLine()[1][0];

            grid = ReadGrid();

            // Start in the first spot in the top row
            start = new(FindRowStart(0), 0);

            // Start by heading right
            deltaIndex = (int)Direction.Right;

            current = start;
            UpdateArrow();

            // This matches all but a lone distance at the end
            var pattern = new Regex(@"((?<distance>[0-9]+)|(?<direction>[LR]))");
            var patternEnd = new Regex(@"[LR](?<distance>[0-9]+)$");

            var matches = pattern.Matches(steps);
            var matchEnd = patternEnd.Match(steps);

            instructions = new();

            foreach (Match match in matches)
            {
                // These are matches in <distance><direction>
                if (!string.IsNullOrEmpty(match.Groups["distance"].Value))
                    instructions.Add(match.Groups["distance"].Value);

                if (!string.IsNullOrEmpty(match.Groups["direction"].Value))
                    instructions.Add(match.Groups["direction"].Value);
            }

            //  Then add the final
            if (matchEnd.Success)
                instructions.Add(matchEnd.Groups["distance"].Value);
        }

        private Point<int> FindNextFlat(Point<int> pos, Point<int> direction)
        {
            // Found a space, keep going...
            do
            {
                pos += direction;

                var g = GetGrid(pos);
                if (g.HasValue && g.Value != ' ')
                    return pos;
            } while (IsInGrid(pos));

            do
            {
                // We have rolled off the grid somewhere, loop us back
                if (direction.x == 0)
                {
                    // Under zero, bring us back to the bottom
                    if (pos.y < 0)
                    {
                        pos.y = grid.Length - 1;
                    }

                    // Over length, bring us back to the top
                    if (pos.y >= grid.Length)
                    {
                        pos.y = 0;
                    }
                }

                if (direction.y == 0)
                {
                    // Under zero, bring us back to the right
                    if (pos.x < 0)
                    {
                        pos.x = grid[pos.y].Length - 1;
                    }

                    // Over length, bring us back to the left
                    if (pos.x >= grid[pos.y].Length)
                    {
                        pos.x = 0;
                    }
                }

                var g = GetGrid(pos);
                if (g.HasValue && g.Value != ' ')
                    return pos;

                // Might have found another space
                pos += direction;
            } while (true);
        }

        private (Point<int> newPos, int newDeltaIndex) FindNextCube(Point<int> pos, Point<int> direction)
        {
            /*
             *   AB
             *   C
             *  DE
             *  F
             * 
             *   F
             *  DAB
             *   C
             * 
             *   A
             *  DCB
             *   E
             * 
             *   C
             *  DEB
             *   F
             *
             * Start facing => end facing
             * A up => F right
             * A left => D right
             * 
             * B down => C left
             * B up => F up
             * B right => E left
             * 
             * C left => D down
             * C right => B up
             * 
             * D up => C right
             * D left => A right
             * 
             * E down => F left
             * E right => B left
             * 
             * F down => B down
             * F left => A down
             * F right => E up
             *  
            */

            pos += direction;

            if (IsInGrid(pos))
            {
                var g = GetGrid(pos);

                if (g.HasValue && g != ' ')
                    return (pos, deltaIndex);
            }

            // We have moved outside one of our spaces
            // Maybe A or B went up
            if (pos.y < 0)
            {
                if (pos.x >= 100)
                {
                    // B up => F up
                    return (new(pos.x - 100, 199), (int)Direction.Up);
                }

                // A up => F right
                return (new(0, pos.x + 100), (int)Direction.Right);
            }

            // C or E to the right
            if (100 <= pos.x && pos.x < 150 && deltaIndex == (int)Direction.Right)
            {
                if (pos.y >= 100)
                {
                    // E right => B left
                    return (new(149, 149 - pos.y), (int)Direction.Left);
                }

                // C right => B up
                return (new(pos.y + 50, 49), (int)Direction.Up);
            }

            // D or F to the left
            if (pos.x < 0)
            {
                if (pos.y >= 150)
                {
                    // F left => A down
                    return (new(pos.y - 100, 0), (int)Direction.Down);
                }

                // D left => A right
                return (new(50, 149 - pos.y), (int)Direction.Right);
            }

            // E down
            if (pos.y >= 150 && pos.x >= 50 && deltaIndex == (int)Direction.Down)
            {
                // E down => F left
                return (new(49, pos.x + 100), (int)Direction.Left);
            }

            // B down or B right
            if (pos.x >= 100)
            {
                // B down => C left
                if (deltaIndex == (int)Direction.Down)
                    return (new(99, pos.x - 50), (int)Direction.Left);

                // B right => E left
                return (new(99, 149 - pos.y), (int)Direction.Left);
            }

            // D up
            if (0 <= pos.x && pos.x < 50 && pos.y < 100 && deltaIndex == (int)Direction.Up)
            {
                // D up => C right
                return (new(50, 50 + pos.x), (int)Direction.Right);
            }

            // F down
            if (pos.y > 199)
            {
                // F down => B down
                return (new(pos.x + 100, 0), (int)Direction.Down);
            }

            // F right
            if (pos.y >= 150)
            {
                // F down => E up
                return (new(pos.y - 100, 149), (int)Direction.Up);
            }

            // Remaining are:
            // A and C left
            if (pos.y <= 49)
            {
                // A left => D right
                return (new(0, 149 - pos.y), (int)Direction.Right);
            }

            // C left => D down
            return (new(pos.y - 50, 100), (int)Direction.Down);
        }

        private bool IsInGrid(Point<int> pos)
        {
            return pos.y >= 0 && pos.y < grid.Length && pos.x >= 0 && pos.x < grid[pos.y].Length;
        }

        private char? GetGrid(Point<int> pos)
        {
            if (IsInGrid(pos))
                return grid[pos.y][pos.x];

            return default;
        }

        private char[][] ReadGrid() => ReadGrid(string.Join('\n', Input.SplitByBlankLine()[0]));

        private char[][] ReadGrid(string input)
        {
            // Read the input into [y][x] char array
            var arr = input.SplitByNewline()
                .Select(line => line.ToCharArray())
                .ToArray();

            // For Part 1, pad so all lines are the same length
            var len = arr.Max(line => line.Length);

            arr = arr
                .Select(line => line.Length < len ? line.Concat(Enumerable.Repeat(' ', len - line.Length)).ToArray() : line)
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

        private void RunInstructions(int part = 1)
        {
            // const bool printGrid = false;
            // if (part == 2 && printGrid) PrintGrid();
            // int count = 0;

            var steps = new Queue<string>(instructions);
            while (steps.Count > 0)
            {
                var step = steps.Dequeue();

                // if (part == 2 && printGrid) Console.WriteLine($"Step: {step}");

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
                    Point<int> newPos = default!;
                    var newDeltaIndex = deltaIndex;

                    if (part == 1)
                        newPos = FindNextFlat(current, deltas[deltaIndex]);
                    else
                        (newPos, newDeltaIndex) = FindNextCube(current, deltas[deltaIndex]);

                    // If this is a wall, end out
                    if (GetGrid(newPos) == '#')
                        break;

                    var temp = current;

                    // Otherwise, it's a valid step
                    current = newPos;

                    // In Part 2, there are edge cases where we stop on an edge and don't
                    // change our direction
                    if (part == 2)
                        deltaIndex = newDeltaIndex;

                    UpdateArrow();

                    // if (part == 2 && count++ < 6000)
                    //     Console.WriteLine($"({current.x},{current.y}) {(Direction)deltaIndex}");
                }

                // if (part == 2 && printGrid) PrintGrid();

                // if (part == 2 && printGrid && count++ > 30) break;
            }
        }

        private int GetHash()
        {
            return ((current.y + 1) * 1000) + ((current.x + 1) * 4) + deltaIndex;
        }

        protected override string? SolvePartOne()
        {
            RunInstructions();

            return GetHash().ToString();
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
            grid = ReadGrid();
            current = start;
            deltaIndex = (int)Direction.Right;
            UpdateArrow();

            RunInstructions(2);

            return GetHash().ToString();
        }
    }
}

