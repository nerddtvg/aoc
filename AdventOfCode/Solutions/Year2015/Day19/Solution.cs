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

        /// <summary>
        /// Returns a Regex similar to: (ABc|Dfa|G|DFasF) based on all of the replacements possible
        /// </summary>
        private Regex GetPart2Regex()
        {
            var regexStart = string.Join("|", this.reverseReplacements.Keys.Select(a => a));

            // This is a case-sensitive search
            return new Regex($"({regexStart})");
        }

        protected override string SolvePartTwo()
        {
            // Start with 'e' and see how many steps it takes to make the medicine in originalFormula
            // We're going to reverse process this
            // We know that none of the replacements are duplicates, so we can find all possible reverse replacements in the formula
            var regex = GetPart2Regex();
            var currentStepResults = new HashSet<string>() { originalFormula };
            var historySteps = new SortedSet<string>() { originalFormula };
            int steps = 1;

            var breakFormula = "e";

            // Now we need to go through each replacement and process it as a possibility
            while(true)
            {
                var tempStepResults = new HashSet<string>();
                bool found = false;

                foreach(var thisFormula in currentStepResults)
                {
                    var matches = regex.Matches(thisFormula);

                    // For each match, we can (a) replace it or (b) ignore it, so let's find all of our possible replacements
                    foreach(Match match in matches)
                    {
                        // Get the before and after of this match
                        var beginFormula = match.Index > 0 ? thisFormula.Substring(0, match.Index) : string.Empty;
                        var endFormula = match.Index + match.Length < thisFormula.Length ? thisFormula.Substring(match.Index + match.Length) : string.Empty;

                        // Replace it and ensure we haven't tried this one before
                        var tempFormula = $"{beginFormula}{this.reverseReplacements[match.Value]}{endFormula}";

                        // See if we have found the stopping point
                        found = string.Equals(tempFormula, breakFormula);
                        if (found) break;

                        // Saves on memory, put into a sortedset will make searching a little faster
                        var md5 = GetMD5Hash(tempFormula);

                        if (!historySteps.Contains(md5))
                        {
                            tempStepResults.Add(tempFormula);
                            historySteps.Add(md5);
                        }
                    }

                    // Going to shortcut this instead of scanning 1 million+
                    // Assume we need to find some of the shortest results after replacement
                    tempStepResults = tempStepResults.OrderBy(a => a.Length).Take(2000).ToHashSet();
                }

                // If we have found a match, we're done
                if (found)
                {
                    break;
                }

                // Start new
                currentStepResults = tempStepResults;

                // Increment
                steps++;
            }

            return steps.ToString();

            var currentFormula = new HashSet<string>() { "e" };
            while(true)
            {
                var tempFormulas = new HashSet<string>();

                foreach (var formula in currentFormula)
                {
                    var stepList = ProcessFormulaStep(formula);
                    
                    foreach(var t in stepList)
                        tempFormulas.Add(t);
                }

                // Identify if we have created the formula or not
                if (tempFormulas.Contains(originalFormula))
                {
                    break;
                }

                // It does not, continue on with new formulas
                currentFormula = tempFormulas;
                steps++;
            }

            return steps.ToString();
        }

        private string GetMD5Hash(string input)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
