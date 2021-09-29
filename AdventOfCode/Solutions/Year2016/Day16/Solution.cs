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

        protected override string SolvePartOne()
        {
            return Checksum(Input, 272);
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
