using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Numerics;


namespace AdventOfCode.Solutions.Year2025
{

    class Day07 : ASolution
    {
        private string[] grid;
        private BigInteger timelineCount = 0;

        public Day07() : base(07, 2025, "Laboratories")
        {
            // DebugInput = @"
            //     .......S.......
            //     ...............
            //     .......^.......
            //     ...............
            //     ......^.^......
            //     ...............
            //     .....^.^.^.....
            //     ...............
            //     ....^.^...^....
            //     ...............
            //     ...^.^...^.^...
            //     ...............
            //     ..^...^.....^..
            //     ...............
            //     .^.^.^.^.^...^.
            //     ...............
            //     ";

            grid = Input.SplitByNewline(true);
        }

        protected override string? SolvePartOne()
        {
            // Part 1: This was a hashset of the index only
            // Part 2: Track the number of times we hit each point so we can total it at the end
            var active = new Dictionary<int, BigInteger>() { { grid[0].IndexOf('S'), 1 } };
            var splitCount = 0;

            foreach (var line in grid.Skip(1))
            {
                var newActive = new Dictionary<int, BigInteger>();

                // We track the 'active' beams traveling down
                // We remove any that hit splitters
                // We add any that need to be added
                // Ensure we don't double count
                line.ForEach((c, idx) =>
                {
                    // If this index is active
                    if (active.ContainsKey(idx))
                    {
                        // Did we get a splitter?
                        if (c == '^')
                        {
                            // Count this splitter
                            splitCount++;

                            // If this splitter position was 'active' above, stop and start new
                            // Overflows don't matter since we are not using these in a loop

                            // Part one we used HashSet.Add, part 2 must check for existence
                            if (newActive.TryGetValue(idx - 1, out BigInteger tmp))
                                newActive[idx - 1] += active[idx];
                            else
                                newActive.Add(idx - 1, active[idx]);

                            if (newActive.TryGetValue(idx + 1, out tmp))
                                newActive[idx + 1] = tmp + active[idx];
                            else
                                newActive.Add(idx + 1, active[idx]);
                        }
                        else
                        {
                            // This continues as-is
                            if (newActive.TryGetValue(idx, out BigInteger tmp))
                                newActive[idx] += active[idx];
                            else
                                newActive.Add(idx, active[idx]);
                        }
                    }
                });

                active = newActive;
            }

            // Save for Part 2
            timelineCount = active.SumBigInteger(itm => itm.Value) ?? BigInteger.Zero;

            // Time  : 00:00:00.0032827
            // Time with Part 2: 00:00:00.0104678
            return splitCount.ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Time: Neglible, saved from part 1
            return timelineCount.ToString();
        }
    }
}

