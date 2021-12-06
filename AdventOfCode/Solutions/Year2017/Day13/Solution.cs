using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2017
{

    class Day13 : ASolution
    {
        private Dictionary<int, int> layers = new Dictionary<int, int>();
        private int pos = 0;

        public Day13() : base(13, 2017, "Packet Scanners")
        {
            Reset();
        }

        private void Reset()
        {
            this.layers.Clear();
            this.pos = 0;

            foreach(var line in Input.SplitByNewline())
            {
                var parts = line.Split(':', 2, StringSplitOptions.TrimEntries);
                this.layers[Int32.Parse(parts[0])] = Int32.Parse(parts[1]);
            }
        }

        protected override string? SolvePartOne()
        {
            // Determine if we are "caught" in each later
            // All layers start at zero, so basically if the below is true, we're caught
            // Scanner takes (depth-1) to reach the end, then back again, so i % (2*(depth-1)) == 0
            // Since i is actually the layer key (layer number), we can simplify it:
            var total = this.layers.Sum(layer => layer.Key % (2 * (layer.Value - 1)) == 0 ? layer.Key * layer.Value : 0);

            return total.ToString();
        }

        protected override string? SolvePartTwo()
        {
            // This is a lowest common multiple problem (skip 0 because it will break us)
            // After solving: I don't think LCM was really needed for this, the number is so large
            var lcm = Utilities.FindLCM(this.layers.Keys.Skip(1).Select(key => (double) key).ToArray());

            // Now go through each step and figure out if we were "caught"
            for (int i = 1; i <= lcm; i++)
            {
                var caught = this.layers.Any(layer => (i + layer.Key) % (2 * (layer.Value - 1)) == 0);

                // Return the first time we're not caught
                if (!caught)
                    return i.ToString();
            }

            return "Not Found";
        }
    }
}

#nullable restore
