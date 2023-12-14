using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2023
{

    class Day14 : ASolution
    {

        public Day14() : base(14, 2023, "Parabolic Reflector Dish")
        {
            // DebugInput = @"O....#....
            //                O.OO#....#
            //                .....##...
            //                OO.#O....O
            //                .O.....O#.
            //                O.#..O.#.#
            //                ..O..#O..O
            //                .......O..
            //                #....###..
            //                #OO..#....";
        }

        protected override string? SolvePartOne()
        {
            // To move rocks north, they can move to the furthest
            // point point in that column up, until they hit the
            // edge, another rock, or a stationary rock
            // We can work down a column and count the positions
            // rocks land on

            var columns = Input
                .SplitByNewline(shouldTrim: true)
                .ToArray()
                .GetColumns();
            var load = 0;

            foreach(var column in columns)
            {
                // Tracks the last rock's position
                // The next rock will be placed into lastRock-1
                var lastRock = column.Length+1;

                // Start at the highest row number (distance from end)
                var row = column.Length;

                column.ForEach((c, idx) =>
                {
                    if (c == 'O')
                    {
                        // Move down
                        lastRock--;

                        // Count this rock
                        load += lastRock;
                    }
                    else if (c == '#')
                    {
                        // Immovable rock, all rocks will stop here
                        lastRock = column.Length - idx;
                    }
                });
            }

            return load.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

