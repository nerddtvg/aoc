using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2018
{

    class Day23 : ASolution
    {
        public class NanoBot
        {
            public Int64 x;
            public Int64 y;
            public Int64 z;
            public UInt64 r;

            public bool IsInRange(NanoBot bot2) =>
                (x, y, z).ManhattanDistance((bot2.x, bot2.y, bot2.z)) <= r;
        }

        public List<NanoBot> bots = new List<NanoBot>();

        public Day23() : base(23, 2018, "Experimental Emergency Teleportation")
        {
//             DebugInput = @"pos=<0,0,0>, r=4
// pos=<1,0,0>, r=1
// pos=<4,0,0>, r=3
// pos=<0,2,0>, r=1
// pos=<0,5,0>, r=3
// pos=<0,0,3>, r=1
// pos=<1,1,1>, r=1
// pos=<1,1,2>, r=1
// pos=<1,3,1>, r=1";

            this.bots.Clear();

            var input = Input.Replace("pos=", "").Replace(" r=", "").Replace("<", "").Replace(">", "").Trim();

            foreach(var line in input.SplitByNewline())
            {
                var s = line.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                this.bots.Add(new NanoBot()
                {
                    x = Int64.Parse(s[0]),
                    y = Int64.Parse(s[1]),
                    z = Int64.Parse(s[2]),
                    r = UInt64.Parse(s[3])
                });
            }
        }

        protected override string? SolvePartOne()
        {
            var largestRadius = this.bots.Max(b => b.r);
            var bot = this.bots.First(b => b.r == largestRadius);

            // Mistake: Don't ignore the source bot itself
            return this.bots.Count(b => bot.IsInRange(b)).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
