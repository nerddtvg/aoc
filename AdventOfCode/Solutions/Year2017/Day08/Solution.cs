using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2017
{

    class Day08 : ASolution
    {

        private Dictionary<string, int> registers = new Dictionary<string, int>();

        public Day08() : base(08, 2017, "I Heard You Like Registers")
        {
            
        }

        // Just to keep things the same
        private int SetRegister(string id, int val) => registers[id] = val;

        // All registers start at 0
        private int GetRegister(string id)
        {
            if (!registers.ContainsKey(id))
                registers[id] = 0;

            return registers[id];
        }

        protected override string? SolvePartOne()
        {
            this.registers.Clear();

            foreach(var line in Input.SplitByNewline(true))
            {
                var parts = line.Split(' ', 7, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                // Check our conditional
                // Sample line spread out:
                //  b  inc  5   if  a   >   1
                // [0] [1] [2] [3] [4] [5] [6]

                var opVal = Int32.Parse(parts[2]);
                var condVal = Int32.Parse(parts[6]);

                var action = false;

                switch(parts[5])
                {
                    case ">":
                        action = GetRegister(parts[4]) > condVal;
                        break;
                        
                    case "<":
                        action = GetRegister(parts[4]) < condVal;
                        break;
                        
                    case ">=":
                        action = GetRegister(parts[4]) >= condVal;
                        break;
                        
                    case "<=":
                        action = GetRegister(parts[4]) <= condVal;
                        break;
                        
                    case "==":
                        action = GetRegister(parts[4]) == condVal;
                        break;
                        
                    case "!=":
                        action = GetRegister(parts[4]) != condVal;
                        break;
                }

                // Not true, carry on
                if (!action)
                    continue;

                // Do whatever we need to do
                switch(parts[1])
                {
                    case "inc":
                        SetRegister(parts[0], GetRegister(parts[0]) + opVal);
                        break;
                        
                    case "dec":
                        SetRegister(parts[0], GetRegister(parts[0]) - opVal);
                        break;
                }
            }

            return this.registers.Max(reg => reg.Value).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
