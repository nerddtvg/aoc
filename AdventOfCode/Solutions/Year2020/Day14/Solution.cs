using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{

    class Day14 : ASolution
    {
        Dictionary<ulong, ulong> memory = new Dictionary<ulong, ulong>();
        string bitmask = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";

        // Length of numerics (36 bits: 0-35)
        int bitlength = 35;

        public Day14() : base(14, 2020, "")
        {
            /*
            DebugInput = @"
            mask = XXXXXXXXXXXXXXXXXXXXXXXXXXXXX1XXXX0X
            mem[8] = 11
            mem[7] = 101
            mem[8] = 0";
            */
        }

        private void SetMemory(ulong address, ulong value) {
            // Using the bitmask, we modify the values
            for(int i=this.bitlength; i>=0; i--) {
                int power = this.bitlength - i;

                // Get the character in the bitmask
                string c = this.bitmask.Substring(i, 1);

                // No action
                if (c == "X") continue;

                ulong newBit = UInt64.Parse(c);

                // Otherwise we need to set the value of that bit to the bitmask value
                // https://www.geeksforgeeks.org/modify-bit-given-position/
                ulong mask = (ulong) 1 << power;
                value = (value & ~mask) | ((newBit << power) & mask);
            }

            this.memory[address] = value;
        }

        protected override string SolvePartOne()
        {
            // Need to read in the input and perform the work
            foreach(var line in Input.SplitByNewline(true, true)) {
                if (line.Substring(0, 7) == "mask = ")
                    bitmask = line.Substring(7);
                else {
                    // Run the memory
                    string[] parts = line.Split("=", 2, StringSplitOptions.TrimEntries);
                    
                    ulong address = UInt64.Parse(parts[0].Replace("mem[", "").Replace("]", ""));
                    ulong value = UInt64.Parse(parts[1]);

                    SetMemory(address, value);
                }
            }

            return this.memory.Select(a => a.Value).Aggregate((a,b) => a+b).ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
