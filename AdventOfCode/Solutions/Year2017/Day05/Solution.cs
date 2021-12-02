using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2017
{

    class Day05 : ASolution
    {
        private List<int> instructions = new List<int>();
        int pos = 0;

        public Day05() : base(05, 2017, "A Maze of Twisty Trampolines, All Alike")
        {
            ResetList();
        }

        private void ResetList()
        {
            this.instructions = Input.SplitByNewline(true).Select(i => Int32.Parse(i)).ToList();
        }

        private bool Run()
        {
            // Order of operations:
            // Move to new position (if outside, exit with false)
            // Increment old position by 1
            int oldPos = this.pos;

            // Being safe here
            if (this.pos < 0 || instructions.Count < this.pos)
                return false;

            // Determine our destination
            var newPos = this.pos + instructions[this.pos];

            if (newPos > instructions.Count-1 || newPos < 0)
                return false;

            // Move
            this.pos = newPos;

            // New value at old position
            instructions[oldPos]++;

            return true;
        }

        protected override string? SolvePartOne()
        {
            int count = 0;
            do
            {
                count++;
            } while (Run());

            return count.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
