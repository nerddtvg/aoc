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

            DebugInput = @"
            mask = 000000000000000000000000000000X1001X
            mem[42] = 100
            mask = 00000000000000000000000000000000X0XX
            mem[26] = 1";
        }

        private ulong ModifyValue(ulong value) {
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

            return value;
        }

        private ulong ModifyValue2(ulong value, string bmask = "") {
            // Using the bitmask, we modify the values
            for(int i=this.bitlength; i>=0; i--) {
                int power = this.bitlength - i;

                // Get the character in the bitmask
                string c = bmask.Substring(i, 1);

                // No action
                if (c == "0") continue;

                ulong newBit = UInt64.Parse(c);

                // Otherwise we need to set the value of that bit to the bitmask value
                // https://www.geeksforgeeks.org/modify-bit-given-position/
                ulong mask = (ulong) 1 << power;
                value = (value & ~mask) | ((newBit << power) & mask);
            }

            return value;
        }

        private void SetMemory(ulong address, ulong value) {
            this.memory[address] = ModifyValue(value);
        }

        private List<string> GetPossibleBitmasks(int start=0) {
            // We look at character at 'start' and see what to do with it
            // First, GetPossibleBitMasks(start+1)
            // Then, if character at start is 0 or 1, return that prepended onto the lists
            // Otherwise, return 0 and 1 prepended

            List<string> children = new List<string>();
            List<string> ret = new List<string>();

            // Get the children if possible
            if (start < this.bitlength)
                children = GetPossibleBitmasks(start+1);

            // Get the character in the bitmask
            string c = this.bitmask.Substring(start, 1);

            // What do we do?
            if (c == "0" || c == "1") {
                // Return this character prepended on the given lists
                if (children.Count > 0)
                    foreach(var l in children)
                        ret.Add(c + l);
                else
                    ret.Add(c);
            } else {
                if (children.Count > 0)
                    for(int i=0; i<=1; i++)
                        foreach(var l in children)
                            ret.Add(i.ToString() + l);
                else
                    for(int i=0; i<=1; i++)
                        ret.Add(i.ToString());
            }

            return ret;
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
            // Reinitialize
            memory = new Dictionary<ulong, ulong>();

            // Need to read in the input and perform the work
            foreach(var line in Input.SplitByNewline(true, true)) {
                if (line.Substring(0, 7) == "mask = ")
                    bitmask = line.Substring(7);
                else {
                    // Run the memory
                    string[] parts = line.Split("=", 2, StringSplitOptions.TrimEntries);
                    
                    ulong address = UInt64.Parse(parts[0].Replace("mem[", "").Replace("]", ""));
                    ulong value = UInt64.Parse(parts[1]);

                    // Get all of the possible bitmasks to apply
                    var bitmasks = GetPossibleBitmasks();

                    // For each possible bitmask, set the memory
                    bitmasks.ForEach(a => {
                        this.memory[ModifyValue2(address, a)] = value;
                    });
                }
            }

            return this.memory.Select(a => a.Value).Aggregate((a,b) => a+b).ToString();
        }
    }
}
