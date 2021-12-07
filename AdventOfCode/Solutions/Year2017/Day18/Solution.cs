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
        public List<SoundProgram> programs = new List<SoundProgram>();

        public Day18() : base(18, 2017, "Duet")
        {

        }

        protected override string? SolvePartOne()
        {
            var program = new SoundProgram(Input);

            bool ret = false;

            do
            {
                ret = program.Run();
            } while (ret);

            return program.firstRcv.ToString();
        }

        protected override string? SolvePartTwo()
        {
            var p0 = new SoundProgram(Input, 0);
            var p1 = new SoundProgram(Input, 1);

            // Set the remote queues up
            p0.remote = p1.queue;
            p1.remote = p0.queue;

            bool ret0 = false;
            bool ret1 = false;
            bool deadlocked = false;

            do
            {
                ret0 = p0.Run();
                ret1 = p1.Run();

                // If deadlocked
                deadlocked = (p0.GetInstruction() == "rcv" && p0.queue.Count == 0 && p1.GetInstruction() == "rcv" && p1.queue.Count == 0);
            } while (ret0 && ret1 && !deadlocked);

            return p1.sendCount.ToString();
        }

        public class SoundProgram
        {
            private Dictionary<char, long> registers = new Dictionary<char, long>();
            private List<string> instructions = new List<string>();
            private int pos = 0;

            private long lastSound = 0;
            private long lastRcv = 0;

            // Remember the first one we recovered
            public long firstRcv = 0;

            private long GetRegister(char c) =>
                this.registers.ContainsKey(c) ? this.registers[c] : 0;

            private void SetRegister(char c, long val) =>
                this.registers[c] = val;

            public Queue<long> queue = new Queue<long>();
            public Queue<long> remote = new Queue<long>();
            public int sendCount = 0;

            private int part = 1;

            public SoundProgram(string Input, int pid = -1)
            {
                this.registers.Clear();

                this.instructions = Input.SplitByNewline(true, true).ToList();

                this.pos = 0;

                // This is for part 2
                this.part = 1;
                if (pid > -1)
                {
                    this.part = 2;
                    this.registers['p'] = pid;
                }
            }

            public string GetInstruction() => this.pos >= 0 && this.pos < this.instructions.Count ? this.instructions[this.pos].Substring(0, 3) : string.Empty;

            public bool Run()
            {
                if (this.pos < 0 || this.pos >= this.instructions.Count)
                    return false;

                // if (part == 1)
                //     Console.WriteLine($"Instruction [{this.pos}]: {this.instructions[this.pos]}");

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
                        // if (part == 1)
                        //     Console.WriteLine($"Play Sound: {regAVal}");
                        this.lastSound = regAVal;

                        // For part 2:
                        this.remote.Enqueue(regAVal);
                        this.sendCount++;
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
                        if (this.part == 1 && regAVal != 0)
                        {
                            if (this.firstRcv == 0)
                                this.firstRcv = this.lastSound;

                            this.lastRcv = this.lastSound;

                            return false;
                        }
                        else if (this.part == 2)
                        {
                            if (this.queue.Count > 0)
                            {
                                SetRegister(regA, this.queue.Dequeue());
                            }
                            else
                            {
                                // Return to this point
                                this.pos--;
                            }
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
        }
    }
}

#nullable restore
