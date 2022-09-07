using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2021
{

    class Day14 : ASolution
    {
        private string start = string.Empty;
        private Dictionary<string, string[]> instructions2 = new Dictionary<string, string[]>();
        private Dictionary<string, UInt64> pairCount = new Dictionary<string, ulong>();

        public Day14() : base(14, 2021, "Extended Polymerization")
        {
//             DebugInput = @"NNCB

// CH -> B
// HH -> N
// CB -> H
// NH -> C
// HB -> C
// HC -> B
// HN -> C
// NN -> C
// BH -> H
// NC -> B
// NB -> B
// BN -> B
// BB -> N
// BC -> B
// CC -> N
// CN -> C";
        }

        private void Reset()
        {
            var s = Input.SplitByBlankLine();

            start = s[0][0];

            instructions2.Clear();
            this.pairCount.Clear();

            foreach(var line in s[1])
            {
                var s2 = line.Split(" -> ");
                instructions2.Add(s2[0], new string[] { s2[0][0] + s2[1], s2[1] + s2[0][1] });
            }
        }

        protected override string? SolvePartOne()
        {
            Reset();

            Utilities.Repeat(() => CountPairs(), 10);

            return SolvePuzzle().ToString();
        }

        private void CountPairs()
        {
            // If pairCount is empty, we need to calculate our first set
            if (this.pairCount.Count == 0)
            {
                this.pairCount = this.start.GetPairs().GroupBy(pair => pair).ToDictionary(grp => grp.Key, grp => (ulong) grp.LongCount());
            }

            // Now we increase the amount
            var newPairCount = new Dictionary<string, UInt64>();

            foreach(var kvp in this.pairCount)
            {
                // Find the output of this pair
                var output = this.instructions2[kvp.Key];

                // Then we increment the output pairs by the value of this pair
                if (!newPairCount.ContainsKey(output[0]))
                    newPairCount[output[0]] = kvp.Value;
                else
                    newPairCount[output[0]] += kvp.Value;

                if (!newPairCount.ContainsKey(output[1]))
                    newPairCount[output[1]] = kvp.Value;
                else
                    newPairCount[output[1]] += kvp.Value;
            }

            // Set the new counts
            this.pairCount = newPairCount;
        }

        private UInt64? SolvePuzzle()
        {
            // Add all of the pairs up if they have the min or max
            var elements = this.pairCount.Keys.Select(key => (key[0], this.pairCount[key]))
                .GroupBy(kvp => kvp.Item1)
                .ToDictionary(grp => grp.Key, grp => grp.Sum(g => g.Item2));

            // We know that in each pair, the second element is the first element of the next pair
            // so we skip the second elements EXCEPT we need to correct for the end element
            elements[start[start.Length - 1]]++;

            return (elements.Max(kvp => kvp.Value) - elements.Min(kvp => kvp.Value));
        }

        protected override string? SolvePartTwo()
        {
            Reset();
            
            Utilities.Repeat(() => CountPairs(), 40);

            return SolvePuzzle().ToString();
        }
    }
}

