using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2025
{

    class Day07 : ASolution
    {
        private string[] grid;

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
            var active = new HashSet<int>([grid[0].IndexOf('S')]);
            var splitCount = 0;

            foreach (var line in grid.Skip(1))
            {
                var newActive = new HashSet<int>();

                // We track the 'active' beams traveling down
                // We remove any that hit splitters
                // We add any that need to be added
                // Ensure we don't double count
                line.ForEach((c, idx) =>
                {
                    // If this index is active
                    if (active.Contains(idx))
                    {
                        // Did we get a splitter?
                        if (c == '^')
                        {
                            // Count this splitter
                            splitCount++;

                            // If this splitter position was 'active' above, stop and start new
                            // Overflows don't matter since we are not using these in a loop
                            newActive.Add(idx - 1);
                            newActive.Add(idx + 1);
                        }
                        else
                        {
                            // This continues as-is
                            newActive.Add(idx);
                        }
                    }
                });

                active = newActive;
            }

            // Time  : 00:00:00.0032827
            return splitCount.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

