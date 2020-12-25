using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

using System.Numerics;

namespace AdventOfCode.Solutions.Year2020
{

    class Day25 : ASolution
    {
        uint cardPubKey {get;set;}
        uint doorPubKey {get;set;}
        uint cardLoopSize {get;set;}
        uint doorLoopSize {get;set;}
        uint pubKeySubject {get;set;}
        uint divisor {get;set;}
        uint encryptionKey {get;set;}

        public Day25() : base(25, 2020, "")
        {
            /** /
            DebugInput = @"5764801
                17807724";
            /**/

            // Parsing the input
            var lines = Input.SplitByNewline(true, true);

            // Get the public keys
            this.cardPubKey = UInt32.Parse(lines[0]);
            this.doorPubKey = UInt32.Parse(lines[1]);

            // Defined in the puzzle
            this.pubKeySubject = 7;
            this.divisor = 20201227;

            // Determine the loop sizes
            this.cardLoopSize = getLoopSize(pubKeySubject, cardPubKey);
            this.doorLoopSize = getLoopSize(pubKeySubject, doorPubKey);
            Console.WriteLine($"Card Loop Size: {this.cardLoopSize}");
            Console.WriteLine($"Door Loop Size: {this.doorLoopSize}");

            // Calculate the encryption key
            this.encryptionKey = this.getEncryptionKey(this.doorPubKey, this.cardLoopSize);
            Console.WriteLine($"Card Encryption Key: {this.encryptionKey}");
            Console.WriteLine($"Door Encryption Key [verify]: {this.getEncryptionKey(this.cardPubKey, this.doorLoopSize)}");
        }

        private uint getEncryptionKey(uint subject, uint loopSize) =>
            UInt32.Parse(
                BigInteger.ModPow(new BigInteger(subject), new BigInteger(loopSize), new BigInteger(this.divisor)).ToString()
            );

        private uint getLoopSize(uint subject, uint remainder) {
            // Figure out how many times we have to loop through the subject to get the remainder
            var biS = new BigInteger(subject);
            var biR = new BigInteger(remainder);
            var biD = new BigInteger(this.divisor);

            // Big Integer math was the fastest, not sure why the first time didn't work well
            for(BigInteger i=0; ; i += 1)
                if (BigInteger.ModPow(subject, i, biD) == biR)
                    return UInt32.Parse(i.ToString());
        }

        protected override string SolvePartOne()
        {
            return this.encryptionKey.ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
