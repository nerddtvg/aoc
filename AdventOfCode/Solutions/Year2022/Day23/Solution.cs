using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2022
{

    class Day23 : ASolution
    {
        public struct Elf
        {
            public int id;
            public int x;
            public int y;
        }

        public enum Direction
        {
            North,
            South,
            West,
            East
        }

        public List<Elf> elves = new();
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
            int id = 0;
            foreach (var line in Input.SplitByNewline(true))
            {
                int x = 0;
                foreach (var c in line.ToCharArray())
                {
                    if (c == '#')
                        elves.Add(new() { x = x, y = y, id = id++ });

                    x++;
                }
                y++;
            }
        }

        public bool ElfExists(int x, int y) => elves.Any(e => e.x == x && e.y == y);

        public bool ElfNorth(Elf elf) => ElfExists(elf.x - 1, elf.y - 1) || ElfExists(elf.x, elf.y - 1) || ElfExists(elf.x + 1, elf.y - 1);
        public bool ElfEast(Elf elf) => ElfExists(elf.x + 1, elf.y - 1) || ElfExists(elf.x + 1, elf.y) || ElfExists(elf.x + 1, elf.y + 1);
        public bool ElfWest(Elf elf) => ElfExists(elf.x - 1, elf.y - 1) || ElfExists(elf.x - 1, elf.y) || ElfExists(elf.x - 1, elf.y + 1);
        public bool ElfSouth(Elf elf) => ElfExists(elf.x - 1, elf.y + 1) || ElfExists(elf.x, elf.y + 1) || ElfExists(elf.x + 1, elf.y + 1);
        public bool HasElfNeighbors(Elf elf) => ElfNorth(elf) || ElfSouth(elf) || ElfWest(elf) || ElfEast(elf);

        public int RunRound()
        {
            // First, figure out which elves are not moving or which want to move

            /// <summary>
            /// These are the elves that proposed moving
            /// </summary>
            var newElves = new List<Elf>();

            /// <summary>
            /// These are the elves that cannot move this round
            /// </summary>
            var finalElves = new List<Elf>();

            // What direction are we checking? Add it back to the end of the list
            var direction = directions.Dequeue();
            directions.Enqueue(direction);

            foreach (var elf in elves)
            {
                // No neighbors, no movement
                if (!HasElfNeighbors(elf))
                {
                    finalElves.Add(elf);
                    continue;
                }

                var proposed = false;

                // Start with the above direction
                // Then try the rest in order
                for (int i = 0; i < 4 && !proposed; i++)
                {
                    var thisDir = (Direction)(((int)direction + i) % 4);

                    if (thisDir == Direction.North && !ElfNorth(elf))
                    {
                        // Propose we move up
                        newElves.Add(elf with { y = elf.y - 1 });
                        proposed = true;
                    }
                    else if (thisDir == Direction.South && !ElfSouth(elf))
                    {
                        // Propose we move down
                        newElves.Add(elf with { y = elf.y + 1 });
                        proposed = true;
                    }
                    else if (thisDir == Direction.West && !ElfWest(elf))
                    {
                        // Propose we move left
                        newElves.Add(elf with { x = elf.x - 1 });
                        proposed = true;
                    }
                    else if (thisDir == Direction.East && !ElfEast(elf))
                    {
                        // Propose we move right
                        newElves.Add(elf with { x = elf.x + 1 });
                        proposed = true;
                    }
                }

                if (!proposed)
                {
                    // Blocked, no movement
                    newElves.Add(elf);
                }
            }

            // Now check the proposals for any overlap
            var overlaps = newElves
                .GroupBy(e => (e.x, e.y))
                .Where(grp => grp.Count() > 1)
                .SelectMany(grp => grp.Select(e => e))
                .ToList();

            var overlapIds = overlaps.Select(e => e.id).ToList();

            // Remove the overlaps
            newElves.RemoveAll(e => overlaps.Contains(e));

            // Add any remaining movements
            finalElves.AddRange(newElves);

            // Add back the original overlap positions
            finalElves.AddRange(elves.Where(e => overlapIds.Contains(e.id)));

            // Finalize the move
            elves = finalElves;

            return newElves.Count;
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
            return string.Empty;
        }
    }
}

