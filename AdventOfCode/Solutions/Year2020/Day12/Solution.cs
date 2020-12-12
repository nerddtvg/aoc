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

        // Part 2
        int wx, wy;

        public Day12() : base(12, 2020, "")
        {
            /*
            DebugInput = @"
            F10
            N3
            F7
            R90
            F11";
            */

            x = 0;
            y = 0;
            dir = ShipDirection.East;
        }

        private (string instruction, int value) ParseInstruction(string line) =>
            (line.Substring(0, 1), Int32.Parse(line.Substring(1)));

        private void MoveShip((string instruction, int value) input) {
            if (input.instruction == "R" || input.instruction == "L") {
                // Turn one way or another as many times as we can divide 90 into it
                int v = ((((input.instruction == "L" ? -1 : 1) * (input.value / 90)) + (int) dir) % 4);

                while (v < 0) v += 4;

                dir = (ShipDirection) v;
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

        private void MoveShip2((string instruction, int value) input) {
            if (input.instruction == "R" || input.instruction == "L") {
                // We need to rotate the waypoint around the ship
                // Count how many times (and what rotation [pos/neg])
                int v = ((input.instruction == "L" ? -1 : 1) * (input.value / 90));
                
                Console.Write($"Rotating waypoint {input.instruction}{input.value} from ({wx}, {wy}) to ");

                // Convert to positive degrees
                while(v < 0) v += 4;

                for(int i=0; i<v; i++) {
                    // Rotating around 90 degrees positive is... (doesn't follow Cartesian plane because we are rotated)
                    // x = -y
                    // y = x

                    int tx = -1 * wy;
                    int ty = wx;

                    wx = tx;
                    wy = ty;
                }
                
                Console.WriteLine($"({wx}, {wy})");
            } else {
                // This is a movement instruction
                switch(input.instruction) {
                    case "N":
                        Console.Write($"Moved waypoint {input.instruction}{input.value} from ({wx}, {wy}) to ");
                        MoveWaypoint(input.value, ShipDirection.North);
                        Console.WriteLine($"({wx}, {wy})");
                        break;
                    
                    case "S":
                        Console.Write($"Moved waypoint {input.instruction}{input.value} from ({wx}, {wy}) to ");
                        MoveWaypoint(input.value, ShipDirection.South);
                        Console.WriteLine($"({wx}, {wy})");
                        break;
                    
                    case "E":
                        Console.Write($"Moved waypoint {input.instruction}{input.value} from ({wx}, {wy}) to ");
                        MoveWaypoint(input.value, ShipDirection.East);
                        Console.WriteLine($"({wx}, {wy})");
                        break;
                    
                    case "W":
                        Console.Write($"Moved waypoint {input.instruction}{input.value} from ({wx}, {wy}) to ");
                        MoveWaypoint(input.value, ShipDirection.West);
                        Console.WriteLine($"({wx}, {wy})");
                        break;
                    
                    case "F":
                        // Now we move in the direction of the waypoint * value times
                        Console.Write($"Moved ship {input.instruction}{input.value} from ({x}, {y}) [({wx}, {wy})] to ");
                        x += wx * input.value;
                        y += wy * input.value;
                        Console.WriteLine($"({x}, {y})");
                        break;
                }
            }
        }

        private void MoveWaypoint(int value, ShipDirection direction) {
            switch(direction) {
                case ShipDirection.North:
                    wy -= value;
                    break;
                
                case ShipDirection.South:
                    wy += value;
                    break;
                
                case ShipDirection.East:
                    wx += value;
                    break;
                
                case ShipDirection.West:
                    wx -= value;
                    break;
            }
        }

        protected override string SolvePartOne()
        {
            foreach(string line in Input.SplitByNewline(true, true))
                MoveShip(ParseInstruction(line));

            return (Math.Abs(x) + Math.Abs(y)).ToString();
        }

        protected override string SolvePartTwo()
        {
            // Start back at 0,0
            x = 0;
            y = 0;

            // Waypoint is at 10,-1 (10 east, 1 north of ship)
            wx = 10;
            wy = -1;

            foreach(string line in Input.SplitByNewline(true, true))
                MoveShip2(ParseInstruction(line));

            return (Math.Abs(x) + Math.Abs(y)).ToString();
        }
    }
}
