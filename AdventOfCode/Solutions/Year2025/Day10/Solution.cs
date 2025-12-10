using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

using Microsoft.Z3;

namespace AdventOfCode.Solutions.Year2025
{

    class Day10 : ASolution
    {
        private struct FactoryLine
        {
            /// <summary>
            /// Track what lights should be on or off
            /// </summary>
            public int[] lights;

            /// <summary>
            /// Track which lights each button toggles
            /// Each array will be a zero if the button does not toggle the light, one if it does
            /// The index of the array will line up with the lights
            /// </summary>
            public int[][] buttons;

            /// <summary>
            /// Part 2: Future
            /// </summary>
            public int[] joltages;
        }

        private readonly FactoryLine[] lines;

        public Day10() : base(10, 2025, "Factory")
        {
            var factoryRegex = new Regex(@"\[(?<light>[\.#])+\] (?<buttons>\((?:[0-9]+,*)+\) *)+ \{(?<joltages>[0-9,]+)\}");

            // DebugInput = @"
            //     [.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
            //     [...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}
            //     [.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}";

            lines = [..Input.SplitByNewline(true, true)
                .Select(line =>
                {
                    var parts = factoryRegex.Match(line);

                    return new FactoryLine()
                    {
                        lights = [..parts.Groups["light"].Captures.Select(c => c.Value == "#" ? 1 : 0)],
                        buttons = [..parts.Groups["buttons"].Captures.Select(c => {
                            var btns = c.Value.Trim().Replace("(", "").Replace(")", "").ToIntArray(",");

                            return Enumerable
                                .Range(0, parts.Groups["light"].Captures.Count)
                                .Select(idx => btns.Contains(idx) ? 1 : 0)
                                .ToArray();
                        })],
                        joltages = parts.Groups["joltages"].Captures[0].Value.ToIntArray(",")
                    };
                })];
        }

        protected override string? SolvePartOne()
        {
            // A set of mathematical formulas that must come together to find the fewest
            // possible button pushes? This sounds like a Z3 solver situation
            // Reference back to 2018 Day 23 for a similar setup

            ulong minButtons = 0;

            foreach (var line in lines)
            {
                // For each of these, we establish a Z3 solver that solves for the minimum number of pushes required to satisfy the given light constraints
                var z3Context = new Context();

                // Define a variable for each "button" and the total of all buttons together
                var buttonVars = line.buttons.Select((btn, idx) => z3Context.MkIntConst($"button{idx}")).ToArray();
                var buttonTotal = z3Context.MkIntConst("buttonTotal");

                // The optimizer / solver instance
                var optimize = z3Context.MkOptimize();

                // Calculate each of the outcomes for the lights
                line.lights.ForEach((light, lightIdx) =>
                {
                    optimize.Add(
                        z3Context.MkEq(
                            z3Context.MkInt(light),
                            z3Context.MkMod(
                                (IntExpr)z3Context.MkAdd(
                                    // Multiple the buttonVar (number of times pushed) against the button value (0 or 1 for toggling light)
                                    buttonVars.Select((btn, btnIdx) => btn * z3Context.MkInt(line.buttons[btnIdx][lightIdx]))
                                ),
                                // Light is on (1) or off (0), modulus 2
                                z3Context.MkInt(2)
                            )
                        )
                    );
                });

                // Ensure we add all the button pushes together
                optimize.Add(
                    z3Context.MkEq(buttonTotal, z3Context.MkAdd(buttonVars))
                );

                // Ensure each button cannot be negative
                buttonVars.ForEach(btn => optimize.Add(z3Context.MkGe(btn, z3Context.MkInt(0))));

                // Ensure we have a value
                optimize.Add(z3Context.MkGt(buttonTotal, z3Context.MkInt(0)));

                // ... with the lowest total button pushes
                var ans = optimize.MkMinimize(buttonTotal);

                if (optimize.Check() != Status.SATISFIABLE)
                    throw new Exception();

                minButtons += ulong.Parse(ans.Lower.ToString());
            }

            // Time  : 00:00:01.8464781
            return minButtons.ToString();
        }

        protected override string? SolvePartTwo()
        {

            ulong minButtons = 0;

            foreach (var line in lines)
            {
                // For each of these, we establish a Z3 solver that solves for the minimum number of pushes required to satisfy the given joltage constraints
                var z3Context = new Context();

                // Define a variable for each "button" and the total of all buttons together
                var buttonVars = line.buttons.Select((btn, idx) => z3Context.MkIntConst($"button{idx}")).ToArray();
                var buttonTotal = z3Context.MkIntConst("buttonTotal");

                // The optimizer / solver instance
                var optimize = z3Context.MkOptimize();

                // Calculate each of the outcomes for the lights
                line.joltages.ForEach((joltage, joltageIdx) =>
                {
                    optimize.Add(
                        z3Context.MkEq(
                            z3Context.MkInt(joltage),
                            z3Context.MkAdd(
                                // Multiple the buttonVar (number of times pushed) against the button value (0 or 1 for toggling light)
                                buttonVars.Select((btn, btnIdx) => btn * z3Context.MkInt(line.buttons[btnIdx][joltageIdx]))
                            )
                        )
                    );
                });

                // Ensure we add all the button pushes together
                optimize.Add(
                    z3Context.MkEq(buttonTotal, z3Context.MkAdd(buttonVars))
                );

                // Ensure each button cannot be negative
                buttonVars.ForEach(btn => optimize.Add(z3Context.MkGe(btn, z3Context.MkInt(0))));

                // Ensure we have a value
                optimize.Add(z3Context.MkGt(buttonTotal, z3Context.MkInt(0)));

                // ... with the lowest total button pushes
                var ans = optimize.MkMinimize(buttonTotal);

                if (optimize.Check() != Status.SATISFIABLE)
                    throw new Exception();

                minButtons += ulong.Parse(ans.Lower.ToString());
            }

            // Time  : 00:00:02.6253885
            return minButtons.ToString();
        }
    }
}
