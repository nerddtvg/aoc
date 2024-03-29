/**
 * This utility class is largely based on:
 * https://github.com/jeroenheijmans/advent-of-code-2018/blob/master/AdventOfCode2018/Util.cs
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Solutions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Based on: https://docs.python.org/3/library/itertools.html#itertools.combinations
        /// Get all possible combinations of the input list
        /// </summary>
        /// <param name="list">The list to work with</param>
        /// <param name="r">Set sizes</param>
        public static IEnumerable<List<T>> GetAllCombos<T>(List<T> list, int r = 2)
        {
            if (r <= 1)
            {
                throw new Exception($"Invalid r value specified: {r}");
            }

            // Generate a list of all indicies repetatively
            List<int> indices = Enumerable.Range(0, r).ToList();

            // Length of input
            int n = list.Count;

            if (r > n)
            {
                throw new Exception($"Unable to provide combinations of length {r} for a list of length {n}. List length must be longer.");
            }

            // Add the first result in the default order of 0, 1, ... r-1
            List<T> tempList = new List<T>();
            indices.ForEach(a => tempList.Add(list[a]));
            yield return tempList;

            // Loop through and get the rest
            while (true)
            {
                bool broke = false;
                int i2 = -1;

                foreach (int i in Enumerable.Range(0, r).Reverse().ToList())
                {
                    // Save for later (since we can't reference the foreach variable outside the loop)
                    i2 = i;

                    if (indices[i] != i + n - r)
                    {
                        // Recreating the for/else loop in Python
                        broke = true;
                        break;
                    }
                }

                // Handle the processing
                // This is done outside the loop so we restart the while() loop each time
                if (broke)
                {
                    indices[i2]++;

                    foreach (int j in Enumerable.Range(i2 + 1, r - i2 - 1))
                    {
                        indices[j] = indices[j - 1] + 1;
                    }

                    // Now return what we know
                    tempList = new List<T>();
                    foreach (int j in indices)
                        tempList.Add(list[j]);

                    yield return tempList;
                }
                else
                {
                    // We've hit the end
                    yield break;
                }
            }
        }

        /// <summary>
        /// Based on: https://docs.python.org/3/library/itertools.html#itertools.combinations
        /// Get all possible combinations of the input list
        /// </summary>
        /// <param name="list">The list to work with</param>
        /// <param name="r">Set sizes</param>
        public static IEnumerable<IList<T>> GetAllCombos<T>(this IList<T> list, int r = 2)
        {
            if (r <= 1)
            {
                throw new Exception($"Invalid r value specified: {r}");
            }

            // Generate a list of all indicies repetatively
            List<int> indices = Enumerable.Range(0, r).ToList();

            // Length of input
            int n = list.Count;

            if (r > n)
            {
                throw new Exception($"Unable to provide combinations of length {r} for a list of length {n}. List length must be longer.");
            }

            // Add the first result in the default order of 0, 1, ... r-1
            List<T> tempList = new List<T>();
            tempList.AddRange(list.Take(r));
            yield return tempList;

            // Loop through and get the rest
            while (true)
            {
                bool broke = false;
                int i2 = -1;

                foreach (int i in Enumerable.Range(0, r).Reverse().ToList())
                {
                    // Save for later (since we can't reference the foreach variable outside the loop)
                    i2 = i;

                    if (indices[i] != i + n - r)
                    {
                        // Recreating the for/else loop in Python
                        broke = true;
                        break;
                    }
                }

                // Handle the processing
                // This is done outside the loop so we restart the while() loop each time
                if (broke)
                {
                    indices[i2]++;

                    foreach (int j in Enumerable.Range(i2 + 1, r - i2 - 1))
                    {
                        indices[j] = indices[j - 1] + 1;
                    }

                    // Now return what we know
                    tempList = new List<T>();
                    foreach (int j in indices)
                        tempList.Add(list[j]);

                    yield return tempList;
                }
                else
                {
                    // We've hit the end
                    yield break;
                }
            }
        }

        /// <summary>
        /// Based on: https://docs.python.org/3/library/itertools.html#itertools.combinations
        /// Get all possible combinations of the input enumerable (this implements Skip and Take)
        /// </summary>
        /// <param name="list">The list to work with</param>
        /// <param name="r">Set sizes</param>
        public static IEnumerable<IEnumerable<T>> GetAllCombos<T>(this IEnumerable<T> list, int r = 2)
        {
            if (r <= 1)
            {
                throw new Exception($"Invalid r value specified: {r}");
            }

            // Generate a list of all indicies repetatively
            List<int> indices = Enumerable.Range(0, r).ToList();

            // Length of input
            int n = list.Count();

            if (r > n)
            {
                throw new Exception($"Unable to provide combinations of length {r} for a list of length {n}. List length must be longer.");
            }

            // Add the first result in the default order of 0, 1, ... r-1
            List<T> tempList = new List<T>();
            tempList.AddRange(list.Take(r));
            yield return tempList;

            // Loop through and get the rest
            while (true)
            {
                bool broke = false;
                int i2 = -1;

                foreach (int i in Enumerable.Range(0, r).Reverse().ToList())
                {
                    // Save for later (since we can't reference the foreach variable outside the loop)
                    i2 = i;

                    if (indices[i] != i + n - r)
                    {
                        // Recreating the for/else loop in Python
                        broke = true;
                        break;
                    }
                }

                // Handle the processing
                // This is done outside the loop so we restart the while() loop each time
                if (broke)
                {
                    indices[i2]++;

                    foreach (int j in Enumerable.Range(i2 + 1, r - i2 - 1))
                    {
                        indices[j] = indices[j - 1] + 1;
                    }

                    // Now return what we know
                    tempList = new List<T>();
                    foreach (int j in indices)
                        tempList.Add(list.Skip(j).First());

                    yield return tempList;
                }
                else
                {
                    // We've hit the end
                    yield break;
                }
            }
        }

        /// <summary>
        /// Based on: https://github.com/tslater2006/AdventOfCode2019
        /// Provides all permutations of a given <see cref="System.Collections.Generic.IEnumerable{T}"/>
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> values)
        {
            return (values.Count() == 1) ? new[] { values } : values.SelectMany(v => Permutations(values.Where(x => Object.Equals(x, v) == false)), (v, p) => p.Prepend(v));
        }

        /// <summary>
        /// Split an <see cref="System.Collections.Generic.IEnumerable{T}"/> into groups of <paramref name="size"/>
        /// </summary>
        /// <param name="array">The input <see cref="System.Collections.Generic.IEnumerable{T}"/></param>
        /// <param name="size">Group sizes</param>
        /// <returns>Groups of <see cref="System.Collections.Generic.IEnumerable{T}"/> with size <paramref name="size"/></returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> array, int size)
        {
            for(var i = 0; i < (float)array.Count() / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

        /// <summary>
        /// https://stackoverflow.com/questions/49190830/is-it-possible-for-string-split-to-return-tuple
        /// </summary>
        /// <param name="list"></param>
        /// <param name="first"></param>
        /// <param name="rest"></param>
        /// <typeparam name="T"></typeparam>
        public static void Deconstruct<T>(this IList<T> list, out T first, out IList<T> rest)
        {
            first = list.Count > 0 ? list[0] : default!; // or throw
            rest = list.Skip(1).ToList();
        }

        public static void Deconstruct<T>(this IList<T> list, out T first, out T second, out IList<T> rest)
        {
            first = list.Count > 0 ? list[0] : default!; // or throw
            second = list.Count > 1 ? list[1] : default!; // or throw
            rest = list.Skip(2).ToList();
        }

        /// <summary>
        /// Computes the sum of the sequence of nullable <see cref="System.UInt64"/> values that are obtained by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <param name="source">A sequence of values that are used to calculate a sum.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <typeparam name="TSource">he type of the elements of <paramref name="source"/>.</typeparam>
        /// <returns>The sum of the projected values.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is <see langword="null" />.</exception>
        /// <exception cref="System.OverflowExeption">The sum is larger than <see cref="System.UInt64.MaxValue"/>.</exception>
        public static UInt64? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, UInt64?> selector)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(selector);

            bool HasValue = false;
            UInt64? returnValue = 0;

            foreach(var item in source)
            {
                // Get our value
                var val = selector(item);

                // May not have a value here
                if (!val.HasValue)
                    continue;

                // Check if there is an overflow
                if (UInt64.MaxValue - returnValue < val)
                    throw new OverflowException();

                // Add it
                returnValue += val;

                // We've had a value
                HasValue = true;
            }

            // No value on this one
            if (!HasValue)
                return null;

            return returnValue;
        }

        /// <summary>
        /// Performs the specified action on each element of the <paramref name="source"/>. This provides missing LINQ functionality for IEnumerable ForEach.
        /// </summary>
        /// <param name="source">The source <see cref="IEnumerable{T}"/></param>
        /// <param name="action">The <see cref="Action{T1}"/> to perform on each element of the <paramref name="source"/>.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            foreach (var item in source)
                action(item);
        }

        /// <summary>
        /// Performs the specified action on each element of the <paramref name="source"/> with index parameters. This provides missing LINQ functionality for IEnumerable ForEach.
        /// </summary>
        /// <param name="source">The source <see cref="IEnumerable{T}"/></param>
        /// <param name="action">The <see cref="Action{T1, T2}"/> to perform on each element of the <paramref name="source"/>.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource, int> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            int idx = 0;
            foreach(var item in source)
            {
                action(item, idx);
                idx++;
            }
        }

        /// <summary>
        /// Generates a sequence of integral numbers within a specified range.
        /// </summary>
        /// <param name="start">The value of the first integer in the sequence.</param>
        /// <param name="count">The number of sequential integers to generate.</param>
        /// <returns>An IEnumerable<Int32> in C# a range of sequential integral numbers.</returns>
        /// <exception cred="System.ArgumentOutOfRangeException"><paramref name="start" /> + <paramref name="count" /> -1 is larger than UInt64.MaxValue.
        public static IEnumerable<ulong> Range(ulong start, ulong count)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(start, ulong.MaxValue - count, nameof(start));

            var end = start + count;
            for (; start < end; start++)
                yield return start;
        }

        /// <summary>
        /// Generates a sequence of integral numbers within a specified range.
        /// </summary>
        /// <param name="start">The value of the first integer in the sequence.</param>
        /// <param name="count">The number of sequential integers to generate.</param>
        /// <returns>An IEnumerable<Int32> in C# a range of sequential integral numbers.</returns>
        /// <exception cred="System.ArgumentOutOfRangeException"><paramref name="start" /> + <paramref name="count" /> -1 is larger than UInt32.MaxValue.
        public static IEnumerable<uint> Range(uint start, uint count)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(start, uint.MaxValue - count, nameof(start));

            var end = start + count;
            for (; start < end; start++)
                yield return start;
        }
    }
}
