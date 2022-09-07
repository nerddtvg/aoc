using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


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

        private enum State
        {
            Clean,
            Weakened,
            Infected,
            Flagged
        };

        private Dictionary<(int x, int y), bool> grid = new Dictionary<(int x, int y), bool>();
        private Dictionary<(int x, int y), State> grid2 = new Dictionary<(int x, int y), State>();

        int posX = 0;
        int posY = 0;
        Direction dir = Direction.Up;

        // Count infected
        int infectedCount1 = 0;
        int infectedCount2 = 0;

        public Day22() : base(22, 2017, "Sporifica Virus")
        {
            // DebugInput = "..#\n#..\n...";

            Reset();
        }

        private bool GetPoint((int x, int y) pos) => this.grid.ContainsKey(pos) ? this.grid[pos] : false;

        private State GetPoint2((int x, int y) pos) => this.grid2.ContainsKey(pos) ? this.grid2[pos] : State.Clean;

        private Direction NewDir((int x, int y) pos, int part)
        {
            // Easy, helpful directions
            var reverse = (Direction)((((int)this.dir) + 2) % 4);
            var right = (Direction)((((int)this.dir) + 1) % 4);
            var left = (Direction)((((int)this.dir) + 3) % 4);

            if (part == 1)
            {
                // If this is clean, turn left
                // If this is infected, turn right
                if (GetPoint(pos))
                {
                    // Infected
                    return right;
                }

                return left;
            }

            // Part 2 has different logic
            var point = GetPoint2(pos);

            if (point == State.Clean)
                return left;

            // No change
            if (point == State.Weakened)
                return this.dir;

            if (point == State.Infected)
                return right;

            return reverse;
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

            this.dir = Direction.Up;

            var lines = Input.SplitByNewline(true).ToList();

            int y = 0;
            foreach(var line in lines)
            {
                var x = 0;

                foreach(var c in line)
                {
                    this.grid[(x, y)] = c == '#';
                    this.grid2[(x, y)] = c == '#' ? State.Infected : State.Clean;
                    x++;
                }

                y++;
            }

            // Get the mid point where we're actually starting (it's a square)
            this.posX = (int)Math.Ceiling(this.grid.Max(pt => pt.Key.x) / 2.0);
            this.posY = (int)Math.Ceiling(this.grid.Max(pt => pt.Key.y) / 2.0);
        }

        private void Run(int part = 1)
        {
            // Our position
            var pos = (this.posX, this.posY);

            // First turn
            this.dir = NewDir(pos, part);

            // Clean or infect
            if (part == 1)
            {
                this.grid[pos] = !GetPoint(pos);
                if (this.grid[pos])
                    this.infectedCount1++;
            }
            else
            {
                this.grid2[pos] = (State)(((int)GetPoint2(pos) + 1) % 4);
                if (this.grid2[pos] == State.Infected)
                    this.infectedCount2++;
            }

            // Move!
            (this.posX, this.posY) = GetNewPoint(pos);
        }

        protected override string? SolvePartOne()
        {
            Utilities.Repeat(() =>
            {
                Run();
            }, 10000);

            return this.infectedCount1.ToString();
        }

        protected override string? SolvePartTwo()
        {
            Reset();

            Utilities.Repeat(() =>
            {
                Run(2);
            }, 10000000);

            return this.infectedCount2.ToString();
        }
    }
}

