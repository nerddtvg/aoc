using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{

    class Day10 : ASolution
    {
        List<int> adapters = new List<int>();

        public Day10() : base(10, 2020, "")
        {
            // Always start at 0
            adapters.Add(0);
            
            // Add the list
            adapters.AddRange(Input.ToIntArray("\n").OrderBy(a => a));

            // Final device is always +3
            adapters.Add(adapters.Last()+3);
        }

        protected override string SolvePartOne()
        {
            int diff1 = 0;
            int diff3 = 0;

            // For each entry, make sure the next higher item is <= 3 difference
            // Count if appropriate
            for(int i=0; i<adapters.Count-1; i++) {
                int diff = adapters[i+1] - adapters[i];

                if (diff == 0 || diff > 3) throw new Exception($"Invalid adapters: {adapters[i]} and {adapters[i+1]}");
                if (diff == 1) diff1++;
                if (diff == 3) diff3++;
            }

            Console.WriteLine(string.Join(", ", adapters));

            return (diff1 * diff3).ToString();
        }

        protected override string SolvePartTwo()
        {
            // Find all possible combinations from 0 to Last() respecting the rules
            // Based on: https://github.com/adrianosmond/adventofcode/blob/master/2020/day10.js
            // Logic is simple:
            // * Work from the end to the start
            // * If the difference between two values <= 3, you can add them on
            // Similar to a partial sum table

            long[] numRoutes = new long[adapters.Last()+1];

            // Set the end path
            numRoutes[adapters.Last()] = 1;

            for(int i = adapters.Count-2; i>=0; i--) {
                // Use the value of the adapter so we ensure we are mapping to the right path count
                // If we just use the indices, we will add extra paths that are not valid
                numRoutes[adapters[i]] = 0;

                for(int j=i+1; j<adapters.Count && j<=i+3; j++) {
                    // We're now checking i+1, i+2, i+3
                    if (adapters[j] - adapters[i] <= 3)
                        numRoutes[adapters[i]] += numRoutes[adapters[j]];
                }
            }

            return numRoutes[0].ToString();
        }
    }
}
