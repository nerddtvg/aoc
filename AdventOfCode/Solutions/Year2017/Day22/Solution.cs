using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2017
{

    class Day22 : ASolution
    {
        private enum Direction
        {
            Up,
            Right,
            Down,
            Left
        };

        private Dictionary<(int x, int y), bool> grid = new Dictionary<(int x, int y), bool>();

        int posX = 0;
        int posY = 0;
        Direction dir = Direction.Up;

        // Count infected
        int infectedCount = 0;

        public Day22() : base(22, 2017, "Sporifica Virus")
        {
            // DebugInput = "..#\n#..\n...";

            Reset();
        }

        private bool GetPoint((int x, int y) pos) => this.grid.ContainsKey(pos) ? this.grid[pos] : false;

        private Direction NewDir((int x, int y) pos)
        {
            // If this is clean, turn left
            // If this is infected, turn right
            if (GetPoint(pos))
            {
                // Infected
                return (Direction)((((int)this.dir) + 1) % 4);
            }

            // Darn modulo!
            var val = (((int)this.dir) - 1);
            if (val < 0) val += 4;

            return (Direction)val;
        }

        private (int x, int y) GetNewPoint((int x, int y) pos)
        {
            switch(this.dir)
            {
                case Direction.Up:
                    return (pos.x, pos.y - 1);
                    
                case Direction.Right:
                    return (pos.x + 1, pos.y);
                    
                case Direction.Down:
                    return (pos.x, pos.y + 1);
                    
                case Direction.Left:
                    return (pos.x - 1, pos.y);
                
                default:
                    throw new Exception();
            }
        }

        private void Reset()
        {
            this.grid.Clear();
            
            var lines = Input.SplitByNewline(true).ToList();

            int y = 0;
            foreach(var line in lines)
            {
                var x = 0;

                foreach(var c in line)
                {
                    this.grid[(x, y)] = c == '#';
                    x++;
                }

                y++;
            }

            // Get the mid point where we're actually starting (it's a square)
            this.posX = (int)Math.Ceiling(this.grid.Max(pt => pt.Key.x) / 2.0);
            this.posY = (int)Math.Ceiling(this.grid.Max(pt => pt.Key.y) / 2.0);
        }

        private void Run()
        {
            // Our position
            var pos = (this.posX, this.posY);

            // First turn
            this.dir = NewDir(pos);

            // Clean or infect
            this.grid[pos] = !GetPoint(pos);
            if (this.grid[pos])
                this.infectedCount++;

            // Move!
            (this.posX, this.posY) = GetNewPoint(pos);
        }

        protected override string? SolvePartOne()
        {
            Utilities.Repeat(() =>
            {
                Run();
            }, 10000);

            return this.infectedCount.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
