using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Solutions.Year2019
{

    class Day09 : ASolution
    {

        public Day09() : base(09, 2019, "")
        {

        }

        protected override string SolvePartOne()
        {
            Intcode intcode = new Intcode(Input, 2);
            intcode.SetInput(1);
            intcode.Run();

            return intcode.output_register.ToString() ?? string.Empty;
        }

        protected override string SolvePartTwo()
        {
            Intcode intcode = new Intcode(Input, 2);
            intcode.SetInput(2);
            intcode.Run();

            return intcode.output_register.ToString() ?? string.Empty;
        }
    }
}
