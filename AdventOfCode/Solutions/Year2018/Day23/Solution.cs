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

            ArithExpr zabs(Context context, ArithExpr x) =>
                (ArithExpr)context.MkITE(context.MkGe(x, context.MkInt(0)), x, context.MkMul(x, context.MkInt(-1)));

            var z3Context = new Microsoft.Z3.Context();
            (var x, var y, var z) = (z3Context.MkIntConst("x"), z3Context.MkIntConst("y"), z3Context.MkIntConst("z"));
            var in_ranges = Enumerable.Range(0, this.bots.Count).Select(c => z3Context.MkIntConst($"in_range_{c}")).ToArray();
            var range_count = z3Context.MkIntConst("sum");

            var optimize = z3Context.MkOptimize();

            for (int i = 0; i < this.bots.Count; i++)
            {
                optimize.Add(
                    z3Context.MkEq(
                        in_ranges[i],
                        z3Context.MkITE(
                            z3Context.MkLe(
                                z3Context.MkAdd(
                                    zabs(z3Context, z3Context.MkSub(x, z3Context.MkInt(this.bots[i].x))),
                                    zabs(z3Context, z3Context.MkSub(y, z3Context.MkInt(this.bots[i].y))),
                                    zabs(z3Context, z3Context.MkSub(z, z3Context.MkInt(this.bots[i].z)))
                                ),
                                z3Context.MkReal(this.bots[i].r)
                            ),
                            z3Context.MkInt(1),
                            z3Context.MkInt(0)
                        )
                    )
                );
            }

            // Add all of the in_ranges to see if the 1s add up to range_count
            optimize.Add(z3Context.MkEq(range_count, z3Context.MkAdd(in_ranges)));

            var distance_from_zero = z3Context.MkIntConst("dist");
            optimize.Add(z3Context.MkEq(distance_from_zero, z3Context.MkAdd(zabs(z3Context, x), zabs(z3Context, y), zabs(z3Context, z))));

            var h1 = optimize.MkMaximize(range_count);
            var h2 = optimize.MkMinimize(distance_from_zero);

            if (optimize.Check() != Status.SATISFIABLE)
                throw new Exception();

            return h2.Lower.ToString();
        }
    }
}

