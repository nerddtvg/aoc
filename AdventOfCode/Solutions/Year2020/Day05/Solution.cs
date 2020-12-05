using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{
    class BoardingPass {
        public string value {get;set;}

        public int row {get;set;}
        public int column {get;set;}
        public int seatId {get;set;}

        public BoardingPass(string input) {
            this.value = input;

            // Calculate only once
            this.row = _Row;
            this.column = _Column;
            this.seatId = _seatId;
        }

        private int _Row {
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

        private int _Column {
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

        private int _seatId {
            get {
                return (row * 8) + column;
            }
        }

        public override string ToString()
        {
            return $"{value}: row {row}, column {column}, seat ID {seatId}";
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
            // Find our seat
            // We have to find empty seats that don't exist
            // And find which have +/-1 seatIds with neighborss
            for(int r=0; r<128; r++) {
                for(int c=0; c<8; c++) {
                    // Generate this seat Id
                    int seatId = (r*8) + c;

                    if (
                        passes.Count(a => a.seatId == seatId) == 0
                        &&
                        passes.Count(a => a.seatId == seatId-1) == 1
                        &&
                        passes.Count(a => a.seatId == seatId+1) == 1
                    ) return seatId.ToString();
                }
            }

            return null;
        }
    }
}
