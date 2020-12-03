using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;
using System.Numerics;

namespace AdventOfCode.Solutions.Year2020
{

    class Day03 : ASolution
    {

        public Day03() : base(03, 2020, "")
        {

        }

        protected override string SolvePartOne()
        {
            return RunIntoTrees(
                new List<(int x, int y)>() {
                    (3, 1)
                })
                .First()
                .ToString();
        }

        protected override string SolvePartTwo()
        {

            return RunIntoTrees(
                new List<(int x, int y)>() {
                    (1, 1),
                    (3, 1),
                    (5, 1),
                    (7, 1),
                    (1, 2)
                })
                .Aggregate((x, y) => x * y)
                .ToString();
        }

        private List<BigInteger> RunIntoTrees(List<(int x, int y)> slopes) {
            List<BigInteger> trees = new List<BigInteger>();
            int[] x = new int[slopes.Count];

            // Set the lists up
            for(int i=0; i<slopes.Count; i++) {
                trees.Add(0);
                x[i] = 0;
            }

            // Line counter
            int lc = 0;

            foreach(string line in Input.SplitByNewline()) {
                for(int i=0; i<slopes.Count; i++) {
                    // Do we check this line?
                    if (lc % slopes[i].y != 0) continue;

                    // Reset x if we are past the length
                    if (x[i] >= line.Length) x[i] -= line.Length;

                    // Is this a tree?
                    if (line.Substring(x[i], 1) == "#") trees[i]++;

                    // Increment
                    x[i] += slopes[i].x;
                }

                lc++;
            }

            return trees;
        }
    }
}
