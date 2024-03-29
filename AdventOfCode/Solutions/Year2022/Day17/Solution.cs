using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

// One hint from the megathread is to use bitwise math for the output
// I'm going to try with each row being bits of 9 characters across
// 256 128  64  32  16   8   4   2   1
//   |   .   .   .   .   .   .   .   | => 256 + 1 => 257
// 256 128  64  32  16   8   4   2   1
// Then each shape will be easy to shift left and right, then check for
// collisions because any AND will return > 0

namespace AdventOfCode.Solutions.Year2022
{

    class Day17 : ASolution
    {
        /// <summary>
        /// Holds the rocks in order of them falling
        /// </summary>
        public Queue<RockType> rocks = new();

        /// <summary>
        /// Holds our input for air jet directions
        /// </summary>
        public Queue<char> airJet = new();

        /// <summary>
        /// Our output grid, top to bottom is 0 to inf
        /// </summary>
        public uint[] output = Array.Empty<uint>();

        private Dictionary<RockType, uint[]> shapes = new()
        {
            {
                RockType.Horiz,
                new uint[]
                {
                    // 256 128  64  32  16   8   4   2   1
                    //   |   .   .   #   #   #   #   .   |
                    // 256 128  64  32  16   8   4   2   1
                    60
                }
            },
            {
                RockType.Plus,
                new uint[]
                {
                    // 256 128  64  32  16   8   4   2   1
                    //   |   .   .   .   #   .   .   .   |
                    //   |   .   .   #   #   #   .   .   |
                    //   |   .   .   .   #   .   .   .   |
                    // 256 128  64  32  16   8   4   2   1
                    16,
                    // 32 + 16 + 8
                    56,
                    16
                }
            },
            {
                RockType.Bracket,
                new uint[]
                {
                    // 256 128  64  32  16   8   4   2   1
                    //   |   .   .   .   .   #   .   .   |
                    //   |   .   .   .   .   #   .   .   |
                    //   |   .   .   #   #   #   .   .   |
                    // 256 128  64  32  16   8   4   2   1
                    8,
                    8,
                    // 32 + 16 + 8
                    56
                }
            },
            {
                RockType.Vertical,
                new uint[]
                {
                    // 256 128  64  32  16   8   4   2   1
                    //   |   .   .   #   .   .   .   .   |
                    //   |   .   .   #   .   .   .   .   |
                    //   |   .   .   #   .   .   .   .   |
                    //   |   .   .   #   .   .   .   .   |
                    // 256 128  64  32  16   8   4   2   1
                    32,
                    32,
                    32,
                    32
                }
            },
            {
                RockType.Box,
                new uint[]
                {
                    // 256 128  64  32  16   8   4   2   1
                    //   |   .   .   #   #   .   .   .   |
                    //   |   .   .   #   #   .   .   .   |
                    // 256 128  64  32  16   8   4   2   1
                    // 32 + 16
                    48,
                    48
                }
            }
        };

        private const uint LeftWall = 256;
        private const uint RightWall = 1;

        public Day17() : base(17, 2022, "Pyroclastic Flow")
        {
            // DebugInput = @">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>";

            ResetEverything();
        }

        private void ResetEverything()
        {
            // Load the queues
            rocks.Clear();
            rocks.Enqueue(RockType.Horiz);
            rocks.Enqueue(RockType.Plus);
            rocks.Enqueue(RockType.Bracket);
            rocks.Enqueue(RockType.Vertical);
            rocks.Enqueue(RockType.Box);

            // And our input
            airJet.Clear();
            Input.ToCharArray().ToList().ForEach(c => airJet.Enqueue(c));

            output = Array.Empty<uint>();
        }

        /// <summary>
        /// Finds the first row index that has a box in it
        /// </summary>
        private int FindFirstRow()
        {
            for (int i = 0; i<output.Length; i++)
            {
                // Look for 2^1 through 2^7
                if ((output[i] & 254) > 0)
                    return i;
            }

            return 0;
        }

