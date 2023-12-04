using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Diagnostics;


namespace AdventOfCode.Solutions.Year2023
{

    class Day03 : ASolution
    {
        private char[][] grid;
        private List<int> partNumbers = new();

        public Day03() : base(03, 2023, "Gear Ratios")
        {
            grid = Input.SplitByNewline()
                .Select(line => line.ToCharArray())
                .ToArray();

            int curNumber = 0;
            bool validNumber = false;

            // Find all symbols
            string symbols = Input
                .ToCharArray()
                .Where(c => c != '.' && c != '\r' && c != '\n' && !('0' <= c && c <= '9'))
                .Distinct()
                .JoinAsString() ?? string.Empty;

            for (var y = 0; y < grid.Length; y++)
            {
                for(var x = 0; x<grid[0].Length; x++)
                {
                    // Read each character
                    // If it is a number, append it to the curNumber and scan for validity
                    // If it is anything else, ignore it
                    if ('0' <= grid[y][x] && grid[y][x] <= '9')
                    {
                        // Shift the number left and append the next digit
                        curNumber = (curNumber * 10) + (grid[y][x] - '0');

                        validNumber = validNumber ||
                            (
                                // Check left + up, same row, and down
                                x > 0 && (
                                    (
                                        y > 0 && symbols.Contains(grid[y - 1][x - 1])
                                    )
                                ||
                                    (
                                        symbols.Contains(grid[y][x - 1])
                                    )
                                ||
                                    (
                                        y < grid.Length - 1 && symbols.Contains(grid[y + 1][x - 1])
                                    )
                                )
                            )
                            ||
                            (
                                // Check up and down
                                (
                                    y > 0 && symbols.Contains(grid[y - 1][x])
                                )
                                ||
                                (
                                    y < grid.Length - 1 && symbols.Contains(grid[y + 1][x])
                                )
                            )
                            ||
                            (
                                // Check right + up, same row, and down
                                x < grid[0].Length - 1 && (
                                    (
                                        y > 0 && symbols.Contains(grid[y - 1][x + 1])
                                    )
                                ||
                                    (
                                        symbols.Contains(grid[y][x + 1])
                                    )
                                ||
                                    (
                                        y < grid.Length - 1 && symbols.Contains(grid[y + 1][x + 1])
                                    )
                                )
                            );
                    }
                    else
                    {
                        if (curNumber > 0 && validNumber)
                            // We've found a valid number by ending on a symbol or period
                            partNumbers.Add(curNumber);

                        curNumber = 0;
                        validNumber = false;
                    }
                }

                // Always reset at the end of the row
                if (curNumber > 0 && validNumber)
                    partNumbers.Add(curNumber);

                curNumber = 0;
                validNumber = false;
            }
        }

        protected override string? SolvePartOne()
        {
            return partNumbers.Sum().ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Part 2 needs to find all numbers adjacent to '*' symbols
            // Any two numbers adjacent are gears
            // Anything besides two numbers is ignored
            // For this is may be easier to find all gears and "remember"
            // what gear they are associated with
            int curNumber = 0;
            (int x, int y) foundGear = default;
            List<((int x, int y) pos, int gear)> gears = new();

            for (var y = 0; y < grid.Length; y++)
            {
                for (var x = 0; x < grid[0].Length; x++)
                {
                    if ('0' <= grid[y][x] && grid[y][x] <= '9')
                    {
                        // Shift the number left and append the next digit
                        curNumber = (curNumber * 10) + (grid[y][x] - '0');

                        if (x > 0)
                        {
                            if (y > 0 && grid[y - 1][x - 1] == '*')
                                foundGear = (x - 1, y - 1);
                            else if (grid[y][x - 1] == '*')
                                foundGear = (x - 1, y);
                            else if (y < grid.Length - 1 && grid[y + 1][x - 1] == '*')
                                foundGear = (x - 1, y + 1);
                        }

                        if (y > 0 && grid[y - 1][x] == '*')
                            foundGear = (x, y - 1);
                        else if (y < grid.Length - 1 && grid[y + 1][x] == '*')
                            foundGear = (x, y + 1);

                        if (x < grid[0].Length - 1)
                        {
                            if (y > 0 && grid[y - 1][x + 1] == '*')
                                foundGear = (x + 1, y - 1);
                            else if (grid[y][x + 1] == '*')
                                foundGear = (x + 1, y);
                            else if (y < grid.Length - 1 && grid[y + 1][x + 1] == '*')
                                foundGear = (x + 1, y + 1);
                        }
                    }
                    else
                    {
                        if (curNumber > 0 && foundGear != default)
                            gears.Add((foundGear, curNumber));

                        curNumber = 0;
                        foundGear = default;
                    }
                }

                // Always reset at the end of the row
                if (curNumber > 0 && foundGear != default)
                    gears.Add((foundGear, curNumber));

                curNumber = 0;
                foundGear = default;
            }

            return gears
                .GroupBy(gear => gear.pos)
                .Where(group => group.Count() == 2)
                .Sum(group => group.First().gear * group.Last().gear)
                .ToString();
        }
    }
}

