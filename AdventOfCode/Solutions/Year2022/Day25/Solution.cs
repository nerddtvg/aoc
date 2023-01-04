using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Diagnostics;

namespace AdventOfCode.Solutions.Year2022
{

    class Day25 : ASolution
    {
        private readonly Dictionary<char, long> values = new()
        {
            { '2', 2 },
            { '1', 1 },
            { '0', 0 },
            { '-', -1 },
            { '=', -2 }
        };

        private readonly Dictionary<long, char> reverseValues;

        public Day25() : base(25, 2022, "Full of Hot Air")
        {
            reverseValues = values.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

            // DebugInput = @"1=-0-2
            //     12111
            //     2=0=
            //     21
            //     2=01
            //     111
            //     20012
            //     112
            //     1=-1=
            //     1-12
            //     12
            //     1=
            //     122";

            var intToSnafuExamples = new Dictionary<int, string>()
            {
                { 1, "1" },
                { 2, "2" },
                { 3, "1=" },
                { 4, "1-" },
                { 5, "10" },
                { 6, "11" },
                { 7, "12" },
                { 8, "2=" },
                { 9, "2-" },
                { 10, "20" },
                { 15, "1=0" },
                { 20, "1-0" },
                { 2022, "1=11-2" },
                { 12345, "1-0---0" },
                { 314159265, "1121-1110-1=0" }
            };

            foreach(var ex in intToSnafuExamples)
            {
                var snafu = LongToSnafu(ex.Key);

                Debug.Assert(Debug.Equals(snafu, ex.Value), $"Input: {ex.Key}\nExpected: {ex.Value}\nActual: {snafu}");
            }
        }

        private long SnafuToLong(string snafu)
        {
            // Converts a SNAFU number to integer
            // base 5 math (1, 5, 25, 125, etc.)
            // using the values dictionary
            int pos = 0;
            long ret = 0;

            foreach(var c in snafu.ToCharArray().Reverse())
            {
                ret += values[c] * (long)Math.Pow(5, pos++);
            }

            return ret;
        }

        private string LongToSnafu(long value)
        {
            // We are going to work from right to left with this
            // A stack would make this easiest
            var output = new Stack<char>();

            int divisor = 5;
            while(value > 0)
            {
                var digit = value % divisor;

                // If the digit is 3 or 4, we have special needs
                if (digit >= 3)
                {
                    // MATH!
                    // First, add another divisor onto value to increase the next digit
                    value += divisor;

                    // To figure out the digit's value, it is digit - divisor:
                    output.Push(reverseValues[digit - divisor]);
                }
                else
                {
                    // Append this value and proceed
                    output.Push(digit.ToString()[0]);
                }

                value /= divisor;
            }

            return output.JoinAsString();
        }

        protected override string? SolvePartOne()
        {
            var longVal = Input.SplitByNewline(true)
                .Select(i => SnafuToLong(i))
                .Sum();

            Console.WriteLine($"Value: {longVal}");

            return LongToSnafu(longVal);
        }

        protected override string? SolvePartTwo()
        {
            return "Last Day";
        }
    }
}

