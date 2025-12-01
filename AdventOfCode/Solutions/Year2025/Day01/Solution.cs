using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Numerics;


namespace AdventOfCode.Solutions.Year2025
{

    class Day01 : ASolution
    {
        const int dialMax = 100;

        public Day01() : base(01, 2025, "Secret Entrance")
        {
            // DebugInput = @"
            //     L68
            //     L30
            //     R48
            //     L5
            //     R60
            //     L55
            //     L1
            //     L99
            //     R14
            //     L82";

            // DebugInput = "L1000";

            // DebugInput = "L1000\nL50";

            // DebugInput = "R1000\nR50";

            // DebugInput = "L50\nR101";

            // DebugInput = "R50\nL101";
        }

        protected override string? SolvePartOne()
        {
            int position = 50;
            int password = 0;

            Input.SplitByNewline(true).ForEach(line =>
            {
                position += (line[0] == 'L' ? -1 : 1) * int.Parse(line[1..]);

                // Because % is not modulo in C#, we must operate with positive numbers only
                position = (position % dialMax + dialMax) % dialMax;

                if (position == 0)
                    password++;
            });

            return password.ToString();
        }

        protected override string? SolvePartTwo()
        {
            int position = 50;
            int password = 0;

            Input.SplitByNewline(true).ForEach(line =>
            {
                var oldPosition = position;
                position += (line[0] == 'L' ? -1 : 1) * int.Parse(line[1..]);

                // Part 2: Count the number of times we pass 0 / 100
                if (position >= dialMax)
                    password += Math.Abs(position / dialMax);
                else if (position < 0)
                    // For negative, we count if we passed 0 on this turn but only if we started above zero
                    password += Math.Abs(position / dialMax) + (oldPosition > 0 ? 1 : 0);
                else if (position == 0)
                    password++;

                position = (position % dialMax + dialMax) % dialMax;
            });

            return password.ToString();
        }
    }
}
