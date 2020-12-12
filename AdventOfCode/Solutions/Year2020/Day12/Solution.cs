using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{
    enum ShipDirection {
        // Starts facing east, +1 is 90 degrees right, -1 is 90 degrees left
        East,
        South,
        West,
        North
    }

    class Day12 : ASolution
    {
        int x, y;
        ShipDirection dir;

        public Day12() : base(12, 2020, "")
        {
            x = 0;
            y = 0;
            dir = ShipDirection.East;
        }

        private (string instruction, int value) ParseInstruction(string line) =>
            (line.Substring(0, 1), Int32.Parse(line.Substring(1)));

        private void MoveShip((string instruction, int value) input) {
            if (input.instruction == "R" || input.instruction == "L") {
                // Turn one way or another as many times as we can divide 90 into it
                dir = (ShipDirection) ((((input.instruction == "L" ? -1 : 1) * (input.value / 90)) + (int) dir) % 4);
            } else {
                // This is a movement instruction
                switch(input.instruction) {
                    case "N":
                        MoveDistance(input.value, ShipDirection.North);
                        break;
                    
                    case "S":
                        MoveDistance(input.value, ShipDirection.South);
                        break;
                    
                    case "E":
                        MoveDistance(input.value, ShipDirection.East);
                        break;
                    
                    case "W":
                        MoveDistance(input.value, ShipDirection.West);
                        break;
                    
                    case "F":
                        MoveDistance(input.value, dir);
                        break;
                    
                }
            }
        }

        private void MoveDistance(int value, ShipDirection direction) {
            switch(direction) {
                case ShipDirection.North:
                    y -= value;
                    break;
                
                case ShipDirection.South:
                    y += value;
                    break;
                
                case ShipDirection.East:
                    x += value;
                    break;
                
                case ShipDirection.West:
                    x -= value;
                    break;
                
            }
        }

        protected override string SolvePartOne()
        {
            foreach(string line in Input.SplitByNewline())
                MoveShip(ParseInstruction(line));

            return (Math.Abs(x) + Math.Abs(y)).ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
