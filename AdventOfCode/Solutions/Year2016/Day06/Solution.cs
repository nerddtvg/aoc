using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{

    class Day06 : ASolution
    {
        private Dictionary<int, List<char>> characters { get; set; } = new();

        public Day06() : base(06, 2016, "")
        {
            LoadChars();
        }

        private void LoadChars()
        {
            this.characters = new Dictionary<int, List<char>>();

            // Figure out the line length first to initialize the dictionary
            var l0 = Input.SplitByNewline().First();

            for (int i = 0; i < l0.Length; i++)
                this.characters[i] = new List<char>();

            // Now add each character to the list
            foreach(var line in Input.SplitByNewline())
            {
                int count = 0;
                foreach(var c in line)
                {
                    this.characters[count].Add(c);
                    count++;
                }
            }
        }

        protected override string SolvePartOne()
        {
            return this.characters.Keys.Select(k =>
            {
                return this.characters[k].GroupBy(c => c).OrderByDescending(c => c.Count()).First().Key;
            }).JoinAsString();
        }

        protected override string SolvePartTwo()
        {
            return this.characters.Keys.Select(k =>
            {
                return this.characters[k].GroupBy(c => c).OrderBy(c => c.Count()).First().Key;
            }).JoinAsString();
        }
    }
}
