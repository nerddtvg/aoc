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

        private (int load, string[] rotate) CountAndRotate(string[] rows, int dir)
        {
            var load = 0;

            var tempRows = new List<string>();

            foreach (var row in rows)
            {
                // Tracks the last rock's position
                // The next rock will be placed into lastRock-1
                var lastRock = row.Length + 1;

                // Make our row string
                var rowStr = Enumerable.Repeat('.', row.Length).ToArray();

                row.ForEach((c, idx) =>
                {
                    if (c == 'O')
                    {
                        // Move down
                        lastRock--;

                        // This is off by one for the char index
                        rowStr[row.Length - lastRock] = 'O';
                    }
                    else if (c == '#')
                    {
                        // Immovable rock, all rocks will stop here
                        lastRock = row.Length - idx;
                        rowStr[idx] = '#';
                    }
                });

                tempRows.Add(rowStr.JoinAsString());
            }

            // Part 2: Because we could be "rotated"
            // we need to count the number of rocks differently
            // Dir == 0 (north) measure distance from bottom
            // Dir == 1 (west) measure distance from "right"
            // Dir == 2 (south) measure distance from top
            // Dir == 3 (east) measure distance from "left"
            if (dir == 0 || dir == 2)
            {
                for (int i = 0; i < tempRows[0].Length; i++)
                {
                    load += (dir == 0 ? tempRows[0].Length - i : i + 1) * tempRows.Count(row => row[i] == 'O');
                }
            }
            else
            {
                // "left" is idx+1
                // "right" is length-idx
                load += tempRows.Select((row, idx) => row.Count(c => c == 'O') * (dir == 1 ? (idx + 1) : (row.Length - idx))).Sum();
            }

            // For Part 2, we rotate these clockwise
            var retRows = new List<string>();
            for(int iChar = tempRows[0].Length-1; iChar >= 0; iChar--)
                retRows.Add(tempRows.Select(row => row[iChar]).JoinAsString());

            return (load, retRows.ToArray());
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

            (var load, var rotate) = CountAndRotate(columns, 0);

            return load.ToString();
        }

        protected override string? SolvePartTwo()
        {

            var columns = Input
                .SplitByNewline(shouldTrim: true)
                .ToArray()
                .GetColumns();

            var loads = new List<int>();
            var count = 1000000000;

            for (int idx = 0; idx < count; idx++)
            {
                // Each cycle is each direction so loop 4 times
                var load = 0;
                Enumerable.Range(0, 4).ForEach(dir => (load, columns) = CountAndRotate(columns, dir));

                if (loads.LastIndexOf(load) is int prevIdx && idx > 50)
                {
                    // Do we have a repeating pattern?
                    // Find how many cycles in between
                    // Check if we have enough space to have a pattern
                    var cycle = loads.Count - prevIdx;

                    if (cycle > 1 && prevIdx - cycle - 1 >= 0)
                    {
                        // Make sure we actually have a pattern
                        if (loads[(prevIdx - cycle)..prevIdx].SequenceEqual(loads[prevIdx..]))
                        {
                            // Then determine where count ends up
                            var delta = (count - loads.Count - 1) % cycle;

                            Console.WriteLine($"Cycle: {cycle}");
                            Console.WriteLine($"Index: {idx}");
                            Console.WriteLine($"Delta: {delta}");

                            return loads[prevIdx + delta].ToString();
                        }
                    }
                }

                loads.Add(load);
            }

            return loads[0].ToString();
        }
    }
}

