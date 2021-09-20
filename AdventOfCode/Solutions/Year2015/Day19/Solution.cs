using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2015
{

    class Day19 : ASolution
    {
        public Dictionary<string, List<string>> replacements = new Dictionary<string, List<string>>();
        public Dictionary<string, string> reverseReplacements = new Dictionary<string, string>();

        public string originalFormula = string.Empty;

        public Day19() : base(19, 2015, "")
        {
            // Parse the input with replacements at the top and the formula to figure out at the bottom
            var input = Input.SplitByBlankLine();

            foreach(var line in input[0])
            {
                // This has one replacement per line
                var split = line.Split(' ', 3, StringSplitOptions.TrimEntries);

                AddReplacement(split[0], split[2]);
            }

            originalFormula = input[1][0].Trim();
        }

        /// <summary>
        /// Add to our list
        /// </summary>
        private void AddReplacement(string key, string replacement)
        {
            if (replacements.ContainsKey(key))
            {
                replacements[key].Add(replacement);
            }
            else
            {
                replacements[key] = new List<string>() { replacement };
            }

            // Add our reverse as well
            if (reverseReplacements.ContainsKey(replacement))
                throw new Exception($"Duplicate replacement found: {replacement}");

            reverseReplacements[replacement] = key;
        }

        /// <summary>
        /// Find possible replacements from the dictionary
        /// </summary>
        private string[] GetReplacements(string key)
        {
            if (replacements.ContainsKey(key))
            {
                return replacements[key].ToArray();
            }
            else
            {
                // Not found so we just use the key as-is
                return new string[] { key };
            }
        }

        private (string currentElement, string currentFormula) GetCurrentElement(string formula)
        {
            var currentElement = formula.Substring(0, 1);
            var tempFormula = formula.Substring(1);
            while(tempFormula.Length > 0)
            {
                var c = tempFormula.Substring(0, 1).ToCharArray()[0];

                if (!(65 <= (int) c && (int) c <= 90))
                {
                    // This is a non-uppercase character, append it to our element
                    currentElement += c.ToString();

                    // Reduce our temp formula
                    if (tempFormula.Length == 1)
                        tempFormula = string.Empty;
                    else
                        tempFormula = tempFormula.Substring(1);

                    continue;
                }

                // Uppercase character, that's a new element, break out
                break;
            }

            return (currentElement, tempFormula);
        }

        private HashSet<string> ProcessFormulaStep(string formula)
        {
            if (formula.Length == 0)
                return new HashSet<string>();

            // We need to take the first element off this formula and:
            // 1. Add all of the possible replacements of this element + the rest of the formula appended (one replacement)
            // 2. Don't replace it, use this element + all possible iterations of the rest of the formula from ProcessFormulaStep()
            (var currentElement, var tempFormula) = GetCurrentElement(formula);

            // Return array
            var ret = new HashSet<string>();

            // Check that there are replacements for this and start the list
            if (this.replacements.ContainsKey(currentElement))
            {
                foreach(var r in this.replacements[currentElement])
                {
                    ret.Add(r + tempFormula);
                }
            }

            // Now we handle not replacing this element
            if (tempFormula.Length > 0)
            {
                foreach (var r2 in ProcessFormulaStep(tempFormula))
                {
                    ret.Add(currentElement + r2);
                }
            }

            return ret;
        }

        protected override string SolvePartOne()
        {
            var replacements = ProcessFormulaStep(originalFormula);
            
            return replacements.Count.ToString();
        }

        protected override string SolvePartTwo()
        {
            // Start with 'e' and see how many steps it takes to make the medicine in originalFormula
            // We're going to reverse process this
            // We know that none of the replacements are duplicates, so we can find all possible reverse replacements in the formula

            // The process going forward is impossible (way too many combinations)
            // The process going backwards is possible, however with this code it takes too long
            // I went to the reddit megathread and found this:
            // https://old.reddit.com/r/adventofcode/comments/3xflz8/day_19_solutions/cy4etju/
            // You can think of Rn Y Ar as the characters ( , ):
            // The new formula is count(tokens) - count("(" or ")") - 2*count(",") - 1.

            // First we need to count all of our tokens in the original formula
            var regex = new Regex("([A-Z][a-z]*)");
            var matches = regex.Matches(originalFormula);

            var tokensTotal = matches.Count;
            var RnTotal = matches.Count(m => m.Value == "Rn");
            var YTotal = matches.Count(m => m.Value == "Y");
            var ArTotal = matches.Count(m => m.Value == "Ar");

            // Huge thank you to /u/askalski
            return (tokensTotal - RnTotal - ArTotal - (2*YTotal) - 1).ToString();
        }
    }
}
