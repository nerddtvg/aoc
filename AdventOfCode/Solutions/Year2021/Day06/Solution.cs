using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2021
{

    class Day06 : ASolution
    {
        public Dictionary<int, UInt64> fish = new Dictionary<int, UInt64>();

        public Day06() : base(06, 2021, "Lanternfish")
        {
            // For these fish, we don't care about how many there are,
            // we only care about their timer states. Each fish with
            // the same timer (say 5) will all produce fish on the same
            // interval. So what we can do to simplify this is to keep
            // a list of timers (0-8) and a simple count of the fish
            // in that state.

            // DebugInput = "3,4,3,1,2";

            Reset();
        }

        private void Reset()
        {
            this.fish.Clear();
            this.fish = new Dictionary<int, UInt64>()
            {
                // -1 will be a place holder
                { -1, 0 },
                { 0, 0 },
                { 1, 0 },
                { 2, 0 },
                { 3, 0 },
                { 4, 0 },
                { 5, 0 },
                { 6, 0 },
                { 7, 0 },
                { 8, 0 },
            };

            // Count our input
            foreach(var i in Input.ToIntArray(","))
            {
                this.fish[i]++;
            }
        }

        private void RunDay()
        {
            // Each day we do the following:
            // Move everything down one state (0-8 moves to -1-7)
            // -1 values move back up to 8 AND 6 (6 is the reset fish, 8 is the new fish)

            for (int i = 0; i <= 8; i++)
            {
                this.fish[i - 1] = this.fish[i];
            }

            // Handle the -1 values which are "reborn"
            this.fish[6] += this.fish[-1];
            this.fish[8] = this.fish[-1];

            // To be safe
            this.fish[-1] = 0;
        }

        protected override string? SolvePartOne()
        {
            Utilities.Repeat(() => RunDay(), 80);

            return this.fish.Sum(fish => fish.Value).ToString();
        }

        protected override string? SolvePartTwo()
        {
            Reset();

            Utilities.Repeat(() => RunDay(), 256);

            return this.fish.Sum(fish => fish.Value).ToString();
        }
    }
}

#nullable restore
