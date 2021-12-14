using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2021
{

    class Day14 : ASolution
    {
        private string start;
        private Dictionary<string, string> instructions = new Dictionary<string, string>();
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

            instructions.Clear();
            instructions2.Clear();
            this.pairCount.Clear();

            foreach(var line in s[1])
            {
                var s2 = line.Split(" -> ");
                instructions.Add(s2[0], s2[0][0] + s2[1] + s2[0][1]);
                instructions2.Add(s2[0], new string[] { s2[0][0] + s2[1], s2[1] + s2[0][1] });
            }
        }

        private IEnumerable<string> GetPairs(string input)
        {
            for (int i = 0; i < input.Length - 1; i++)
            {
                yield return (input[i].ToString() + input[i + 1]);
            }
        }

        private string ReplacePairs(string input)
        {
            var ret = string.Empty;

            foreach(var pair in GetPairs(input))
            {
                var newPair = pair;

                if (this.instructions.ContainsKey(pair))
                {
                    newPair = this.instructions[pair];
                }

                // If we already have some string, trim off the first char
                newPair = (ret.Length == 0 ? newPair : newPair.Substring(1));
                ret += newPair;
            }

            return ret;
        }

        protected override string? SolvePartOne()
        {
            // var poly = this.start;

            // Utilities.Repeat(() => poly = ReplacePairs(poly), 10);

            // return (poly.GroupBy(ch => ch).Max(grp => grp.Count()) - poly.GroupBy(ch => ch).Min(grp => grp.Count())).ToString();

            // For part 2, we're refactoring
            // Hints from the megathread (because I was just too tired to figure out the concept)
            // point to simply counting the number of pairs
            // for each translation. CH => CBH {CB, BH}
            Reset();

            Utilities.Repeat(() => CountPairs(), 10);

            return SolvePuzzle().ToString();
        }

        private void CountPairs()
        {
            // If pairCount is empty, we need to calculate our first set
            if (pairCount.Count == 0)
            {
                foreach(var pair in GetPairs(this.start))
                {
                    this.pairCount[pair] = 1;
                }
            }

            // Now we increase the amount
            var newPairCount = new Dictionary<string, UInt64>();

            foreach(var key in this.pairCount.Keys)
            {
                if (!this.instructions2.ContainsKey(key))
                {
                    if (newPairCount.ContainsKey(key))
                        newPairCount[key] += this.pairCount[key];
                    else
                        newPairCount[key] = this.pairCount[key];
                }
                else
                {
                    // Find the output of this pair
                    var output = this.instructions2[key];

                    // Then we increment the output pairs by the value of this pair
                    if (!newPairCount.ContainsKey(output[0]))
                        newPairCount[output[0]] = this.pairCount[key];
                    else
                        newPairCount[output[0]] += this.pairCount[key];

                    if (!newPairCount.ContainsKey(output[1]))
                        newPairCount[output[1]] = this.pairCount[key];
                    else
                        newPairCount[output[1]] += this.pairCount[key];
                }
            }

            // Set the new counts
            this.pairCount = newPairCount;
        }

        private UInt64? SolvePuzzle()
        {
            // Add all of the pairs up if they have the min or max
            var elements = this.pairCount.Keys.SelectMany(key =>
            {
                var characters = key.ToCharArray();
                return characters.Select(ch => (ch, this.pairCount[key]));
            })
            .GroupBy(kvp => kvp.ch)
            .ToDictionary(grp => grp.Key, grp => grp.Sum(g => g.Item2));

            // EVerything is counted twice because pairs overlap EXCEPT the first and last letter so we account for that by dividing by two
            elements[start[0]]++;
            elements[start[start.Length - 1]]++;

            // Now divide by two
            elements.Keys.ToList().ForEach(key => elements[key] /= 2);

            return (elements.Max(kvp => kvp.Value) - elements.Min(kvp => kvp.Value));
        }

        protected override string? SolvePartTwo()
        {
            // For part 2, we're refactoring
            // Hints from the megathread (because I was just too tired to figure out the concept)
            // point to simply counting the number of pairs
            // for each translation. CH => CBH {CB, BH}
            Reset();

            Utilities.Repeat(() => CountPairs(), 40);

            return SolvePuzzle().ToString();
        }
    }
}

#nullable restore
