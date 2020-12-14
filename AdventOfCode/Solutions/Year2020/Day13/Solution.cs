using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;
using System.Numerics;

namespace AdventOfCode.Solutions.Year2020
{

    class Day13 : ASolution
    {

        List<(BigInteger bus, BigInteger remainder)> buses = new List<(BigInteger bus, BigInteger remainder)>();
        BigInteger timestamp = 0;

        public Day13() : base(13, 2020, "")
        {
            string[] lines = Input.SplitByNewline(true, true);

            timestamp = UInt64.Parse(lines[0]);

            var split = lines[1].Split(",");
            for(int i = 0; i<split.Length; i++) {
                // Ignore the x's
                if (split[i] == "x") continue;

                // Get the bus ID
                buses.Add((new BigInteger(UInt64.Parse(split[i])), new BigInteger(Int32.Parse(split[i]) - i)));
            }
        }

        protected override string SolvePartOne()
        {
            // Find an easy multiple of the bus IDs greater than our timestamp
            for(BigInteger i=0; i<timestamp + 100000; i++) {
                // Check each bus to see if it is a good multiple
                foreach(var b in buses) {
                    if ((timestamp+i)% b.bus == 0) {
                        Console.WriteLine($"Bus ID: {b}");
                        Console.WriteLine($"Timestamp: {timestamp+i}");
                        Console.WriteLine($"Difference: {i}");

                        // Found one!
                        return (b.bus * i).ToString();
                    }
                }
            }

            return null;
        }

        // Returns modulo inverse of a with respect to m using extended 
        // Euclid Algorithm. Refer below post for details: 
        // https://www.geeksforgeeks.org/multiplicative-inverse-under-modulo-m/ 
        private BigInteger inv(BigInteger a, BigInteger m) {
            BigInteger m0 = m, t, q; 
            BigInteger x0 = 0, x1 = 1; 
        
            if (m == 1) 
            return 0; 
        
            // Apply extended Euclid Algorithm 
            while (a > 1) 
            { 
                // q is quotient 
                q = a / m; 
        
                t = m; 
        
                // m is remainder now, process same as 
                // euclid's algo 
                m = a % m;
                a = t;
        
                t = x0; 
        
                x0 = x1 - q * x0; 
        
                x1 = t; 
            } 
        
            // Make x1 positive 
            if (x1 < 0) 
            x1 += m0; 
        
            return x1; 
        }

        protected override string SolvePartTwo()
        {
            // Chinese Remainder Theorem
            // We know that all of our bus IDs are prime
            // https://www.geeksforgeeks.org/chinese-remainder-theorem-set-2-implementation/
            /*
                x =  ( ∑ (rem[i]*pp[i]*inv[i]) ) % prod
                Where 0 <= i <= n-1

                rem[i] is given array of remainders

                prod is product of all given numbers
                prod = num[0] * num[1] * ... * num[k-1]

                pp[i] is product of all divided by num[i]
                pp[i] = prod / num[i]

                inv[i] = Modular Multiplicative Inverse of 
                        pp[i] with respect to num[i]
            */

            BigInteger prod = buses.Select(bus => bus.bus).Aggregate((a,b) => a*b);

            BigInteger result = 0;

            for(int i=0; i<buses.Count; i++) {
                BigInteger pp = prod / buses[i].bus;
                result += buses[i].remainder * inv(pp, buses[i].bus) * pp;
            }

            return (result % prod).ToString();
        }
    }
}
