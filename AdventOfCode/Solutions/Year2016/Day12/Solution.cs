using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{

    class Day12 : ASolution
    {
        private Dictionary<char, int> registers = new();
        private int pos;
        private bool running;

        public Day12() : base(12, 2016, "")
        {
            Reset();
        }

        private void Reset()
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

        private void RunProgram()
        {
            var lines = Input.SplitByNewline();

            do
            {
                // Check if we're done
                if (this.pos < 0 || this.pos >= lines.Length)
                {
                    running = false;
                    break;
                }

                ProcessLine(lines[this.pos]);
            } while (running);
        }

        private void ProcessLine(string line)
        {
            var copy = (new Regex("cpy ([a-d]|[0-9]+) ([a-d])")).Match(line);
            var inc = (new Regex("inc ([a-d])")).Match(line);
            var dec = (new Regex("dec ([a-d])")).Match(line);
            var jnz = (new Regex(@"jnz ([a-d]|[0-9]+) ([\-0-9]+)")).Match(line);

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
                int val;
                if (Int32.TryParse(jnz.Groups[1].Value, out val))
                {
                    if (val != 0)
                    {
                        this.pos += Int32.Parse(jnz.Groups[2].Value);
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
                        this.pos += Int32.Parse(jnz.Groups[2].Value);
                    }
                    else
                    {
                        this.pos++;
                    }
                }

                return;
            }

            throw new Exception($"Somehow we got to a bad line: {line}");
        }

        protected override string SolvePartOne()
        {
            Reset();
            RunProgram();

            return this.registers['a'].ToString();
        }

        protected override string SolvePartTwo()
        {
            // This takes many minutes to complete. I'm sure I was supposed to optimize something but didn't
            Reset();
            this.registers['c'] = 1;
            RunProgram();

            return this.registers['a'].ToString();
        }
    }
}
