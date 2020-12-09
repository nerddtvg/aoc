using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{

    class Day09 : ASolution
    {
        List<int> puzzle = new List<int>();

        public Day09() : base(09, 2020, "")
        {
            puzzle = Input.ToIntArray("\n").ToList();
        }

        protected override string SolvePartOne()
        {
            for(int i=25; i<puzzle.Count; i++) {
                int sum = puzzle[i];
                bool found = false;

                // Search through the previous 25 numbers
                foreach(int q in Enumerable.Range(i-25, 25)) {
                    foreach(int p in Enumerable.Range(q+1, i-q+1)) {
                        if (puzzle[q] + puzzle[p] == sum) {
                            found = true;
                            break;
                        }
                    }

                    if (found) break;
                }

                if (found == false) {
                    return sum.ToString();
                }
            }

            return null;
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
