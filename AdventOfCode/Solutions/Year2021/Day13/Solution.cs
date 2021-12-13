using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2021
{

    class Day13 : ASolution
    {
        private Dictionary<(int x, int y), bool> grid = new Dictionary<(int x, int y), bool>();
        private List<string> instructions = new List<string>();

        public Day13() : base(13, 2021, "Transparent Origami")
        {
//             DebugInput = @"6,10
// 0,14
// 9,10
// 0,3
// 10,4
// 4,11
// 6,0
// 6,12
// 4,1
// 0,13
// 10,12
// 3,4
// 3,0
// 8,4
// 1,10
// 2,14
// 8,10
// 9,0

// fold along y=7
// fold along x=5";

            var split = Input.SplitByBlankLine();

            // Load the paper
            foreach(var line in split[0])
            {
                var s = line.Split(',');
                this.grid[(Int32.Parse(s[0]), Int32.Parse(s[1]))] = true;
            }

            instructions = split[1].ToList();
        }

        private void Run(string line)
        {
            var split = line.Split('=');
            var dir = split[0][split[0].Length - 1];
            var val = Int32.Parse(split[1]);

            // For this problem, we can assume some things:
            // 1. We will always fold on a blank line
            // 2. We will always fold in the middle of the sheet in either direction
            // 3. The fold line goes away

            var maxX = this.grid.Max(kvp => kvp.Key.x);
            var maxY = this.grid.Max(kvp => kvp.Key.y);

            var newGrid = new Dictionary<(int x, int y), bool>();

            // We only have to loop half of the grid
            if (dir == 'x')
                maxX = val;
            else
                maxY = val;

            for (int y = 0; y <= maxY; y++)
            {
                for (int x = 0; x <= maxX; x++)
                {
                    var pt1 = (x, y);
                    var pt2 = ((dir == 'x' ? maxX + val - x : x), (dir == 'y' ? maxY + val - y : y));

                    newGrid[pt1] = (this.grid.ContainsKey(pt1) && this.grid[pt1]) || (this.grid.ContainsKey(pt2) && this.grid[pt2]);
                }
            }

            // Remove the extra line
            if (dir == 'x')
                newGrid.Where(kvp => kvp.Key.x == maxX).Select(kvp => kvp.Key).ToList().ForEach(key => newGrid.Remove(key));
            else
                newGrid.Where(kvp => kvp.Key.y == maxY).Select(kvp => kvp.Key).ToList().ForEach(key => newGrid.Remove(key));

            // Set the new grid
            this.grid = newGrid;
        }

        protected override string? SolvePartOne()
        {
            Run(this.instructions[0]);

            return this.grid.Count(kvp => kvp.Value).ToString();
        }

        private void PrintGrid()
        {
            var maxX = this.grid.Max(kvp => kvp.Key.x);
            var maxY = this.grid.Max(kvp => kvp.Key.y);

            // The grid may not be fully formed (keys may only be for dots)
            // So we can't simply check the count
            if (maxX * maxY > 1000) return;

            for (int y = 0; y <= maxY; y++)
            {
                for (int x = 0; x <= maxX; x++)
                {
                    if (!this.grid.ContainsKey((x, y)) || !this.grid[(x,y)])
                        Console.Write(".");
                    else
                        Console.Write("#");
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }

        protected override string? SolvePartTwo()
        {
            foreach(var line in this.instructions.Skip(1))
            {
                Run(line);
            }

            PrintGrid();

            return "See Printed Grid";
        }
    }
}

#nullable restore
