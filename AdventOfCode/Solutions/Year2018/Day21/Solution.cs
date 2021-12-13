using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2018
{

    class Day21 : ASolution
    {
        Day19.OpCodeComputer p;

        public Day21() : base(21, 2018, "Chronal Conversion")
        {
            p = new Day19.OpCodeComputer(Input);
        }

        protected override string? SolvePartOne()
        {
            int minInput = Int32.MaxValue;
            int minCount = Int32.MaxValue;

            // This range was determined by running the Console.WriteLine code for ipVal == 28
            // That is the line that exits in the code
            // So we needed r[0] == r[2] which was shown to be 8797248
            for (int a = 8797247; a < 8797300; a++)
            {
                p.Reset();
                p.registers[0] = a;

                // Find if this runs shorter (don't go past 100,000)
                int count = 0;
                while(count < minCount && count < 10000 && p.RunOperation())
                {
                    count++;

                    // This helps us figure out why the programs aren't exiting
                    // if (p.ipVal == 28)
                    // {
                    //     // This is one place we can exit
                    //     Console.WriteLine($"input={a} ipVal={p.ipVal} r2={p.registers[2]} r0={p.registers[0]}");

                    //     break;
                    // }
                }

                if (count < minCount)
                {
                    minCount = count;
                    minInput = a;
                }
            }

            return minInput.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
