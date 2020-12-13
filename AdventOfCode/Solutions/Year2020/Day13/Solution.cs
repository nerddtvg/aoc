using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{

    class Day13 : ASolution
    {

        List<int> buses = new List<int>();
        int timestamp = 0;

        public Day13() : base(13, 2020, "")
        {
            string[] lines = Input.SplitByNewline(true, true);

            timestamp = Int32.Parse(lines[0]);

            foreach(var bus in lines[1].Split(",")) {
                // Ignore the x's
                if (bus == "x") continue;

                // Get the bus ID
                buses.Add(Int32.Parse(bus));
            }
        }

        protected override string SolvePartOne()
        {
            // Find an easy multiple of the bus IDs greater than our timestamp
            for(int i=0; i<timestamp + 100000; i++) {
                // Check each bus to see if it is a good multiple
                foreach(var b in buses) {
                    if ((timestamp+i)%b == 0) {
                        Console.WriteLine($"Bus ID: {b}");
                        Console.WriteLine($"Timestamp: {timestamp+i}");
                        Console.WriteLine($"Difference: {i}");

                        // Found one!
                        return (b * i).ToString();
                    }
                }
            }

            return null;
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
