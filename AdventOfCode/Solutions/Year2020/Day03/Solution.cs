using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{

    class Day03 : ASolution
    {

        public Day03() : base(03, 2020, "")
        {

        }

        protected override string SolvePartOne()
        {
            int x = 0;
            int trees = 0;

            foreach(string line in Input.SplitByNewline()) {
                // Reset x if we are past the length
                if (x >= line.Length) x -= line.Length;

                // Is this a tree?
                if (line.Substring(x, 1) == "#") trees++;

                // Increment
                x += 3;
            }

            return trees.ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
