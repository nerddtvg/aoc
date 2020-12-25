using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

using System.Numerics;

namespace AdventOfCode.Solutions.Year2020
{

    class Day25 : ASolution
    {
        int cardPubKey {get;set;}
        int doorPubKey {get;set;}
        int cardLoopSize {get;set;}
        int doorLoopSize {get;set;}
        int pubKeySubject {get;set;}
        int divisor {get;set;}

        public Day25() : base(25, 2020, "")
        {
            /** /
            DebugInput = @"5764801
                17807724";
            /**/

            // Parsing the input
            var lines = Input.SplitByNewline(true, true);

            // Get the public keys
            this.cardPubKey = Int32.Parse(lines[0]);
            this.doorPubKey = Int32.Parse(lines[1]);

            // Defined in the puzzle
            this.pubKeySubject = 7;
            this.divisor = 20201227;

            // Determine the loop sizes
            this.cardLoopSize = getLoopSize(pubKeySubject, cardPubKey);
            this.doorLoopSize = getLoopSize(pubKeySubject, doorPubKey);

            Console.WriteLine($"Card Loop Size: {cardLoopSize}");
            Console.WriteLine($"Door Loop Size: {doorLoopSize}");
        }

        private int getLoopSize(int subject, int remainder) {
            // Figure out how many times we have to loop through the subject to get the remainder
            var biR = new BigInteger(remainder);
            var biD = new BigInteger(this.divisor);

            // We know that pow(subject, X) % divisor == remainder for some value of X
            // What if we simply loop at (divisor * Y) + remainder == pow(subject, X) for all values of Y
            // Formula: ((divisor * Y) + remainder) / log(subject) = X
            // That might be faster?
            for(UInt64 y=0; y<=UInt64.MaxValue; y++) {
                var largeNumber = (biD * y) + biR;

                // Need to find if this is an integer power or not
                var biExponent = BigInteger.Log(largeNumber) / Math.Log(subject);

                // If this is an integer, it means we found a good loop size
                if (biExponent % 1 == 0)
                    return (int) (biExponent);
            }

            return 0;
        }

        protected override string SolvePartOne()
        {
            return null;
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
