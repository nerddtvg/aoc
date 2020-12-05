using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{
    class BoardingPass {
        public string value {get;set;}

        public BoardingPass(string input) {
            this.value = input;
        }

        public int Row {
            get {
                // Return value
                int row = 0;

                // Power of 2
                int step = 6;

                foreach(char c in value.Substring(0, 7).ToCharArray()) {
                    // Move to the back half by adding the power of 2 required
                    if (c == 'B')
                        row += (int) Math.Pow(2, step);
                    
                    step--;
                }

                return row;
            }
        }

        public int Column {
            get {
                // Return value
                int col = 0;

                // Power of 2
                int step = 2;

                foreach(char c in value.Substring(7, 3).ToCharArray()) {
                    // Move to the back half by adding the power of 2 required
                    if (c == 'R')
                        col += (int) Math.Pow(2, step);
                    
                    step--;
                }

                return col;
            }
        }

        public int seatId {
            get {
                return (Row * 8) + Column;
            }
        }

        public override string ToString()
        {
            return $"{value}: row {Row}, column {Column}, seat ID {seatId}";
        }
    }

    class Day05 : ASolution
    {
        List<BoardingPass> passes = new List<BoardingPass>();

        public Day05() : base(05, 2020, "")
        {
            Console.WriteLine((new BoardingPass("BFFFBBFRRR")).ToString());
            Console.WriteLine((new BoardingPass("FFFBBBFRRR")).ToString());
            Console.WriteLine((new BoardingPass("BBFFBBFRLL")).ToString());

            foreach(string line in Input.SplitByNewline(true))
                passes.Add(new BoardingPass(line));
        }

        protected override string SolvePartOne()
        {
            return passes.Max(a => a.seatId).ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