        private void ProcessShape()
        {
            // First find our starting row
            var firstRow = FindFirstRow();

            // Get our rock shape, re-queue to the end
            var rock = rocks.Dequeue();
            rocks.Enqueue(rock);
            var shape = shapes[rock];

            // Change our minimum line
            // 3 above the highest rock, plus the number of rows in this shape
            firstRow -= (3 + shape.Length);

            int row = firstRow;

            do
            {
                // First, apply an air jet
                var air = airJet.Dequeue();
                airJet.Enqueue(air);

                if (air == '<')
                {
                    // Attempt to move left
                    if (shape.Any(row => ((row << 1) & LeftWall) > 0))
                    {
                        // Can't move left, wall
                        // Didn't shift yet, nothing to do but move down
                    }
                    else
                    {
                        var collision = false;

                        // Check if we hit another rock
                        for (int outputIndex = row, shapeIndex = 0; shapeIndex < shape.Length; outputIndex++, shapeIndex++)
                        {
                            // Non-existent row
                            if (outputIndex < 0)
                                continue;

                            if ((output[outputIndex] & (shape[shapeIndex] << 1)) > 0)
                            {
                                // Hit something!
                                collision = true;
                                break;
                            }
                        }

                        // No hit, shift it
                        if (!collision)
                            shape = shape.Select(c => c << 1).ToArray();
                    }
                }
                else
                {
                    // Attempt to move right
                    if (shape.Any(row => ((row >> 1) & RightWall) > 0))
                    {
                        // Can't move right, wall
                        // Didn't shift yet, nothing to do but move down
                    }
                    else
                    {
                        var collision = false;

                        // Check if we hit another rock
                        for (int outputIndex = row, shapeIndex = 0; shapeIndex < shape.Length; outputIndex++, shapeIndex++)
                        {
                            // Non-existent row
                            if (outputIndex < 0)
                                continue;

                            // If we have hit the bottom, we're done
                            if ((output[outputIndex] & (shape[shapeIndex] >> 1)) > 0)
                            {
                                // Hit something!
                                collision = true;
                                break;
                            }
                        }

                        // No hit, shift it
                        if (!collision)
                            shape = shape.Select(c => c >> 1).ToArray();
                    }
                }

                // Now move down!
                // If we are below 0, we move down no matter what because there is nothing else below us
                // Otherwise check for a collision (need to be sure to check every row of the shape)
                var bottomIndex = row + shape.Length;

                // Consider it a collision if we are at the bottom of the tower
                var downCollision = bottomIndex == output.Length;

                if (!downCollision && bottomIndex >= 0)
                {
                    // Do a bitwise check from the bottom up
                    for (int outputIndex = bottomIndex, shapeIndex = shape.Length - 1; !downCollision && outputIndex >= 0 && shapeIndex >= 0; outputIndex--, shapeIndex--)
                    {
                        if ((output[outputIndex] & shape[shapeIndex]) > 0)
                            downCollision = true;
                    }
                }

                if (downCollision)
                {
                    // Can't move down, found the stop
                    if (row < 0)
                    {
                        // Need to shift the array to make room
                        var shift = (int)Math.Abs(row);
                        var newLength = output.Length + shift;
                        var newOutput = new uint[newLength];

                        // Copy the old data over
                        Array.Copy(output, 0, newOutput, shift, output.Length);

                        // Append the new rows
                        for (int i = 0; i < shift; i++)
                            newOutput[i] = LeftWall | RightWall;

                        // Reset!
                        row = 0;
                        output = newOutput;
                    }

                    // We have a non-negative row number now
                    // Combine the row values
                    for (int outputIndex = row, shapeIndex = 0; shapeIndex < shape.Length; outputIndex++,shapeIndex++)
                    {
                        output[outputIndex] |= shape[shapeIndex];
                    }
                    break;
                }
                else
                {
                    row++;
                }
            } while (true);
        }

