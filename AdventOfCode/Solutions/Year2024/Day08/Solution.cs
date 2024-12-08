using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    class Day08 : ASolution
    {
        private Dictionary<char, List<(int x, int y)>> antennas = [];
        private Dictionary<(int x, int y), int> antinodes = [];

        private char[][] grid;

        int maxY = 0;
        int maxX = 0;

        public Day08() : base(08, 2024, "Resonant Collinearity")
        {
            // DebugInput = @"............
            // ........0...
            // .....0......
            // .......0....
            // ....0.......
            // ......A.....
            // ............
            // ............
            // ........A...
            // .........A..
            // ............
            // ............";

            grid = Input.ToCharGrid();
            grid.ForEach((line, y) => line.ForEach((c, x) =>
            {
                if (c == '.') return;

                if (!antennas.ContainsKey(c))
                    antennas[c] = [];

                antennas[c].Add((x, y));
            }));

            maxY = grid.Length - 1;
            maxX = grid[0].Length - 1;
        }

        private bool InBounds((int x, int y) p) => 0 <= p.x && 0 <= p.y && p.x <= maxX && p.y <= maxY;
        private bool AddAntiNode((int x, int y) p)
        {
            if (!InBounds(p)) return false;

            if (!antinodes.ContainsKey(p))
                antinodes[p] = 1;
            else
                antinodes[p]++;

            return true;
        }

        private void FindAntinodes(char antenna, bool part2 = false)
        {
            // For each antenna type:
            // 1. Find all pairs of antennas
            // 2. Get distances between points
            // 3. Find the antinodes
            // 4. Add to the list

            foreach (var pair in antennas[antenna].GetAllCombos())
            {
                var dx = pair[1].x - pair[0].x;
                var dy = pair[1].y - pair[0].y;

                // Track if we added any, if not we're out of bounds in all directions
                var added = true;

                // Part 1 is only d=1
                // Part 2 the distances are 0 ... N where N is when no new antinodes are added
                for (int d = part2 ? 0 : 1; d == 1 || (part2 && added); d++)
                {
                    var p1 = (pair[0].x - (dx * d), pair[0].y - (dy * d));
                    var p2 = (pair[1].x + (dx * d), pair[1].y + (dy * d));

                    added = AddAntiNode(p1);
                    added = AddAntiNode(p2) || added;
                }
            }
        }

        private void PrintGrid()
        {
            grid.ForEach((line, y) =>
            {
                line.ForEach((c, x) =>
            {
                if (antinodes.ContainsKey((x, y)))
                    Console.Write('#');
                else
                    Console.Write(c);
            });

                Console.WriteLine();
            });
        }

        protected override string? SolvePartOne()
        {
            // Time: 00:00:00.0092212
            antennas.Keys.ForEach(antenna => FindAntinodes(antenna, false));

            return antinodes.Keys.Count.ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Time: 00:00:00.0008538
            antinodes = [];

            antennas.Keys.ForEach(antenna => FindAntinodes(antenna, true));

            // PrintGrid();

            return antinodes.Keys.Count.ToString();
        }
    }
}

