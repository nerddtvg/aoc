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
            // Rewrote Part 2: Assume dictionaries exist filled with zeros, don't need to create a new one each time
            var active = grid[0].Select((c, idx) => (c, idx)).ToDictionary(itm => itm.idx, itm => itm.c == 'S' ? BigInteger.One : BigInteger.Zero);

            var splitCount = 0;

            foreach (var line in grid.Skip(1))
            {
                // We track the 'active' beams traveling down
                // We remove any that hit splitters
                // We add any that need to be added
                // Ensure we don't double count
                line.ForEach((c, idx) =>
                {
                    // Did we get a splitter?
                    if (c == '^' && active[idx] > 0)
                    {
                        // Count this splitter
                        splitCount++;

                        // If this splitter position was 'active' above, stop and start new
                        // Overflows don't matter since we are not using these in a loop

                        // Part one we used HashSet.Add, part 2 must check for existence
                        // Rewrote part 2: Assume there is a value
                        if (0 <= idx - 1)
                            active[idx - 1] += active[idx];

                        if (idx + 1 < grid[0].Length)
                            active[idx + 1] += active[idx];

                        // Remove this position as a splitter stopped the beam
                        active[idx] = 0;
                    }
                });
            }

            // Save for Part 2
            timelineCount = active.SumBigInteger(itm => itm.Value) ?? BigInteger.Zero;

            // Time  : 00:00:00.0032827
            // Time with Part 2: 00:00:00.0104678
            // Rewrote time (simplified logic): 00:00:00.0095771
            return splitCount.ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Time: Neglible, saved from part 1
            return timelineCount.ToString();
        }
    }
}

