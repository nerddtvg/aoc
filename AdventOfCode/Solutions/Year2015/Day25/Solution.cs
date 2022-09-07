using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

using System.Numerics;

namespace AdventOfCode.Solutions.Year2015
{

    class Day25 : ASolution
    {
        // This day is about big integer math and exponents
        public BigInteger row { get; set; }
        public BigInteger col { get; set; }

        public Day25() : base(25, 2015, "")
        {
            var matches = new Regex("To continue, please consult the code grid in the manual.  Enter the code at row ([0-9]+), column ([0-9]+).").Match(Input);

            if (!matches.Success)
                throw new Exception("Bad regex");

            this.row = new BigInteger(Int32.Parse(matches.Groups[1].Value));
            this.col = new BigInteger(Int32.Parse(matches.Groups[2].Value));
        }

        protected override string SolvePartOne()
        {
            // So, to find the second code (which ends up in row 2, column 1),
            // start with the previous value, 20151125. Multiply it by 252533
            // to get 5088824049625. Then, divide that by 33554393, which
            // leaves a remainder of 31916031. That remainder is the second code.

            // From webpage
            var firstCode = new BigInteger(20151125);

            var @base = new BigInteger(252533);
            var modulus = new BigInteger(33554393);
            var exp = ((row + col - 2) * (row + col - 1) / 2) + col - 1;

            return ((BigInteger.ModPow(@base, exp, modulus) * firstCode) % modulus).ToString();
        }

        protected override string SolvePartTwo()
        {
            return string.Empty;
        }
    }
}
