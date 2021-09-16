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
    }
}
