using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2017
{

    class Day18 : ASolution
    {
        private Dictionary<char, long> registers = new Dictionary<char, long>();
        private List<string> instructions = new List<string>();
        private int pos = 0;

        private long lastSound = 0;
        private long lastRcv = 0;

        // Remember the first one we recovered
        private long firstRcv = 0;

        public Day18() : base(18, 2017, "")
        {
            
        }

        private long GetRegister(char c) =>
            this.registers.ContainsKey(c) ? this.registers[c] : 0;

        private void SetRegister(char c, long val) =>
            this.registers[c] = val;

        private void Reset()
        {
            this.registers.Clear();

            this.instructions = Input.SplitByNewline(true, true).ToList();

            this.pos = 0;
        }

        private bool Run()
        {
            if (this.pos < 0 || this.pos >= this.instructions.Count)
                return false;

            Console.WriteLine($"Instruction [{this.pos}]: {this.instructions[this.pos]}");

            // Get register A in each instruction
            var regA = this.instructions[this.pos][4];
            long regAVal = Int32.MinValue;

            if (!long.TryParse(regA.ToString(), out regAVal))
            {
                regAVal = GetRegister(regA);
            }

            // The rest of the instruction
            long rest = 0;

            if (this.instructions[this.pos].Length > 5)
            {
                // This could be a value or a register
                if (!long.TryParse(this.instructions[this.pos].Substring(6), out rest))
                {
                    // Found a register
                    rest = GetRegister(this.instructions[this.pos][6]);
                }
            }

            // Get the instruction value
            switch(this.instructions[this.pos].Substring(0, 3))
            {
                case "snd":
                    Console.WriteLine($"Play Sound: {regAVal}");
                    this.lastSound = regAVal;
                    break;
                    
                case "set":
                    SetRegister(regA, rest);
                    break;
                    
                case "add":
                    SetRegister(regA, regAVal + rest);
                    break;
                    
                case "mul":
                    SetRegister(regA, regAVal * rest);
                    break;

                case "mod":
                    SetRegister(regA, regAVal % rest);
                    break;

                case "rcv":
                    if (regAVal != 0)
                    {
                        if (this.firstRcv == 0)
                            this.firstRcv = this.lastSound;

                        this.lastRcv = this.lastSound;

                        return false;
                    }
                    break;
                    
                case "jgz":
                    // Jumps can be defined by integers, not just registers
                    if (regAVal > 0)
                    {
                        // Offset by the fact we increment later
                        this.pos += (int) rest - 1;
                    }
                    break;
            }

            // Next instruction
            this.pos++;

            return true;
        }

        protected override string? SolvePartOne()
        {
            Reset();

            bool ret = false;

            do
            {
                ret = Run();
            } while (ret);

            return this.firstRcv.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
