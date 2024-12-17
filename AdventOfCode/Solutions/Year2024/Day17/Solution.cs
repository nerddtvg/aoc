using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    partial class Day17 : ASolution
    {
        public required int a;
        public required int b;
        public required int c;
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

            a = int.Parse(matches.Groups["A"].Value);
            b = int.Parse(matches.Groups["B"].Value);
            c = int.Parse(matches.Groups["C"].Value);
            program = matches.Groups["Program"].Value.ToIntArray(",");

            instruction = 0;
        }

        int GetCombo(int operand) => operand switch
        {
            0 or 1 or 2 or 3 => operand,
            4 => a,
            5 => b,
            6 => c,
            _ => throw new Exception("Invalid operand"),
        };

        string RunComputer()
        {
            List<int> output = [];

            while (0 <= instruction && instruction < program.Length - 1)
            {
                switch (program[instruction])
                {
                    case 0:
                        // adv Division: A / combo
                        a /= (int)Math.Pow(2, GetCombo(program[instruction + 1]));
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
                        instruction += 2;
                        break;

                    case 6:
                        // bdv Division: B = A / combo
                        b = a / (int)Math.Pow(2, GetCombo(program[instruction + 1]));
                        instruction += 2;
                        break;

                    case 7:
                        // cdv Division: C = A / combo
                        c = a / (int)Math.Pow(2, GetCombo(program[instruction + 1]));
                        instruction += 2;
                        break;
                }
            }

            return string.Join(",", output);
        }

        protected override string? SolvePartOne()
        {
            // Time: 00:00:00.0019541
            return RunComputer();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

