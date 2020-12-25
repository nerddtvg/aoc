using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

using System.Numerics;

namespace AdventOfCode.Solutions.Year2020
{

    class Day25 : ASolution
    {

        public Day25() : base(25, 2020, "")
        {
            DebugInput = @"5764801
                17807724";

            var lines = Input.SplitByNewline(true, true);
            int cardPubKey = Int32.Parse(lines[0]);
            int doorPubKey = Int32.Parse(lines[1]);

            int pubKeySubject = 7;

            var cardLoopSize = getLoopSize(pubKeySubject, cardPubKey);
            var doorLoopSize = getLoopSize(pubKeySubject, doorPubKey);

            Console.WriteLine(cardLoopSize);
            Console.WriteLine(doorLoopSize);
        }

        private int getLoopSize(int subject, int remainder) {
            // Figure out how many times we have to loop through the subject to get the remainder
            int i = 0;
            var biS = new BigInteger(subject);
            var biR = new BigInteger(remainder);
            var biD = new BigInteger(20201227);

            for(i=1; i<50; i++) {
                biS *= subject;
                BigInteger r = biS % biD;

                // Did we find it?
                if (r == biR) break;
            }

            return i+1;
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
