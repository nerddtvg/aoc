using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{

    class Day01 : ASolution
    {

        public List<int> input;

        public Day01() : base(01, 2020, "")
        {
            input = Input.ToIntArray("\n").ToList();
        }

        protected override string SolvePartOne()
        {
            // Specifically, they need you to find the two entries that sum to 2020 and then multiply those two numbers together.
            return input.GetAllCombos(2)
                .Where(a => a.Sum() == 2020)
                .First()
                .Aggregate(1, (x, y) => x * y)
                .ToString();
        }

        protected override string SolvePartTwo()
        {
            // They offer you a second one if you can find three numbers in your expense report that meet the same criteria.
            return input.GetAllCombos(3)
                .Where(a => a.Sum() == 2020)
                .First()
                .Aggregate(1, (x, y) => x * y)
                .ToString();
        }
    }
}
