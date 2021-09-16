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

        public static int ManhattanDistance((int x, int y) a, (int x, int y) b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        }

        public static int ManhattanDistance((int x, int y, int z) a, (int x, int y, int z) b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z);
        }

        public static double FindGCD(double a, double b) => (a % b == 0) ? b : FindGCD(b, a % b);

        public static double FindLCM(double a, double b) => a * b / FindGCD(a, b);

        public static void Repeat(this Action action, int count)
        {
            for(int i = 0; i < count; i++) action();
        }

        // Based on: https://stackoverflow.com/a/45508660 and https://stackoverflow.com/a/60022011
        public static T[] GetDigits<T>(T source) where T : unmanaged, IComparable, IEquatable<T>
        {
            Stack<T> digits = new Stack<T>();

            // Verify we have a type we can work with
            if (
                typeof(T) != typeof(Int16) &&
                typeof(T) != typeof(Int32) &&
                typeof(T) != typeof(Int64) &&
                typeof(T) != typeof(UInt16) &&
                typeof(T) != typeof(UInt32) &&
                typeof(T) != typeof(UInt64)
            )
                throw new ArgumentException($"Invalid type '{typeof(T).Name}' provided, expected int or uint.", paramName: nameof(source));

            // We use this to compare objects
            T? compare = null;

            switch(typeof(T).Name)
            {
                case "Int16":
                    compare = (T)(object)(Int16)0;
                    break;
                    
                case "Int32":
                    compare = (T)(object)(Int32)0;
                    break;
                    
                case "Int64":
                    compare = (T)(object)(Int64)0;
                    break;

                case "UInt16":
                    compare = (T)(object)(UInt16)0;
                    break;

                case "UInt32":
                    compare = (T)(object)(UInt32)0;
                    break;

                case "UInt64":
                    compare = (T)(object)(UInt64)0;
                    break;
            }

            // CompareTo == 0 means they're equal, < 0 means the source is negative
            while (compare.HasValue && source.CompareTo(compare.Value) > 0)
            {
                T? digit = null;

                switch(typeof(T).Name)
                {
                    case "Int16":
                        digit = (T)(object)((Int16)(object)source % 10);
                        source = (T)(object)((Int16)(object)source / 10);
                        break;
                        
                    case "Int32":
                        digit = (T)(object)((Int32)(object)source % 10);
                        source = (T)(object)((Int32)(object)source / 10);
                        break;
                        
                    case "Int64":
                        digit = (T)(object)((Int64)(object)source % 10);
                        source = (T)(object)((Int64)(object)source / 10);
                        break;

                    case "UInt16":
                        digit = (T)(object)((UInt16)(object)source % 10);
                        source = (T)(object)((UInt16)(object)source / 10);
                        break;

                    case "UInt32":
                        digit = (T)(object)((UInt32)(object)source % 10);
                        source = (T)(object)((UInt32)(object)source / 10);
                        break;

                    case "UInt64":
                        digit = (T)(object)((UInt64)(object)source % 10);
                        source = (T)(object)((UInt64)(object)source / 10);
                        break;
                }

                if (digit.HasValue)
                    digits.Push(digit.Value);
            }

            return digits.ToArray();
        }

        public static (int, int) Add(this (int x, int y) a, (int x, int y) b) => (a.x + b.x, a.y + b.y);
    }
}