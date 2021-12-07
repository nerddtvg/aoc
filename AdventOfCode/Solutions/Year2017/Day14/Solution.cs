using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2017
{

    class Day14 : ASolution
    {
        // Hold our row strings
        private List<string> rows = new List<string>();
        private Dictionary<(int x, int y), bool> grid = new Dictionary<(int x, int y), bool>();

        public Day14() : base(14, 2017, "")
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

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
