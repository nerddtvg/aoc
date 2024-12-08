using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;

namespace AdventOfCode.Solutions;

/// <summary>
/// Common directions in Enum form, North is 0
/// </summary>
public enum Direction
{
    North,
    East,
    South,
    West
}

/// <summary>
/// Common direction characters in Enum form, North is 0
/// </summary>
public enum DirectionChar
{
    North = '^',
    East = '>',
    South = 'v',
    West = '<'
}

/// <summary>
/// Provides easy to reference Direction maps
/// </summary>
public static class Directions
{
    /// <summary>
    /// Direction to delta references in tuple format
    /// </summary>
    public readonly static Dictionary<Direction, (int x, int y)> directionTuple = new()
    {
        [Direction.North] = (0, -1),
        [Direction.East] = (1, 0),
        [Direction.South] = (0, 1),
        [Direction.West] = (-1, 0)
    };

    /// <summary>
    /// Direction to delta references in <see cref="Point{T}"/> format
    /// </summary>
    public readonly static Dictionary<Direction, Point<int>> directionPoint = new()
    {
        [Direction.North] = new(0, -1),
        [Direction.East] = new(1, 0),
        [Direction.South] = new(0, 1),
        [Direction.West] = new(-1, 0)
    };

    /// <summary>
    /// Characters commonly representing directions
    /// </summary>
    public readonly static Dictionary<char, Direction> charToDirection = new()
    {
        ['^'] = Direction.North,
        ['>'] = Direction.East,
        ['v'] = Direction.South,
        ['<'] = Direction.West,
    };

    /// <summary>
    /// Directions re-cast to common characters
    /// </summary>
    public readonly static Dictionary<Direction, char> directionToChar = new()
    {
        [Direction.North] = '^',
        [Direction.East] = '>',
        [Direction.South] = 'v',
        [Direction.West] = '<'
    };
}
