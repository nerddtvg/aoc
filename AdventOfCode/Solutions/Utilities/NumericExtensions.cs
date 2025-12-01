/**
 * This utility class is largely based on:
 * https://github.com/jeroenheijmans/advent-of-code-2018/blob/master/AdventOfCode2018/Util.cs
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AdventOfCode.Solutions
{
    public static partial class NumericExtensions
    {
        /// <summary>
        /// Get all divisors of a number (must be positive)
        /// </summary>
        /// <param name="input">The input number</param>
        /// <returns>An array of divisors</returns>
        public static UInt64[] GetDivisors(this UInt64 input)
        {
            if (input == 0) throw new ArgumentException("Input cannot be zero.");

            var divisors = new SortedSet<UInt64>();

            // Save this so we only do it once
            var sqrt = (UInt64)Math.Sqrt(input);

            for (UInt64 i = 1; i <= sqrt; i++)
            {
                if (input % i == 0)
                {
                    // Add this known divisor
                    divisors.Add(i);

                    // Make sure we're not adding a square
                    var d2 = input / i;
                    if (i != d2)
                    {
                        divisors.Add(d2);
                    }
                }
            }

            return divisors.ToArray();
        }

        /// <summary>
        /// Get all divisors of a number (must be positive)
        /// </summary>
        /// <param name="input">The input number</param>
        /// <returns>An array of divisors</returns>
        public static UInt16[] GetDivisors(this UInt16 input) =>
            // We can use the uint function and down-cast them since we know the divisors can't be larger than input
            ((UInt64)input)
                .GetDivisors()
                .Select(a => (UInt16)a)
                .ToArray();

        /// <summary>
        /// Get all divisors of a number (must be positive)
        /// </summary>
        /// <param name="input">The input number</param>
        /// <returns>An array of divisors</returns>
        public static UInt32[] GetDivisors(this UInt32 input) =>
            // We can use the uint function and down-cast them since we know the divisors can't be larger than input
            ((UInt64)input)
                .GetDivisors()
                .Select(a => (UInt32)a)
                .ToArray();

        /// <summary>
        /// Get all divisors of a number (must be positive)
        /// </summary>
        /// <param name="input">The input number</param>
        /// <returns>An array of divisors</returns>
        public static Int16[] GetDivisors(this Int16 input)
        {
            if (input < 0)
                throw new ArgumentException($"The given input must be positive.", nameof(input));

            // We can use the uint function and down-cast them since we know the divisors can't be larger than input
            return ((UInt64)input)
                .GetDivisors()
                .Select(a => (Int16)a)
                .ToArray();
        }

        /// <summary>
        /// Get all divisors of a number (must be positive)
        /// </summary>
        /// <param name="input">The input number</param>
        /// <returns>An array of divisors</returns>
        public static Int32[] GetDivisors(this Int32 input)
        {
            if (input < 0)
                throw new ArgumentException($"The given input must be positive.", nameof(input));

            // We can use the uint function and down-cast them since we know the divisors can't be larger than input
            return ((UInt64)input)
                .GetDivisors()
                .Select(a => (Int32)a)
                .ToArray();
        }

        /// <summary>
        /// Get all divisors of a number (must be positive)
        /// </summary>
        /// <param name="input">The input number</param>
        /// <returns>An array of divisors</returns>
        public static Int64[] GetDivisors(this Int64 input)
        {
            if (input < 0)
                throw new ArgumentException($"The given input must be positive.", nameof(input));

            // We can use the uint function and down-cast them since we know the divisors can't be larger than input
            return ((UInt64)input)
                .GetDivisors()
                .Select(a => (Int64)a)
                .ToArray();
        }

        /// <summary>
        /// Get the individual digits that make up this number (must be positive)
        /// </summary>
        /// <param name="source">The source number</param>
        /// <returns>An array of digits in order</returns>
        public static UInt64[] GetDigits(this UInt64 source)
        {
            // What we will return
            var digits = new Stack<UInt64>();

            while (source > 0)
            {
                // Add this digit
                digits.Push(source % 10);

                // Move on
                source = source / 10;
            }

            return digits.ToArray();
        }

        /// <summary>
        /// Get the individual digits that make up this number (must be positive)
        /// </summary>
        /// <param name="source">The source number</param>
        /// <returns>An array of digits in order</returns>
        public static UInt16[] GetDigits(this UInt16 input) =>
            // We can use the uint function and down-cast them since we know the digits are really just 0-9
            ((UInt64)input)
                .GetDigits()
                .Select(a => (UInt16)a)
                .ToArray();

        /// <summary>
        /// Get the individual digits that make up this number (must be positive)
        /// </summary>
        /// <param name="source">The source number</param>
        /// <returns>An array of digits in order</returns>
        public static UInt32[] GetDigits(this UInt32 input) =>
            // We can use the uint function and down-cast them since we know the digits are really just 0-9
            ((UInt64)input)
                .GetDigits()
                .Select(a => (UInt32)a)
                .ToArray();

        /// <summary>
        /// Get the individual digits that make up this number (must be positive)
        /// </summary>
        /// <param name="source">The source number</param>
        /// <returns>An array of digits in order</returns>
        public static Int16[] GetDigits(this Int16 input)
        {
            if (input < 0)
                throw new ArgumentException($"The given input must be positive.", nameof(input));

            // We can use the uint function and down-cast them since we know the digits are really just 0-9
            return ((UInt64)input)
                .GetDigits()
                .Select(a => (Int16)a)
                .ToArray();
        }

        /// <summary>
        /// Get the individual digits that make up this number (must be positive)
        /// </summary>
        /// <param name="source">The source number</param>
        /// <returns>An array of digits in order</returns>
        public static Int32[] GetDigits(this Int32 input)
        {
            if (input < 0)
                throw new ArgumentException($"The given input must be positive.", nameof(input));

            // We can use the uint function and down-cast them since we know the digits are really just 0-9
            return ((UInt64)input)
                .GetDivisors()
                .Select(a => (Int32)a)
                .ToArray();
        }

        /// <summary>
        /// Get the individual digits that make up this number (must be positive)
        /// </summary>
        /// <param name="source">The source number</param>
        /// <returns>An array of digits in order</returns>
        public static Int64[] GetDigits(this Int64 input)
        {
            if (input < 0)
                throw new ArgumentException($"The given input must be positive.", nameof(input));

            // We can use the uint function and down-cast them since we know the digits are really just 0-9
            return ((UInt64)input)
                .GetDigits()
                .Select(a => (Int64)a)
                .ToArray();
        }
    }
}
