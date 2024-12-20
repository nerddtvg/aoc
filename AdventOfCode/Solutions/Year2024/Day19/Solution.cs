using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    class Day19 : ASolution
    {
        Dictionary<string, ulong> combinations = [];
        string[] desired = [];

        Regex patterns = new("^$");

        public Day19() : base(19, 2024, "Linen Layout")
        {
            DebugInput = @"r, wr, b, g, bwu, rb, gb, br

            brwrr
            bggr
            gbbr
            rrbgbr
            ubwu
            bwurrg
            brgr
            bbrgwb";
        }

        protected override string? SolvePartOne()
        {
            List<string> validPatterns = [];

            List<string> tempPatterns = [.. Input
                .SplitByBlankLine(shouldTrim: true)[0][0]
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .OrderByDescending(str => str.Length)
                .ThenBy(str => str)
            ];

            desired = Input.SplitByBlankLine(shouldTrim: true)[1];

            // We need to reduce the patterns down
            // A lot of these can be combined ot make the longer ones such as 'r' and 'b' can make 'rb' but also 'br' making 'rb' redundant
            while (tempPatterns.Count > 1)
            {
                // Combine validPatterns with the other patterns
                patterns = new($"^({string.Join('|', tempPatterns[1..])})+$");

                // If we can match tempPatterns[0], then it is a duplicate
                if (!patterns.IsMatch(tempPatterns[0]))
                {
                    // Found a valid option
                    validPatterns.Add(tempPatterns[0]);

                    // Only one way to make this
                    combinations[tempPatterns[0]] = 1;
                }
                else
                {
                    // Work down the tempPatterns array to determine how many ways this can be made
                    HashSet<string> variations = [];

                    for (int i = 1; i < tempPatterns.Count; i++)
                    {
                        Regex shortPattern = new($"^(({string.Join(")|(", tempPatterns[i..])}))+$");

                        var matches = shortPattern.Match(tempPatterns[0]);

                        // No more matching, exit early
                        if (!matches.Success)
                            break;

                        // Otherwise let's add this to the variations list
                        var str = "";

                        foreach (Group grp in matches.Groups.Values.Skip(1))
                            if (!string.IsNullOrEmpty(grp.Value))
                                str += $"|{grp.Value}";

                        variations.Add(str);
                    }

                    // Add one for the duplicate
                    combinations[tempPatterns[0]] = (ulong)variations.Count + 1;
                }

                tempPatterns.RemoveAt(0);
            }

            // Add the last one on
            combinations[tempPatterns[0]] = 1;
            validPatterns.Add(tempPatterns[0]);

            // Regenerate
            // Include the duplicates in here (change from original Part 1 code)
            patterns = new($"^({string.Join('|', validPatterns)})+$");

            // All the time is in the constructor
            // Time: 00:00:00.1188499
            return desired.Count(patterns.IsMatch).ToString();
        }

        ulong CountCombinations(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return 0;
            }

            if (combinations.TryGetValue(str, out ulong val))
            {
                // Console.WriteLine($"*{str}: {val}");
                return val;
            }

            ulong count = 0;

            // Need to go through the permutations
            // Since we know some of the longer strings, start with those and see if we can shortcut a bit
            foreach(var key in combinations.Keys.OrderByDescending(key => key.Length).ThenBy(key => key))
            {
                // On the first match of the longest one possible, find the math for the rest of the string
                if (str.StartsWith(key))
                {
                    // We canot have str == key here because that would have been caught earlier
                    count = combinations[key] * CountCombinations(str[key.Length..]);

                    // Found a match!
                    break;
                }
            }

            Console.WriteLine($"{str}: {count}");

            // Cache for later
            // combinations[str] = count;

            return count;
        }

        protected override string? SolvePartTwo()
        {
            return desired
                .Where(pattern => patterns.IsMatch(pattern))
                .Sum(pattern => CountCombinations(pattern))
                .ToString();
        }
    }
}

