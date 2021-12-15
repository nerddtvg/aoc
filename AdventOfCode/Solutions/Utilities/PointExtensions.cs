using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Solutions
{
    public static class PointExtensions
    {
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
        /// <param name="IsLine">If <c>true</c>, treat the points as a line instead of an area. Default: <c>false</c></param>
        /// <returns>An array of tuple of integer (x, y) values</returns>
        public static (int, int)[] GetPointsBetweenInclusive(this (int x, int y) a, (int x, int y) b, bool IsLine = false)
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

            if (!IsLine)
            {
                // Generate the list for an Area
                for (int y = a.y; fy(y); y = incy(y))
                {
                    for (int x = a.x; fx(x); x = incx(x))
                    {
                        ret.Add((x, y));
                    }
                }
            }
            else
            {
                // Override some functions special for lines
                // The reason we exclude when both endpoints are equal
                // is because then the function will go into an infinite loop
                if (a.x != b.x && a.y == b.y)
                {
                    // Disable incrementing y
                    incy = y => y;
                }
                else if (a.x == b.x && a.y != b.y)
                {
                    // Disable incrementing x
                    incx = x => x;
                }

                // Generate the list for a Line
                for (int y = a.y, x = a.x; fx(x) && fy(y); x = incx(x), y = incy(y))
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
        /// <param name="IsLine">If <c>true</c>, treat the points as a line instead of an area. Default: <c>false</c></param>
        /// <returns>An array of tuple of integer (x, y) values</returns>
        public static (int, int)[] GetPointsBetween(this (int x, int y) a, (int x, int y) b, bool IsLine = false)
        {
            // Start with our base array
            var ret = a.GetPointsBetweenInclusive(b, IsLine);

            // Return the array
            return ret.Skip(1).Take(ret.Length-2).ToArray();
        }

        /// <summary>
        /// Get the Manhatten Distance between two (x, y) points
        /// </summary>
        /// <param name="a">A tuple of integer (x, y) values</param>
        /// <param name="b">A tuple of integer (x, y) values</param>
        /// <returns>An <see cref="System.Int32"/> distance</returns>
        public static uint ManhattanDistance(this (int x, int y) a, (int x, int y) b)
        {
            return (uint)Math.Abs(a.x - b.x) + (uint)Math.Abs(a.y - b.y);
        }

        /// <summary>
        /// Get the Manhatten Distance between two (x, y, z) points
        /// </summary>
        /// <param name="a">A tuple of integer (x, y, z) values</param>
        /// <param name="b">A tuple of integer (x, y, z) values</param>
        /// <returns>An <see cref="System.Int32"/> distance</returns>
        public static uint ManhattanDistance(this (int x, int y, int z) a, (int x, int y, int z) b)
        {
            return (uint)Math.Abs(a.x - b.x) + (uint)Math.Abs(a.y - b.y) + (uint)Math.Abs(a.z - b.z);
        }

        /// <summary>
        /// Get the Manhatten Distance between two (x, y) points
        /// </summary>
        /// <param name="a">A tuple of integer (x, y) values</param>
        /// <param name="b">A tuple of integer (x, y) values</param>
        /// <returns>An <see cref="System.Int32"/> distance</returns>
        public static UInt64 ManhattanDistance(this (Int64 x, Int64 y) a, (Int64 x, Int64 y) b)
        {
            return (UInt64)Math.Abs(a.x - b.x) + (UInt64)Math.Abs(a.y - b.y);
        }

        /// <summary>
        /// Get the Manhatten Distance between two (x, y, z) points
        /// </summary>
        /// <param name="a">A tuple of integer (x, y, z) values</param>
        /// <param name="b">A tuple of integer (x, y, z) values</param>
        /// <returns>An <see cref="System.Int32"/> distance</returns>
        public static UInt64 ManhattanDistance(this (Int64 x, Int64 y, Int64 z) a, (Int64 x, Int64 y, Int64 z) b)
        {
            return (UInt64)Math.Abs(a.x - b.x) + (UInt64)Math.Abs(a.y - b.y) + (uint)Math.Abs(a.z - b.z);
        }
    }
}