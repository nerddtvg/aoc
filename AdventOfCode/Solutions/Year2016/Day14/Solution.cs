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
        private Dictionary<uint, (string hash, Match match)> found = new Dictionary<uint, (string hash, Match match)>();
        private List<string> keys = new List<string>();

        public Day14() : base(14, 2016, "")
        {
            
        }

        private void FindTriples(string prefix, uint start)
        {
            var r = new Regex(@"([a-z0-9])\1\1");

            // Search for start + the next 1000 hashes and look for triple matches
            for (uint i = Math.Max(start, this.lastSearched); i <= start + 1000; i++)
            {
                var hash = Utilities.MD5HashString($"{prefix}{i}");

                // "Only consider the first such triplet in a hash."
                // This line got me.
                var match = r.Match(hash);

                if (match.Success)
                {
                    this.found[i] = (hash, match);
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

                // We have a triple, so we get a regular expression to match against
                var match = new Regex(this.found[i].match.Groups[1].Value[0] + @"{5}");

                // We need to see if there are ANY matches in the next 1000 of the group character * 5 (a => aaaaa)
                // These are the keys that are in our range
                var keys = this.found.Keys.Where(k => k > i && k <= i + 1000).ToList();

                foreach(var key in keys)
                {
                    // For each key, we need to check if the values match anything
                    if (match.IsMatch(this.found[key].hash))
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
