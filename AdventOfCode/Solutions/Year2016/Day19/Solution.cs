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
        private List<uint> indices = new List<uint>();
        private uint pos = 0;
        private uint next = 0;
        private uint count = 0;

        public Day19() : base(19, 2016, "An Elephant Named Joseph")
        {
            Reset();
        }

        private void Reset()
        {
            // Initiate our list
            this.count = UInt32.Parse(Input);
            this.pos = 0;
            this.next = 0;
            this.elves.Clear();
            this.indices.Clear();
            for (uint i = 0; i < count; i++)
            {
                elves.Add(i, 1);
                this.indices.Add(i);
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

        private void RunRound2()
        {
            // If we're done...
            if (this.elves.Count <= 1)
                return;

            var steal = (int) ((this.pos + (this.indices.Count / 2)) % this.indices.Count);

            // Get the elf at that index
            var stolenElf = this.indices.ElementAt(steal);

            // Remove the elf from our dictionary and index list
            this.elves.Remove(stolenElf);

            // RemoveAt is slow, recommending we recreate the list
            // https://stackoverflow.com/questions/6926554/how-to-quickly-remove-items-from-a-list
            this.indices.RemoveAt(steal);

            // Move forward
            if (this.pos >= this.indices.Count)
                this.pos = (uint) this.indices.Count - 1;

            this.pos = (this.pos + 1) % (uint) this.indices.Count;
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
            // Let's try to find a pattern
            // Brute forcing this is bad
            // Nothing works on both a dictionary and index based system right?
            /* for (int i = 5; i <= 100; i++)
            {
                DebugInput = i.ToString();
                Reset();

                do
                {
                    RunRound2();
                } while (this.elves.Count > 1);

                Console.WriteLine($"{i} Elves: {this.elves.First().Key + 1}");
            } */

            /* Reset();

            do
            {
                RunRound2();
            } while (this.elves.Count > 1);

            // We're off by one in the index
            return (this.elves.First().Key+1).ToString();
            // return null; */

            // I don't know the formulas for this part
            // Looking at the mega thread, there are MANY different
            // solutions for Part 2
            // Here is just one: https://old.reddit.com/r/adventofcode/comments/5j4lp1/2016_day_19_solutions/dbdihvu/

            return GetFormulaCrossPosition(Int32.Parse(Input)).ToString();
        }

        public static int GetFormulaCrossPosition(int n)
        {
            int pow = (int)Math.Floor(Math.Log(n) / Math.Log(3));
            int b = (int)Math.Pow(3, pow);
            if (n == b)
                return n;
            if (n - b <= b)
                return n - b;
            return 2 * n - 3 * b;
        }
    }
}
