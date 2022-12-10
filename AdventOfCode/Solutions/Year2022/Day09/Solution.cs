using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2022
{

    class Day09 : ASolution
    {
        /// <summary>
        /// Direction letters from the inputs
        /// </summary>
        public enum Direction
        {
            D,
            U,
            L,
            R,
            UL,
            UR,
            DL,
            DR
        }

        /// <summary>
        /// (x, y) changes when moving a direction
        /// </summary>
        public Dictionary<Direction, Point<int>> Moves = new()
        {
            { Direction.U, new(0, -1) },
            { Direction.D, new(0, 1) },
            { Direction.L, new(-1, 0) },
            { Direction.R, new(1, 0) },
            { Direction.UL, new(-1, -1) },
            { Direction.UR, new(1, -1) },
            { Direction.DL, new(-1, 1) },
            { Direction.DR, new(1, 1) }
        };

        public HashSet<Point<int>> visited = new();

        public Day09() : base(09, 2022, "Rope Bridge")
        {

        }

        public void ResetGame()
        {
            // The tail visits the start
            visited.Clear();
            visited.Add(new(0, 0));
        }

        private void ProcessLine(string line, List<Point<int>> knots)
        {
            // D ##, U ##, L ##, R ##
            var inputs = line.Split(" ", 2, StringSplitOptions.TrimEntries);
            var direction = Enum.Parse<Direction>(inputs[0]);
            var move = Moves[direction];
            var distance = Int32.Parse(inputs[1]);

            // For each step in that direction:
            // Move the head
            // Move the tail
            // Add tail pos to hashtset
            for (int i = 0; i < distance; i++)
            {
                knots[0] += move;

                for (int q = 1; q < knots.Count; q++)
                {
                    // If we are "touching", then we don't move
                    if (
                        knots[q - 1] %knots[q] <= 1
                        ||
                        knots[q - 1] == knots[q] + Moves[Direction.UL]
                        ||
                        knots[q - 1] == knots[q] + Moves[Direction.UR]
                        ||
                        knots[q - 1] == knots[q] + Moves[Direction.DL]
                        ||
                        knots[q - 1] == knots[q] + Moves[Direction.DR]
                    )
                        continue;

                    // If we are the same x or same y, then we move easily
                    if (knots[q - 1]['x'] == knots[q]['x'])
                    {
                        if (knots[q - 1]['y'] > knots[q]['y'])
                            knots[q] = knots[q] + Moves[Direction.D];
                        else
                            knots[q] = knots[q] + Moves[Direction.U];
                    }
                    else if (knots[q - 1]['y'] == knots[q]['y'])
                    {
                        if (knots[q - 1]['x'] > knots[q]['x'])
                            knots[q] = knots[q] + Moves[Direction.R];
                        else
                            knots[q] = knots[q] + Moves[Direction.L];
                    }
                    else
                    {
                        // Different row and column
                        // "the tail always moves one step diagonally to keep up"
                        if (knots[q - 1]['x'] < knots[q]['x'])
                        {
                            if (knots[q - 1]['y'] < knots[q]['y'])
                                knots[q] = knots[q] + Moves[Direction.UL];
                            else
                                knots[q] = knots[q] + Moves[Direction.DL];
                        }
                        else
                        {
                            if (knots[q - 1]['y'] < knots[q]['y'])
                                knots[q] = knots[q] + Moves[Direction.UR];
                            else
                                knots[q] = knots[q] + Moves[Direction.DR];
                        }
                    }
                }

                // Add to the hashset
                visited.Add(knots.Last());
            }
        }

        public void RunGame(string input, int count = 2)
        {
            ResetGame();

            // Generate a list of knots to use
            var knots = Enumerable.Repeat<Point<int>>(new(0, 0), count).ToList();

            foreach(var line in input.SplitByNewline())
                ProcessLine(line, knots);
        }

        protected override string? SolvePartOne()
        {
            RunGame(Input);
            return visited.Count.ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Head + 9 knots behind = 10 knots
            RunGame(Input, 10);
            return visited.Count.ToString();
        }
    }
}

