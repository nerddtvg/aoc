using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2023
{

    class Day15 : ASolution
    {

        public Day15() : base(15, 2023, "Lens Library")
        {

        }

        private uint GetHash(string str)
        {
            uint hash = 0;

            str.ForEach(c =>
            {
                hash += c;
                hash *= 17;
                hash %= 256;
            });

            return hash;
        }

        protected override string? SolvePartOne()
        {
            return Input.Split(",").Sum(split => GetHash(split)).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

