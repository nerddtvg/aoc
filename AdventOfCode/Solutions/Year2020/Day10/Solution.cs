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

                if (diff > 3) throw new Exception($"Invalid adapters: {adapters[i]} and {adapters[i+1]}");
                if (diff == 1) diff1++;
                if (diff == 3) diff3++;
            }

            Console.WriteLine(string.Join(", ", adapters));

            return (diff1 * diff3).ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
