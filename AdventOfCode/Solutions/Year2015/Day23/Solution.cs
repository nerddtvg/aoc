using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2015
{

    class Day23 : ASolution
    {
        public Dictionary<char, uint> registers { get; set; }

        public Dictionary<int, string> instructions = new Dictionary<int, string>();

        public int position { get; set; } = 0;

        public Day23() : base(23, 2015, "")
        {
            ResetRegisters();

            // We need to read the instructions
            int c = 0;
            Input.SplitByNewline().ToList().ForEach(a =>
            {
                this.instructions[c++] = a;
            });
        }

        private void ResetRegisters()
        {
            this.registers = new Dictionary<char, uint>()
            {
                { 'a', 0 },
                { 'b', 0 }
            };
        }

        private int RunLine()
        {
            // Return 1 if we need to exit
            if (!this.instructions.ContainsKey(this.position))
                return 1;

            // Each operator should be 3 characters, then we can figure out what to do
            var instruction = this.instructions[this.position];
            var op = instruction.Substring(0, 3);

            // Most operations use this character, so grab it early
            var reg = instruction.Substring(4, 1).ToCharArray()[0];

            switch(op)
            {
                case "hlf":
                    this.registers[reg] /= 2;
                    this.position++;
                    break;
                
                case "tpl":
                    this.registers[reg] *= 3;
                    this.position++;
                    break;
                
                case "inc":
                    this.registers[reg]++;
                    this.position++;
                    break;
                
                case "jmp":
                    // JUMP!
                    this.position += ParseOffset(instruction.Substring(4));
                    break;
                
                case "jie":
                    {
                        if (this.registers[reg] % 2 == 0)
                        {
                            // JUMP!
                            this.position += ParseOffset(instruction.Substring(6));
                        }
                        else
                        {
                            this.position++;
                        }
                    }
                    break;
                
                case "jio":
                    {
                        if (this.registers[reg] == 1)
                        {
                            // JUMP!
                            this.position += ParseOffset(instruction.Substring(6));
                        }
                        else
                        {
                            this.position++;
                        }
                    }
                    break;

                default:
                    throw new Exception($"Shouldn't be here: {op}");
            }

            return 0;
        }

        private int ParseOffset(string offset)
        {
            // Start with a trim
            offset = offset.Trim();

            // Grab the first character to determine direction
            var dir = offset.Substring(0, 1) == "+" ? 1 : -1;

            // Parse the rest as a number
            return dir * Int32.Parse(offset.Substring(1));
        }

        protected override string SolvePartOne()
        {
            // Ensure we're at the start
            this.position = 0;
            int ret = 0;

            do
            {
                ret = RunLine();
            } while (ret == 0);

            return this.registers['b'].ToString();
        }

        protected override string SolvePartTwo()
        {
            // Ensure we're at the start
            this.position = 0;
            int ret = 0;

            // Part 2 means starting with reg 'a' as 1
            ResetRegisters();
            this.registers['a'] = 1;

            do
            {
                ret = RunLine();
            } while (ret == 0);

            return this.registers['b'].ToString();
        }
    }
}
