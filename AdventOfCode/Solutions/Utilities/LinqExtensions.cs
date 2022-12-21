using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Solutions;

/// <summary>
/// Based on: https://stackoverflow.com/questions/32309807/how-use-long-type-for-skip-in-linq
/// </summary>
static public class LinqExtensions
{
    /// <summary>
    /// Implementing <see cref="IEnumerable{T}.Skip{T}" /> with <see cref="long" />.
    /// </summary>
    /// <param name="count">Number of items to skip</param>
    /// <returns>An <see cref="IEnumerable{T}" /> that contains the elements that occur after the specified index in the input sequence.</returns>
    public static IEnumerable<T> BigSkip<T>(this IEnumerable<T> items, long count)
    {
        var segmentSize = Int32.MaxValue;

        long segmentCount = Math.DivRem(count, segmentSize,
            out long remainder);

        for (long i = 0; i < segmentCount; i += 1)
            items = items.Skip(segmentSize);

        if (remainder != 0)
            items = items.Skip((int)remainder);

        return items;
    }

    /// <summary>
    /// Implementing <see cref="IEnumerable{T}.Take{T}" /> with <see cref="long" />.
    /// </summary>
    /// <param name="count">Number of items to take</param>
    /// <returns>An <see cref="IEnumerable{T}" /> that contains the specified number of elements from the start of the input sequence.</returns>
    public static IEnumerable<T> BigTake<T>(this IEnumerable<T> items, long count)
    {
        var segmentSize = Int32.MaxValue;

        long segmentCount = Math.DivRem(count, segmentSize,
            out long remainder);

        for (long i = 0; i < segmentCount; i += 1)
            items = items.Take(segmentSize);

        if (remainder != 0)
            items = items.Take((int)remainder);

        return items;
    }
}
