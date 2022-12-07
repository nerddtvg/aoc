using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Diagnostics;

namespace AdventOfCode.Solutions.Year2018
{

    class Day25 : ASolution
    {

        public Day25() : base(25, 2018, "Four-Dimensional Adventure")
        {
            var examples = new Dictionary<string, int>()
            {
                {
                    @" 0,0,0,0
                    3,0,0,0
                    0,3,0,0
                    0,0,3,0
                    0,0,0,3
                    0,0,0,6
                    9,0,0,0
                    12,0,0,0",
                    2
                },
                {
                    @"-1,2,2,0
                    0,0,2,-2
                    0,0,0,-2
                    -1,2,0,0
                    -2,-2,-2,2
                    3,0,2,-1
                    -1,3,2,2
                    -1,0,-1,0
                    0,2,1,-2
                    3,0,0,0",
                    4
                },
                {
                    @"1,-1,0,1
                    2,0,-1,0
                    3,2,-1,0
                    0,0,3,1
                    0,0,-1,-1
                    2,3,-2,0
                    -2,2,0,0
                    2,-2,0,-1
                    1,-1,0,-1
                    3,2,0,2",
                    3
                },
                {
                    @"1,-1,-1,-2
                    -2,-2,0,1
                    0,2,1,3
                    -2,3,-2,1
                    0,2,3,-2
                    -1,-1,1,-2
                    0,-2,-1,0
                    -2,2,3,-1
                    1,2,2,0
                    -1,-2,0,-2",
                    8
                },
            };

            // Check each example against the logic
            foreach (var example in examples)
            {
                var count = CountConstellations(example.Key);
                Debug.Assert(Debug.Equals(count, example.Value), $"Expected: {example.Value}\nActual: {count}");
            }
        }

        private int CountConstellations(string input)
        {
            // Each line is a star coord in (x, y, z, t)
            var stars = input.SplitByNewline(true)
                .Select(line => line.Split(",", options: StringSplitOptions.TrimEntries).Select(pt => Int32.Parse(pt)).ToArray())
                .Select(pt => (pt[0], pt[1], pt[2], pt[3]))
                .ToList();

            var point = stars[0];
            var constellations = new List<List<(int x, int y, int z, int t)>>()
            {
                new() { stars[0] }
            };

            for (int i = 1; i < stars.Count; i++)
            {
                var thisPoint = stars[i];

                // Check if we make a chain
                if (point.ManhattanDistance(thisPoint) <= 3)
                {
                    // Save in case we need distance later
                    constellations.Last().Add(thisPoint);
                }
                else
                {
                    if (constellations.Last().Last().ManhattanDistance(thisPoint) <= 3)
                    {
                        constellations.Last().Add(thisPoint);
                        continue;
                    }

                    // New!
                    constellations.Add(new() { thisPoint });

                    // Save the old point
                    point = thisPoint;
                }
            }

            return constellations.Count(c => c.Count > 1);
        }

        protected override string? SolvePartOne()
        {
            return CountConstellations(Input).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

