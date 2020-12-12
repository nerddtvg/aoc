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
        long x, y;
        ShipDirection dir;

        // Part 2
        long wx, wy;

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

                // Convert to positive degrees
                while(v < 0) v += 4;

                for(int i=0; i<v; i++) {
                    // Rotating around 90 degrees positive is...
                    // x = y
                    // y = -x

                    long tx = wy;
                    long ty = -1 * wx;

                    wx = tx;
                    wy = ty;
                }
            } else {
                // This is a movement instruction
                switch(input.instruction) {
                    case "N":
                        MoveWaypoint(input.value, ShipDirection.North);
                        break;
                    
                    case "S":
                        MoveWaypoint(input.value, ShipDirection.South);
                        break;
                    
                    case "E":
                        MoveWaypoint(input.value, ShipDirection.East);
                        break;
                    
                    case "W":
                        MoveWaypoint(input.value, ShipDirection.West);
                        break;
                    
                    case "F":
                        // Now we move in the direction of the waypoint * value times
                        x += wx * input.value;
                        y += wy * input.value;
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
            foreach(string line in Input.SplitByNewline())
                MoveShip(ParseInstruction(line));

            return (Math.Abs(x) + Math.Abs(y)).ToString();
        }

        protected override string SolvePartTwo()
        {
            // Start back at 0,0
            x = 0;
            y = 0;

            // Waypoint is at -10,1
            wx = 10;
            wy = -1;

            foreach(string line in Input.SplitByNewline())
                MoveShip2(ParseInstruction(line));

            return (Math.Abs(x) + Math.Abs(y)).ToString();
        }
    }
}
