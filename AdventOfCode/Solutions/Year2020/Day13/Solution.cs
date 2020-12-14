using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{

    class Day13 : ASolution
    {

        List<(ulong bus, ulong remainder)> buses = new List<(ulong bus, ulong remainder)>();
        ulong timestamp = 0;

        public Day13() : base(13, 2020, "")
        {
            string[] lines = Input.SplitByNewline(true, true);

            timestamp = UInt64.Parse(lines[0]);

            var split = lines[1].Split(",");
            for(ulong i = 0; i<Convert.ToUInt64(split.LongLength); i++) {
                // Ignore the x's
                if (split[i] == "x") continue;

                // Get the bus ID
                buses.Add((UInt64.Parse(split[i]), i));
            }
        }

        protected override string SolvePartOne()
        {
            // Find an easy multiple of the bus IDs greater than our timestamp
            for(ulong i=0; i<timestamp + 100000; i++) {
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

            ulong prod = 1;
            buses.ForEach(a => prod = prod * a.bus);

            // Generate the pp[] array
            List<ulong> pp = new List<ulong>();
            for(int i=0; i<buses.Count; i++)
                pp.Add(prod / buses[i].bus);
            

            // Generate the inv[] array
            List<ulong> inv = new List<ulong>();
            for(int i=0; i<buses.Count; i++)
                inv.Add((pp[i] * buses[i].remainder) % buses[i].bus);

            ulong sum = 0;
            Enumerable.Range(0, buses.Count).ToList().ForEach(i => sum += buses[i].remainder * pp[i] * inv[i]);

            return (sum % prod).ToString();
        }
    }
}
