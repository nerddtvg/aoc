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
        public Dictionary<Direction, (int x, int y)> Moves = new()
        {
            { Direction.U, (0, -1) },
            { Direction.D, (0, 1) },
            { Direction.L, (-1, 0) },
            { Direction.R, (1, 0) },
            { Direction.UL, (-1, -1) },
            { Direction.UR, (1, -1) },
            { Direction.DL, (-1, 1) },
            { Direction.DR, (1, 1) }
        };

        public HashSet<(int x, int y)> visited = new();

        public Day09() : base(09, 2022, "Rope Bridge")
        {

        }

        public void ResetGame()
        {
            // The tail visits the start
            visited.Clear();
            visited.Add((0, 0));
        }

        private void ProcessLine(string line, List<(int x, int y)> knots)
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
                knots[0] = knots[0].Add(move);

                for (int q = 1; q < knots.Count; q++)
                {
                    // If we are "touching", then we don't move
                    if (
                        knots[q - 1].ManhattanDistance(knots[q]) <= 1
                        ||
                        knots[q - 1] == knots[q].Add(Moves[Direction.UL])
                        ||
                        knots[q - 1] == knots[q].Add(Moves[Direction.UR])
                        ||
                        knots[q - 1] == knots[q].Add(Moves[Direction.DL])
                        ||
                        knots[q - 1] == knots[q].Add(Moves[Direction.DR])
                    )
                        continue;

                    // If we are the same x or same y, then we move easily
                    if (knots[q - 1].x == knots[q].x)
                    {
                        if (knots[q - 1].y > knots[q].y)
                            knots[q] = knots[q].Add(Moves[Direction.D]);
                        else
                            knots[q] = knots[q].Add(Moves[Direction.U]);
                    }
                    else if (knots[q - 1].y == knots[q].y)
                    {
                        if (knots[q - 1].x > knots[q].x)
                            knots[q] = knots[q].Add(Moves[Direction.R]);
                        else
                            knots[q] = knots[q].Add(Moves[Direction.L]);
                    }
                    else
                    {
                        // Different row and column
                        // "the tail always moves one step diagonally to keep up"
                        if (knots[q - 1].x < knots[q].x)
                        {
                            if (knots[q - 1].y < knots[q].y)
                                knots[q] = knots[q].Add(Moves[Direction.UL]);
                            else
                                knots[q] = knots[q].Add(Moves[Direction.DL]);
                        }
                        else
                        {
                            if (knots[q - 1].y < knots[q].y)
                                knots[q] = knots[q].Add(Moves[Direction.UR]);
                            else
                                knots[q] = knots[q].Add(Moves[Direction.DR]);
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
            var knots = Enumerable.Repeat<(int x, int y)>((0, 0), count).ToList();

            foreach(var line in input.SplitByNewline())
                ProcessLine(line, knots);
        }

        protected override string? SolvePartOne()
        {
            return null;
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

