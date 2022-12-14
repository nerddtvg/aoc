using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2022
{

    class Day14 : ASolution
    {
        char[][] grid = Array.Empty<char[]>();
        (int x, int y) start = (500, 0);

        public Day14() : base(14, 2022, "Regolith Reservoir")
        {
            // Initialize a blank grid of dots
            grid = Enumerable.Range(0, 1000)
                .Select(y => Enumerable.Range(0, 1000).Select(x => '.').ToArray())
                .ToArray();

            // The start is at 500, 0
            grid[start.y][start.x] = '+';

            // Draw the grid
            foreach(var line in Input.SplitByNewline())
            {
                // Gather all of our line segments
                var points = line
                    .Split(" -> ", StringSplitOptions.TrimEntries)
                    .Select(pt =>
                    {
                        var s = pt.Split(',');
                        return (Int32.Parse(s[0]), Int32.Parse(s[1]));
                    })
                    .ToArray();

                for (int i = 0; i < points.Length - 1; i++)
                {
                    // Draw from points[i] to points[i+1]
                    var p = points[i].GetPointsBetweenInclusive(points[i + 1]).ToList();

                    p.ForEach(pt => grid[pt.Item2][pt.Item1] = '#');
                }
            }
        }

        bool ProcessSand((int x, int y) pos)
        {
            // Start at pos and attempt to move:
            // down
            // down-left
            // down-right
            // settled

            if (grid[pos.y][pos.x] == 'o')
                return false;

            var newPos = pos.Add((0, 1));

            if (newPos.Item2 >= grid.Length) return false;

            if (grid[newPos.Item2][newPos.Item1] == '.')
            {
                // Found a direction
                return ProcessSand(newPos);
            }

            // No go, try down-left
            newPos = pos.Add((-1, 1));

            if (newPos.Item1 < 0) return false;

            if (grid[newPos.Item2][newPos.Item1] == '.')
            {
                // Found a direction
                return ProcessSand(newPos);
            }

            // No go, try down-right
            newPos = pos.Add((1, 1));

            if (newPos.Item1 >= grid[0].Length) return false;

            if (grid[newPos.Item2][newPos.Item1] == '.')
            {
                // Found a direction
                return ProcessSand(newPos);
            }

            // Settled
            grid[pos.y][pos.x] = 'o';

            return true;
        }

        private void PrintGrid()
        {
            foreach(var line in grid)
                Console.WriteLine(line.JoinAsString());
        }

        protected override string? SolvePartOne()
        {
            // Run until we get false (indicating we have fallen outside the grid)
            while(ProcessSand(start))
            {
                // Look pretty here
            }

            PrintGrid();

            return grid.Sum(line => line.Count(c => c == 'o')).ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Find the lowest '#' and change the end of the grid to that +2
            int maxY = grid.Length;
            for (int i = grid.Length - 1; i > 0; i--)
            {
                if (grid[i].Any(c => c == '#'))
                {
                    maxY = i;
                    break;
                }
            }

            grid = grid
                .Take(maxY + 2)
                // Add a wall
                .Append(Enumerable.Range(0, grid[0].Length).Select(c => '#').ToArray())
                .ToArray();

            // Run until we get false (indicating we have fallen outside the grid)
            while (ProcessSand(start))
            {
                // Look pretty here
            }

            Console.WriteLine();

            PrintGrid();

            return grid.Sum(line => line.Count(c => c == 'o')).ToString();
        }
    }
}

