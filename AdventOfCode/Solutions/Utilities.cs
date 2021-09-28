/**
 * This utility class is largely based on:
 * https://github.com/jeroenheijmans/advent-of-code-2018/blob/master/AdventOfCode2018/Util.cs
 */

using System;
using System.Collections.Generic;
using System.Linq;

// Text Encoding
using System.Text;

// MD5
using System.Security.Cryptography;

namespace AdventOfCode.Solutions
{

    public static class Utilities
    {
        // https://stackoverflow.com/a/3150821/419956 by @RonWarholic
        public static IEnumerable<T> Flatten<T>(this T[,] map)
        {
            for(int row = 0; row < map.GetLength(0); row++)
            {
                for(int col = 0; col < map.GetLength(1); col++)
                {
                    yield return map[row, col];
                }
            }
        }

        /// <summary>
        /// Get the Manhatten Distance between two (x, y) points
        /// </summary>
        /// <param name="a">A tuple of integer (x, y) values</param>
        /// <param name="b">A tuple of integer (x, y) values</param>
        /// <returns>An <see cref="System.Int32"/> distance</returns>
        public static int ManhattanDistance((int x, int y) a, (int x, int y) b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        }

        /// <summary>
        /// Get the Manhatten Distance between two (x, y, z) points
        /// </summary>
        /// <param name="a">A tuple of integer (x, y, z) values</param>
        /// <param name="b">A tuple of integer (x, y, z) values</param>
        /// <returns>An <see cref="System.Int32"/> distance</returns>
        public static int ManhattanDistance((int x, int y, int z) a, (int x, int y, int z) b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z);
        }

        /// <summary>
        /// Finds the Greatest Common Denominator between two numbers
        /// </summary>
        /// <param name="a">The first number</param>
        /// <param name="b">The second number</param>
        /// <returns>The discovered GCD</returns>
        public static double FindGCD(double a, double b) => (a % b == 0) ? b : FindGCD(b, a % b);

        /// <summary>
        /// Finds the Lowest Common Multiple between two numbers
        /// </summary>
        /// <param name="a">The first number</param>
        /// <param name="b">The second number</param>
        /// <returns>The discovered LCM</returns>
        public static double FindLCM(double a, double b) => a * b / FindGCD(a, b);

        /// <summary>
        /// Repeats a given <paramref name="action"/> <paramref name="count"/> times
        /// </summary>
        /// <param name="action">The action to repeat</param>
        /// <param name="count">The number of times to repeat the given <paramref name="action"/></param>
        public static void Repeat(this Action action, int count)
        {
            for(int i = 0; i < count; i++) action();
        }

        #nullable enable
        /// <summary>
        /// Hashing a string with MD5
        /// </summary>
        /// <param name="input">The string to generate a MD5 hash for.</param>
        /// <param name="encoding">A reference to a <see cref="System.Text.Encoding"/> class. Default: <see cref="System.Text.Encoding.ASCII"/></param>
        /// <returns>A lowercase MD5 hash</returns>
        public static string MD5HashString(string input, Encoding? encoding = null)
        {
            // Our default is ASCII
            if (encoding == null)
                encoding = Encoding.ASCII;

            return string.Join("", MD5.HashData(encoding.GetBytes(input)).SelectMany(a => a.ToString("X2").ToLowerInvariant()));
        }
        #nullable restore
    }
}
