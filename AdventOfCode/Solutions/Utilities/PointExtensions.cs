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
        /// <returns>An array of tuple of integer (x, y) values</returns>
        public static (int, int)[] GetPointsBetweenInclusive(this (int x, int y) a, (int x, int y) b, bool IsArea = true)
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

            if (IsArea)
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
        /// <returns>An array of tuple of integer (x, y) values</returns>
        public static (int, int)[] GetPointsBetween(this (int x, int y) a, (int x, int y) b, bool IsArea = true)
        {
            // Start with our base array
            var ret = a.GetPointsBetweenInclusive(b);

            // Return the array
            return ret.Skip(1).Take(ret.Length-2).ToArray();
        }

        /// <summary>
        /// Get the Manhatten Distance between two (x, y) points
        /// </summary>
        /// <param name="a">A tuple of integer (x, y) values</param>
        /// <param name="b">A tuple of integer (x, y) values</param>
        /// <returns>An <see cref="System.Int32"/> distance</returns>
        public static int ManhattanDistance(this (int x, int y) a, (int x, int y) b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        }

        /// <summary>
        /// Get the Manhatten Distance between two (x, y, z) points
        /// </summary>
        /// <param name="a">A tuple of integer (x, y, z) values</param>
        /// <param name="b">A tuple of integer (x, y, z) values</param>
        /// <returns>An <see cref="System.Int32"/> distance</returns>
        public static int ManhattanDistance(this (int x, int y, int z) a, (int x, int y, int z) b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z);
        }
    }
}