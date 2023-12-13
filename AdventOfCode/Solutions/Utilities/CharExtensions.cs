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

        /// <summary>
        /// Returns the character from the same index of each char array, emulating a column select
        /// </summary>
        /// <param name="chars">Char arrays to source from</param>
        /// <param name="column">The index for the character for each row</param>
        /// <returns>The column returned</returns>
        public static char[] GetColumn(this char[][] chars, int column)
        {
            ArgumentNullException.ThrowIfNull(chars);
            ArgumentNullException.ThrowIfNull(column);
            ArgumentOutOfRangeException.ThrowIfLessThan(column, 0, nameof(column));

            var ret = new List<char>();

            for (int i = 0; i < chars.Length; i++)
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(column, chars[i].Length, nameof(column));
                ret.Add(chars[i][column]);
            }

            return ret.ToArray();
        }

        /// <summary>
        /// Transposes rows of chars into the equivalent columns
        /// </summary>
        /// <param name="chars">Char arrays to source from</param>
        /// <returns>The columns returned</returns>
        public static char[][] GetColumns(this char[][] chars)
        {
            ArgumentNullException.ThrowIfNull(chars);

            var length = chars[0].Length;
            if (!chars.All(s => s.Length == length))
                throw new ArgumentException(message: "The char arrays are not of the same length.", nameof(chars));

            // Seed a list with the first character of each
            var ret = chars[0].Select(c => new List<char>(c)).ToList();

            for (int i = 1; i < chars.Length; i++)
                for (int c = 0; c < length; c++)
                    ret[c].Add(chars[i][c]);

            return ret.Select(col => col.ToArray()).ToArray();
        }
    }
}