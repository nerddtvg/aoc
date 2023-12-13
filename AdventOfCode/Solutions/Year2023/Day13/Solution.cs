using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Reflection;
using System.Diagnostics;


namespace AdventOfCode.Solutions.Year2023
{

    class Day13 : ASolution
    {
        private string[][] groups;
        private List<int> results = new();

        public Day13() : base(13, 2023, "Point of Incidence")
        {
            // For Part 2, make these 0/1 strings so we can convert to numbers quickly
            groups = Input.SplitByBlankLine(true)
                .Select(group => group.Select(line => line.Replace('.', '0').Replace('#', '1')).ToArray())
                .ToArray();
        }

        private int GetMirrorPosition(uint[] rows, int cache, int part)
        {
            // Work down each row provided
            // Look at the current row and next row
            // If they match:
            // * Work backwards and check if those rows match
            // * Until reaching the end of the rows list in either direction
            // * If the end is reached and rows matched, return the original row index

            // Part 2: Only change is a "smudge"
            // One smudge per group, so flag if we have used that and if so and
            // there is any other row that doesn't match, this isn't a match
            // We also MUST ignore the original result from part 1 (cache)
            for (int i = 0, q = 1; i < rows.Length-1; i++, q++)
            {
                // If we have a cache and we match, skip this for part 2
                // part 2 demands different results
                if (cache > 0 && i + 1 == cache)
                    continue;

                // If rows[i] and rows[q] match, then we need to work backwards
                if (rows[i] == rows[q] || (part == 2 && Part2Match(rows[i], rows[q])))
                {
                    // Keep track if things have matched
                    bool matched = true;

                    // Need to track if we have used the "smudge"
                    // For part 1, we keep this true to prevent issues later
                    var usedSmudge = part == 1 || rows[i] != rows[q];

                    for (int i2 = i - 1, q2 = q + 1; matched && 0 <= i2 && q2 < rows.Length; i2--, q2++)
                    {
                        if (rows[i2] != rows[q2])
                        {
                            // If we haven't used the "smudge" yet, we could here
                            if (!usedSmudge && Part2Match(rows[i2], rows[q2]))
                            {
                                usedSmudge = true;
                                continue;
                            }

                            matched = false;
                        }
                    }

                    // Return the 1-indexed position
                    // Not starting from zero
                    // usedSmudge is always true for part 1
                    // usedSmudge makes sure in part 2 we have used a "smudge"
                    if (matched && usedSmudge) return i + 1;
                }
            }

            return 0;
        }

        private bool Part2Match(uint a, uint b)
        {
            // Allow for one bit to be different
            return System.Numerics.BitOperations.PopCount(a ^ b) == 1;
        }

        private uint[] StringsToInts(string[] group)
        {
            // Convert from base2 to decimal integers
            return group.Select(line => Convert.ToUInt32(line, 2)).ToArray();
        }

        private int FindMirror(string[] group, int cache=0)
        {
            // If we have a cache we are in part 2, need to _NOT_
            // find the same value
            int part = cache == 0 ? 1 : 2;

            // Provided a group of rows, find the mirror position
            var cols = StringsToInts(group.GetColumns());

            var mirror = GetMirrorPosition(cols, cache, part);

            if (mirror > 0)
                return mirror;

            mirror = GetMirrorPosition(StringsToInts(group), cache / 100, part);

            Debug.Assert(mirror > 0);

            return 100 * mirror;
        }

        protected override string? SolvePartOne()
        {
            groups.ForEach(group => results.Add(FindMirror(group)));

            return results.Sum().ToString();
        }

        protected override string? SolvePartTwo()
        {
            var newResults = groups.Select((group, idx) => FindMirror(group, results[idx])).ToList();

            return newResults.Sum().ToString();
        }
    }
}

