using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{

    class Day18 : ASolution
    {
        public List<string> tiles;

        public Day18() : base(18, 2016, "Like a Rogue")
        {
            this.tiles = Input.SplitByNewline().ToList();
        }

        private void AddRow()
        {
            // Takes the last line in the list and calculates the next one
            var currentRow = this.tiles[this.tiles.Count - 1];
            var newRow = string.Empty;

            for (int i = 0; i < currentRow.Length; i++)
            {
                var chars = GetChars(currentRow, i);

                if (chars == "^^." || chars == ".^^" || chars == "^.." || chars == "..^")
                    newRow += '^';
                else
                    newRow += '.';
            }

            this.tiles.Add(newRow);
        }

        private string GetChars(string line, int pos)
        {
            // Gets pos-1, pos, and pos+1 tiles
            string ret = string.Empty;

            if (pos-1 < 0)
                ret += '.';
            else
                ret += line[pos - 1];

            ret += line[pos];

            if (pos+1 >= line.Length)
                ret += '.';
            else
                ret += line[pos + 1];

            return ret;
        }

        protected override string SolvePartOne()
        {
            while(this.tiles.Count < 40)
            {
                AddRow();
            }

            return this.tiles.Sum(line => line.Count(ch => ch == '.')).ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
