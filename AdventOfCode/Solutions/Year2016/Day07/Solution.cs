using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{

    class Day07 : ASolution
    {
        private List<string> matchesTLS = new List<string>();
        private List<string> matchesSSL = new List<string>();

        public Day07() : base(07, 2016, "")
        {
            FindValidItems();
        }

        private void FindValidItems()
        {
            this.matchesTLS = new List<string>();
            this.matchesSSL = new List<string>();

            // Find all of the valid items
            var regex = new Regex(@"([a-z]+|\[[a-z]+\])");

            var aba_str = new List<string>();

            // Brute force build this matcher
            var abba_str = string.Empty;
            for (char a = 'a'; a <= 'z'; a++)
            {
                for (char b = 'a'; b <= 'z'; b++)
                {
                    if (b == a)
                        continue;

                    abba_str += $"{a}{b}{b}{a}|";
                    aba_str.Add($"{a}{b}{a}");
                }
            }

            // Trim off the trailing '|'
            var abba_regex = new Regex($"({abba_str.Substring(0, abba_str.Length - 1)})");

            foreach (var line in Input.SplitByNewline())
            {
                var match = regex.Matches(line);

                if (match.Count > 0)
                {
                    var supernets = new List<Match>();
                    var hypernets = new List<Match>();

                    foreach(Match m in match)
                    {
                        if (m.Value.Substring(0, 1) == "[")
                            hypernets.Add(m);
                        else
                            supernets.Add(m);
                    }

                    // Part 1
                    if (supernets.Any(s => abba_regex.IsMatch(s.Value)) && !hypernets.Any(h => abba_regex.IsMatch(h.Value)))
                        this.matchesTLS.Add(line);

                    // Get every ABA that has a match inside a supernet
                    var aba_matches = aba_str
                        .SelectMany(aba => supernets.Where(s => s.Value.Contains(aba)).Select(s => aba))
                        .Distinct()
                        .ToList();

                    // Part 2
                    if (aba_matches.Count > 0)
                    {
                        // For each of the psossible matches, we need to find if we have a BAB sequence
                        foreach(var g in aba_matches)
                        {
                            if (g.Length != 3)
                                continue;

                            // We have a three character string (ABA) we change to BAB
                            var bab = $"{g.Substring(1, 1)}{g.Substring(0, 1)}{g.Substring(1, 1)}";

                            if (hypernets.Any(s => s.Value.Contains(bab)))
                            {
                                this.matchesSSL.Add(line);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Bad Parse: {line}");
                }
            }
        }

        protected override string SolvePartOne()
        {
            return this.matchesTLS.Count.ToString();
        }

        protected override string SolvePartTwo()
        {
            return this.matchesSSL.Count.ToString();
        }
    }
}
