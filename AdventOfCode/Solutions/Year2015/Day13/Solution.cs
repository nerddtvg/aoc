using System;
using System.Collections.Generic;
using System.Text;

using System.Text.RegularExpressions;
using System.Linq;

namespace AdventOfCode.Solutions.Year2015
{

    class Day13 : ASolution
    {
        private Dictionary<string, Dictionary<string, int>> _guests = new Dictionary<string, Dictionary<string, int>>();

        public Day13() : base(13, 2015, "")
        {
            var matches = Regex.Matches(Input, "([a-z]+) would (gain|lose) ([0-9]+) happiness units by sitting next to ([a-z]+).", RegexOptions.IgnoreCase);

            // Now we have our matches
            if (matches.Count > 0)
            {
                foreach(Match match in matches)
                {
                    var guest1 = match.Groups[1].Value;
                    int factor = (match.Groups[2].Value == "gain" ? 1 : -1);
                    int delta = Int32.Parse(match.Groups[3].Value);
                    var guest2 = match.Groups[4].Value;

                    // Now add this information
                    if (!this._guests.ContainsKey(guest1))
                        this._guests[guest1] = new Dictionary<string, int>();

                    this._guests[guest1][guest2] = factor * delta;
                }
            }
        }

        // Calculate the happiness factor for this setup
        private int GetHappiness(string[] names)
        {
            int ret = 0;

            for (int i = 0; i < names.Length; i++)
            {
                var ig0 = i == 0 ? names.Length - 1 : i - 1;
                var ig2 = (i + 1) % names.Length;

                // Get the three we're calculating here (looking from the center to both sides)
                var guest0 = names[ig0];
                var guest1 = names[i];
                var guest2 = names[ig2];

                ret += this._guests[guest1][guest0] + this._guests[guest1][guest2];
            }

            return ret;
        }

        protected override string SolvePartOne()
        {
            int maxHappiness = Int32.MinValue;

            // Get all combinations possible of our happiness factors
            foreach(var order in this._guests.Keys.Permutations())
            {
                // Find this happiness value
                var tHappiness = this.GetHappiness(order.ToArray());

                maxHappiness = Math.Max(maxHappiness, tHappiness);
            }

            return maxHappiness.ToString();
        }

        protected override string SolvePartTwo()
        {
            // Add ourself to each
            var currentKeys = this._guests.Keys;
            this._guests["self"] = new Dictionary<string, int>();

            foreach(var key in currentKeys)
            {
                this._guests[key]["self"] = 0;
                this._guests["self"][key] = 0;
            }

            // Find the new max order
            int maxHappiness = Int32.MinValue;

            // Get all combinations possible of our happiness factors
            foreach(var order in this._guests.Keys.Permutations())
            {
                // Find this happiness value
                var tHappiness = this.GetHappiness(order.ToArray());

                maxHappiness = Math.Max(maxHappiness, tHappiness);
            }

            return maxHappiness.ToString();
        }
    }
}
