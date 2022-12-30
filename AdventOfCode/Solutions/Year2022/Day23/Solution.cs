using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2022
{

    class Day23 : ASolution
    {
        public enum Direction
        {
            North,
            South,
            West,
            East
        }

        public HashSet<(int x, int y)> elves = new();
        public Queue<Direction> directions = new();

        public Day23() : base(23, 2022, "Unstable Diffusion")
        {
            // DebugInput = @"....#..
            // ..###.#
            // #...#.#
            // .#...##
            // #.###..
            // ##.#.##
            // .#..#..";

            ReadElves();
        }

        private void ReadElves()
        {
            elves.Clear();

            directions = new(new Direction[] { Direction.North, Direction.South, Direction.West, Direction.East });

            int y = 0;
            foreach (var line in Input.SplitByNewline(true))
            {
                int x = 0;
                foreach (var c in line.ToCharArray())
                {
                    if (c == '#')
                        elves.Add((x, y));

                    x++;
                }
                y++;
            }
        }

        public bool ElfExists(int x, int y) => elves.Contains((x, y));

        public bool ElfNorth((int x, int y) elf) => ElfExists(elf.x - 1, elf.y - 1) || ElfExists(elf.x, elf.y - 1) || ElfExists(elf.x + 1, elf.y - 1);
        public bool ElfEast((int x, int y) elf) => ElfExists(elf.x + 1, elf.y - 1) || ElfExists(elf.x + 1, elf.y) || ElfExists(elf.x + 1, elf.y + 1);
        public bool ElfWest((int x, int y) elf) => ElfExists(elf.x - 1, elf.y - 1) || ElfExists(elf.x - 1, elf.y) || ElfExists(elf.x - 1, elf.y + 1);
        public bool ElfSouth((int x, int y) elf) => ElfExists(elf.x - 1, elf.y + 1) || ElfExists(elf.x, elf.y + 1) || ElfExists(elf.x + 1, elf.y + 1);
        public bool HasElfNeighbors((int x, int y) elf) => ElfNorth(elf) || ElfSouth(elf) || ElfWest(elf) || ElfEast(elf);

        public int RunRound()
        {
            // First, figure out which elves are not moving or which want to move

            /// <summary>
            /// These are the elves that proposed moving
            /// </summary>
            var newElves = new HashSet<(int x, int y)>();

            /// <summary>
            /// These are the elves that cannot move this round
            /// </summary>
            var finalElves = new HashSet<(int x, int y)>();

            // What direction are we checking? Add it back to the end of the list
            var direction = directions.Dequeue();
            directions.Enqueue(direction);

            int movedCount = 0;

            foreach (var elf in elves)
            {
                // No neighbors, no movement
                if (!HasElfNeighbors(elf))
                {
                    newElves.Add(elf);
                    continue;
                }

                var proposed = false;

                // Start with the above direction
                // Then try the rest in order

                // Optimization from mega thread:
                // A conflict can only occur during movement
                // and from the opposite direction, so we can handle it here
                for (int i = 0; i < 4 && !proposed; i++)
                {
                    var thisDir = (Direction)(((int)direction + i) % 4);

                    if (thisDir == Direction.North && !ElfNorth(elf))
                    {
                        // Propose we move up
                        var newElf = elf with { y = elf.y - 1 };
                        if (newElves.Contains(newElf))
                        {
                            // No movement
                            newElves.Add(elf);

                            newElves.Remove(newElf);
                            newElves.Add(newElf with { y = newElf.y - 1 });

                            movedCount--;
                        }
                        else
                        {
                            newElves.Add(newElf);

                            movedCount++;
                        }

                        proposed = true;
                    }
                    else if (thisDir == Direction.South && !ElfSouth(elf))
                    {
                        // Propose we move down
                        var newElf = elf with { y = elf.y + 1 };
                        if (newElves.Contains(newElf))
                        {
                            // No movement
                            newElves.Add(elf);

                            newElves.Remove(newElf);
                            newElves.Add(newElf with { y = newElf.y + 1 });

                            movedCount--;
                        }
                        else
                        {
                            newElves.Add(newElf);

                            movedCount++;
                        }

                        proposed = true;
                    }
                    else if (thisDir == Direction.West && !ElfWest(elf))
                    {
                        // Propose we move left
                        var newElf = elf with { x = elf.x - 1 };
                        if (newElves.Contains(newElf))
                        {
                            // No movement
                            newElves.Add(elf);

                            newElves.Remove(newElf);
                            newElves.Add(newElf with { x = newElf.x - 1 });

                            movedCount--;
                        }
                        else
                        {
                            newElves.Add(newElf);

                            movedCount++;
                        }

                        proposed = true;
                    }
                    else if (thisDir == Direction.East && !ElfEast(elf))
                    {
                        // Propose we move right
                        var newElf = elf with { x = elf.x + 1 };
                        if (newElves.Contains(newElf))
                        {
                            // No movement
                            newElves.Add(elf);

                            newElves.Remove(newElf);
                            newElves.Add(newElf with { x = newElf.x + 1 });

                            movedCount--;
                        }
                        else
                        {
                            newElves.Add(newElf);

                            movedCount++;
                        }

                        proposed = true;
                    }
                }

                if (!proposed)
                {
                    // Blocked, no movement
                    newElves.Add(elf);
                }
            }

            // Finalize the move
            elves = newElves;

            return movedCount;
        }

        private void PrintGrid()
        {
            int minX = elves.Min(e => e.x);
            int minY = elves.Min(e => e.y);
            int maxX = elves.Max(e => e.x);
            int maxY = elves.Max(e => e.y);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    if (ElfExists(x, y))
                        Console.Write('#');
                    else
                        Console.Write('.');
                }
                Console.WriteLine();
            }

            Console.WriteLine();
        }

        private int GetScore()
        {
            // Determine the bounding box area
            // Subtract the elves count
            int minX = elves.Min(e => e.x);
            int minY = elves.Min(e => e.y);
            int maxX = elves.Max(e => e.x);
            int maxY = elves.Max(e => e.y);

            return ((maxX - minX + 1) * (maxY - minY + 1)) - elves.Count;
        }

        protected override string? SolvePartOne()
        {
            for (int i = 0; i<10; i++)
            {
                RunRound();
            }

            return GetScore().ToString();
        }

        protected override string? SolvePartTwo()
        {
            for (int i = 1; i < 1000; i++)
            {
                var moved = RunRound();

                // We are continuing from Part 1
                if (moved == 0)
                    return (10 + i).ToString();
            }

            return string.Empty;
        }
    }
}

