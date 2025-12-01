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
        /// Providing a Modulo operator for common functions (handling negative operations).
        /// </summary>
        /// <param name="dividend">The number to divide by <see cref="divisor"/> </param>
        /// <param name="divisor">The divisor of <see cref="dividend"/></param>
        /// <returns>Non-negative remainder</returns>
        /// <seealso cref="https://stackoverflow.com/a/1082938"/>
        public static Int16 Modulo(this Int16 dividend, Int16 divisor)
        {
            return IntegerExtensions<Int16>.Modulo(dividend, divisor);
        }

        /// <summary>
        /// Providing a Modulo operator for common functions (handling negative operations).
        /// </summary>
        /// <param name="dividend">The number to divide by <see cref="divisor"/> </param>
        /// <param name="divisor">The divisor of <see cref="dividend"/></param>
        /// <returns>Non-negative remainder</returns>
        /// <seealso cref="https://stackoverflow.com/a/1082938"/>
        public static Int32 Modulo(this Int32 dividend, Int32 divisor)
        {
            return IntegerExtensions<Int32>.Modulo(dividend, divisor);
        }

        /// <summary>
        /// Providing a Modulo operator for common functions (handling negative operations).
        /// </summary>
        /// <param name="dividend">The number to divide by <see cref="divisor"/> </param>
        /// <param name="divisor">The divisor of <see cref="dividend"/></param>
        /// <returns>Non-negative remainder</returns>
        /// <seealso cref="https://stackoverflow.com/a/1082938"/>
        public static Int64 Modulo(this Int64 dividend, Int64 divisor)
        {
            return IntegerExtensions<Int64>.Modulo(dividend, divisor);
        }

        /// <summary>
        /// Providing a Modulo operator for common functions (handling negative operations).
        /// </summary>
        /// <param name="dividend">The number to divide by <see cref="divisor"/> </param>
        /// <param name="divisor">The divisor of <see cref="dividend"/></param>
        /// <returns>Non-negative remainder</returns>
        /// <seealso cref="https://stackoverflow.com/a/1082938"/>
        public static Int128 Modulo(this Int128 dividend, Int128 divisor)
        {
            return IntegerExtensions<Int128>.Modulo(dividend, divisor);
        }

        /// <summary>
        /// Providing a Modulo operator for common functions (handling negative operations).
        /// </summary>
        /// <param name="dividend">The number to divide by <see cref="divisor"/> </param>
        /// <param name="divisor">The divisor of <see cref="dividend"/></param>
        /// <returns>Non-negative remainder</returns>
        /// <seealso cref="https://stackoverflow.com/a/1082938"/>
        public static UInt16 Modulo(this UInt16 dividend, UInt16 divisor)
        {
            return IntegerExtensions<UInt16>.Modulo(dividend, divisor);
        }

        /// <summary>
        /// Providing a Modulo operator for common functions (handling negative operations).
        /// </summary>
        /// <param name="dividend">The number to divide by <see cref="divisor"/> </param>
        /// <param name="divisor">The divisor of <see cref="dividend"/></param>
        /// <returns>Non-negative remainder</returns>
        /// <seealso cref="https://stackoverflow.com/a/1082938"/>
        public static UInt32 Modulo(this UInt32 dividend, UInt32 divisor)
        {
            return IntegerExtensions<UInt32>.Modulo(dividend, divisor);
        }

        /// <summary>
        /// Providing a Modulo operator for common functions (handling negative operations).
        /// </summary>
        /// <param name="dividend">The number to divide by <see cref="divisor"/> </param>
        /// <param name="divisor">The divisor of <see cref="dividend"/></param>
        /// <returns>Non-negative remainder</returns>
        /// <seealso cref="https://stackoverflow.com/a/1082938"/>
        public static UInt64 Modulo(this UInt64 dividend, UInt64 divisor)
        {
            return IntegerExtensions<UInt64>.Modulo(dividend, divisor);
        }

        /// <summary>
        /// Providing a Modulo operator for common functions (handling negative operations).
        /// </summary>
        /// <param name="dividend">The number to divide by <see cref="divisor"/> </param>
        /// <param name="divisor">The divisor of <see cref="dividend"/></param>
        /// <returns>Non-negative remainder</returns>
        /// <seealso cref="https://stackoverflow.com/a/1082938"/>
        public static UInt128 Modulo(this UInt128 dividend, UInt128 divisor)
        {
            return IntegerExtensions<UInt128>.Modulo(dividend, divisor);
        }
    }

    /// <summary>
    /// Providing generic operations
    /// </summary>
    public static partial class IntegerExtensions<T> where T : IBinaryInteger<T>
    {
        /// <summary>
        /// Providing a Modulo operator for common functions (handling negative operations).
        /// </summary>
        /// <typeparam name="T">An integer numeric</typeparam>
        /// <param name="dividend">The number to divide by <see cref="divisor"/> </param>
        /// <param name="divisor">The divisor of <see cref="dividend"/></param>
        /// <returns>Non-negative remainder</returns>
        /// <seealso cref="https://stackoverflow.com/a/1082938"/>
        public static T Modulo(T dividend, T divisor)
        {
            ArgumentOutOfRangeException.ThrowIfZero(divisor);

            return (dividend % divisor + divisor) % divisor;
        }
    }
}
