using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2021
{

    class Day03 : ASolution
    {
        int gamma = 0;
        int epsilon = 0;

        // This keeps track of the zeros and ones in each position
        private List<int> zeros = new List<int>();
        private List<int> ones = new List<int>();

        public Day03() : base(03, 2021, "")
        {

        }

        protected override string? SolvePartOne()
        {
            var lines = Input.SplitByNewline(true);
            var length = lines[0].Length;

            for (int i = 0; i < length; i++)
            {
                var digit = lines
                    // Select this specific character from each line
                    .Select(line => line[i])
                    // Group and count the occurance of each character (zero or one)
                    .GroupBy(ch => ch)
                    .OrderByDescending(grp => grp.Count())
                    // Select the key (zero or one) in order of greatest to least
                    .Select(grp => grp.Key)
                    .ToArray();

                // First shift the values over
                gamma = gamma << 1;
                epsilon = epsilon << 1;

                // And the most common digit goes to gamma
                if (digit[0] == '1')
                    gamma += 1;
                else
                    epsilon += 1;
            }

            Console.WriteLine($"Gamma:   {gamma} [{String.Format("{0," + length + "}", Convert.ToString(gamma, toBase: 2))}]");
            Console.WriteLine($"Epsilon: {epsilon} [{String.Format("{0," + length + "}", Convert.ToString(epsilon, toBase: 2))}]");

            return (gamma * epsilon).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
