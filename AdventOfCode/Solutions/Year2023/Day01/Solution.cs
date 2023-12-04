using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using Microsoft.VisualBasic;


namespace AdventOfCode.Solutions.Year2023
{
    partial class Day01 : ASolution
    {
        [GeneratedRegex("[0-9]")]
        private static partial Regex regex();

        [GeneratedRegex(@"((?<=o)ne|(?<=t)wo|(?<=t)hree|four|five|six|seven|(?<=e)ight|(?<=n)ine|[0-9])")]
        private static partial Regex regexWithWords();

        public Day01() : base(01, 2023, "Trebuchet?!")
        {

        }

        private int getSum(string input, int part = 1)
        {
            // For each of the lines in the input
            return input.SplitByNewline()
                // Match all digits
                .Select(line => (part == 1 ? regex() : regexWithWords()).Matches(line))
                // Then find the first and last digit, pair them together and parse into an integer
                .Select(allNumbers => $"{allNumbers[0].Value}{allNumbers[^1].Value}")
                // For part 2, we need to replace words
                .Select(numberStr =>
                    numberStr
                        .Replace("wo", "2")
                        .Replace("hree", "3")
                        .Replace("four", "4")
                        .Replace("five", "5")
                        .Replace("six", "6")
                        .Replace("seven", "7")
                        .Replace("ight", "8")
                        .Replace("ine", "9")
                        // Ordered at bottom to not conflict with "ine"
                        .Replace("ne", "1")
                )
                .Select(int.Parse)
                // Get the total
                .Sum();
        }

        protected override string? SolvePartOne()
        {
            return getSum(Input).ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Words can be overlapping so we need to account for consumed characters
            // The lookbehinds will consume the letters at the starts of words
            // that start with e, o, r, x, n, t
            // That impacts: one, two, three, eight, nine
            return getSum(Input, 2).ToString();
        }
    }
}
