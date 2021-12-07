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
        private int minFuel = Int32.MaxValue;
        private int minPos = Int32.MaxValue;

        public Day07() : base(07, 2021, "")
        {
            // Load the crabs!
            this.crabs = Input.ToIntArray(",").ToList();
        }

        protected override string? SolvePartOne()
        {
            var max = this.crabs.Max();

            // Search from 0 to max
            // Find out how much fuel is required for that
            for (int i = 0; i <= max; i++)
            {
                var tFuel = this.crabs.Sum(crab => (int) Math.Abs(crab - i));

                if (tFuel < this.minFuel)
                {
                    // Found one possible
                    this.minFuel = tFuel;
                    this.minPos = i;
                }
            }

            return this.minFuel.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
