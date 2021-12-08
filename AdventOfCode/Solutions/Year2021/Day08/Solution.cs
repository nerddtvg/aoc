using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2021
{

    class Day08 : ASolution
    {
        public Dictionary<string, string> digits = new Dictionary<string, string>()
        {
            { "0", "abcefg" },
            { "1", "cf" },
            { "2", "acdeg" },
            { "3", "acdfg" },
            { "4", "bcdf" },
            { "5", "abdfg" },
            { "6", "abdefg" },
            { "7", "acf" },
            { "8", "abcdefg" },
            { "9", "abcdfg" }
        };

        public Day08() : base(08, 2021, "Seven Segment Search")
        {

        }

        protected override string? SolvePartOne()
        {
            // Digits 1, 4, 7, 8 all use different length sequences
            // These are the sequence lengths
            var uniqueLengths = this.digits.Values.GroupBy(v => v.Length).Where(grp => grp.Count() == 1).Select(grp => grp.Key).ToArray();

            return Input.SplitByNewline()
                // For every line...
                .Sum(line =>
                    // Get the output side
                    line.Split('|', 2, StringSplitOptions.TrimEntries)[1]
                    // Get all output groups
                    .Split(' ', StringSplitOptions.TrimEntries)
                    // Group them by length
                    .GroupBy(grp => grp.Length)
                    // If they're in the unique list, count them
                    .Where(grp => uniqueLengths.Contains(grp.Key))
                    .Sum(grp => grp.Count())
                ).ToString();
        }

        /// <summary>
        /// Removes the known characters from the replacement map
        /// </summary>
        private string RemoveKnown(Dictionary<char, char> map, string original)
        {
            foreach(var ch in map.Values)
            {
                original = original.Replace(ch.ToString(), "");
            }

            return original;
        }

        public int DetermineDisplay(string line)
        {
            // Start with 1 to find c & f
            // Then use 7 to determine a with c & f
            // Then 4 to determine b and d
            // Then 5 to determine f and g
            // From that we can get c
            // Then 3 to determine d
            // From that we can get b and then e

            Dictionary<char, char> map = new Dictionary<char, char>()
            {
                { 'a', '0' },
                { 'b', '0' },
                { 'c', '0' },
                { 'd', '0' },
                { 'e', '0' },
                { 'f', '0' },
                { 'g', '0' }
            };

            var outputGroups = line
                .Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[1]
                .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(str => str.OrderBy(ch => ch).JoinAsString())
                .ToList();

            // All groups
            var groups = line
                .Replace('|', ' ')
                .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(str => str.OrderBy(ch => ch).JoinAsString())
                .ToList();

            // Find the possible c and f
            var cf = groups.Where(grp => grp.Length == 2).FirstOrDefault();

            if (cf == default)
                throw new Exception($"No CF: {line}");

            // Use the 7 to find 'a'
            var searchA = groups.Where(grp => grp.Length == 3).FirstOrDefault();

            if (searchA == default)
                throw new Exception($"No A: {line}");

            // Find the difference
            map['a'] = searchA.Where(ch => !cf.Contains(ch)).First();

            // Use 4 to identify possible b and d
            var bd = groups
                .Where(grp => grp.Length == 4)
                .FirstOrDefault()?
                .Replace(cf[0].ToString(), "")
                .Replace(cf[1].ToString(), "") ?? string.Empty;

            if (string.IsNullOrEmpty(bd))
                throw new Exception($"No BD: {line}");

            // Use the 5 to find 'f' from cf and bd (must have a and f, but no c)
            var searchGF = RemoveKnown(map, groups
                .Where(grp => grp.Length == 5 && grp.Contains(map['a']) && grp.Contains(bd[0]) && grp.Contains(bd[1]) && (grp.Contains(cf[0]) ^ grp.Contains(cf[1])))
                .FirstOrDefault()  ?? string.Empty)
                .Replace(bd[0].ToString(), "")
                .Replace(bd[1].ToString(), "");

            if (string.IsNullOrEmpty(searchGF))
                throw new Exception($"No GF: {line}");

            // searchGF gives us... a 'g' and 'f'
            map['f'] = searchGF.First(ch => cf.Contains(ch));
            map['g'] = RemoveKnown(map, searchGF)[0];

            // From that we now have 'c'
            map['c'] = cf.First(ch => ch != map['f']);

            // Currently we have a, [bd], c, f, and g
            // Use a 3 to find d then b
            var searchD = RemoveKnown(map, groups
                .Where(grp => grp.Length == 5 && grp.Contains(map['a']) && grp.Contains(map['c']) && grp.Contains(map['f']) && grp.Contains(map['g']))
                .FirstOrDefault()  ?? string.Empty);

            if (string.IsNullOrEmpty(searchD))
                throw new Exception($"No DE: {line}");

            map['d'] = searchD[0];
            map['b'] = RemoveKnown(map, bd)[0];

            // Find e just be process of elimination
            map['e'] = RemoveKnown(map, "abcdefg")[0];

            // Quick convert to dictionary to make lookups easier
            var replacements = map.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

            // We have our letters! Huzzah!
            var outString = outputGroups
                // For each group (digit) in the output, replace the characters appropriately
                .Select(grp => grp.Select(ch => replacements[ch]).OrderBy(ch => ch).JoinAsString())
                .ToList()
                .Select(numStr => this.digits.First(dig => dig.Value == numStr).Key)
                .JoinAsString();

            return Int32.Parse(outString);
        }

        protected override string? SolvePartTwo()
        {
            return Input.SplitByNewline()
                // For every line...
                .Sum(line =>
                    // Get the output side
                    DetermineDisplay(line)
                ).ToString();
        }
    }
}

#nullable restore
