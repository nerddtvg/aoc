/**
 * This utility class is largely based on:
 * https://github.com/jeroenheijmans/advent-of-code-2018/blob/master/AdventOfCode2018/Util.cs
 */

using System;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Add two (x, y) points together
        /// </summary>
        /// <param name="a">A tuple of integer (x, y) values</param>
        /// <param name="b">A tuple of integer (x, y) values</param>
        /// <returns>A tuple of integer (x, y) values</returns>
        public static (int, int) Add(this (int x, int y) a, (int x, int y) b) => (a.x + b.x, a.y + b.y);

        /// <summary>
        /// Returns all points between <paramref name="a"/> and <paramref name="b"/> inclusive of the endpoints
        /// </summary>
        /// <param name="a">A tuple of integer (x, y) values</param>
        /// <param name="b">A tuple of integer (x, y) values</param>
        /// <returns>An array of tuple of integer (x, y) values</returns>
        public static (int, int)[] GetPointsBetweenInclusive(this (int x, int y) a, (int x, int y) b)
        {
            // Return object
            List<(int x, int y)> ret = new List<(int x, int y)>();

            // Determine our comparison and increment functions
            Func<int, bool> fx = x => x <= b.x;
            Func<int, int> incx = x => x + 1;
            Func<int, bool> fy = y => y <= b.y;
            Func<int, int> incy = y => y + 1;

            // Less than, we're moving backwards
            if (a.x > b.x)
            {
                fx = x => x >= b.x;
                incx = x => x - 1;
            }
            
            if (a.y > b.y)
            {
                fy = y => y >= b.y;
                incy = y => y - 1;
            }

            // Generate the list
            for (int x = a.x; fx(x); x = incx(x))
            {
                for (int y = a.y; fy(y); y = incy(y))
                {
                    ret.Add((x, y));
                }
            }

            // Return the array
            return ret.ToArray();
        }

        /// <summary>
        /// Returns all points between <paramref name="a"/> and <paramref name="b"/> exclusive
        /// </summary>
        /// <param name="a">A tuple of integer (x, y) values</param>
        /// <param name="b">A tuple of integer (x, y) values</param>
        /// <returns>An array of tuple of integer (x, y) values</returns>
        public static (int, int)[] GetPointsBetween(this (int x, int y) a, (int x, int y) b)
        {
            // Start with our base array
            var ret = a.GetPointsBetweenInclusive(b);

            // Return the array
            return ret.Skip(1).Take(ret.Length-2).ToArray();
        }
    }
}