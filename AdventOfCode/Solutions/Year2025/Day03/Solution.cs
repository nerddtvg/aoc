using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Diagnostics;


namespace AdventOfCode.Solutions.Year2025
{

    class Day03 : ASolution
    {

        public Day03() : base(03, 2025, "Lobby")
        {
            var tests = new[]
            {
                ("987654321111111", 98),
                ("811111111111119", 89),
                ("234234234234278", 78),
                ("818181911112111", 92)
            };

            foreach((var line, var expected) in tests)
            {
                var result = MaxJoltage(line);
                Debug.Assert(result == expected, $"Line '{line}' returned '{result}', expected '{expected}'");
            }
        }

        protected override string? SolvePartOne()
        {
            return Input.SplitByNewline().Sum(MaxJoltage).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }

        private int MaxJoltage(string line)
        {
            // Search LTR and find the highest value + index
            // Then search LTR index+1 to find the next highest value
            var ret = 0;
            var max = line[0];
            var maxI = 0;

            for (int i = 1; i < line.Length - 1; i++)
            {
                if (line[i] > max)
                {
                    max = line[i];
                    maxI = i;
                }
            }

            // Convert char int to int without parse
            ret += (max - 48) * 10;

            max = line[maxI + 1];
            for (int i = maxI + 2; i < line.Length; i++)
            {
                if (line[i] > max)
                {
                    max = line[i];
                }
            }

            ret += max - 48;

            return ret;
        }
    }
}

