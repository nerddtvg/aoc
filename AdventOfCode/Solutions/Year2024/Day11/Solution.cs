using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    class Day11 : ASolution
    {
        public Dictionary<ulong, ulong> stones = new();

        public Day11() : base(11, 2024, "Plutonian Pebbles")
        {
            // DebugInput = "125 17";

            Input
                .ToIntArray(" ")
                .Select(Convert.ToUInt64)
                .ForEach(v => {
                    if (stones.ContainsKey(v))
                        stones[v]++;
                    else
                        stones[v] = 1;
                });
        }

        public void RunStones()
        {
            // Peforms a single pass of the stone math
            // To avoid cycling the same stones over and over,
            // keep track of how many of each stone "value"
            // we have
            Dictionary<ulong, ulong> newStones = [];

            // The wording states the order remains consistent
            // however the order has no impact on the outcome

            // To make it easier, only run each stone value once
            // and add that number of "outcomes" to the newStones
            // dictionary. So if stone '0' becomes '1', and you have
            // 35 stone '0' entries, then you now have 25 stone '1'
            // entries.
            foreach(var stone in stones.Keys)
            {
                var digits = stone.GetDigits();

                var stone1 = ulong.MaxValue;
                var stone2 = ulong.MaxValue;

                if (stone == 0)
                {
                    stone1 = 1;
                }
                else if (digits.Length % 2 == 0)
                {
                    // Split this in two
                    stone1 = ulong.Parse(digits[..(digits.Length / 2)].JoinAsString());
                    stone2 = ulong.Parse(digits[(digits.Length / 2)..].JoinAsString());
                }
                else
                {
                    stone1 = stone * 2024;
                }

                if (newStones.ContainsKey(stone1))
                    newStones[stone1] += stones[stone];
                else
                    newStones[stone1] = stones[stone];

                if (stone2 != ulong.MaxValue)
                {
                    if (newStones.ContainsKey(stone2))
                        newStones[stone2] += stones[stone];
                    else
                        newStones[stone2] = stones[stone];
                }
            }

            // Save the new stones
            stones = newStones;
        }

        protected override string? SolvePartOne()
        {
            // Time: 00:00:02.6260900
            // Rewrite for Part 2: 00:00:00.0116831
            Utilities.Repeat(RunStones, 25);

            return stones.Sum(kvp => kvp.Value).ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Time: 00:00:00.1522481
            Utilities.Repeat(RunStones, 50);

            return stones.Sum(kvp => kvp.Value).ToString();
        }
    }
}

