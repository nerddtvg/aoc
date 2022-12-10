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

        public (int x, int y) head = (0, 0);
        public (int x, int y) tail = (0, 0);

        public Day09() : base(09, 2022, "Rope Bridge")
        {
            
        }

        public void ResetGame()
        {
            head = (0, 0);
            tail = (0, 0);

            // The tail visits the start
            visited.Clear();
            visited.Add(tail);
        }

        private void ProcessLine(string line)
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
                head = head.Add(move);

                // If we are "touching", then we don't move
                if (
                    head.ManhattanDistance(tail) == 1
                    ||
                    head == tail.Add(Moves[Direction.UL])
                    ||
                    head == tail.Add(Moves[Direction.UR])
                    ||
                    head == tail.Add(Moves[Direction.DL])
                    ||
                    head == tail.Add(Moves[Direction.DR])
                )
                    continue;

                // If we are the same x or same y, then we move easily
                if (head.x == tail.x)
                {
                    if (head.y > tail.y)
                        tail = tail.Add(Moves[Direction.D]);
                    else if (head.y < tail.y)
                        tail = tail.Add(Moves[Direction.U]);
                }
                else if (head.y == tail.y)
                {
                    if (head.x > tail.x)
                        tail = tail.Add(Moves[Direction.R]);
                    else if (head.x < tail.x)
                        tail = tail.Add(Moves[Direction.L]);
                }
                else
                {
                    // Different row and column
                    // "the tail always moves one step diagonally to keep up"
                    if (head.x < tail.x)
                    {
                        if (head.y < tail.y)
                            tail = tail.Add(Moves[Direction.UL]);
                        else
                            tail = tail.Add(Moves[Direction.DL]);
                    }
                    else
                    {
                        if (head.y < tail.y)
                            tail = tail.Add(Moves[Direction.UR]);
                        else
                            tail = tail.Add(Moves[Direction.DR]);
                    }
                }

                // Add to the hashset
                visited.Add(tail);
            }
        }

        public void RunGame(string input)
        {
            ResetGame();

            foreach(var line in input.SplitByNewline())
                ProcessLine(line);
        }

        protected override string? SolvePartOne()
        {
            RunGame(Input);
            return visited.Count.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

