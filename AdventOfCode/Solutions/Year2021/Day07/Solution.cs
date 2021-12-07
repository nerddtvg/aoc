using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2021
{

    class Day07 : ASolution
    {
        private List<int> crabs = new List<int>();

        public Day07() : base(07, 2021, "")
        {
            // DebugInput = "16,1,2,0,4,2,7,1,2,14";

            // Load the crabs!
            this.crabs = Input.ToIntArray(",").ToList();
        }

        protected override string? SolvePartOne()
        {
            int minPos = Int32.MaxValue, minFuel = Int32.MaxValue;

            var max = this.crabs.Max();

            // Search from 0 to max
            // Find out how much fuel is required for that
            for (int i = 0; i <= max; i++)
            {
                var tFuel = this.crabs.Sum(crab => (int) Math.Abs(crab - i));

                if (tFuel < minFuel)
                {
                    // Found one possible
                    minFuel = tFuel;
                    minPos = i;
                }
            }

            return minFuel.ToString();
        }

        protected override string? SolvePartTwo()
        {
            int minPos = Int32.MaxValue, minFuel = Int32.MaxValue;

            var max = this.crabs.Max();

            // Search from 0 to max
            // Find out how much fuel is required for that
            for (int i = 0; i <= max; i++)
            {
                // Part 2: Every step requires 1 additional fuel
                // 1 => 1
                // 2 => 1 + 2
                // 3 => 1 + 2 + 3
                // This is a summation of a series of n entries A: Sn = (n*(A1 + An))/2
                var tFuel = this.crabs.Sum(crab => (((int) Math.Abs(crab - i)) * (1 + ((int) Math.Abs(crab - i)))) / 2);

                if (tFuel < minFuel)
                {
                    // Found one possible
                    minFuel = tFuel;
                    minPos = i;
                }
            }

            return minFuel.ToString();
        }
    }
}

#nullable restore
