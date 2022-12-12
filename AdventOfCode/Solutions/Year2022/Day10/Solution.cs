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

        private char[] output = Enumerable.Repeat('.', (width * height)).ToArray();
        private const int width = 40;
        private const int height = 6;

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
            Console.WriteLine($"{cycle}: {register}");
            // If this is a signal strength location, add it
            if (cycle <= 220 && (cycle - 20) % 40 == 0)
            {
                signalStrength += cycle * register;
            }

            // Process our output here
            // Sprite is 3-wide centered at register location
            // If register-1, register, register+1 == cycle then draw it
            var xPos = (cycle - 1) % width;
            if (register - 1 <= xPos && xPos <= register + 1)
                // Change from 1 based to 0 based
                output[cycle - 1] = '#';
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
            for (int i = 0; i < output.Length / width; i++)
                Console.WriteLine(output.Skip(i * width).Take(width).JoinAsString());

            return "Printed Answer";
        }
    }
}

