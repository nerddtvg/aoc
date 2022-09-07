using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

using System.Numerics;


namespace AdventOfCode.Solutions.Year2017
{

    class Day15 : ASolution
    {
        private BigInteger genAFactor = new BigInteger(16807);
        private BigInteger genBFactor = new BigInteger(48271);
        private BigInteger divisor = new BigInteger(2147483647);

        private BigInteger genAStart = BigInteger.Zero;
        private BigInteger genBStart = BigInteger.Zero;

        // Comparison area (lowest 16 bits)
        private BigInteger comparison = new BigInteger(65535);

        public Day15() : base(15, 2017, "Dueling Generators")
        {
            // DebugInput = "65\n8921";
        }

        private void Reset()
        {
            // Starting values are in the input as the last string
            var lines = Input.SplitByNewline();
            genAStart = new BigInteger(Int32.Parse(lines[0].Split(" ").Last()));
            genBStart = new BigInteger(Int32.Parse(lines[1].Split(" ").Last()));
        }

        // Fixing this bitwise comparison brought the time down considerably
        private bool CompareLowerBits(BigInteger a, BigInteger b) =>
            BigInteger.Equals((a & comparison), (b & comparison));

        private BigInteger GenerateValue(BigInteger start, BigInteger factor, BigInteger divisor, int part = 1, int multiples = 0)
        {
            var val = start;

            do
            {
                val = BigInteger.Remainder(BigInteger.Multiply(val, factor), divisor);

                if (part == 1)
                    return val;

                // If this is part 2, we check the multiples
                if (BigInteger.Remainder(val, multiples) == BigInteger.Zero)
                    return val;
            } while (true);
        }

        protected override string? SolvePartOne()
        {
            Reset();

            var count = 0;

            // I know this is going to be a "reduce the math" simplification problem
            // But I'm not good at those, so let's brute for this for no good reason
            // This took about 8 seconds to complete
            for (uint i = 0; i < 40000000; i++)
            {
                genAStart = GenerateValue(genAStart, genAFactor, divisor);
                genBStart = GenerateValue(genBStart, genBFactor, divisor);

                if (CompareLowerBits(genAStart, genBStart))
                    count++;
            }

            return count.ToString();
        }

        protected override string? SolvePartTwo()
        {
            Reset();
            
            var count = 0;

            // This took about 6 seconds to complete
            for (uint i = 0; i < 5000000; i++)
            {
                genAStart = GenerateValue(genAStart, genAFactor, divisor, 2, 4);
                genBStart = GenerateValue(genBStart, genBFactor, divisor, 2, 8);

                if (CompareLowerBits(genAStart, genBStart))
                    count++;
            }

            return count.ToString();
        }
    }
}