        private void PrintOutput()
        {
            for (int i = 0; i < output.Length; i++)
            {
                if ((output[i] & LeftWall) > 0)
                    Console.Write('|');
                else
                    Console.Write('.');

                if ((output[i] & (2 << 6)) > 0)
                    Console.Write('#');
                else
                    Console.Write('.');

                if ((output[i] & (2 << 5)) > 0)
                    Console.Write('#');
                else
                    Console.Write('.');

                if ((output[i] & (2 << 4)) > 0)
                    Console.Write('#');
                else
                    Console.Write('.');

                if ((output[i] & (2 << 3)) > 0)
                    Console.Write('#');
                else
                    Console.Write('.');

                if ((output[i] & (2 << 2)) > 0)
                    Console.Write('#');
                else
                    Console.Write('.');

                if ((output[i] & (2 << 1)) > 0)
                    Console.Write('#');
                else
                    Console.Write('.');

                if ((output[i] & (2 << 0)) > 0)
                    Console.Write('#');
                else
                    Console.Write('.');

                if ((output[i] & RightWall) > 0)
                    Console.Write('|');
                else
                    Console.Write('.');

                Console.WriteLine();
            }

            Console.WriteLine("+-------+");
            Console.WriteLine();
        }

        protected override string? SolvePartOne()
        {
            for (int i = 0; i < 2022; i++)
            {
                ProcessShape();

                // if (i < 10)
                //     PrintOutput();
            }

            // Count the height (remove the bottom row and empty rows at the top)
            return GetTowerHeight().ToString();
        }

        private uint GetTowerHeight() => (uint)output.Length - (uint)FindFirstRow();

        protected override string? SolvePartTwo()
        {
            // This question is built on periods, so we need to identify the period of our tower
            // We can do this by tracking the height and, say the last 25 rows, hashed together
            // Tested with 5, 10, and 25 rows but it wasn't enough to capture the period (25 was enough for the example)
            var periodHashes = new Dictionary<string, (uint index, ulong height)>();
            var cycleHeights = new Dictionary<ulong, ulong>();

            ResetEverything();

            // Final cycle count
            var finalCount = (ulong)1000000000000;

            var periodBuffer = 50;

            for (uint i = 0; i < 10000; i++)
            {
                ProcessShape();
                
                // If we have enough rows...
                if (i >= periodBuffer)
                {
                    var start = FindFirstRow();
                    var arr = new uint[periodBuffer];

                    for (int q = 0; q + start < output.Length && q < periodBuffer; q++)
                        arr[q] = output[q + start];

                    // Add it to the dictionary
                    var hash = string.Join("-", arr);
                    var height = GetTowerHeight();

                    cycleHeights.Add(i, height);

                    if (periodHashes.ContainsKey(hash))
                    {
                        // Found a possible period
                        // 0 .... P1 .... P2 .... P3 .... etc.
                        // P1 is our first height, P2 is "i"
                        // The height of the period is height-heights[P1]
                        var period = i - periodHashes[hash].index;
                        var periodHeight = height - periodHashes[hash].height;

                        // To complete the math...
                        // finalCount - heights[hash].index => cycles after period starts
                        // Then find out how many full periods there are and how many cycles remain to complete it
                        var cyclesAfterPeriodStarts = finalCount - (uint)periodHashes[hash].index;
                        var fullPeriods = cyclesAfterPeriodStarts / period;
                        var remainderCycles = cyclesAfterPeriodStarts % period;

                        // In theory, find the cycleHeights[period cycle + remainderCycles] to add on to our new height
                        // cycleHeights[periodHashes[hash].index-1] + (periodHeight * fullPeriods) + (cycleHeights[period cycle + remainderCycles] - periodHashes[hash].height)
                        return (cycleHeights[periodHashes[hash].index - 1] + (periodHeight * fullPeriods) + (cycleHeights[periodHashes[hash].index + remainderCycles] - cycleHeights[periodHashes[hash].index])).ToString();
                    }
                    else
                    {
                        periodHashes.Add(hash, (i, height));
                    }
                }
            }

            return string.Empty;
        }

        public enum RockType
        {
            Horiz,
            Plus,
            Bracket,
            Vertical,
            Box
        }
    }
}

