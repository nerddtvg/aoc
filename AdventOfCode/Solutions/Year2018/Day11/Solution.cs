using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2018
{
    class FuelCell {
        public int x {get;set;}
        public int y {get;set;}
        public int gridSerialNumber {get;set;}

        public int powerLevel {
            get {
                // rackId = X coordinate plus 10
                int rackId = x + 10;

                // Begin with a power level of the rack ID times the Y coordinate
                int powerLevel = rackId * y;

                // Increase by the gridSerialNumber
                powerLevel += gridSerialNumber;

                // Set the power level to itself multiplied by the rack ID.
                powerLevel *= rackId;

                // Get the hundreds digit
                powerLevel = (int) (powerLevel / 100) % 10;

                // Subtract 5
                powerLevel -= 5;

                return powerLevel;
            }
        }
    }

    class Day11 : ASolution
    {
        // Using a dictionary means finding groups of x,y is much faster
        Dictionary<(int x, int y), FuelCell> fuelCells = new Dictionary<(int x, int y), FuelCell>();
        int gridSerialNumber = 0;

        public Day11() : base(11, 2018, "")
        {
            // Load 'em up!
            gridSerialNumber = Int32.Parse(Input.Trim());

            // Debug:
            // Serial 42 => 21,61: 30
            // Serial 18 => 33,45: 29

            for(int x=1; x<=300; x++)
                for(int y=1; y<=300; y++)
                    fuelCells.Add((x, y), new FuelCell() { x = x, y = y, gridSerialNumber = gridSerialNumber });

        }

        protected override string SolvePartOne()
        {
            // Search all 3x3 areas for the highest power level we have
            int highest = Int32.MinValue;
            int hx = Int32.MinValue;
            int hy = Int32.MinValue;

            bool draw = false;

            // Searching 3x3 areas
            for(int y=1; y<=298; y++) {
                for(int x=1; x<=298; x++){
                    int tempPowerLevel = 
                          fuelCells[(x  , y  )].powerLevel
                        + fuelCells[(x+1, y  )].powerLevel
                        + fuelCells[(x+2, y  )].powerLevel
                        + fuelCells[(x  , y+1)].powerLevel
                        + fuelCells[(x+1, y+1)].powerLevel
                        + fuelCells[(x+2, y+1)].powerLevel
                        + fuelCells[(x  , y+2)].powerLevel
                        + fuelCells[(x+1, y+2)].powerLevel
                        + fuelCells[(x+2, y+2)].powerLevel;

                    if (tempPowerLevel > highest) {
                        highest = tempPowerLevel;
                        hx = x;
                        hy = y;
                    }

                    if (draw) Console.Write(tempPowerLevel.ToString("  00; -00"));
                }

                if (draw) Console.WriteLine();
            }


            return $"[Serial: {gridSerialNumber}] {hx},{hy}: {highest}";
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
