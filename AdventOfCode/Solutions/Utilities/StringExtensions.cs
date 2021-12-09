/**
 * This utility class is largely based on:
 * https://github.com/jeroenheijmans/advent-of-code-2018/blob/master/AdventOfCode2018/Util.cs
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Solutions
{
    public static partial class StringExtensions
    {
        /// <summary>
        /// Convert a string into an array of integers.
        /// 
        /// Samples:
        /// 12345           => 1,2,3,4,5 (delimiter: empty)
        /// 1,2,3,4,5       => 1,2,3,4,5 (delimiter: ,)
        /// 1\n2\n3\n4\n5   => 1,2,3,4,5 (delimiter: \n new line)
        /// </summary>
        /// <param name="str">The input string</param>
        /// <param name="delimiter">A string delimiter (Default: <see cref="System.String.Empty"/></param>
        /// <returns><see cref="System.Int32[]"/></returns>
        public static int[] ToIntArray(this string str, string delimiter = "")
        {
            // Return the parsing
            return str.CustomSplit(delimiter)
                .Where(s => int.TryParse(s, out int n))
                .Select(s => int.Parse(s))
                .ToArray();
        }

        /// <summary>
        /// Convert a string into an array of long.
        /// 
        /// Samples:
        /// 12345           => 1,2,3,4,5 (delimiter: empty)
        /// 1,2,3,4,5       => 1,2,3,4,5 (delimiter: ,)
        /// 1\n2\n3\n4\n5   => 1,2,3,4,5 (delimiter: \n new line)
        /// </summary>
        /// <param name="str">The input string</param>
        /// <param name="delimiter">A string delimiter (Default: <see cref="System.String.Empty"/></param>
        /// <returns><see cref="System.Int64[]"/></returns>
        public static long[] ToLongArray(this string str, string delimiter = "")
        {
            // Return the parsing
            return str.CustomSplit(delimiter)
                .Where(s => long.TryParse(s, out long n))
                .Select(s => long.Parse(s))
                .ToArray();
        }

        /// <summary>
        /// Implements a custom split operation based on our utility code in <see cref="StringExtensions.ToIntArray(string, string)"/>
        /// </summary>
        /// <param name="str">The input string</param>
        /// <param name="delimiter">A string delimiter (Default: <see cref="System.String.Empty"/></param>
        /// <returns>A <see cref="System.String[]"/></returns>
        public static string[] CustomSplit(this string str, string delimiter = "")
        {
            return 
                string.IsNullOrEmpty(delimiter)
                ?
                // Split by character
                str.ToCharArray().Select(c => c.ToString()).ToArray()
                :
                (
                    string.Equals(delimiter, "\n", StringComparison.InvariantCultureIgnoreCase)
                    ?
                    // Split by new line
                    str.SplitByNewline()
                    :
                    // Split by the built-in Split() utility
                    str.Split(delimiter)
                );
        }

        /// <summary>
        /// Splits an input string into chunks of <paramref name="length"/>. If it is uneven, the last chunk may not be the correct length.
        /// </summary>
        /// <param name="str">The input string</param>
        /// <param name="length">The maximum chunk size.</param>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable{T}"/> of <see cref="System.String"/> chunks</returns>
        public static IEnumerable<string> SplitBySize(this string str, int length)
        {
            // Handles uneven strings in case they happen
            return Enumerable.Range(0, str.Length / length)
                .Select(i => str.Length < (i + 1 * length) ? str.Substring(i * length) : str.Substring(i * length, length));
        }

        /// <summary>
        /// A shortcut to use <see cref="System.String.Join(string?, object?[])"/> with no joining character or string
        /// </summary>
        /// <param name="items">The enumerable list of items</param>
        /// <returns>A combined <see cref="System.String"/></returns>
        public static string JoinAsString<T>(this IEnumerable<T> items) =>
            string.Join("", items);

        /// <summary>
        /// Split a multi-line input string into groups based on empty lines.
        /// </summary>
        /// <param name="input">A multi-line string</param>
        /// <param name="shouldTrim">Should each line be trimed? (Default: <c>false</c>)</param>
        /// <returns>Groups of individual lines</returns>
        public static string[][] SplitByBlankLine(this string input, bool shouldTrim = false) =>
            input
                .Split(new[] { "\r\r", "\n\n", "\r\n\r\n" }, StringSplitOptions.None)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.SplitByNewline(shouldTrim, true))
                .ToArray();

        /// <summary>
        /// Split a multi-line input string into individual lines.
        /// </summary>
        /// <param name="input">A multi-line string</param>
        /// <param name="shouldTrim">Should each line be trimed? (Default: <c>false</c>)</param>
        /// <param name="shouldExcludeEmpty">Should empty lines be skipped? (Default: <c>true</c>)</param>
        /// <returns>Individual lines</returns>
        public static string[] SplitByNewline(this string input, bool shouldTrim = false, bool shouldExcludeEmpty = true) =>
            input
                .Split(new[] { "\r", "\n", "\r\n" }, StringSplitOptions.None)
                .Where(s => (!shouldExcludeEmpty || !string.IsNullOrWhiteSpace(s)))
                .Select(s => shouldTrim ? s.Trim() : s)
                .ToArray();

        /// <summary>
        /// Reverse a string quickly
        /// </summary>
        /// <param name="str">The input string</param>
        /// <returns>A reversed string</returns>
        public static string Reverse(this string str)
        {
            char[] arr = str.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        /// <summary>
        /// Caesar shift an entire string. This will only shift a-z or A-Z characters
        /// </summary>
        /// <param name="input">The string to shift</param>
        /// <param name="shift">The number of character to shift it (positive or negative)</param>
        /// <returns>A shifted string</returns>
        public static string Shift(this string str, int shift)
        {
            // Output string
            string o = string.Empty;

            // Loop each character, if a valid character to shift, do it
            foreach(var c in str)
            {
                // Make sure this is an easy ASCII alphabet character
                if (('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z'))
                {
                    o += c.Shift(shift);
                }
                else
                {
                    o += c;
                }
            }

            return o;
        }
    }
}
