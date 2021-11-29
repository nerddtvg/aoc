using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2017
{

    class Day01 : ASolution
    {

        public Day01() : base(01, 2017, "Inverse Captcha")
        {
            
        }

        protected override string? SolvePartOne()
        {
            var split = Input.ToCharArray();
            var total = 0;
            for (int i = 0; i < split.Length; i++)
            {
                var next = (i + 1) % split.Length;

                if (split[i] == split[next])
                {
                    total += Int32.Parse(split[i].ToString());
                }
            }

            return total.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
