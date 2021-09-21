using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{
    enum Direction
    {
        North   = 0,
        East    = 1,
        South   = 2,
        West    = 3
    }

    class Day01 : ASolution
    {
        public (int x, int y) pos { get; set; } = (0, 0);
        public Direction dir { get; set; } = Direction.North;

        private List<(int x, int y)> stack = new List<(int x, int y)>();

        private (int x, int y) firstDup = (0, 0);

        public Day01() : base(01, 2016, "")
        {
            /*
            DebugInput = "R2, L3";
            ResetPos();
            ProcessInput();
            Console.WriteLine($"Sample '{Input}': {Utilities.ManhattanDistance((0, 0), pos)}");
            
            DebugInput = "R2, R2, R2";
            ResetPos();
            ProcessInput();
            Console.WriteLine($"Sample '{Input}': {Utilities.ManhattanDistance((0, 0), pos)}");
            
            DebugInput = "R5, L5, R5, R3";
            ResetPos();
            ProcessInput();
            Console.WriteLine($"Sample '{Input}': {Utilities.ManhattanDistance((0, 0), pos)}");
            
            DebugInput = "R2, R2, R2, R2";
            ResetPos();
            ProcessInput();
            Console.WriteLine($"Sample '{Input}': {Utilities.ManhattanDistance((0, 0), pos)}");
            */
            
            DebugInput = "R8, R4, R4, R8";
            ResetPos();
            ProcessInput();
            Console.WriteLine($"Part 2 Sample '{Input}': {Utilities.ManhattanDistance((0, 0), this.firstDup)}");

            // Real work
            DebugInput = null;
            ResetPos();
            ProcessInput();
        }

        private void ResetPos()
        {
            this.dir = Direction.North;
            this.pos = (0, 0);
            this.firstDup = (0, 0);

            // A dictionary works better for the key handling/searching
            this.stack = new List<(int x, int y)>();
        }

        private void ProcessInput()
        {
            // Our directions!
            var regex = new Regex("([LR])([0-9]+)+");
            var matches = regex.Matches(Input);

            bool foundFirst = false;

            // For each of these, figure out our position starting from 0,0 facing north (0)
            foreach(Match match in matches)
            {
                // Change direction (keep it from going negative for modulus math)
                int newDir = ((int)dir + (match.Groups[1].Value == "L" ? 3 : 1));
                dir = (Direction)(newDir % 4);

                (int x, int y) move = (0, 0);

                var steps = Int32.Parse(match.Groups[2].Value);

                switch(dir)
                {
                    case Direction.North:
                        move.y = steps;
                        break;
                        
                    case Direction.East:
                        move.x = steps;
                        break;
                        
                    case Direction.South:
                        move.y = -1 * steps;
                        break;
                        
                    case Direction.West:
                        move.x = -1 * steps;
                        break;
                }

                // Move!
                var tempPos = this.pos.Add(move);

                // Check if we found this twice
                if (!foundFirst)
                {
                    foreach(var pt in this.pos.GetPointsBetweenInclusive(tempPos).Skip(1))
                    {
                        if (this.stack.Contains(pt))
                        {
                            foundFirst = true;
                            this.firstDup = pt;
                            break;
                        }

                        // Add it to the stack
                        this.stack.Add(pt);
                    }
                }

                this.pos = tempPos;
            }
        }

        protected override string SolvePartOne()
        {
            return Utilities.ManhattanDistance((0, 0), this.pos).ToString();
        }

        protected override string SolvePartTwo()
        {
            return Utilities.ManhattanDistance((0, 0), this.firstDup).ToString();
        }
    }
}
