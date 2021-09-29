using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{

    class Day16 : ASolution
    {

        public Day16() : base(16, 2016, "")
        {
            var a = "1";
            var b = DragonCurve(a);
            Console.WriteLine($"Test Curve: Input: '{a}', Output: {b}");
            
            a = "0";
            b = DragonCurve(a);
            Console.WriteLine($"Test Curve: Input: '{a}', Output: {b}");
            
            a = "11111";
            b = DragonCurve(a);
            Console.WriteLine($"Test Curve: Input: '{a}', Output: {b}");
            
            a = "111100001010";
            b = DragonCurve(a);
            Console.WriteLine($"Test Curve: Input: '{a}', Output: {b}");

            Console.WriteLine($"Test Checksum: Data: '110010110100', Length: 12, Checksum: {Checksum("110010110100", 12)}");

            Console.WriteLine($"Test Checksum: Data: '10000', Length: 20, Checksum: {Checksum("10000", 20)}");
        }

        private string DragonCurve(string input)
        {
            var b = input.Reverse().Select(c => c == '0' ? '1' : '0').JoinAsString();

            return input + '0' + b;
        }

        private string Checksum(string input, int length)
        {
            string checksum = string.Empty;

            // Extend strings that are too short
            while(input.Length < length)
            {
                input = DragonCurve(input);
            }

            do
            {
                checksum = string.Empty;

                // Trim the string down to our maximum length if needed
                input = input.Substring(0, Math.Min(length, input.Length));

                for (int i = 0; i < input.Length - 1; i += 2)
                {
                    // Get our pair
                    string t = $"{input[i]}{input[i + 1]}";

                    if (t == "00" || t == "11")
                    {
                        checksum += "1";
                    }
                    else
                    {
                        checksum += "0";
                    }
                }

                // For the next loop:
                input = checksum;
            } while (checksum.Length % 2 == 0);

            return checksum;
        }

        private string Solve(ulong input_parity, ulong disk_size, uint inputLength)
        {
            ulong increment = FindLowest1(disk_size);
            ulong previous_parity = 0;

            string checksum = string.Empty;

            for (ulong length = increment; length <= disk_size; length += increment) {
                // number of dragon bits
                ulong dragons = length / (inputLength + 1);
                // number of complete cycles (forward and reverse) of the input
                ulong input_cycles    = (length - dragons) / (inputLength * 2);
                // remainder of input bits
                int input_remainder = (int)((length - dragons) % (inputLength * 2));

                // parity of the dragon bits
                ulong p = DragonParity(dragons);
                // plus parity of all complete input cycles
                p ^= input_cycles & inputLength;
                // plus parity of the remainder
                p ^= input_parity >> input_remainder;
                // only need the least significant bit
                p &= 1;

                // checksum digit is the inverted parity bit,
                // XOR with the previous parity calculation
                checksum += (p ^ previous_parity) == 0 ? '1' : '0';

                previous_parity = p;
            }

            return checksum;
        }

        // https://docs.microsoft.com/en-us/cpp/cpp/unary-plus-and-negation-operators-plus-and?view=msvc-160
        // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#unary-minus-operator
        // ulong is not supported natively
        // Unary operator -n => 2^b - n where b is the number of bits in the number
        private ulong FindLowest1(ulong n) => n & (((uint)Math.Pow((uint)2, (uint)64)) - n);

        private ulong DragonParity(ulong n)
        {
            ulong gray = n ^ (n >> 1);
            return (gray ^ CountBits(n & gray)) & 1;
        }

        private uint CountBits(ulong input)
        {
            // https://stackoverflow.com/a/9950662
            // Replacement for __builtin_popcountl
            uint count = 0;
            for (int i = 0; i < sizeof(ulong) * 8; i++)
            {
                count += (uint)((input >> i) & 0x01);
            }

            return count;
        }

        private ulong InputParity(string input)
        {
            ulong input_parity = 0;
            ulong parity = 0;

            // Process the input

            // find input parity forward
            for (int i = 0; i < input.Length; i++) {
                parity ^= (ulong) (input[i] == '1' ? 1 : 0);
                input_parity ^= parity << (i + 1);
            }
            // ...and reversed complement
            for (int i = 1; i <= input.Length; i++) {
                parity ^= (ulong)  (input[input.Length - i] != '1' ? 1 : 0);
                input_parity ^= parity << (input.Length + i);
            }

            return input_parity;
        }

        protected override string SolvePartOne()
        {
            return Checksum(Input, 272);
        }

        protected override string SolvePartTwo()
        {
            // This is an algorithm that will scale badly
            // The point is to reduce the math down to something predictable because it's easily repeated
            // Brute force this requires over 17 million dragon calculations
            // Went to the mega thread because I'm not good at finding optimizations
            // https://old.reddit.com/r/adventofcode/comments/5imh3d/2016_day_16_solutions/db9erfp/
            // https://www.reddit.com/r/adventofcode/comments/5ititq/2016_day_16_c_how_to_tame_your_dragon_in_under_a/
            /*
             * If a is Input and b is Input inversed and reversed then:
             * a
             * a 0 b
             * a 0 b 0 a 1 b
             * a 0 b 0 a 1 b 0 a 0 b 1 a 1 b
             * a 0 b 0 a 1 b 0 a 0 b 1 a 1 b 0 a 0 b 0 a 1 b 1 a 0 b 1 a 1 b
             */
            // https://gist.github.com/Voltara/b379ff6f04c39c9f9860542054f90555

            return Solve(InputParity(Input), 35651584, (uint) Input.Length).ToString();
        }
    }
}
