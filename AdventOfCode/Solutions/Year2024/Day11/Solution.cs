using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    class Day11 : ASolution
    {
        public List<ulong> stones;

        public Day11() : base(11, 2024, "Plutonian Pebbles")
        {
            // DebugInput = "125 17";

            stones = [.. Input.ToIntArray(" ").Select(Convert.ToUInt64)];
        }

        public void RunStones()
        {
            // Peforms a single pass of the stone math
            for (int i = 0; i < stones.Count; i++)
            {
                var digits = stones[i].GetDigits();

                if (stones[i] == 0)
                {
                    stones[i] = 1;
                }
                else if (digits.Length % 2 == 0)
                {
                    // Split this in two
                    stones[i] = ulong.Parse(digits[..(digits.Length / 2)].JoinAsString());
                    stones.Insert(i + 1, ulong.Parse(digits[(digits.Length / 2)..].JoinAsString()));

                    // Skip the next stone we just made
                    i++;
                }
                else
                {
                    stones[i] *= 2024;
                }
            }
        }

        protected override string? SolvePartOne()
        {
            // Time: 00:00:02.6260900
            Utilities.Repeat(RunStones, 25);

            return stones.Count.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

