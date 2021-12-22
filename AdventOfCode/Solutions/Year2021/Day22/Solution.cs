using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2021
{

    class Day22 : ASolution
    {
        private Dictionary<(int x, int y, int z), bool> grid = new Dictionary<(int x, int y, int z), bool>();

        public Day22() : base(22, 2021, "Reactor Reboot")
        {
            // I'll regret this later, but part 1 only cares about -50..50 in each direction
            this.grid.Clear();

            for (int x = -50; x <= 50; x++)
                for (int y = -50; y <= 50; y++)
                    for (int z = -50; z <= 50; z++)
                        this.grid[(x, y, z)] = false;
        }

        private (bool success, bool on, int sx, int ex, int sy, int ey, int sz, int ez) ParseLine(string line)
        {
            var matches = Regex.Match(line, @"(on|off) x=([\-0-9]+)\.\.([\-0-9]+),y=([\-0-9]+)\.\.([\-0-9]+),z=([\-0-9]+)\.\.([\-0-9]+)");

            if (!matches.Success)
                throw new Exception();

            var sx = Int32.Parse(matches.Groups[2].Value);
            var ex = Int32.Parse(matches.Groups[3].Value);
            var sy = Int32.Parse(matches.Groups[4].Value);
            var ey = Int32.Parse(matches.Groups[5].Value);
            var sz = Int32.Parse(matches.Groups[6].Value);
            var ez = Int32.Parse(matches.Groups[7].Value);

            // If we're outside the bounds, ignore this
            if (sx > 50 || ex < -50 || sy > 50 || ey < -50 || sz > 50 || ez < -50)
            {
                return (false, false, 0, 0, 0, 0, 0, 0);
            }

            // Bounded by -50 to 50 in all directions
            return (true, matches.Groups[1].Value == "on", Math.Max(-50, sx), Math.Min(50, ex), Math.Max(-50, sy), Math.Min(50, ey), Math.Max(-50, sz), Math.Min(50, ez));
        }

        protected override string? SolvePartOne()
        {
            foreach(var line in Input.SplitByNewline())
            {
                var bounds = ParseLine(line);

                if (!bounds.success) continue;

                for (int x = bounds.sx; x <= bounds.ex; x++)
                    for (int y = bounds.sy; y <= bounds.ey; y++)
                        for (int z = bounds.sz; z <= bounds.ez; z++)
                            this.grid[(x, y, z)] = bounds.on;
            }

            return this.grid.Count(kvp => kvp.Value).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
