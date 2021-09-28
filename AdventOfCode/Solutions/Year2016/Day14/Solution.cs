using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{

    class Day14 : ASolution
    {
        private uint lastSearched = 0;
        private Dictionary<uint, (string hash, MatchCollection matches)> found = new Dictionary<uint, (string hash, MatchCollection matches)>();
        private List<string> keys = new List<string>();

        public Day14() : base(14, 2016, "")
        {
            DebugInput = "abc";
        }

        private void FindTriples(string prefix, uint start)
        {
            var r = new Regex(@"([a-z0-9])\1\1");

            // Search for start + the next 1000 hashes and look for triple matches
            for (uint i = Math.Max(start, this.lastSearched); i <= start + 1000; i++)
            {
                var hash = Utilities.MD5HashString($"{prefix}{i}");

                var matches = r.Matches(hash);

                if (matches.Count > 0)
                {
                    this.found[i] = (hash, matches);
                }
            }

            // Only increase this if we did anything
            this.lastSearched = Math.Max(start + 1000, this.lastSearched);
        }

        protected override string SolvePartOne()
        {
            // We need to find keys
            for (uint i = 0; i <= uint.MaxValue && this.keys.Count < 64; i++)
            {
                // First we need to find triples in this range to make it easier
                FindTriples(Input, i);

                // if we don't have a triple at this key, move on
                if (!this.found.ContainsKey(i))
                    continue;

                // We have a triple, or multiple triples, so we get regular expressions to match against
                var matches = this.found[i].matches
                    .Select(m => m.Groups[1].Value[0])
                    .Distinct()
                    .Select(m => new Regex(m + @"{5}"))
                    .ToList();

                // We need to see if there are ANY matches in the next 1000 of the group character * 5 (a => aaaaa)
                // These are the keys that are in our range
                var keys = this.found.Keys.Where(k => k > i && k <= i + 1000).ToList();

                foreach(var key in keys)
                {
                    // For each key, we need to check if the values match anything
                    if (matches.Any(m => m.IsMatch(this.found[key].hash)))
                    {
                        // Found one!
                        this.keys.Add(this.found[i].hash);

                        Console.WriteLine($"Found {this.keys.Count} [{i}]: {this.found[i].hash}");

                        // Find #64
                        if (this.keys.Count == 64)
                        {
                            return i.ToString();
                        }

                        break;
                    }
                }
            }

            return null;
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
