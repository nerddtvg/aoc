using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2015
{

    class Day05 : ASolution
    {
        private char[] vowels = new char[] { 'a', 'e', 'i', 'o', 'u' };

        public Day05() : base(05, 2015, "")
        {
            
        }

        private bool IsValidPart1(string str) =>
            !(str.Contains("ab") || str.Contains("cd") || str.Contains("pq") || str.Contains("xy"))
            &&
            (new Regex(@"([a-z])\1").IsMatch(str))
            &&
            str.ToCharArray().GroupBy(a => a).Where(a => vowels.Contains(a.Key)).Sum(a => a.Count()) >= 3;

        private bool IsValidPart2(string str) =>
            // One letter that repeats with exactly one letter between them
            (new Regex(@"([a-z])[a-z]\1").IsMatch(str))
            &&
            // Any pair of two letters that repeats itself
            (new Regex(@"([a-z][a-z]).*\1").IsMatch(str));

        protected override string SolvePartOne()
        {
            int count = 0;
            foreach(var line in Input.SplitByNewline())
            {
                //Console.WriteLine($"{line}: {IsValidPart1(line)}");

                if (IsValidPart1(line))
                    count++;
            }
            return count.ToString();
        }

        protected override string SolvePartTwo()
        {
            int count = 0;
            foreach(var line in Input.SplitByNewline())
            {
                //Console.WriteLine($"{line}: {IsValidPart2(line)}");

                if (IsValidPart2(line))
                    count++;
            }
            return count.ToString();
        }
    }
}
