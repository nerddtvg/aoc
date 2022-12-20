using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

// One hint from the megathread is to use bitwise math for the output
// I'm going to try with each row being bits of 9 characters across
//   |.......| => 512 + 1 => 513
// 512       1
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
                    30
                }
            },
            {
                RockType.Plus,
                new uint[]
                {
                    8,
                    // 16 + 8 + 4
                    28,
                    8
                }
            },
            {
                RockType.Bracket,
                new uint[]
                {
                    4,
                    4,
                    // 16 + 8 + 4
                    28
                }
            },
            {
                RockType.Vertical,
                new uint[]
                {
                    16,
                    16,
                    16,
                    16
                }
            },
            {
                RockType.Box,
                new uint[]
                {
                    // 16 + 8
                    24,
                    24
                }
            }
        };

        private const uint LeftWall = 512;
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
        }

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

            throw new Exception();
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
                    if (shape.Any(row => (row & LeftWall) > 0))
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
                    // Attempt to move left
                    if (shape.Any(row => (row & RightWall) > 0))
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
            } while (true);
        }

        protected override string? SolvePartOne()
        {
            return string.Empty;
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

