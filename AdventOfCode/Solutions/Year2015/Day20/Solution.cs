using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2015
{

    class Day20 : ASolution
    {

        public Day20() : base(20, 2015, "")
        {

        }

        private int GetHousePresents(int HouseNumber)
        {
            // Every elf number that is a divisor has delivered (elf number * 10 presents) to this house
            var elves = HouseNumber.GetDivisors().Sum();

            // Each elf delivered 10*elf number presents
            return elves * 10;
        }

        protected override string SolvePartOne()
        {
            // Figure out what house gives us at least the Input number of presents
            var input = Int32.Parse(Input);

            int HouseNumber = 1;
            do
            {
                // Check this house
                var presents = GetHousePresents(HouseNumber);
                if (presents >= input)
                {
                    System.Console.WriteLine($"Found: {presents} at house {HouseNumber}");
                    return HouseNumber.ToString();
                }

                // Lots of output here
                //System.Console.WriteLine($"House {HouseNumber} got {presents} presents.");

                HouseNumber++;
            } while (true);
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
