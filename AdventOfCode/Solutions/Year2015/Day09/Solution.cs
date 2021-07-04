using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2015
{

    class Day09 : ASolution
    {
        private Dictionary<string, Dictionary<string, int>> distances = new Dictionary<string, Dictionary<string, int>>();

        public Day09() : base(09, 2015, "")
        {
            // Load up the information
            // Sample: London to Dublin = 464
            foreach(var line in Input.SplitByNewline())
            {
                var matches = Regex.Match(line, "^([A-Za-z]+) to ([A-Za-z]+) = ([0-9]+)$");

                if (matches.Success)
                {
                    // Define a new entry
                    if (!distances.ContainsKey(matches.Groups[1].Value))
                        distances[matches.Groups[1].Value] = new Dictionary<string, int>();

                    distances[matches.Groups[1].Value][matches.Groups[2].Value] = Int32.Parse(matches.Groups[3].Value);
                }
            }
        }

        // Distinctify all of our locations
        private string[] GetAllLocations() =>
            this.distances.SelectMany(a => a.Value.Keys).Union(this.distances.Keys).Distinct().ToArray();

        // Find the appropriate distance
        private int FindDistance(string a, string b) =>
            this.distances.ContainsKey(a) && this.distances[a].ContainsKey(b) ? this.distances[a][b] : (this.distances.ContainsKey(b) && this.distances[b].ContainsKey(a) ? this.distances[b][a] : 0);

        protected override string SolvePartOne()
        {
            var combinations = Utilities.Permutations<string>(GetAllLocations());

            int min = Int32.MaxValue;

            foreach(var combo in combinations)
            {
                var thisCombo = combo.ToArray();
                var thisTotal = 0;

                for (var i = 0; i < thisCombo.Length - 1; i++)
                {
                    // Find the distance between thisCombo[i] and [i+1];
                    thisTotal += this.FindDistance(thisCombo[i], thisCombo[i + 1]);
                }

                if (thisTotal < min)
                {
                    min = thisTotal;
                }
            }

            return min.ToString();
        }

        protected override string SolvePartTwo()
        {
            var combinations = Utilities.Permutations<string>(GetAllLocations());

            int max = Int32.MinValue;

            foreach(var combo in combinations)
            {
                var thisCombo = combo.ToArray();
                var thisTotal = 0;

                for (var i = 0; i < thisCombo.Length - 1; i++)
                {
                    // Find the distance between thisCombo[i] and [i+1];
                    thisTotal += this.FindDistance(thisCombo[i], thisCombo[i + 1]);
                }

                if (thisTotal > max)
                {
                    max = thisTotal;
                }
            }

            return max.ToString();
        }
    }
}
