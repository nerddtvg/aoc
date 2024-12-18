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
        public required int[] programReversed = [];
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
            instruction = 0;
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

                        // For part 2, return our first output
                        if (part2)
                        {
                            return [output[^1]];
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

        BigInteger FindValue(BigInteger loopA, int index)
        {
            // We have found the end, return the value
            if (index == program.Length)
                return loopA;

            // The code uses one byte at a time, does some modulo and XOR
            // operations to print out a specific number. However, no matter
            // what the rest of the number is, only that byte matters for
            // each output. So we will test 0 to 7 and continue to shift
            // left each time to determine the next step in the list.
            for (BigInteger newLoopA = 0; newLoopA < 8; newLoopA++)
            {
                // Get the value of running this newLoopA
                // Multiple loopA by 8 because we need to shift over 1 byte
                var tA = loopA * 8 + newLoopA;
                a = tA;
                b = 0;
                c = 0;

                var output = RunComputer(true);

                // The highest bits define the last numbers in program,
                // so we must reverse the array to get the ordering for discovery
                if (output.Length > 0 && output[0] == programReversed[index])
                {
                    // Found the value, get the next one...
                    var result = FindValue(tA, index + 1);

                    if (result > BigInteger.Zero)
                        return result;
                }
            }

            // Didn't find a result
            return BigInteger.Zero;
        }

        protected override string? SolvePartOne()
        {
            // Time: 00:00:00.0019541
            return ComputerOutput(RunComputer());
        }

        protected override string? SolvePartTwo()
        {
            ResetComputer();

            // Taking a hint from the subreddit to work backwards
            // That only 8 bits are required to determine the output value
            // So find the value that makes it work and keep adding more digits on
            // https://old.reddit.com/r/adventofcode/comments/1hg38ah/2024_day_17_solutions/m2glx6y/
            // Thank you to /u/i_have_no_biscuits for that comment and code sample
            programReversed = program.Reverse().ToArray();
            return FindValue(0, 0).ToString();
        }
    }
}

