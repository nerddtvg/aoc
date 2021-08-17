using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2015
{

    class Day11 : ASolution
    {
        private char[] _validLetters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'm', 'n', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        public Day11() : base(11, 2015, "")
        {

        }

        private string Increment(string input)
        {
            // We break up the string into a int char array
            // a = (char) 97
            // z = (char) 122
            // At 123, we wrap over to 97
            var chars = input.ToCharArray().Select(a => (int)a).ToArray();

            var newPassword = string.Empty;

            do
            {
                // We need to run this at least once
                for (var i = chars.Length - 1; i >= 0; i--)
                {
                    // For this position, we will be incrementing it
                    if (chars[i] == 122)
                    {
                        // Rollover to the next character
                        chars[i] = 97;
                    }
                    else
                    {
                        // No rollover, we increment this and check for a valid pass
                        chars[i]++;
                        break;
                    }
                }

                // Do this only once
                newPassword = chars.Select(a => (char)a).JoinAsStrings();
            } while (!this.IsValid1(newPassword));

            return newPassword;
        }

        private bool IsValid1(string input)
        {
            var matches = Regex.Matches(input, @".*([a-z])\1.*([a-z])\2");
            var hasGroups = matches.SelectMany(a => a.Groups.Keys).GroupBy(a => a).Count() > 1;

            return hasGroups
            &&
            (
                !input.Contains("i")
                &&
                !input.Contains("l")
                &&
                !input.Contains("o")
            )
            &&
            (
                input.Contains("abc")
                ||
                input.Contains("bcd")
                ||
                input.Contains("cde")
                ||
                input.Contains("def")
                ||
                input.Contains("efg")
                ||
                input.Contains("fgh")
                // Skip those with i, l, o
                ||
                input.Contains("pqr")
                ||
                input.Contains("qrs")
                ||
                input.Contains("rst")
                ||
                input.Contains("stu")
                ||
                input.Contains("tuv")
                ||
                input.Contains("uvx")
                ||
                input.Contains("vwx")
                ||
                input.Contains("wxy")
                ||
                input.Contains("xyz")
            );
        }

        protected override string SolvePartOne()
        {
            return Increment(Input);
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
