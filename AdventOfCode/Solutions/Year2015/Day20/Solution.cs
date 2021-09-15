using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2015
{

    class Day20 : ASolution
    {
        private int StartPart2 = 0;

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

        private int GetHousePresentsPart2(int HouseNumber)
        {
            // Every elf number that is a divisor, and it is within the first 50 houses for that elf,
            // has delivered (elf number * 10 presents) to this house
            var elves = HouseNumber.GetDivisors();

            // We need to determine if this house falls in the elve's first 50 houses (50 * elf number)
            elves = elves.Where(a => HouseNumber <= a * 50).ToArray();

            // Each elf delivered 11*elf number presents
            return elves.Sum() * 11;
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

                    StartPart2 = HouseNumber;

                    return HouseNumber.ToString();
                }

                // Lots of output here
                //System.Console.WriteLine($"House {HouseNumber} got {presents} presents.");

                HouseNumber++;
            } while (true);
        }

        protected override string SolvePartTwo()
        {
            // Figure out what house gives us at least the Input number of presents
            var input = Int32.Parse(Input);

            // We skip ahead because we know this has to be after Part 1's finish
            int HouseNumber = StartPart2;
            do
            {
                // Check this house
                var presents = GetHousePresentsPart2(HouseNumber);
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
    }
}
