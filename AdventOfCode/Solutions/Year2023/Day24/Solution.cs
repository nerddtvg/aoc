using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using Microsoft.Z3;

namespace AdventOfCode.Solutions.Year2023
{
    using Stone = (string px, string py, string pz, string vx, string vy, string vz);

    class Day24 : ASolution
    {
        public required Stone[] stones;

        public Day24() : base(24, 2023, "Never Tell Me The Odds")
        {
            // DebugInput = @"19, 13, 30 @ -2,  1, -2
            //                18, 19, 22 @ -1, -1, -2
            //                20, 25, 34 @ -2, -2, -4
            //                12, 31, 28 @ -1, -2, -1
            //                20, 19, 15 @  1, -5, -3";

            var regex = new Regex(@"(?<px>[\-0-9]+), +(?<py>[\-0-9]+), +(?<pz>[\-0-9]+) +@ +(?<vx>[\-0-9]+), +(?<vy>[\-0-9]+), +(?<vz>[\-0-9]+)");
            stones = Input.SplitByNewline(shouldTrim: true)
                .Select(line => regex.Match(line))
                .Select(match => (Stone)(match.Groups["px"].Value, match.Groups["py"].Value, match.Groups["pz"].Value, match.Groups["vx"].Value, match.Groups["vy"].Value, match.Groups["vz"].Value))
                .ToArray();
        }

        protected override string? SolvePartOne()
        {
            // Using Z3, the magical solver that I don't understand
            // For each hail stone equation, check if it matches any of the other equations
            // Don't ask me to explain this, I wish I understood Z3 better
            // This is modeled after the solution from 2018 Day 23
            int intersect = 0;
            var z3Context = new Context();

            // X and Y must be within this range
            var minXY = z3Context.MkReal("200000000000000");
            var maxXY = z3Context.MkReal("400000000000000");

            var zero = z3Context.MkReal(0);

            // Prepare our equations
            var t = stones.Select((stone, idx) => z3Context.MkRealConst($"t{idx}")).ToList();
            var eqX = stones.Select((stone, idx) => z3Context.MkAdd(z3Context.MkReal(stone.px), z3Context.MkMul(t[idx], z3Context.MkReal(stone.vx)))).ToList();
            var eqY = stones.Select((stone, idx) => z3Context.MkAdd(z3Context.MkReal(stone.py), z3Context.MkMul(t[idx], z3Context.MkReal(stone.vy)))).ToList();

            // Get a hailstone to start with
            for (int i=0; i<stones.Length-1; i++)
            {
                // Now for each other hailstone line (we haven't tested yet), see if we equal X and Y
                for(int q=i+1; q<stones.Length; q++)
                {
                    // Make them equal
                    // This checks to see if there is any point they are acceptable
                    var solver = z3Context.MkOptimize();
                    solver.Add(z3Context.MkEq(eqX[i], eqX[q]));
                    solver.Add(z3Context.MkEq(eqY[i], eqY[q]));

                    // Our range with positive time
                    solver.Add(z3Context.MkGe(t[i], zero));
                    solver.Add(z3Context.MkGe(t[q], zero));

                    solver.Add(z3Context.MkGe(eqX[i], minXY));
                    solver.Add(z3Context.MkLe(eqX[i], maxXY));

                    solver.Add(z3Context.MkGe(eqY[i], minXY));
                    solver.Add(z3Context.MkLe(eqY[i], maxXY));

                    if (solver.Check() == Status.SATISFIABLE)
                        intersect++;
                }
            }

            return intersect.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

