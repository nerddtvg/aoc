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
            return (values.Count() == 1) ? new[] { values } : values.SelectMany(v => Permutations(values.Where(x => x.Equals(v) == false)), (v, p) => p.Prepend(v));
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
            first = list.Count > 0 ? list[0] : default(T); // or throw
            rest = list.Skip(1).ToList();
        }

        public static void Deconstruct<T>(this IList<T> list, out T first, out T second, out IList<T> rest)
        {
            first = list.Count > 0 ? list[0] : default(T); // or throw
            second = list.Count > 1 ? list[1] : default(T); // or throw
            rest = list.Skip(2).ToList();
        }
    }
}
