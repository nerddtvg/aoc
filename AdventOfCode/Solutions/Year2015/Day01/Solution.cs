using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2015
{

    class Day01 : ASolution
    {

        public Day01() : base(01, 2015, "")
        {

        }

        protected override string SolvePartOne()
        {
            int floor = 0;
            Input.ToCharArray().ToList().ForEach(a =>
            {
                if (a == '(') floor++;
                else if (a == ')') floor--;
            });

            return floor.ToString();
        }

        protected override string SolvePartTwo()
        {
            int floor = 0;
            int pos = 1;
            foreach(var a in Input.ToCharArray())
            {
                if (a == '(') floor++;
                else if (a == ')') floor--;

                if (floor < 0) break;
                pos++;
            }

            return pos.ToString();
        }
    }
}
