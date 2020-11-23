using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2019
{

    class Day01 : ASolution
    {

        public Day01() : base(01, 2019, "")
        {

        }

        protected override string SolvePartOne() => Input.ToIntArray("\n").Select(Fuel).Sum().ToString();

        protected override string SolvePartTwo() => Input.ToIntArray("\n").Select(FuelFuel).Sum().ToString();

        int Fuel(int module) => module / 3 - 2;

        int FuelFuel(int module)
        {
            int fuel = Fuel(module);
            return fuel <= 0 ? 0 : fuel + FuelFuel(fuel);
        }
    }
}
