using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2017
{

    class Day19 : ASolution
    {
        private char[][] grid;

        private string CollectedLetters = string.Empty;

        private int x = 0;
        private int y = 0;
        private char dir = 'D';   // Up, Down, Left, Right

        private int steps = 0;

        public Day19() : base(19, 2017, "A Series of Tubes")
        {
//             DebugInput = @"     |          
//      |  +--+    
//      A  |  C    
//  F---|----E|--+ 
//      |  |  |  D 
//      +B-+  +--+ ";

            // Load the grid
            this.grid = Input.SplitByNewline(false, false).Select(line => line.ToCharArray()).Where(chArr => chArr.Length > 0).ToArray();

            // Find the starting point (y is always zero)
            this.x = Enumerable.Range(0, this.grid[0].Length).First(index => this.grid[0][index] == '|');
        }

        private char GetPoint(int x, int y)
        {
            if (y < 0 || y >= this.grid.Length)
                return ' ';

            if (x < 0 || x >= this.grid[0].Length)
                return ' ';

            return this.grid[y][x];
        }

        private bool Run()
        {
            // Get the character in our next pos
            int nextX = this.dir == 'L' ? this.x - 1 : (this.dir == 'R' ? this.x + 1 : this.x);
            int nextY = this.dir == 'U' ? this.y - 1 : (this.dir == 'D' ? this.y + 1 : this.y);

            var next = GetPoint(nextX, nextY);

            // If this is a turn, find the next-next position and change our direction accordingly
            if (next == '+')
            {
                // Find the proper next position
                // There should only be one exit from this turn that isn't our current position
                if (this.dir == 'U' || this.dir == 'D')
                {
                    // Look left/right
                    if (GetPoint(nextX-1, nextY) != ' ')
                    {
                        // Change direction to the left
                        this.dir = 'L';
                    }
                    else
                    {
                        this.dir = 'R';
                    }
                }
                else
                {
                    // Look up/down
                    if (GetPoint(nextX, nextY-1) != ' ')
                    {
                        // Change direction to up
                        this.dir = 'U';
                    }
                    else
                    {
                        this.dir = 'D';
                    }
                }
            }
            else if (next == '|' || next == '-')
            {
                // Move on...
            }
            else if (next >= 'A' && next <= 'Z')
            {
                // Found a letter, keep moving
                this.CollectedLetters += next;
            }
            else if (next == ' ')
            {
                // The end!
                return false;
            }

            this.x = nextX;
            this.y = nextY;

            return true;
        }

        protected override string? SolvePartOne()
        {
            var ret = true;

            do
            {
                steps++;
                ret = Run();
            } while (ret);

            return this.CollectedLetters;
        }

        protected override string? SolvePartTwo()
        {
            return this.steps.ToString();
        }
    }
}

