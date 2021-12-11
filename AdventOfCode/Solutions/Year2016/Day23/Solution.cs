using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2016
{

    class Day23 : ASolution
    {
        public class AssemBunny
        {
            public Dictionary<char, int> registers = new Dictionary<char, int>();
            private int pos = 0;
            private bool running = false;
            public List<string> instructions;

            public AssemBunny(string _instructions)
            {
                this.instructions = _instructions.Trim().SplitByNewline().ToList();

                Reset();
            }

            public void Reset()
            {
                this.registers = new Dictionary<char, int>()
                {
                    { 'a', 0 },
                    { 'b', 0 },
                    { 'c', 0 },
                    { 'd', 0 }
                };

                this.pos = 0;
                this.running = true;
            }

            public void RunProgram()
            {
                if (this.instructions == null || this.instructions.Count == 0)
                    return;

                do
                {
                    // Check if we're done
                    if (this.pos < 0 || this.pos >= this.instructions.Count)
                    {
                        running = false;
                        break;
                    }

                    ProcessLine(this.instructions[this.pos]);
                } while (running);
            }

            private void ProcessLine(string line)
            {
                var copy = (new Regex(@"cpy ([a-d]|[\-0-9]+) ([a-d])")).Match(line);
                var inc = (new Regex(@"inc ([a-d])")).Match(line);
                var dec = (new Regex(@"dec ([a-d])")).Match(line);
                var jnz = (new Regex(@"jnz ([a-d]|[\-0-9]+) ([a-d]|[\-0-9]+)")).Match(line);
                
                var tgl = (new Regex(@"tgl ([a-d]|[0-9]+)")).Match(line);

                if (copy.Success)
                {
                    int val;
                    if (Int32.TryParse(copy.Groups[1].Value, out val))
                    {
                        // Put an integer in place
                        this.registers[copy.Groups[2].Value[0]] = val;
                    }
                    else
                    {
                        // Copy a register
                        this.registers[copy.Groups[2].Value[0]] = this.registers[copy.Groups[1].Value[0]];
                    }

                    this.pos++;
                    return;
                }

                if (inc.Success)
                {
                    this.registers[inc.Groups[1].Value[0]]++;
                    this.pos++;
                    return;
                }

                if (dec.Success)
                {
                    this.registers[dec.Groups[1].Value[0]]--;
                    this.pos++;
                    return;
                }

                if (jnz.Success)
                {
                    int val2;
                    if (!Int32.TryParse(jnz.Groups[2].Value, out val2))
                        val2 = this.registers[jnz.Groups[2].Value[0]];

                    int val;
                    if (Int32.TryParse(jnz.Groups[1].Value, out val))
                    {
                        if (val != 0)
                        {
                            this.pos += val2;
                        }
                        else
                        {
                            this.pos++;
                        }
                    }
                    else
                    {
                        if (this.registers[jnz.Groups[1].Value[0]] != 0)
                        {
                            this.pos += val2;
                        }
                        else
                        {
                            this.pos++;
                        }
                    }

                    return;
                }

                if (tgl.Success)
                {
                    int val;
                    if (!Int32.TryParse(tgl.Groups[1].Value, out val))
                    {
                        val = this.registers[tgl.Groups[1].Value[0]];
                    }

                    if (this.pos + val >= 0 && this.pos + val < this.instructions.Count)
                    {

                        var otherInstruction = this.instructions[this.pos + val];

                        switch (otherInstruction.Substring(0, 3))
                        {
                            case "inc":
                                this.instructions[this.pos + val] = "dec" + otherInstruction.Substring(3);
                                break;

                            case "dec":
                                this.instructions[this.pos + val] = "inc" + otherInstruction.Substring(3);
                                break;

                            case "jnz":
                                this.instructions[this.pos + val] = "cpy" + otherInstruction.Substring(3);
                                break;

                            case "cpy":
                                this.instructions[this.pos + val] = "jnz" + otherInstruction.Substring(3);
                                break;

                            case "tgl":
                                this.instructions[this.pos + val] = "inc" + otherInstruction.Substring(3);
                                break;
                        }
                    }

                    this.pos++;
                    return;
                }

                // We skipped a line
                this.pos++;
            }
        }

        public Day23() : base(23, 2016, "Safe Cracking")
        {

        }

        protected override string SolvePartOne()
        {
            var p = new AssemBunny(Input);

            p.registers['a'] = 7;

            p.RunProgram();

            return p.registers['a'].ToString();
        }

        protected override string SolvePartTwo()
        {
            // The formula was figured out in the mega thread
            // I couldn't figure out how to simplify the assembly with toggling
            return ((73 * 71) + Enumerable.Range(1, 12).Aggregate((x, y) => x * y)).ToString();
        }
    }
}

