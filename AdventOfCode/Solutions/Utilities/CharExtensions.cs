using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Solutions
{
    public static class CharExtensions
    {
        /// <summary>
        /// Caesar shift a character. This will only shift a-z and A-Z, otherwise it returns the original unmodified.
        /// </summary>
        /// <param name="input">The character to shift (a-z or A-Z)</param>
        /// <param name="shift">The number of character to shift it (positive or negative)</param>
        /// <returns>A shifted character</returns>
        public static char Shift(this char input, int shift)
        {
            // Make sure this is valid
            if (!(('a' <= input && input <= 'z') || ('A' <= input && input <= 'Z')))
                return input;

            // Our starting and stopping points
            char min = 'a';
            char max = 'z';

            // Maybe we have upper case characters?
            if ('A' <= input && input <= 'Z')
            {
                min = 'A';
                max = 'Z';
            }

            // Get our character (maybe)
            var o = ((int) input + shift);

            // Check if we're too high
            while(o > (int) max)
                o -= 26;

            // Check if we're too low
            // No modulus because it doesn't like negative numbers
            while(o < (int) min)
                o += 26;

            // Return the character
            return (char)o;
        }
    }
}