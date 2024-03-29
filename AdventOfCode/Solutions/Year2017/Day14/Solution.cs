using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2017
{

    class Day14 : ASolution
    {
        // Hold our row strings
        private List<string> rows = new List<string>();
        private Dictionary<(int x, int y), bool> grid = new Dictionary<(int x, int y), bool>();
        private HashSet<(int x, int y)> found = new HashSet<(int x, int y)>();

        public Day14() : base(14, 2017, "Disk Defragmentation")
        {
            // DebugInput = "flqrgnkx";

            this.rows.Clear();
            this.grid.Clear();

            // Get the hashes
            for (int i = 0; i <= 127; i++)
            {
                rows.Add(Day10.GetHash($"{Input}-{i}"));
            }

            for (int y = 0; y <= 127; y++)
            {
                int x = 0;

                // Parse each of these characters as a hex value
                foreach(var digit in this.rows[y]
                    .Select(ch => 
                        // Convert each character to digit from base 16 (hex), then to binary string (0000)
                        Convert.ToString(Convert.ToInt32(ch.ToString(), 16), 2)
                        .PadLeft(4, '0')
                    )
                    .JoinAsString())
                {
                    // Set the grid
                    if (digit == '0')
                        this.grid[(x, y)] = false;
                    else
                        this.grid[(x, y)] = true;

                    x++;
                }
            }
        }

        protected override string? SolvePartOne()
        {
            return this.grid.Count(pt => pt.Value).ToString();
        }

        private void FindAdjacent((int x, int y) pos)
        {
            // Only search vertical and horizontal
            if (this.found.Contains(pos))
                return;

            // Doesn't exist
            if (!this.grid.ContainsKey(pos))
                return;

            // Add this to our list
            this.found.Add(pos);

            // If this isn't in use, then we don't actually find adjacencies
            // But we use the above state to make things faster
            if (!this.grid[pos])
                return;

            // Look for adjacent items
            FindAdjacent((pos.x - 1, pos.y));
            FindAdjacent((pos.x + 1, pos.y));
            FindAdjacent((pos.x, pos.y - 1));
            FindAdjacent((pos.x, pos.y + 1));
        }

        protected override string? SolvePartTwo()
        {
            // Count adjacent grid points that are in use
            this.found.Clear();

            var groupCount = 0;

            // Work through every cell that is "in use"
            foreach(var key in this.grid.Where(pt => pt.Value).Select(pt => pt.Key))
            {
                if (!this.found.Contains(key))
                {
                    // Found a group!
                    groupCount++;

                    // Find everything attached
                    FindAdjacent(key);
                }
            }

            return groupCount.ToString();
        }
    }
}

