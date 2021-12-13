using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2018
{

    class Day19 : ASolution
    {
        public class OpCodeComputer
        {
            enum WristOpCode {
                addr,
                addi,
                mulr,
                muli,
                banr,
                bani,
                borr,
                bori,
                setr,
                seti,
                gtir,
                gtri,
                gtrr,
                eqir,
                eqri,
                eqrr
            }

            enum WristInstruction {
                op,
                A,
                B,
                C
            }

            /// <summary>
            /// Instruction Pointer
            /// </summary>
            public int ipRegister = 0;
            public int ipVal = 0;

            public int startingIp = 0;

            public List<string> instructions = new List<string>();

            // Registers, start at zero
            public Dictionary<int, int> registers = new Dictionary<int, int>();

            public OpCodeComputer(string input)
            {
                this.instructions = input.Trim().SplitByNewline().ToList();

                // Save this for future resets
                if (this.instructions[0].StartsWith("#ip"))
                {
                    this.startingIp = Int32.Parse(this.instructions[0].Split(' ', 2)[1]);
                    this.instructions.RemoveAt(0);
                }

                Reset();
            }

            public void Reset()
            {
                this.ipRegister = this.startingIp;
                this.registers = new Dictionary<int, int>()
                {
                    { 0, 0 },
                    { 1, 0 },
                    { 2, 0 },
                    { 3, 0 },
                    { 4, 0 },
                    { 5, 0 }
                };
            }
            
            public bool RunOperation(bool printDebug = false) {
                // Are we out of bounds?
                if (this.ipVal < 0 || this.ipVal >= this.instructions.Count)
                {
                    return false;
                }

                // First we write the value of ipVal to ipRegister
                this.registers[this.ipRegister] = this.ipVal;

                var instruction = this.instructions[this.ipVal];

                // Easier referencing
                var split = instruction.Split(' ', 4);
                var code = Enum.Parse<WristOpCode>(split[0], true);
                var a = Int32.Parse(split[1]);
                var b = Int32.Parse(split[2]);
                var c = Int32.Parse(split[3]);

                if (printDebug)
                {
                    Console.Write($"ip={ipVal} [{string.Join(", ", this.registers.Select(kvp => kvp.Value.ToString()))}] {instruction} ");
                }

                switch(code) {
                    case WristOpCode.addr:
                        registers[c] = (registers[a] + registers[b]);
                        break;
                    
                    case WristOpCode.addi:
                        registers[c] = (registers[a] + b);
                        break;
                    
                    case WristOpCode.mulr:
                        registers[c] = (registers[a] * registers[b]);
                        break;
                    
                    case WristOpCode.muli:
                        registers[c] = (registers[a] * b);
                        break;
                    
                    case WristOpCode.banr:
                        registers[c] = (registers[a] & registers[b]);
                        break;
                    
                    case WristOpCode.bani:
                        registers[c] = (registers[a] & b);
                        break;
                    
                    case WristOpCode.borr:
                        registers[c] = (registers[a] | registers[b]);
                        break;
                    
                    case WristOpCode.bori:
                        registers[c] = (registers[a] | b);
                        break;
                    
                    case WristOpCode.setr:
                        registers[c] = registers[a];
                        break;
                    
                    case WristOpCode.seti:
                        registers[c] = a;
                        break;
                    
                    case WristOpCode.gtir:
                        registers[c] = (a > registers[b] ? 1 : 0);
                        break;
                    
                    case WristOpCode.gtri:
                        registers[c] = (registers[a] > b ? 1 : 0);
                        break;
                    
                    case WristOpCode.gtrr:
                        registers[c] = (registers[a] > registers[b] ? 1 : 0);
                        break;
                    
                    case WristOpCode.eqir:
                        registers[c] = (a == registers[b] ? 1 : 0);
                        break;
                    
                    case WristOpCode.eqri:
                        registers[c] = (registers[a] == b ? 1 : 0);
                        break;
                    
                    case WristOpCode.eqrr:
                        registers[c] = (registers[a] == registers[b] ? 1 : 0);
                        break;
                }

                // After the instruction is run, read the ipVal back from ipRegister and increment by one:
                this.ipVal = this.registers[this.ipRegister];
                this.ipVal++;

                if (printDebug)
                {
                    Console.WriteLine($"[{string.Join(", ", this.registers.Select(kvp => kvp.Value.ToString()))}]");
                }

                return true;
            }
        }

        public Day19() : base(19, 2018, "Go With The Flow")
        {
//             DebugInput = @"#ip 0
// seti 5 0 1
// seti 6 0 2
// addi 0 1 0
// addr 1 2 3
// setr 1 0 0
// seti 8 0 4
// seti 9 0 5";
        }

        protected override string? SolvePartOne()
        {
            var p = new Day19.OpCodeComputer(Input);

            while(p.RunOperation())
            {
                // Run it
            }

            return p.registers[0].ToString();
        }

        protected override string? SolvePartTwo()
        {
            // This is supposed to be a reduction problem where
            // the assembly is broken down and determine what is being done
            var p = new Day19.OpCodeComputer(Input);

            /*
            Initial breakdown:
            goto A, r4=16, r4++ (r4=17 in the end)
            C: r1 = 1, r4++
            E: r2 = 1, r4++
            D: r3 = r1 * r2
            if (r3 == r5) r3=1 else r3=0, r4++
            r4 += r3, r4++
            r4 += 1, r4++
            r0 += r1, r4++
            r2 += 1, r4++
            if (r2 > r5) r3=1 else r3=0, r4++
            r4 += r3, r4++ (jump over one if r2 > r5)
            r4 = 2, r4++ (jump to 2+1, goto D)
            r1 += 1
            if (r1 > r5) r3=1 else r3=0, r4++
            r4 += r3, r4++ (jump over one if r1 > r5)
            r4=1, r4++ (jump to 1+1, goto E)
            r4 *= r4, r4++ [end?]
            A: r5 += 2, r4++
            r5 *= r5 (square), r4++
            r5 *= r4, r4++
            r5 *= 11, r4++
            r3 += 4, r4++
            r3 *= r4, r4++
            r3 += 21, r4++
            r5 += r3, r4++
            r4 += r0, r4++, [r0=1 so goto B:]
            seti 0 5 4
            B: r3 = r1 + r4, r4++
            r3 *= r4, r4++
            r3 += r4, r4++
            r3 *= r4, r4++
            r3 *= 14, r4++
            r3 *= r4, r4++
            r5 += r3, r4++
            r0 = 0, r4++
            r4 = 0, r4++, goto C (top+1)
            */

            /*
            goto A, r4=16, r4++ (r4=17 in the end)
            do {
                // First entrance here
                r0 = 0
                r1 = 0
                r2 = 0
                r3 = 10550400
                r4 = 1
                r5 = 10551345

                C:
                r1 = 1, r4++
                E:
                r2 = 1, r4++
                
                do {
                    r3 = r1 * r2, r4++
                    if (r3 != r5) r4 += r3, r4++
                    r4 += 1, r4++
                    r0 += r1, r4++
                    r2 += 1, r4++
                } while(r2 <= r5)

                r1 += 1
                if (r1 > r5) r3=1 else r3=0, r4++
                r4 += r3, r4++ (jump over one if r1 > r5)
                r4=1, r4++ (jump to 1+1, goto E)

                r4 *= r4, r4++ [end?]

                A: r5 += 2, r4++
                r5 *= r5 (square), r4++
                r5 *= r4, r4++
                r5 *= 11, r4++
                r3 += 4, r4++
                r3 *= r4, r4++
                r3 += 21, r4++
                r5 += r3, r4++
                r4 += r0, r4++, [r0=1 so goto B:]
                r4 = 0, r4++ 
                B: r3 = r1 + r4, r4++
                r3 *= r4, r4++
                r3 += r4, r4++
                r3 *= r4, r4++
                r3 *= 14, r4++
                r3 *= r4, r4++
                r5 += r3, r4++
                r0 = 0, r4++

                // At this point (beginning) generated here
                r0 = 0
                r1 = 0
                r2 = 0
                r3 = 10550400
                r4 = 1
                r5 = 10551345
            } while(true)
            */

            // The top part seems to be counting how many factors/divisors
            // there are from 1 to 10551345 in 10551345, then 2 to 10551345, then 3 to 10551345...
            // Each step adds the number of factors there

            var max = 10551345;
            var divisors = max.GetDivisors();
            var ret = 0;

            for (int i = 1; i <= max; i++)
            {
                ret += divisors.Count(v => v >= i);
            }

            return ret.ToString();
        }
    }
}

#nullable restore
