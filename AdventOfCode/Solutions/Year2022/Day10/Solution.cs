using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2022
{

    class Day10 : ASolution
    {
        private int register = 0;

        private int cycle = 0;

        private int signalStrength = 0;

        public Day10() : base(10, 2022, "Cathode-Ray Tube")
        {
            
        }

        private void ResetComputer()
        {
            register = 1;
            cycle = 1;
            signalStrength = 0;
        }

        private void CheckSignalStrength()
        {
            // If this is a signal strength location, add it
            if (cycle <= 220 && (cycle - 20) % 40 == 0)
            {
                signalStrength += cycle * register;
            }
        }

        private void ProcessInstruction(string line)
        {
            var instructions = line.Split(" ", 2, StringSplitOptions.TrimEntries);

            CheckSignalStrength();
            cycle++;

            if (instructions[0] == "addx")
            {
                // Two-cycle step
                CheckSignalStrength();

                cycle++;
                register += Int32.Parse(instructions[1]);
            }
        }

        protected override string? SolvePartOne()
        {
            ResetComputer();
            foreach(var line in Input.SplitByNewline())
                ProcessInstruction(line);

            return signalStrength.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

