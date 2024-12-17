using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Numerics;


namespace AdventOfCode.Solutions.Year2024
{

    partial class Day17 : ASolution
    {
        public required BigInteger a;
        public required BigInteger b;
        public required BigInteger c;
        public required int[] program;
        public required int instruction;

        [GeneratedRegex(@"Register A: (?<A>[0-9]+)\s*Register B: (?<B>[0-9]+)\s*Register C: (?<C>[0-9]+)\s*Program: (?<Program>[0-9,]+)", RegexOptions.Singleline, "en-US")]
        private static partial Regex ProgramRegex();

        public Day17() : base(17, 2024, "Chronospatial Computer")
        {
            // DebugInput = @"Register A: 729
            // Register B: 0
            // Register C: 0

            // Program: 0,1,5,4,3,0";

            // ResetComputer();

            // if (RunComputer() != "4,6,3,5,6,3,5,2,1,0")
            // {
            //     throw new Exception("Expected: 4,6,3,5,6,3,5,2,1,0");
            // }

            // DebugInput = @"Register A: 0
            // Register B: 0
            // Register C: 9

            // Program: 2,6";

            // ResetComputer();

            // RunComputer();
            // if (b != 1)
            // {
            //     throw new Exception("Expected b = 1");
            // }

            // DebugInput = @"Register A: 10
            // Register B: 0
            // Register C: 0

            // Program: 5,0,5,1,5,4";

            // ResetComputer();

            // if (RunComputer() != "0,1,2")
            // {
            //     throw new Exception("Expected: 0,1,2");
            // }

            // DebugInput = @"Register A: 2024
            // Register B: 0
            // Register C: 0

            // Program: 0,1,5,4,3,0";

            // ResetComputer();

            // if (RunComputer() != "4,2,5,6,7,7,7,7,3,1,0" || a != 0)
            // {
            //     throw new Exception("Expected a = 0 and 4,2,5,6,7,7,7,7,3,1,0");
            // }

            // DebugInput = @"Register A: 0
            // Register B: 29
            // Register C: 0

            // Program: 1,7";

            // ResetComputer();

            // RunComputer();
            // if (b != 26)
            // {
            //     throw new Exception("Expected b = 26");
            // }

            // DebugInput = @"Register A: 0
            // Register B: 2024
            // Register C: 43690

            // Program: 4,0";

            // ResetComputer();

            // RunComputer();
            // if (b != 44354)
            // {
            //     throw new Exception("Expected b = 44354");
            // }

            // Fully reset
            ResetComputer();
        }

        void ResetComputer()
        {
            var matches = ProgramRegex().Match(Input);

            if (!matches.Success)
                throw new Exception("Unable to parse program.");

            a = BigInteger.Parse(matches.Groups["A"].Value);
            b = BigInteger.Parse(matches.Groups["B"].Value);
            c = BigInteger.Parse(matches.Groups["C"].Value);
            program = matches.Groups["Program"].Value.ToIntArray(",");

            instruction = 0;
        }

        BigInteger GetCombo(int operand) => operand switch
        {
            0 or 1 or 2 or 3 => operand,
            4 => a,
            5 => b,
            6 => c,
            _ => throw new Exception("Invalid operand"),
        };

        int BigIntToInt(BigInteger bigInt) => int.Parse(bigInt.ToString());

        BigInteger[] RunComputer(bool part2 = false)
        {
            List<BigInteger> output = [];

            while (0 <= instruction && instruction < program.Length - 1)
            {
                switch (program[instruction])
                {
                    case 0:
                        // adv Division: A / combo
                        a /= BigInteger.Pow(2, BigIntToInt(GetCombo(program[instruction + 1])));
                        instruction += 2;
                        break;

                    case 1:
                        // bxl Bitwise XOR: B XOR literal
                        b ^= program[instruction + 1];
                        instruction += 2;
                        break;

                    case 2:
                        // bst: B = Combo % 8
                        b = GetCombo(program[instruction + 1]) % 8;
                        instruction += 2;
                        break;

                    case 3:
                        // jnz: Nothing if A is 0
                        // Otherwise: instruction += A
                        // No +2 if jumped
                        if (a == 0)
                        {
                            instruction += 2;
                            break;
                        }

                        instruction = program[instruction + 1];
                        break;

                    case 4:
                        // bxc Bitwise XOR: B XOR C
                        b ^= c;
                        instruction += 2;
                        break;

                    case 5:
                        // out: Puts Combo%8 in output
                        output.Add(GetCombo(program[instruction + 1]) % 8);

                        // For part 2, make sure this value lines up
                        if (part2 && output[^1] != program[output.Count - 1])
                        {
                            return [.. output];
                        }

                        instruction += 2;
                        break;

                    case 6:
                        // bdv Division: B = A / combo
                        b = a / BigInteger.Pow(2, BigIntToInt(GetCombo(program[instruction + 1])));
                        instruction += 2;
                        break;

                    case 7:
                        // cdv Division: C = A / combo
                        c = a / BigInteger.Pow(2, BigIntToInt(GetCombo(program[instruction + 1])));
                        instruction += 2;
                        break;
                }
            }

            return [.. output];
        }

        string ComputerOutput(int[] ints) => string.Join(",", ints);

        string ComputerOutput(BigInteger[] ints) => string.Join(",", ints);

        protected override string? SolvePartOne()
        {
            // Time: 00:00:00.0019541
            return ComputerOutput(RunComputer());
        }

        protected override string? SolvePartTwo()
        {
            ResetComputer();

            // Save these values so we don't re-parse each time
            var tB = b;
            var tC = c;

            BigInteger cycle = 1;
            BigInteger foundA = 0;

            List<BigInteger> cycles = [];

            // We need to find a value for Register A that outputs "program"
            // Brute force?
            // Brute force doesn't work of course. Trying to identify a cycle.
            for (BigInteger loopA = 0; ; loopA += cycle)
            {
                // Reset quicker than parsing
                a = loopA;
                b = tB;
                c = tC;
                instruction = 0;

                var output = RunComputer(true);

                if (output.Length > 0)
                {
                    if (output.Length == program.Length)
                        if (ComputerOutput(output) == ComputerOutput(program))
                            return loopA.ToString();

                    // If the first program output is the same, see if we have a cycle
                    // Tried with 1, 3, and 5 outputs to find the right cycle
                    if (output.Length >= 7 && cycle == 1)
                    {
                        if (
                            output[0] == program[0]
                            &&
                            output[1] == program[1]
                            &&
                            output[2] == program[2]
                            &&
                            output[3] == program[3]
                            &&
                            output[4] == program[4]
                            &&
                            output[5] == program[5]
                            &&
                            output[6] == program[6]
                        )
                        {
                            if (foundA > 0)
                            {
                                cycles.Add(loopA - cycles[^1]);
                            }
                            else
                            {
                                cycles.Add(loopA);
                            }

                            // If we have found enough cycles, lets get the LCM of the list
                            // Because otherwise we might miss the correct cycle
                            if (cycles.Count == 3)
                            {
                                cycle = (int)Utilities.FindLCM([.. cycles.Select(c => double.Parse(c.ToString()))]);
                            }
                        }
                    }
                }
            }

            return string.Empty;
        }
    }
}

