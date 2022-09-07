using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2018
{

    class Day01 : ASolution
    {
        int? twice;
        int frequency = 0;
        HashSet<int> frequencies = new();

        public Day01() : base(01, 2018, "Chronal Calibration")
        {
            // DebugInput = "+1\n-1";
            // DebugInput = "+3\n+3\n+4\n-2\n-4";
        }

        protected override string SolvePartOne()
        {
            frequencies.Add(0);
            RunInput();

            return frequency.ToString();
        }

        private void RunInput()
        {
            foreach(var line in Input.SplitByNewline())
            {
                var dir = line[0] == '+' ? 1 : -1;
                frequency += dir * Int32.Parse(line.Substring(1));

                if (frequencies.Contains(frequency) && twice == default)
                    twice = frequency;

                frequencies.Add(frequency);
            }
        }

        protected override string SolvePartTwo()
        {
            while (twice == default)
            {
                RunInput();
            }

            return twice.ToString() ?? string.Empty;
        }
    }
}
