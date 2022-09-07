using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{

    class Day02 : ASolution
    {
        private (int x, int y) pos { get; set; }

        private string code { get; set; } = string.Empty;

        private Dictionary<string, (int x, int y)> moves = new Dictionary<string, (int x, int y)>()
        {
            { "U", (0, 1) },
            { "R", (1, 0) },
            { "D", (0, -1) },
            { "L", (-1, 0) }
        };

        private Dictionary<(int x, int y), string> numpad = new Dictionary<(int x, int y), string>()
        {
            { (-1, 1), "1" },
            { (0, 1), "2" },
            { (1, 1), "3" },
            { (-1, 0), "4" },
            { (0, 0), "5" },
            { (1, 0), "6" },
            { (-1, -1), "7" },
            { (0, -1), "8" },
            { (1, -1), "9" },
        };

        private Dictionary<(int x, int y), string> numpad2 = new Dictionary<(int x, int y), string>()
        {
            { (0, 2), "1" },
            
            { (-1, 1), "2" },
            { (0, 1), "3" },
            { (1, 1), "4" },

            
            { (-2, 0), "5" },
            { (-1, 0), "6" },
            { (0, 0), "7" },
            { (1, 0), "8" },
            { (2, 0), "9" },

            
            { (-1, -1), "A" },
            { (0, -1), "B" },
            { (1, -1), "C" },
            
            { (0, -2), "D" }
        };

        public Day02() : base(02, 2016, "")
        {

        }

        private void ResetPos()
        {
            this.pos = (0, 0);
            this.code = string.Empty;
        }

        private void ProcessLine(string line, int part = 1)
        {
            // Go through each character on the line and determine what number we move to
            foreach(var c in line.ToCharArray())
            {
                var move = this.moves[c.ToString()];
                var tempPos = this.pos.Add(move);

                // If we don't have a number in the new position, we continue to the next character
                if (part == 1 && this.numpad.ContainsKey(tempPos))
                    this.pos = tempPos;
                else if (part == 2 && this.numpad2.ContainsKey(tempPos))
                    this.pos = tempPos;
            }

            // At the end of the line, add the current number to the code
            if (part == 1)
                this.code += this.numpad[this.pos];
            else
                this.code += this.numpad2[this.pos];
        }

        protected override string SolvePartOne()
        {
            ResetPos();
            foreach(var line in Input.SplitByNewline())
                ProcessLine(line);

            return this.code;
        }

        protected override string SolvePartTwo()
        {
            ResetPos();
            foreach(var line in Input.SplitByNewline())
                ProcessLine(line, 2);

            return this.code;
        }
    }
}
