using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using Microsoft.Z3;

namespace AdventOfCode.Solutions.Year2024
{

    class Day13 : ASolution
    {

        public Day13() : base(13, 2024, "Claw Contraption")
        {
            // DebugInput = @"Button A: X+94, Y+34
            // Button B: X+22, Y+67
            // Prize: X=8400, Y=5400

            // Button A: X+26, Y+66
            // Button B: X+67, Y+21
            // Prize: X=12748, Y=12176

            // Button A: X+17, Y+86
            // Button B: X+84, Y+37
            // Prize: X=7870, Y=6450

            // Button A: X+69, Y+23
            // Button B: X+27, Y+71
            // Prize: X=18641, Y=10279";
        }

        public int GetMinimumNumberOfCoins(string[] strings)
        {
            var input = strings.JoinAsString();

            // This will use Z3, which I don't completely get still
            var buttonMatch = new Regex(@"X\+(?<X>[0-9]+), Y\+(?<Y>[0-9]+)").Matches(input);
            var prizeMatch = new Regex(@"X=(?<X>[0-9]+), Y=(?<Y>[0-9]+)").Matches(input);

            var z3Context = new Context();

            var prizeX = z3Context.MkInt(prizeMatch[0].Groups["X"].Value);
            var prizeY = z3Context.MkInt(prizeMatch[0].Groups["Y"].Value);

            var a = z3Context.MkIntConst($"a");
            var b = z3Context.MkIntConst($"b");

            // a*ax + b*bx
            var eqX = z3Context.MkAdd(z3Context.MkMul(a, z3Context.MkInt(buttonMatch[0].Groups["X"].Value)), z3Context.MkAdd(z3Context.MkMul(b, z3Context.MkInt(buttonMatch[1].Groups["X"].Value))));
            var eqY = z3Context.MkAdd(z3Context.MkMul(a, z3Context.MkInt(buttonMatch[0].Groups["Y"].Value)), z3Context.MkAdd(z3Context.MkMul(b, z3Context.MkInt(buttonMatch[1].Groups["Y"].Value))));

            var solver = z3Context.MkOptimize();
            solver.Add(z3Context.MkEq(prizeX, eqX));
            solver.Add(z3Context.MkEq(prizeY, eqY));

            solver.Add(z3Context.MkGe(a, z3Context.MkInt(0)));
            solver.Add(z3Context.MkGe(b, z3Context.MkInt(0)));

            // Setting maximums to save time
            solver.Add(z3Context.MkLe(a, z3Context.MkInt(100000)));
            solver.Add(z3Context.MkLe(b, z3Context.MkInt(100000)));

            // Coin cost: 3a + b
            var eqCoins = z3Context.MkAdd(z3Context.MkMul(a, z3Context.MkInt(3)), b);

            var h1 = solver.MkMinimize(eqCoins);

            // No answer
            if (solver.Check() != Status.SATISFIABLE)
                return 0;

            if (!h1.Value.IsIntNum)
                return 0;

            // return h1.Value
            return ((IntNum)h1.Value).Int;
        }

        protected override string? SolvePartOne()
        {
            // Time: 00:00:01.9734085
            return Input
                .SplitByBlankLine(shouldTrim: true)
                .Sum(GetMinimumNumberOfCoins)
                .ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

