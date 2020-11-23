using System;
using System.Collections.Generic;
using System.Text;

using AdventOfCode.Solutions.Year2019;

namespace AdventOfCode.Solutions.Year2019
{

    class Day02 : ASolution
    {

        public Day02() : base(02, 2019, "")
        {

        }

        protected override string SolvePartOne()
        {
            long[] input = Input.ToLongArray(",");

            input[1] = 12;
            input[2] = 2;

            //input = new long[] {1,0,0,0,99};
            //input = new long[] {2,3,0,3,99};
            //input = new long[] {2,4,4,5,99,0};
            //input = new long[] {1,1,1,4,99,5,6,0,99};

            Intcode intcode = new Intcode(input, 0);
            intcode.Run();
            
            return intcode.memory[0].ToString();
        }

        protected override string SolvePartTwo()
        {
            long[] input = Input.ToLongArray(",");

            input[1] = 57;
            input[2] = 41;

            //input = new long[] {1,0,0,0,99};
            //input = new long[] {2,3,0,3,99};
            //input = new long[] {2,4,4,5,99,0};
            //input = new long[] {1,1,1,4,99,5,6,0,99};

            Intcode intcode = new Intcode(input, 0);
            intcode.Run();
            
            return intcode.memory[0].ToString();
        }
    }
}
