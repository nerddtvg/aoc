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
//             DebugInput = @"............
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
        private void AddAntiNode((int x, int y) p) {
            if (!InBounds(p)) return;

            if (!antinodes.ContainsKey(p))
                antinodes[p] = 1;
            else
                antinodes[p]++;
        }
        
        private void FindAntinodes(char antenna) {
            // For each antenna type:
            // 1. Find all pairs of antennas
            // 2. Get distances between points
            // 3. Find the antinodes
            // 4. Add to the list

            foreach (var pair in antennas[antenna].GetAllCombos()) {
                var dx = pair[1].x - pair[0].x;
                var dy = pair[1].y - pair[0].y;

                var p1 = (pair[0].x - dx, pair[0].y - dy);
                var p2 = (pair[1].x + dx, pair[1].y + dy);

                AddAntiNode(p1);
                AddAntiNode(p2);
            }
        }

        protected override string? SolvePartOne()
        {
            // Time: 00:00:00.0092212
            antennas.Keys.ForEach(FindAntinodes);

            return antinodes.Keys.Count.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

