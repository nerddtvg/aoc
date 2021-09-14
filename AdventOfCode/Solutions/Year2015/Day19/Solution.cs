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

        private HashSet<string> ProcessFormulaStep(string formula)
        {
            if (formula.Length == 0)
                return new HashSet<string>();

            // We need to take the first element off this formula and:
            // 1. Add all of the possible replacements of this element + the rest of the formula appended (one replacement)
            // 2. Don't replace it, use this element + all possible iterations of the rest of the formula from ProcessFormulaStep()
            var currentElement = formula.Substring(0, 1);
            var tempFormula = formula.Substring(1);
            while(tempFormula.Length > 0)
            {
                var c = tempFormula.Substring(0, 1).ToCharArray()[0];

                // Reduce our temp formula
                if (tempFormula.Length == 1)
                    tempFormula = string.Empty;
                else
                    tempFormula = tempFormula.Substring(1);

                if (!(65 <= (int) c && (int) c <= 90))
                {
                    // This is a non-uppercase character, append it to our element
                    currentElement += c.ToString();
                    continue;
                }

                // Uppercase character, that's a new element, fix the tempFormula and break out
                tempFormula = c.ToString() + tempFormula;
                break;
            }

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
            else
            {
                System.Console.WriteLine($"No transformation: {currentElement}");
            }

            // Now we handle not replacing this element
            if (tempFormula.Length > 0)
            {
                foreach (var r2 in ProcessFormulaStep(tempFormula))
                {
                    ret.Add(currentElement + r2);
                }
            }
            else
            {
                // No formula to process, this is the end!
                ret.Add(currentElement);
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
            return null;
        }
    }
}
