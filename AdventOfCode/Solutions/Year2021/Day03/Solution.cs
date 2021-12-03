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
        int oxygen = 0;
        int co2 = 0;
        int length = 0;

        // This keeps track of the zeros and ones in each position
        private List<int> zeros = new List<int>();
        private List<int> ones = new List<int>();

        public Day03() : base(03, 2021, "Binary Diagnostic")
        {

        }

        protected override string? SolvePartOne()
        {
            var lines = Input.SplitByNewline(true);
            length = lines[0].Length;

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

            Console.WriteLine($"Gamma:        {gamma} [{String.Format("{0," + length + "}", Convert.ToString(gamma, toBase: 2))}]");
            Console.WriteLine($"Epsilon:      {epsilon} [{String.Format("{0," + length + "}", Convert.ToString(epsilon, toBase: 2))}]");

            return (gamma * epsilon).ToString();
        }

        private string BitCriteria(char DefaultVal = '1')
        {
            var index = 0;
            var lines = Input.SplitByNewline(true).ToList();

            while(lines.Count > 1 && index < length)
            {
                // Let's determine things!
                var digitsEnum = lines
                    // Select this specific character from each line
                    .Select(line => line[index])
                    // Group and count the occurance of each character (zero or one)
                    .GroupBy(ch => ch);

                // Order correctly
                if (DefaultVal == '1')
                    digitsEnum = digitsEnum.OrderByDescending(grp => grp.Count());
                else
                    digitsEnum = digitsEnum.OrderBy(grp => grp.Count());

                // Get the values
                var digits = digitsEnum
                    // Select the key (zero or one) in order of greatest to least
                    .Select(grp => (grp.Key, grp.Count()))
                    .ToArray();

                // Get our next character
                var nextChar = DefaultVal;
                
                // If we have a difference, then our ordering takes precedence
                if (digits.Length == 1 || digits[0].Item2 != digits[1].Item2)
                    nextChar = digits[0].Key;

                // Reduce our lines
                lines = lines.Where(line => line[index] == nextChar).ToList();

                index++;
            }

            if (lines.Count == 0)
                throw new InvalidOperationException();

            return lines[0];
        }

        protected override string? SolvePartTwo()
        {
            oxygen = Convert.ToInt32(BitCriteria(), 2);
            co2 = Convert.ToInt32(BitCriteria('0'), 2);

            Console.WriteLine($"Oxygen Gen:   {oxygen} [{String.Format("{0," + length + "}", Convert.ToString(oxygen, toBase: 2))}]");
            Console.WriteLine($"CO2 Scrubber: {co2} [{String.Format("{0," + length + "}", Convert.ToString(co2, toBase: 2))}]");

            return (oxygen * co2).ToString();
        }
    }
}

#nullable restore
