using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2015
{

    class Day10 : ASolution
    {
        private ulong inVal = 0;

        public Day10() : base(10, 2015, "")
        {
            inVal = ulong.Parse(Input.Trim());
        }

        private ulong[] GenerateNextNumber(ulong[] digits)
        {
            var newDigits = new List<ulong>();

            // Go through and generate our new digits based on the count of them in a row
            /*            
            1 becomes 11 (1 copy of digit 1).
            11 becomes 21 (2 copies of digit 1).
            21 becomes 1211 (one 2 followed by one 1).
            1211 becomes 111221 (one 1, one 2, and two 1s).
            111221 becomes 312211 (three 1s, two 2s, and one 1).
            */
            for (var i = 0; i < digits.Length; i++)
            {
                // Start with the digit we are on
                ulong count = 1;

                int j = 1;

                while(i+j < digits.Length && digits[i+j] == digits[i])
                {
                    // Found another
                    count++;
                    j++;
                }

                // We now have our digits
                newDigits.Add(count);
                newDigits.Add(digits[i]);

                // Skip ahead if required
                i += j - 1;
            }

            return newDigits.ToArray();
        }

        protected override string SolvePartOne()
        {
            // Starting with the digits in your puzzle input, apply this process 40 times. What is the length of the result?
            ulong[] result = Utilities.GetDigits(inVal);
            for (int i = 0; i < 40; i++)
                result = GenerateNextNumber(result);

            return result.Length.ToString();
        }

        protected override string SolvePartTwo()
        {
            // Starting with the digits in your puzzle input, apply this process 40 times. What is the length of the result?
            ulong[] result = Utilities.GetDigits(inVal);
            for (int i = 0; i < 50; i++)
                result = GenerateNextNumber(result);

            return result.Length.ToString();
        }
    }
}
