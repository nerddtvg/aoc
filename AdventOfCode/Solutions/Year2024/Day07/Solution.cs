using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Numerics;


namespace AdventOfCode.Solutions.Year2024
{

    class Day07 : ASolution
    {
        private (BigInteger testValue, BigInteger[] numbers)[] lines;

        public Day07() : base(07, 2024, "Bridge Repair")
        {
//             DebugInput = @"190: 10 19
// 3267: 81 40 27
// 83: 17 5
// 156: 15 6
// 7290: 6 8 6 15
// 161011: 16 10 13
// 192: 17 8 14
// 21037: 9 7 18 13
// 292: 11 6 16 20";

            lines = Input.SplitByNewline().Select(line =>
            {
                var split = line.Split(':', StringSplitOptions.TrimEntries);
                return (BigInteger.Parse(split[0]), split[1].Split(' ').Select(s => BigInteger.Parse(s)).ToArray());
            }).ToArray();
        }

        private IEnumerable<BigInteger> GetPossibleSums(BigInteger[] numbers)
        {
            // Left to right means we need to get the two possible values of 0 and 1 now
            var try1 = numbers[0] + numbers[1];
            var try2 = numbers[0] * numbers[1];

            if (numbers.Length == 2)
            {
                // If these are the only two values left, return them
                yield return try1;
                yield return try2;
            }
            else
            {
                // Otherwise, return every possible value including them
                foreach (var num in GetPossibleSums(new[] { try1 }.Concat(numbers[2..]).ToArray()))
                    yield return num;

                foreach (var num in GetPossibleSums(new[] { try2 }.Concat(numbers[2..]).ToArray()))
                    yield return num;
            }
        }

        private bool IsValid(BigInteger testValue, BigInteger[] numbers)
        {
            // Using Any will shortcut the Enumerable
            if (GetPossibleSums(numbers).Any(s => s == testValue))
                return true;

            return false;
        }

        protected override string? SolvePartOne()
        {
            // Time: 00:00:00.1922788
            return lines
                .SumBigInteger(test => IsValid(test.testValue, test.numbers) ? test.testValue : BigInteger.Zero)
                .ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

