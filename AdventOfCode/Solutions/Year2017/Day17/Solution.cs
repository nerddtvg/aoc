using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2017
{

    class Day17 : ASolution
    {
        private List<int> buffer = new List<int>();
        private int pos = 0;
        private int step = 0;

        public Day17() : base(17, 2017, "")
        {
            Reset();
        }

        private void Reset()
        {
            this.pos = 0;
            this.buffer = new List<int>(){ 0 };
            this.step = Int32.Parse(Input);
        }

        private void Run(int val)
        {
            // Get the new index
            var newIndex = (this.pos + step + 1) % this.buffer.Count;

            // If this wrapped around to 0, it goes at the end of the list
            if (newIndex == 0)
            {
                this.buffer.Add(val);
                this.pos = this.buffer.Count - 1;
            }
            else
            {
                this.buffer.Insert(newIndex, val);
                this.pos = newIndex;
            }
        }

        protected override string? SolvePartOne()
        {
            for (int i = 1; i < 2018; i++)
            {
                Run(i);
            }

            return this.buffer[(this.pos + 1) % this.buffer.Count].ToString();
        }

        protected override string? SolvePartTwo()
        {
            // After trying to brute force this which I knew would be bad, I reviewed some discussion
            // Since zero doesn't move, we only care about the number that is in position 1 (after 0)
            // We don't actually need to do anything, do we?
            int nextToZero = this.buffer[1];

            int currentPos = 0;
            int currentCount = 1;

            for (int i = 1; i <= 50000000; i++)
            {
                // Get the new index
                var newIndex = (currentPos + step + 1) % currentCount;

                // If we have inserted into position 1, we are next to zero
                if (newIndex == 1)
                    nextToZero = i;

                // Our internal loop vars
                currentPos = newIndex;
                currentCount++;

                // Check this position for zero based entries
                if (currentPos == 0)
                    currentPos = currentCount - 1;
            }

            return nextToZero.ToString();
        }
    }
}

#nullable restore
