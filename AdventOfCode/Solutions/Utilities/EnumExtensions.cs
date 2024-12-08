using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AdventOfCode.Solutions
{
    public static partial class EnumExtensions
    {
        /// <summary>
        /// "Rotates" <typeparamref name="T"/> by increasing or decreasing (if <paramref name="negative"/> is true) its value by <paramref name="delta"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The <see cref="Enum"/> to operate on</param>
        /// <param name="delta">The number of steps to take</param>
        /// <param name="negative">If true, decrease the value</param>
        /// <returns>The new <typeparamref name="T"/> value</returns>
        /// <exception cref="ArgumentException"></exception>
        public static T Rotate<T>(this T value, int delta = 1, bool negative = false) where T : struct, Enum
        {
            var enumType = typeof(T);
            var underlyingType = Enum.GetUnderlyingType(enumType);
            var valueAsType = Convert.ChangeType(value, underlyingType);
            Int64 count = Enum.GetValues<T>().Length;

            // Using Int64 supports negative values and we will likely never go above a dozen enum
            // values to worry about overflowing or conversions between 64 and lower-bit values
            switch (Type.GetTypeCode(underlyingType))
            {
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    var newValue = Convert.ToInt64(valueAsType) + (delta * (negative ? -1 : 1));

                    // Correct for negative mods
                    while (newValue < 0)
                        newValue += count;

                    return (T)Convert.ChangeType(newValue % count, underlyingType);
            }

            throw new ArgumentException("Enum must be backed by an integral type.");
        }
    }
}