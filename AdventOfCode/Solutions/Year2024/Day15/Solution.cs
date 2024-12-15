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
        const char boxLeft = '[';
        const char boxRight = ']';

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

            // DebugInput = @"#######
            // #...#.#
            // #.....#
            // #..OO@#
            // #..O..#
            // #.....#
            // #######

            // <vv<<^^<<^^";

            ResetInput();
        }

        public void ResetInput(bool part2 = false)
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

            // If part2, fix things up
            if (!part2) return;

            string newGrid = string.Empty;

            for (int y = 0; y < grid.Length; y++)
            {
                var line = "";
                for (int x = 0; x < grid[0].Length; x++)
                {
                    switch (grid[y][x])
                    {
                        case wall:
                            line += "##";
                            break;

                        case box:
                            line += "[]";
                            break;

                        case open:
                            line += "..";
                            break;
                    }
                }
                newGrid += line + '\n';
            }

            grid = newGrid.ToCharGrid();
            pos = new(pos.x * 2, pos.y);
        }

        /// <summary>
        /// Quickly determine if a point is valid inside grid
        /// </summary>
        public bool InGrid(Point<int> pt) => 0 <= pt.x && pt.x < grid[0].Length && 0 <= pt.y && pt.y < grid.Length;

        private void Move(char instruction)
        {
            var newPos = pos.Clone();
            var moveDelta = Directions.directionPoint[Directions.charToDirection[instruction]];
            newPos += moveDelta;

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
            var boxPoint = newPos + moveDelta;
            while (InGrid(boxPoint))
            {
                if (grid[boxPoint.y][boxPoint.x] == open)
                {
                    // Empty space, we can move
                    // Move one box here and remove it from the newPos location
                    grid[boxPoint.y][boxPoint.x] = box;
                    grid[newPos.y][newPos.x] = open;
                    pos = newPos;
                    return;
                }

                if (grid[boxPoint.y][boxPoint.x] == wall)
                    // Can't move.
                    return;

                // Found another box, check the next point
                boxPoint += moveDelta;
            }
        }

        private void Move2(char instruction)
        {
            // PrintGrid();

            var newPos = pos.Clone();
            var dir = Directions.charToDirection[instruction];
            var moveDelta = Directions.directionPoint[dir];
            newPos += moveDelta;

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
            if (grid[newPos.y][newPos.x] != boxLeft && grid[newPos.y][newPos.x] != boxRight)
                throw new Exception("Unknown char.");

            // Moving left and right is essentially the same
            if (dir == Direction.East || dir == Direction.West)
            {
                var boxPoint1 = newPos + moveDelta;
                var boxPoint2 = newPos + moveDelta + moveDelta;
                while (InGrid(boxPoint1))
                {
                    if (grid[boxPoint2.y][boxPoint2.x] == open)
                    {
                        // Empty space, we can move
                        // Move one box here and remove it from the newPos location
                        var endBox = newPos + moveDelta;
                        var startX = Math.Min(boxPoint2.x, endBox.x);
                        var endX = Math.Max(boxPoint2.x, endBox.x);

                        for (var dx = 0; dx <= endX - startX; dx++)
                        {
                            grid[newPos.y][startX + dx] = dx % 2 == 0 ? boxLeft : boxRight;
                        }

                        grid[newPos.y][newPos.x] = open;
                        pos = newPos;
                        return;
                    }

                    if (grid[boxPoint2.y][boxPoint2.x] == wall)
                        // Can't move.
                        return;

                    // Found another box, check the next point
                    boxPoint1 += moveDelta;
                    boxPoint2 += moveDelta;
                }
            }

            //Moving up or down, we need to check multiple directions at once
            // Keep a list of points we are moving
            HashSet<Point<int>> moving = [];
            // And a list of points to check
            Stack<Point<int>> check = [];

            check.Push(newPos);

            while(check.Count > 0)
            {
                var pt = check.Pop();

                if (!moving.Add(pt))
                    continue;

                // Check the other side of this box
                switch (grid[pt.y][pt.x])
                {
                    // Check Left + Right
                    case boxLeft:
                        check.Push(pt + Directions.directionPoint[Direction.East]);
                        break;

                    case boxRight:
                        check.Push(pt + Directions.directionPoint[Direction.West]);
                        break;
                }

                // We are at a point, we need to check up/down to see if it is valid
                var nextPos = pt + moveDelta;

                if (!InGrid(nextPos)) return;

                switch(grid[nextPos.y][nextPos.x])
                {
                    case wall:
                        return;

                    case open:
                        // Valid, continue
                        break;

                    default:
                        check.Push(nextPos);
                        break;
                }
            }

            // We've made it this far it is valid

            // We need to work with a cloned grid here
            var newGrid = grid.Select(line => line.Select(c => c).ToArray()).ToArray();

            // Clear the spaces
            moving.ForEach(pt =>
            {
                newGrid[pt.y][pt.x] = open;
            });

            // For every moving, we need to move that char one moveDelta
            moving.ForEach(pt =>
            {
                var newPt = pt + moveDelta;

                newGrid[newPt.y][newPt.x] = grid[pt.y][pt.x];
            });

            newGrid[pos.y][pos.x] = open;
            pos = newPos;

            grid = newGrid;
        }

        public int[] GetBoxGPS()
        {
            return Enumerable
                .Range(0, grid.Length)
                .SelectMany(y => Enumerable
                    .Range(0, grid[0].Length)
                    .Select(x => grid[y][x] == box || grid[y][x] == boxLeft ? 100 * y + x : 0)
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
            ResetInput(true);

            instructions.ForEach(Move2);

            // PrintGrid();

            // Time: 00:00:00.1507149
            return GetBoxGPS().Sum().ToString();
        }
    }
}

