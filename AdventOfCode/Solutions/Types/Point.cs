using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;

namespace AdventOfCode.Solutions;

/// <summary>
/// Quick references to common 2D Moves
/// </summary>
public readonly struct Point2D
{
    /// <summary>
    /// Common 2D Translation: Move Up
    /// </summary>
    public readonly static Point<int> MoveUp = new(0, -1);

    /// <summary>
    /// Common 2D Translation: Move Right
    /// </summary>
    public readonly static Point<int> MoveRight = new(1, 0);

    /// <summary>
    /// Common 2D Translation: Move Down
    /// </summary>
    public readonly static Point<int> MoveDown = new(0, 1);

    /// <summary>
    /// Common 2D Translation: Move Left
    /// </summary>
    public readonly static Point<int> MoveLeft = new(-1, 0);
}

/// <summary>
/// A simple (x, y) coordinate class with corresponding operators.
/// </summary>
public readonly struct Point<T> where T : INumber<T>
{
    /// <summary>
    /// The array of coordinates for this point.
    /// </summary>
    private readonly T[] coordinates;

    /// <summary>
    /// How many dimensions does this <see cref="Point" /> consist of. (Ex: (x, y) is 2-dimensions)
    /// </summary>
    public int Dimensions { get => coordinates.Length; }

    /// <summary>
    /// Initialize a new point with the given coordinate values.
    /// </summary>
    public Point(params T[] coordinates)
    {
        if (coordinates.Length < 2)
            throw new ArgumentOutOfRangeException(nameof(coordinates), "At least two coordinate values must be specified.");
        this.coordinates = coordinates;
    }

    public static Point<T> operator +(Point<T> a) => a;
    public static Point<T> operator -(Point<T> a) => new Point<T>(a.coordinates.Select(c => -c).ToArray());

    /// <summary>
    /// Compare the lengths of two <see cref="Point" /> to ensure they have the same number of dimensions.
    /// </summary>
    private static void EnsureSameDimensions(Point<T> a, Point<T> b)
    {
        if (a.coordinates.Length != b.coordinates.Length)
            throw new Exception("The given points do not have the same dimensions.");
    }

    /// <summary>
    /// Adds points <paramref name="a" /> and <paramref name="b" />
    /// </summary>
    public static Point<T> operator +(Point<T> a, Point<T> b)
    {
        EnsureSameDimensions(a, b);

        return new Point<T>(
            Enumerable.Range(0, a.coordinates.Length)
            .Select(i => a.coordinates[i] + b.coordinates[i])
            .ToArray()
        );
    }

    /// <summary>
    /// Subtract point <paramref name="b" /> from <paramref name="a" />
    /// </summary>
    public static Point<T> operator -(Point<T> a, Point<T> b)
    {
        EnsureSameDimensions(a, b);

        return new Point<T>(
            Enumerable.Range(0, a.coordinates.Length)
            .Select(i => a.coordinates[i] - b.coordinates[i])
            .ToArray()
        );
    }

    /// <summary>
    /// Add <paramref name="b" /> to all coordinate values
    /// </summary>
    public static Point<T> operator +(Point<T> a, T b)
    {
        return new Point<T>(
            Enumerable.Range(0, a.coordinates.Length)
            .Select(i => a.coordinates[i] + b)
            .ToArray()
        );
    }

    /// <summary>
    /// Subtract <paramref name="b" /> from all coordinate values
    /// </summary>
    public static Point<T> operator -(Point<T> a, T b)
    {
        return new Point<T>(
            Enumerable.Range(0, a.coordinates.Length)
            .Select(i => a.coordinates[i] - b)
            .ToArray()
        );
    }

    /// <summary>
    /// Multiply all coordinate values by <paramref name="b" />
    /// </summary>
    public static Point<T> operator *(Point<T> a, T b)
    {
        return new Point<T>(
            Enumerable.Range(0, a.coordinates.Length)
            .Select(i => a.coordinates[i] * b)
            .ToArray()
        );
    }

    /// <summary>
    /// Divide all coordinate values by <paramref name="b" />
    /// </summary>
    public static Point<T> operator /(Point<T> a, T b)
    {
        return new Point<T>(
            Enumerable.Range(0, a.coordinates.Length)
            .Select(i => a.coordinates[i] / b)
            .ToArray()
        );
    }

    /// <summary>
    /// Not implemented.
    /// </summary>
    public static Point<T> operator *(Point<T> a, Point<T> b)
        => throw new NotImplementedException();

    /// <summary>
    /// Not implemented.
    /// </summary>
    public static Point<T> operator /(Point<T> a, Point<T> b)
        => throw new NotImplementedException();

    /// <summary>
    /// Manhattan Distance calculation
    /// </summary>
    public static ulong operator %(Point<T> a, Point<T> b)
    {
        EnsureSameDimensions(a, b);

        return Enumerable.Range(0, a.Dimensions)
            .Sum(i =>
            {
                var t = a.coordinates[i] - b.coordinates[i];
                if (t < T.Zero)
                    return ulong.CreateChecked(-t);
                return ulong.CreateChecked(t);
            }) ?? throw new OverflowException();
    }

    /// <summary>
    /// Modulus operation against all values in <paramref name="a" />
    /// </summary>
    public static Point<T> operator %(Point<T> a, T b)
    {
        return new Point<T>(
            a
                .coordinates
                // If d is less than 0, % returns the remainder and we have to add the divisor again
                // Example:
                // -6 % 4 = -2
                // So -2 + 4 = 2 to get the modulus
                .Select(d => d < T.Zero ? (d % b) + b : d % b)
                .ToArray()
        );
    }

    /// <summary>
    /// Not implemented.
    /// </summary>
    public static Point<T> operator &(Point<T> a, Point<T> b)
        => throw new NotImplementedException();

    /// <summary>
    /// Not implemented.
    /// </summary>
    public static Point<T> operator |(Point<T> a, Point<T> b)
        => throw new NotImplementedException();

    /// <summary>
    /// Not implemented.
    /// </summary>
    public static Point<T> operator ^(Point<T> a, Point<T> b)
        => throw new NotImplementedException();

    /// <summary>
    /// Not implemented.
    /// </summary>
    public static Point<T> operator <<(Point<T> a, Point<T> b)
        => throw new NotImplementedException();

    /// <summary>
    /// Not implemented.
    /// </summary>
    public static Point<T> operator >>(Point<T> a, Point<T> b)
        => throw new NotImplementedException();

    /// <summary>
    /// Not implemented.
    /// </summary>
    public static Point<T> operator >>>(Point<T> a, Point<T> b)
        => throw new NotImplementedException();

    /// <summary>
    /// Increment all dimensions by 1.
    /// </summary>
    public static Point<T> operator ++(Point<T> a)
    {
        return new Point<T>(
            a.coordinates
            .Select(c => c + T.One)
            .ToArray()
        );
    }

    /// <summary>
    /// Decrement all dimensions by 1.
    /// </summary>
    public static Point<T> operator --(Point<T> a)
    {
        return new Point<T>(
            a.coordinates
            .Select(c => c - T.One)
            .ToArray()
        );
    }

    /// <summary>
    /// Ensure all dimensions are equal.
    /// </summary>
    public static bool operator ==(Point<T> a, Point<T> b)
    {
        if (a.coordinates.Length != b.coordinates.Length)
            return false;

        return Enumerable.Range(0, a.coordinates.Length)
            .All(i => a.coordinates[i] == b.coordinates[i]);
    }

    /// <summary>
    /// Ensure at least one dimension is not equal or they are different sizes.
    /// </summary>
    public static bool operator !=(Point<T> a, Point<T> b)
    {
        if (a.coordinates.Length != b.coordinates.Length)
            return true;

        return !Enumerable.Range(0, a.coordinates.Length)
            .All(i => a.coordinates[i] == b.coordinates[i]);
    }

    /// <summary>
    /// Overloading Object.Equals as required.
    /// </summary>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is Point<T> b)
        {
            return this == b;
        }

        return false;
    }

    /// <summary>
    /// Overloading GetHashCode as required.
    /// </summary>
    public override int GetHashCode()
    {
        // Our string is simply our coordinates which works better than getting
        // the Hash Code of the array.
        return ToString().GetHashCode();
    }

    /// <summary>
    /// Compare if <paramref name="a" /> is less than <paramref name="b" />
    /// </summary>
    public static bool operator <(Point<T> a, Point<T> b)
    {
        EnsureSameDimensions(a, b);

        return Enumerable.Range(0, a.coordinates.Length)
            .All(i => a.coordinates[i] < b.coordinates[i]);
    }

    /// <summary>
    /// Compare if <paramref name="a" /> is greater than <paramref name="b" />
    /// </summary>
    public static bool operator >(Point<T> a, Point<T> b)
    {
        EnsureSameDimensions(a, b);

        return Enumerable.Range(0, a.coordinates.Length)
            .All(i => a.coordinates[i] > b.coordinates[i]);
    }

    /// <summary>
    /// Compare if <paramref name="a" /> is less than or equal <paramref name="b" />
    /// </summary>
    public static bool operator <=(Point<T> a, Point<T> b)
    {
        EnsureSameDimensions(a, b);

        return Enumerable.Range(0, a.coordinates.Length)
            .All(i => a.coordinates[i] <= b.coordinates[i]);
    }

    /// <summary>
    /// Compare if <paramref name="a" /> is greater than or equal <paramref name="b" />
    /// </summary>
    public static bool operator >=(Point<T> a, Point<T> b)
    {
        EnsureSameDimensions(a, b);

        return Enumerable.Range(0, a.coordinates.Length)
            .All(i => a.coordinates[i] >= b.coordinates[i]);
    }

    /// <summary>
    /// Create a new <see cref="Point" /> with the existing coordinates and a new dimension.
    /// </summary>
    public Point<T> AddDimension(T value)
        => new Point<T>(coordinates.Append(value).ToArray());

    /// <summary>
    /// Direct coordinate values
    /// </summary>
    public T this[int i]
    {
        get
        {
            if (i >= coordinates.Length)
                throw new IndexOutOfRangeException();

            return coordinates[i];
        }
        set
        {
            if (i >= coordinates.Length)
                throw new IndexOutOfRangeException();

            coordinates[i] = value;
        }
    }

    /// <summary>
    /// Direct coordinate values based on characters. X, Y, Z are 0, 1, and 2 respectfully. A, B, C, ... to w. are 0 through 22 respectfully.
    /// </summary>
    public T this[char c]
    {
        get
        {
            return this[GetIndex(c)];
        }
        set
        {
            this[GetIndex(c)] = value;
        }
    }

    /// <summary>
    /// Helper to provide more consistent access to x like an object property
    /// </summary>
    public T x
    {
        get => this[nameof(x)[0]];
        set => this[nameof(x)[0]] = value;
    }

    /// <summary>
    /// Helper to provide more consistent access to y like an object property
    /// </summary>
    public T y
    {
        get => this[nameof(y)[0]];
        set => this[nameof(y)[0]] = value;
    }

    /// <summary>
    /// Helper to provide more consistent access to z like an object property
    /// </summary>
    public T z
    {
        get => this[nameof(z)[0]];
        set => this[nameof(z)[0]] = value;
    }

    private int GetIndex(char c)
    {
        // A-Z
        if (65 <= c && c <= 90)
        {
            c -= (char)65;

            // X-Z => 0-3
            if (23 <= c)
                c -= (char)23;

            return (int)c;
        }

        // A-Z
        if (97 <= c && c <= 122)
        {
            c -= (char)97;

            // X-Z => 0-3
            if (23 <= c)
                c -= (char)23;

            return (int)c;
        }

        throw new ArgumentException("Specified character index must be A-Z or a-z.", nameof(c));
    }

    /// <summary>
    /// Handling displaying this data
    /// </summary>
    public override string ToString()
    {
        return $"({string.Join(", ", coordinates.Select(c => c.ToString()))})";
    }
}
