using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2017
{

    class Day23 : ASolution
    {

        public Day23() : base(23, 2017, "Coprocessor Conflagration")
        {

        }

        protected override string? SolvePartOne()
        {
            var p = new Day18.SoundProgram(Input);

            var ret = true;
            while(ret)
            {
                ret = p.Run();
            }

            return p.mulCount.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
