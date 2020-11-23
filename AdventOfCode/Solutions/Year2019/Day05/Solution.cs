using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Solutions.Year2019
{

    class Day05 : ASolution
    {

        public Day05() : base(05, 2019, "")
        {

        }

        protected override string SolvePartOne()
        {
            Intcode intcode = new Intcode(Input);
            intcode.SetInput(1);
            intcode.Run();
            
            return intcode.output_register.ToString();
        }

        protected override string SolvePartTwo()
        {
            Intcode intcode = new Intcode(Input);
            intcode.SetInput(5);
            intcode.Run();
            
            return intcode.output_register.ToString();
        }
    }
}
