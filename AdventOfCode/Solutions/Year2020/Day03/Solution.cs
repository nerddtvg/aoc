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
            int x = 0;
            int trees = 0;

            foreach(string line in Input.SplitByNewline()) {
                // Reset x if we are past the length
                if (x >= line.Length) x -= line.Length;

                // Is this a tree?
                if (line.Substring(x, 1) == "#") trees++;

                // Increment
                x += 3;
            }

            return trees.ToString();
        }

        protected override string SolvePartTwo()
        {
            int[] x = new int[] { 0, 0, 0, 0, 0 };
            int[] mx = new int[] { 1, 3, 5, 7, 1 };
            int[] my = new int[] { 1, 1, 1, 1, 2 };

            BigInteger[] trees = new BigInteger[] { 0, 0, 0, 0, 0 };

            int lc = 0;

            foreach(string line in Input.SplitByNewline()) {
                for(int i=0; i<x.Length; i++) {
                    // Do we check this line?
                    if (lc % my[i] != 0) continue;

                    // Reset x if we are past the length
                    if (x[i] >= line.Length) x[i] -= line.Length;

                    // Is this a tree?
                    if (line.Substring(x[i], 1) == "#") trees[i]++;

                    // Increment
                    x[i] += mx[i];
                }

                lc++;
            }

            return trees.Aggregate((x, y) => x * y).ToString();
        }
    }
}
