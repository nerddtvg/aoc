using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{

    class Day24 : ASolution
    {
        public char[][] grid = new char[][] { };
        public int width = 0;
        public int height = 0;
        public Dictionary<char, (int x, int y)> positions = new Dictionary<char, (int x, int y)>();

        public Day24() : base(24, 2016, "Air Duct Spelunking")
        {
            Reset();
        }

        private void Reset()
        {
            this.grid = Input.SplitByNewline().Select(line => line.Trim().ToCharArray()).ToArray();
            this.width = this.grid[0].Length;
            this.height = this.grid.Length;

            // Remove whitespace for an accurate count
            var reparsed = Input.Replace("\n", "").Replace("\r", "");
            this.positions = Enumerable
                .Range(0, 10)
                // Find each of the characters 0 through 9
                .Select(idx => new { Key = (char)('0' + idx), Value = reparsed.IndexOf((char)('0' + idx)) })
                // Make sure we found it
                .Where(obj => obj.Value >= 0)
                // Convert to our positions
                .ToDictionary(obj => obj.Key, obj => (obj.Value % this.width, obj.Value / this.width));
        }

        protected override string SolvePartOne()
        {
            return null;
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
