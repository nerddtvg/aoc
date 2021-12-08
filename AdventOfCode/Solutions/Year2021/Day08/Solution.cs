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

        public int DetermineDisplay(string line)
        {
            // Start with 1 to find c & f
            // Then use 7 to determine a with c & f
            // Then 4 to determine b and d
            // Then 5 to determine f and g
            // From that we can get c
            // Then 3 to determine d
            // From that we can get b and then e

            char a = '0';
            char b = '0';
            char c = '0';
            char d = '0';
            char e = '0';
            char f = '0';
            char g = '0';

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
            a = searchA.Where(ch => !cf.Contains(ch)).First();

            // Use 4 to identify possible b and d
            var bd = groups
                .Where(grp => grp.Length == 4)
                .FirstOrDefault()?
                .Replace(cf[0], ' ')
                .Replace(cf[1], ' ')
                .Replace(" ", "") ?? string.Empty;

            if (string.IsNullOrEmpty(bd))
                throw new Exception($"No BD: {line}");

            // Use the 5 to find 'f' from cf and bd (must have a and f, but no c)
            var searchGF = groups
                .Where(grp => grp.Length == 5 && grp.Contains(a) && grp.Contains(bd[0]) && grp.Contains(bd[1]) && (grp.Contains(cf[0]) ^ grp.Contains(cf[1])))
                .FirstOrDefault()?
                .Replace(a, ' ')
                .Replace(bd[0], ' ')
                .Replace(bd[1], ' ')
                .Replace(" ", "") ?? string.Empty;

            if (string.IsNullOrEmpty(searchGF))
                throw new Exception($"No GF: {line}");

            // searchGF gives us... a 'g' and 'f'
            f = searchGF.First(ch => cf.Contains(ch));
            g = searchGF.Replace(f.ToString(), "")[0];

            // From that we now have 'c'
            c = cf.First(ch => ch != f);

            // Currently we have a, [bd], c, f, and g
            // Use a 3 to find d
            var searchD = groups
                .Where(grp => grp.Length == 5 && grp.Contains(a) && grp.Contains(c) && grp.Contains(f) && grp.Contains(g))
                .FirstOrDefault()?
                .Replace(a, ' ')
                .Replace(c, ' ')
                .Replace(f, ' ')
                .Replace(g, ' ')
                .Replace(" ", "") ?? string.Empty;

            if (string.IsNullOrEmpty(searchD))
                throw new Exception($"No DE: {line}");

            d = searchD[0];
            b = bd.Replace(d.ToString(), "")[0];

            // Find e just be process of elimination
            e = "abcdefg"
                .Replace(a.ToString(), "")
                .Replace(b.ToString(), "")
                .Replace(c.ToString(), "")
                .Replace(d.ToString(), "")
                .Replace(f.ToString(), "")
                .Replace(g.ToString(), "")[0];

            // Quick convert to dictionary to make lookups easier
            var replacements = new Dictionary<char, char>()
            {
                { a, 'a' },
                { b, 'b' },
                { c, 'c' },
                { d, 'd' },
                { e, 'e' },
                { f, 'f' },
                { g, 'g' }
            };

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
