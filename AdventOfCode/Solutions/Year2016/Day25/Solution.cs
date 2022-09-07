using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{

    class Day25 : ASolution
    {
        public class AssemBunny
        {
            public Dictionary<char, int> registers = new Dictionary<char, int>();
            private int pos = 0;
            private bool running = false;
            public List<string> instructions;

            public string output = string.Empty;

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

                this.output = string.Empty;
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

                    // For this, we break out specific output
                    // Part 1: Check for 0, 1, 0, 1, 0, 1 repeating
                    if (this.output.Length >= 6)
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
                
                var print = (new Regex(@"out ([a-d]|[0-9]+)")).Match(line);

                if (print.Success)
                {
                    int val;
                    if (Int32.TryParse(print.Groups[1].Value, out val))
                    {
                        // Print a value
                        this.output += val.ToString();
                    }
                    else
                    {
                        // Print a register
                        this.output += this.registers[print.Groups[1].Value[0]];
                    }

                    this.pos++;
                    return;
                }

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

        public string ParsedCode(int inVal, string check)
        {
            // Registers
            int a = inVal;
            int b = 0;
            // int c = 0;
            // int d = 0;
            string ret = string.Empty;
            var foundRet = false;

            // d = a;
            // c = 15;
            // do
            // {
            //     b = 170;
            //     do
            //     {
            //         d++;
            //         b--;
            //     } while (b != 0);
            //     c--;
            // } while (c != 0);

            // d += 170*15
            // d += 2550;

            do
            {
                a = inVal + 2550;
                do
                {
                    // b = a;
                    // a = 0;

                    b = a;
                    a = b / 2;

                    if (b % 2 == 0)
                    {
                        ret += '0';
                    }
                    else
                    {
                        ret += '1';
                    }

                    // Otherwise
                    // c = 0;
                    // b -= 2;
                    // a++;

                    // var breakLoop = false;
                    // b = 1000 => 998, c = 0, a = 1;
                    // b = 998 => 996, c = 0, a = 2;
                    // b = 2 => 0, c = 0, a = 500;
                    // do
                    // {

                        // c = 2;
                        // do
                        // {
                        //     // if (b == 0)
                        //     // {
                        //     //     breakLoop = true;
                        //     //     break;
                        //     // }
                        //     b--;
                        //     c--;
                        // } while (!breakLoop && c != 0);
                        // a++;
                    // } while (!breakLoop);
                    // b = 2;
                    // do
                    // {
                    //     if (c == 0) break;
                    //     b--;
                    //     c--;
                    // } while (true);

                    // Output
                    //ret += b.ToString();

                    foundRet = ret.Length >= check.Length || check.Substring(0, ret.Length) != ret;
                } while (a != 0 && !foundRet);

                // Break out when we have a long enough string
            } while (!foundRet);

            return ret;
        }

        public Day25() : base(25, 2016, "Clock Signal")
        {

        }

        protected override string SolvePartOne()
        {
            // Brute forcing this takes a long time, so we refactored the ocde into C#
            int a = -1;
            string ret = string.Empty;
            string check = "01010101010101010101010101";
            var p = new AssemBunny(Input);
            do
            {
                ret = ParsedCode(++a, check);

                // p.Reset();
                // p.registers['a'] = a;

                // p.RunProgram();

                // Console.WriteLine($"Input: {a}");
                // Console.WriteLine($"Out 1: {ret}");
                // Console.WriteLine($"Out 2: {p.output}");
            } while (ret.Length < check.Length || ret.Substring(0, check.Length) != check);

            return a.ToString();
        }

        protected override string SolvePartTwo()
        {
            return string.Empty;
        }
    }
}
