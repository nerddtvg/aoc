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

        private (bool on, int sx, int ex, int sy, int ey, int sz, int ez) ActualParseLine(string line)
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

            return (matches.Groups[1].Value == "on", sx, ex, sy, ey, sz, ez);
        }

        private (bool success, bool on, int sx, int ex, int sy, int ey, int sz, int ez) ParseLine(string line)
        {
            (bool on, int sx, int ex, int sy, int ey, int sz, int ez) = ActualParseLine(line);

            // If we're outside the bounds, ignore this
            if (sx > 50 || ex < -50 || sy > 50 || ey < -50 || sz > 50 || ez < -50)
            {
                return (false, false, 0, 0, 0, 0, 0, 0);
            }

            // Bounded by -50 to 50 in all directions
            return (true, on, Math.Max(-50, sx), Math.Min(50, ex), Math.Max(-50, sy), Math.Min(50, ey), Math.Max(-50, sz), Math.Min(50, ez));
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
            // For part 2, we're going to load up a list of these bounds
            // And simply traverse them backwards to find something that matches
            // each possible point
            var bounds = Input.SplitByNewline().Select(line => ActualParseLine(line)).Reverse().ToArray();

            // Find our max bounds
            var sx = bounds.Min(b => b.sx);
            var ex = bounds.Max(b => b.ex);
            var sy = bounds.Min(b => b.sy);
            var ey = bounds.Max(b => b.ey);
            var sz = bounds.Min(b => b.sz);
            var ez = bounds.Max(b => b.ez);

            ulong count = 0;

            for (int x = sx; x <= ex; x++)
                for (int y = sy; y <= ey; y++)
                    for (int z = sz; z <= ez; z++)
                    {
                        var b = bounds.FirstOrDefault(b => b.sx <= x && x <= b.ex && b.sy <= y && y <= b.ey && b.sz <= z && z <= b.ez);

                        // Default will be false (null bool)
                        if (b.on)
                            count++;
                    }

            return count.ToString();
        }
    }
}

#nullable restore
