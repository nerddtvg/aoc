using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

using System.Numerics;

namespace AdventOfCode.Solutions.Year2019
{

    class Day22 : ASolution
    {
        private List<int> deck = new();
        BigInteger count = new BigInteger(0);

        public Day22() : base(22, 2019, "")
        {

        }

        private void resetDeck(int count=10007) {
            this.deck = new List<int>();

            for(int i=0; i<count; i++)
                this.deck.Add(i);
        }

        private void runOperation(string operation) {
            var parts = operation.Split(" ");

            if (operation == "deal into new stack") {
                this.deck.Reverse();
            } else if (parts[0] == "deal" && parts[1] == "with") {
                int index = Int32.Parse(parts[3]);
                int divisor = this.deck.Count / index;

                // Dealing out is fun
                var newList = new Dictionary<int, int>();

                // for each old position, we need to know the new index
                // it will be (old_pos * index) % this.deck.count
                for(int i=0; i<this.deck.Count; i++)
                    newList.Add((i * index) % this.deck.Count, this.deck[i]);
                
                // Save the new list
                this.deck = newList.OrderBy(a => a.Key).Select(a => a.Value).ToList();
            } else if (parts[0] == "cut") {
                int index = Int32.Parse(parts[1]);
                int start = 0;

                // Start from the back
                if (index < 0)
                    start = this.deck.Count + index;
                
                var removed = this.deck.GetRange(start, Math.Abs(index));
                this.deck.RemoveRange(start, Math.Abs(index));

                // If this was from the back, add it to the front
                if (start > 0)
                    this.deck.InsertRange(0, removed);
                else
                    this.deck.AddRange(removed);
            }
        }

        private (BigInteger a, BigInteger b) runOperation(string operation, (BigInteger a, BigInteger b) op) {
            var parts = operation.Split(" ");

            (BigInteger a, BigInteger b) delta = (0, 0);

            if (operation == "deal into new stack") {
                // This just reverses the deck
                // x' = ax + b where a = -1, b = -1
                delta.a = -1;
                delta.b = -1;
            } else if (parts[0] == "deal" && parts[1] == "with") {
                // x' = ax + b where a = index, b = 0
                int index = Int32.Parse(parts[3]);
                delta.a = index;
                delta.b = 0;
            } else if (parts[0] == "cut") {
                // x' = ax + b where a = 1, b = -1 * index
                int index = Int32.Parse(parts[1]);
                delta.a = 1;
                delta.b = -1 * index;
            }

            BigInteger ta = (delta.a * op.a);
            while(ta < 0) ta += this.count;

            BigInteger tb = ((delta.a * op.b) + delta.b);
            while(tb < 0) tb += this.count;

            return ((ta % this.count), (tb % this.count));
        }

        private int findCard(int v) {
            for(int i=0; i<this.deck.Count; i++)
                if (this.deck[i] == v)
                    return i;
            
            return -1;
        }

        protected override string SolvePartOne()
        {
            this.resetDeck(10007);

            foreach(var line in Input.SplitByNewline(true, true))
                this.runOperation(line);

            return findCard(2019).ToString();
        }

        protected override string SolvePartTwo()
        {
            this.count = new BigInteger(119315717514047);

            (BigInteger a, BigInteger b) ab = (1, 0);

            foreach(var line in Input.SplitByNewline(true, true))
                ab = this.runOperation(line, ab);
            
            // Now we want to run this deck through the process 101741582076661 times
            // Based on
            // https://topaz.github.io/paste/#XQAAAQAgBQAAAAAAAAAzHIoib6pENkSmUIKIED8dy140D1lKWSMhNhZz+hjKgIgfJKPuwdqIBP14lxcYH/qI+6TyUGZUnsGhS4MQYaEtf9B1X3qIIO2JSejFjoJr8N1aCyeeRSnm53tWsBtER8F61O2YFrnp7zwG7y303D8WR4V0eGFqtDhF/vcF1cQdZLdxi/WhfyXZuWC+hs8WQCBmEtuId6/G0PeMA1Fr78xXt96Um/CIiLCievFE2XuRMAcBDB5We73jvDO95Cjg0CF2xgF4yt3v4RB9hmxa+gmt6t7wRI4vUIGoD8kX2k65BtmhZ7zSZk1Hh5p1obGZ6nuuFIHS7FpuSuv1faQW/FuXlcVmhJipxi37mvPNnroYrDM3PFeMw/2THdpUwlNQj0EDsslC7eSncZQPVBhPAHfYojh/LlqSf4DrfsM926hSS9Fdjarb9xBYjByQpAxLDcmDCMRFH5hkmLYTYDVguXbOCHcY+TFbl+G/37emZRFh/d+SkeGqbFSf64HJToM2I7N2zMrWP7NDDY5FWehD5gzKsJpEg34+sG7x2O82wO39qBlYHcYg1Gz4cLBrH1K1P+KWvEdcdj/NBtrl6yftMlCu6pH4WTGUe9oidaiRuQZOGtw71QsTQUuhpdoWO4mEH0U9+CiPZCZLaQolFDSky1J9nDhZZHy3+ETcUeDOfSu+HI3WuKC0AtIRPdG8B9GhtxZQKAx+5kyi/ek7A2JAY9SjrTuvRADxx5AikbHWXIsegZQkupAc2msammSkwY8dRMk0ilf5vh6kR0jHNbSi0g0KJLCJfqggeX24fKk5Mdh8ULZXnMfMZOmwEGfegByYbu91faLijfW4hoXCB1nlsWTPZEw2PCZqqhl9oc1q25H2YkkvKLxEZWl6a9eFuRzxhB840I1zdBjUVgfKd9/V4VdodzU2Z2e+VEh7RbJjQNFC/rG8dg==

            // For a, this is same as computing (a ** M) % n, which is in the computable
            // realm with fast exponentiation.
            // For b, this is same as computing ... + a**2 * b + a*b + b
            // == b * (a**(M-1) + a**(M) + ... + a + 1) == b * (a**M - 1)/(a-1)
            // That's again computable, but we need the inverse of a-1 mod n.
            BigInteger loop = new BigInteger(101741582076661);
            BigInteger Ma = BigInteger.ModPow(ab.a, loop, this.count);
            BigInteger Mb = (ab.b * (Ma - 1) * BigInteger.ModPow(ab.a-1, this.count-2, this.count)) % this.count;

            BigInteger temp = ((2020 - Mb) * BigInteger.ModPow(Ma, this.count-2, this.count));
            
            // We have to "fix" modulus for negative values
            // Don't use a loop here, it will take way too long
            if (temp < 0) {
                temp = this.count - (BigInteger.Abs(temp) % this.count);
            }

            return (temp % this.count).ToString();
        }
    }
}
