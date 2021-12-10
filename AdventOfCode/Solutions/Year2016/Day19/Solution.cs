using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{

    class Day19 : ASolution
    {
        private Dictionary<uint, uint> elves = new Dictionary<uint, uint>();
        private uint pos = 0;
        private uint next = 0;
        private uint count = 0;

        public Day19() : base(19, 2016, "An Elephant Named Joseph")
        {
            // Initiate our list
            this.count = UInt32.Parse(Input);
            for (uint i = 0; i < count; i++)
            {
                elves.Add(i, 1);
            }
        }

        private uint FindNext(uint start)
        {
            for (uint i = 0; i < count; i++)
            {
                var index = (start + i) % count;

                if (this.elves[index] > 0)
                    return index;
            }

            throw new Exception("None found!");
        }

        private void RunRound()
        {
            if (this.elves[this.pos] == 0)
            {
                // Search for the next present starting from the last stolen spot + 1
                this.pos = FindNext(this.next);
            }

            // We're done here
            if (this.elves[this.pos] == count)
                return;

            // Who are we stealing from?
            this.next = FindNext(this.pos + 1);

            this.elves[this.pos] += this.elves[this.next];
            this.elves[this.next] = 0;

            // Move one more up
            this.pos = (this.next + 1) % count;
            this.next += 2;
        }

        protected override string SolvePartOne()
        {
            do
            {
                RunRound();
            } while (this.elves[this.pos] != count);

            // We're off by one in the index
            return (this.pos + 1).ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
