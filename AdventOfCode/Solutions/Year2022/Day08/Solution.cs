using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2022
{

    class Day08 : ASolution
    {

        private int[][] trees;

        public Day08() : base(08, 2022, "Treetop Tree House")
        {
            trees = Input.SplitByNewline(true)
                .Select(line => line.Select(c => c - 48).ToArray())
                .ToArray();
        }

        private int[] GetCol(int[][] trees, int x)
        {
            return trees.Select(line => line[x]).ToArray();
        }

        private bool IsVisible(int[][] trees, int x, int y)
        {
            // Determine if the tree at x,y is visible
            // Look left, up, right, and down
            // If all of the trees in that direction are less than our height, it is visible
            // If this is an edge tree, it is visible

            if (x == 0 || x == trees[0].Length - 1)
                return true;

            if (y == 0 || y == trees.Length - 1)
                return true;

            return
                trees[y].Take(x).All(t => t < trees[y][x])
                ||
                trees[y].Skip(x + 1).All(t => t < trees[y][x])
                ||
                GetCol(trees, x).Take(y).All(t => t < trees[y][x])
                ||
                GetCol(trees, x).Skip(y + 1).All(t => t < trees[y][x]);
        }

        protected override string? SolvePartOne()
        {
            int visible = 0;

            for (int y = 0; y < trees.Length; y++)
                for (int x = 0; x < trees[0].Length; x++)
                    if (IsVisible(trees, x, y))
                        visible++;

            return visible.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

