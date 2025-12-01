using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2025
{

    class Day01 : ASolution
    {

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
        }

        protected override string? SolvePartOne()
        {
            int position = 50;
            int password = 0;

            Input.SplitByNewline(true).ForEach(line =>
            {
                position += (line[0] == 'L' ? -1 : 1) * int.Parse(line[1..]);

                // Change negative to positive
                // Because % is not modulo in C#, we must operate with positive numbers only
                while (position < 0)
                    position += 100;

                position %= 100;

                if (position == 0)
                    password++;
            });

            return password.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

