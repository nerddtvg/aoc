using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    class Day15 : ASolution
    {
        public required char[][] grid;
        public required char[] instructions;
        public required Point<int> pos;

        const char box = 'O';
        const char wall = '#';
        const char open = '.';
        const char robot = '@';

        public Day15() : base(15, 2024, "Warehouse Woes")
        {
            // DebugInput = @"##########
            // #..O..O.O#
            // #......O.#
            // #.OO..O.O#
            // #..O@..O.#
            // #O#..O...#
            // #O..O..O.#
            // #.OO.O.OO#
            // #....O...#
            // ##########

            // <vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<<<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^>^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^<><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^";

            // DebugInput = @"########
            // #..O.O.#
            // ##@.O..#
            // #...O..#
            // #.#.O..#
            // #...O..#
            // #......#
            // ########

            // <^^>>>vv<v>>v<<";

            ResetInput();
        }

        public void ResetInput()
        {
            var split = Input.SplitByBlankLine(true);
            grid = string.Join('\n', split[0]).ToCharGrid();
            instructions = split[1].JoinAsString().ToCharArray();

            bool found = false;
            for (int y = 0; y < grid.Length && found == false; y++)
                for (int x = 0; x < grid[0].Length; x++)
                {
                    if (grid[y][x] == robot)
                    {
                        found = true;
                        pos = new(x, y);
                        grid[y][x] = open;
                        break;
                    }
                }
        }

        /// <summary>
        /// Quickly determine if a point is valid inside grid
        /// </summary>
        public bool InGrid(Point<int> pt) => 0 <= pt.x && pt.x < grid[0].Length && 0 <= pt.y && pt.y < grid.Length;

        private void Move(char instruction)
        {
            var newPos = pos.Clone();
            var dir = Directions.directionPoint[Directions.charToDirection[instruction]];
            newPos += dir;

            if (!InGrid(newPos))
                return;

            // Wall
            if (grid[newPos.y][newPos.x] == wall)
                return;

            // Empty space
            if (grid[newPos.y][newPos.x] == open)
            {
                pos = newPos;
                return;
            }

            // Other: Boxes!
            if (grid[newPos.y][newPos.x] != box)
                throw new Exception("Unknown char.");

            // We need to check all the way to the end of this set of boxes (if there is more than one)
            var boxPoint = newPos + dir;
            while (InGrid(boxPoint))
            {
                if (grid[boxPoint.y][boxPoint.x] == open)
                {
                    // Empty space, we can move
                    // Move one box here and remove it from the newPos location
                    grid[boxPoint.y][boxPoint.x] = box;
                    grid[newPos.y][newPos.x] = open;
                    pos = newPos;
                    break;
                }

                if (grid[boxPoint.y][boxPoint.x] == wall)
                    // Can't move.
                    break;

                // Found another box, check the next point
                boxPoint += dir;
            }
        }

        public int[] GetBoxGPS()
        {
            return Enumerable
                .Range(0, grid.Length)
                .SelectMany(y => Enumerable
                    .Range(0, grid[0].Length)
                    .Select(x => grid[y][x] == box ? 100 * y + x : 0)
                )
                .ToArray();
        }

        private void PrintGrid()
        {
            Console.WriteLine(string.Join('\n', grid.Select(line => line.JoinAsString()).ToArray()));
        }

        protected override string? SolvePartOne()
        {
            instructions.ForEach(Move);

            // PrintGrid();

            // Time: 00:00:00.0438353
            return GetBoxGPS().Sum().ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

