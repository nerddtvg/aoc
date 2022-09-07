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

            return string.Empty;
        }

        protected override string SolvePartTwo()
        {
            // Find a contiguous set of numbers that sum to this
            int sum = 85848519;

            // Go through each one and now we check if we can make a sum
            for(int i=0; i<puzzle.Count; i++) {
                int tSum = 0;
                int min = Int32.MaxValue;
                int max = Int32.MinValue;

                for(int q=i+1; q<puzzle.Count && tSum < sum; q++) {
                    tSum += puzzle[q];

                    if (puzzle[q] < min) min = puzzle[q];
                    if (puzzle[q] > max) max = puzzle[q];
                }
                
                // If it matches, return our value!
                if (tSum == sum) {
                    return (min+max).ToString();
                }
            }

            return string.Empty;
        }
    }
}
