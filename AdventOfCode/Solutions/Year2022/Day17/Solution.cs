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
            DebugInput = @">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>";

            // Load the queues
            rocks.Enqueue(RockType.Horiz);
            rocks.Enqueue(RockType.Plus);
            rocks.Enqueue(RockType.Bracket);
            rocks.Enqueue(RockType.Vertical);
            rocks.Enqueue(RockType.Box);

            // And our input
            Input.ToCharArray().ToList().ForEach(c => airJet.Enqueue(c));

            ResetOutput();
        }

        /// <summary>
        /// Remove all output
        /// </summary>
        private void ResetOutput() => output = Array.Empty<uint>();

        /// <summary>
        /// Finds the first row index that has a box in it
        /// </summary>
        private int FindFirstRow()
        {
            for (int i = 0; i<output.Length; i++)
            {
                // Look for 2<<2 through 2<<8
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
                // Otherwise check for a collision
                var bottomIndex = row + shape.Length;
                if (bottomIndex == output.Length || (bottomIndex >= 0 && (output[bottomIndex] & shape[^1]) > 0))
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
                        output[outputIndex] = output[outputIndex] | shape[shapeIndex];
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

                if (i < 10)
                    PrintOutput();
            }

            // Count the height (remove the bottom row and empty rows at the top)
            return (output.Length - FindFirstRow() - 1).ToString();
        }

        protected override string? SolvePartTwo()
        {
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

