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
        // Based on: https://docs.python.org/3/library/itertools.html#itertools.combinations
        // Get all possible combinations of the input list
        public static IEnumerable<List<T>> GetAllCombos<T>(List<T> list, int r = 2)
        {
            if (r <= 1) {
                throw new Exception($"Invalid r value specified: {r}");
            }

            // Generate a list of all indicies repetatively
            List<int> indices = Enumerable.Range(0, r).ToList();

            // Length of input
            int n = list.Count;

            if (r > n) {
                throw new Exception($"Unable to provide combinations of length {r} for a list of length {n}. List length must be longer.");
            }

            // Add the first result in the default order of 0, 1, ... r-1
            List<T> tempList = new List<T>();
            indices.ForEach(a => tempList.Add(list[a]));
            yield return tempList;

            // Loop through and get the rest
            while(true) {
                bool broke = false;
                int i2 = -1;

                foreach(int i in Enumerable.Range(0, r).Reverse().ToList()) {
                    // Save for later (since we can't reference the foreach variable outside the loop)
                    i2 = i;

                    if (indices[i] != i + n - r) {
                        // Recreating the for/else loop in Python
                        broke = true;
                        break;
                    }
                }

                // Handle the processing
                // This is done outside the loop so we restart the while() loop each time
                if (broke) {
                    indices[i2]++;

                    foreach(int j in Enumerable.Range(i2+1, r-i2-1)) {
                        indices[j] = indices[j-1] + 1;
                    }

                    // Now return what we know
                    tempList = new List<T>();
                    foreach(int j in indices)
                        tempList.Add(list[j]);
                    
                    yield return tempList;
                } else {
                    // We've hit the end
                    yield break;
                }
            }
        }

        public static int[] ToIntArray(this string str, string delimiter = "")
        {
            if(delimiter == "")
            {
                var result = new List<int>();
                foreach(char c in str) if(int.TryParse(c.ToString(), out int n)) result.Add(n);
                return result.ToArray();
            }
            else if (delimiter == "\n")
            {
                var result = new List<int>();
                foreach(string c in str.SplitByNewline()) if(int.TryParse(c, out int n)) result.Add(n);
                return result.ToArray();
            }
            else
            {
                return str
                    .Split(delimiter)
                    .Where(n => int.TryParse(n, out int v))
                    .Select(n => Convert.ToInt32(n))
                    .ToArray();
            }

        }
        

        public static long[] ToLongArray(this string str, string delimiter = "")
        {
            if(delimiter == "")
            {
                var result = new List<long>();
                foreach(char c in str) if(long.TryParse(c.ToString(), out long n)) result.Add(n);
                return result.ToArray();
            }
            else if (delimiter == "\n")
            {
                var result = new List<long>();
                foreach(string c in str.SplitByNewline()) if(long.TryParse(c, out long n)) result.Add(n);
                return result.ToArray();
            }
            else
            {
                return str
                    .Split(delimiter)
                    .Where(n => long.TryParse(n, out long v))
                    .Select(n => Convert.ToInt64(n))
                    .ToArray();
            }

        }


        public static int MinOfMany(params int[] items)
        {
            var result = items[0];
            for(int i = 1; i < items.Length; i++)
            {
                result = Math.Min(result, items[i]);
            }
            return result;
        }

        public static int MaxOfMany(params int[] items)
        {
            var result = items[0];
            for(int i = 1; i < items.Length; i++)
            {
                result = Math.Max(result, items[i]);
            }
            return result;
        }

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

        public static string JoinAsString<T>(this IEnumerable<T> items)
        {
            return string.Join("", items);
        }

        public static string[][] SplitByBlankLine(this string input, bool shouldTrim = false)
        {
            return input
                .Split(new[] { "\r\r", "\n\n", "\r\n\r\n" }, StringSplitOptions.None)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.SplitByNewline(shouldTrim, true))
                .ToArray();
        }

        public static string[] SplitByNewline(this string input, bool shouldTrim = false, bool shouldExcludeEmpty = true)
        {
            return input
                .Split(new[] { "\r", "\n", "\r\n" }, StringSplitOptions.None)
                .Where(s => (!shouldExcludeEmpty || !string.IsNullOrWhiteSpace(s)))
                .Select(s => shouldTrim ? s.Trim() : s)
                .ToArray();
        }

        public static string Reverse(this string str)
        {
            char[] arr = str.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
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

        // https://github.com/tslater2006/AdventOfCode2019
        public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> values)
        {
            return (values.Count() == 1) ? new[] { values } : values.SelectMany(v => Permutations(values.Where(x => x.Equals(v) == false)), (v, p) => p.Prepend(v));
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> array, int size)
        {
            for(var i = 0; i < (float)array.Count() / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

        // https://stackoverflow.com/questions/49190830/is-it-possible-for-string-split-to-return-tuple
        public static void Deconstruct<T>(this IList<T> list, out T first, out IList<T> rest)
        {
            first = list.Count > 0 ? list[0] : default(T); // or throw
            rest = list.Skip(1).ToList();
        }

        public static void Deconstruct<T>(this IList<T> list, out T first, out T second, out IList<T> rest)
        {
            first = list.Count > 0 ? list[0] : default(T); // or throw
            second = list.Count > 1 ? list[1] : default(T); // or throw
            rest = list.Skip(2).ToList();
        }

        public static (int, int) Add(this (int x, int y) a, (int x, int y) b) => (a.x + b.x, a.y + b.y);
    }
}