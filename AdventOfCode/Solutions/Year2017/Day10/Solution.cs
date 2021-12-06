using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2017
{

    class Day10 : ASolution
    {
        int pos = 0;
        int skip = 0;
        private List<int> values = new List<int>();
        private List<int> lengths = new List<int>();

        public Day10() : base(10, 2017, "Knot Hash")
        {
            //DebugInput = "3, 4, 1, 5";
            Reset();
        }

        private void Reset(bool debug = false)
        {
            this.pos = 0;
            this.skip = 0;
            this.lengths = Input.ToIntArray(",").ToList();

            if (string.IsNullOrEmpty(DebugInput))
            {
                this.values = Enumerable.Range(0, 256).ToList();
            }
            else
            {
                this.values = Enumerable.Range(0, 5).ToList();
            }
        }

        private void Swap(int a, int b)
        {
            // Swap the values at indexes a and b
            a = a % values.Count;
            b = b % values.Count;

            // Same index
            if (a == b) return;

            int temp = this.values[a];
            this.values[a] = this.values[b];
            this.values[b] = temp;
        }

        private void RunRound(int length)
        {
            // We swap from pos to length
            int offset = length - 1;
            for (int i = this.pos; offset >= 0; i++, offset-=2)
            {
                // Swap these two values
                Swap(i, i + offset);
            }

            // Move the position forward
            this.pos += (length + this.skip) % this.values.Count;

            // Skip increases
            this.skip++;
        }

        protected override string? SolvePartOne()
        {
            lengths.ForEach(len => RunRound(len));

            return (this.values[0] * this.values[1]).ToString();
        }

        protected override string? SolvePartTwo()
        {
            Reset();

            // We need to run 64 rounds without resetting between them
            // Also, our lengths are now different
            this.lengths = Input.ToCharArray().Select(ch => (int)ch).ToList();
            this.lengths.AddRange(new int[] { 17, 31, 73, 47, 23 });

            // 64 rounds, no resets or changes
            Utilities.Repeat(() => lengths.ForEach(len => RunRound(len)), 64);

            var endHash = string.Empty;

            for (int offset = 0; offset < 256; offset+=16)
            {
                // Grabs the 16 values and XOR's them, then converts that to Hexadecimal
                endHash += this.values.GetRange(offset, 16).Aggregate((a, b) => a ^ b).ToString("X2").ToLowerInvariant();
            }

            return endHash;
        }
    }
}

#nullable restore
