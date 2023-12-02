using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2023
{
    partial class Day01 : ASolution
    {
        [GeneratedRegex("[0-9]")]
        private static partial Regex regex();

        public Day01() : base(01, 2023, "Trebuchet?!")
        {

        }

        private int getSum(string input)
        {
            // For each of the lines in the input
            return input.SplitByNewline()
                // Match all digits
                .Select(line => regex().Matches(line))
                // Then find the first and last digit, pair them together and parse into an integer
                .Select(allNumbers => int.Parse($"{allNumbers[0].Value}{allNumbers[^1].Value}"))
                // Get the total
                .Sum();
        }

        protected override string? SolvePartOne()
        {
            return getSum(Input).ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Words can be overlapping so replacing them with digits + first/last characters makes the original code work
            // oneight => o1e8t
            var input = Input;

            input = input.Replace("one", "o1e");
            input = input.Replace("two", "t2o");
            input = input.Replace("three", "t3e");
            input = input.Replace("four", "f4r");
            input = input.Replace("five", "f5e");
            input = input.Replace("six", "s6x");
            input = input.Replace("seven", "s7n");
            input = input.Replace("eight", "e8t");
            input = input.Replace("nine", "n9e");

            return getSum(input).ToString();
        }
    }
}
