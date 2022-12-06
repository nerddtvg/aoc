using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

using Microsoft.Z3;


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

            public bool IsInRange((Int64 x, Int64 y, Int64 z) pt) =>
                (x, y, z).ManhattanDistance(pt) <= r;
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

            foreach (var line in input.SplitByNewline())
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
            // I'm not familiar with any of this math or theorems
            // I will be using a Z3 based solution from:
            // https://www.reddit.com/r/adventofcode/comments/a8s17l/comment/ecdbux2/

            // Helper because Z3 .NET doesn't expose an easy Abs function for us
            ArithExpr zabs(ArithExpr x) =>
                (ArithExpr)x.Context.MkITE(x.Context.MkGe(x, x.Context.MkInt(0)), x, x.Context.MkMul(x, x.Context.MkInt(-1)));

            // Create the z3Context we use to create objects
            var z3Context = new Microsoft.Z3.Context();

            // Define the integer variables that will be used during processing
            (var x, var y, var z) = (z3Context.MkIntConst("x"), z3Context.MkIntConst("y"), z3Context.MkIntConst("z"));

            // An array of booleans (1 or 0) that will determine if a specific bot is in range
            var in_ranges = Enumerable.Range(0, this.bots.Count).Select(c => z3Context.MkIntConst($"in_range_{c}")).ToArray();

            // A total summation of bots in range
            var range_count = z3Context.MkIntConst("sum");

            // The optimizer / solver instance
            var optimize = z3Context.MkOptimize();

            // For each bot...
            for (int i = 0; i < this.bots.Count; i++)
            {
                // Determine if the bot is in range of the new x, y, z
                // If so, set in_ranges[i] to 1
                optimize.Add(
                    z3Context.MkEq(
                        in_ranges[i],
                        z3Context.MkITE(
                            z3Context.MkLe(
                                z3Context.MkAdd(
                                    zabs(z3Context.MkSub(x, z3Context.MkInt(this.bots[i].x))),
                                    zabs(z3Context.MkSub(y, z3Context.MkInt(this.bots[i].y))),
                                    zabs(z3Context.MkSub(z, z3Context.MkInt(this.bots[i].z)))
                                ),
                                z3Context.MkReal(this.bots[i].r)
                            ),
                            z3Context.MkInt(1),
                            z3Context.MkInt(0)
                        )
                    )
                );
            }

            // range_count is the total of in_ranges[1] (1 if in range, 0 otherwise)
            // So this tells us the maximum number of bots in range
            optimize.Add(z3Context.MkEq(range_count, z3Context.MkAdd(in_ranges)));

            // Determine our distance from origin (0, 0, 0)
            var distance_from_zero = z3Context.MkIntConst("dist");
            optimize.Add(z3Context.MkEq(distance_from_zero, z3Context.MkAdd(zabs(x), zabs(y), zabs(z))));

            // We want the highest number of bots in range ...
            var h1 = optimize.MkMaximize(range_count);

            // ... with the lowest distance
            var h2 = optimize.MkMinimize(distance_from_zero);

            if (optimize.Check() != Status.SATISFIABLE)
                throw new Exception();

            // Debug output:
            var xVal = optimize.Model.Consts.First(c => c.Key.Name is StringSymbol s && s.String == "x").Value;
            var yVal = optimize.Model.Consts.First(c => c.Key.Name is StringSymbol s && s.String == "y").Value;
            var zVal = optimize.Model.Consts.First(c => c.Key.Name is StringSymbol s && s.String == "z").Value;
            var dist = h2.Lower;
            Console.WriteLine($"Position: ({xVal},{yVal},{zVal})");
            Console.WriteLine($"Distance: {dist}");
            Console.WriteLine($"In Range: {h1.Upper}");

            return dist.ToString();
        }
    }
}
