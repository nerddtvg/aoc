using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{

    class Day07 : ASolution
    {
        private List<string> matches = new List<string>();

        public Day07() : base(07, 2016, "")
        {
            
        }

        private void FindValidItems()
        {
            this.matches = new List<string>();

            // Find all of the valid items
            var regex = new Regex(@"([a-z]+|\[[a-z]+\])");

            // Brute force build this matcher
            var abba_str = string.Empty;
            for (char a = 'a'; a <= 'z'; a++)
            {
                for (char b = 'a'; b <= 'z'; b++)
                {
                    if (b == a)
                        continue;

                    abba_str += $"{a}{b}{b}{a}|";
                }
            }

            // Trim off the trailing '|'
            var abba_regex = new Regex($"({abba_str.Substring(0, abba_str.Length - 1)})");

            foreach (var line in Input.SplitByNewline())
            {
                var match = regex.Matches(line);

                if (match.Count > 0)
                {
                    // Since we can have many groups, we track what we have
                    var valid = false;
                    var invalid = false;

                    foreach(Match m in match)
                    {
                        var matches = abba_regex.IsMatch(m.Value);
                        var hypernet = m.Value.Substring(0, 1) == "[";

                        if (matches)
                        {
                            if (hypernet)
                            {
                                invalid = true;
                                break;
                            }
                            else
                            {
                                valid = true;
                            }
                        }
                    }

                    if (valid && !invalid)
                        this.matches.Add(line);
                }
                else
                {
                    Console.WriteLine($"Bad Parse: {line}");
                }
            }
        }

        protected override string SolvePartOne()
        {
            FindValidItems();
            return this.matches.Count.ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
