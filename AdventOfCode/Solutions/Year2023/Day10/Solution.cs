using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Diagnostics;


namespace AdventOfCode.Solutions.Year2023
{
    using Point = (int x, int y);

    class Day10 : ASolution
    {
        /// <summary>
        /// Types of tile
        /// </summary>
        public enum Tile
        {
            Vertical,
            Horizontal,
            NorthEast,
            NorthWest,
            SouthWest,
            SouthEast,
            Ground,
            Start
        }

        public Dictionary<char, Tile> TileMap = new()
        {
            { '|', Tile.Vertical },
            { '-', Tile.Horizontal },
            { 'L', Tile.NorthEast },
            { 'J', Tile.NorthWest },
            { '7', Tile.SouthWest },
            { 'F', Tile.SouthEast },
            { '.', Tile.Ground },
            { 'S', Tile.Start },
        };

        /// <summary>
        /// Possible directions to travel for each tile type
        /// </summary>
        public Dictionary<Tile, List<Point>> Directions = new()
        {
            // South and north
            { Tile.Vertical, new() { (0, 1), (0, -1) } },
            // East and west
            { Tile.Horizontal, new() { (1, 0), (-1, 0) } },
            // North and east
            { Tile.NorthEast, new() { (0, -1), (1, 0) } },
            // North and wet
            { Tile.NorthWest, new() { (0, -1), (-1, 0) } },
            // South and west
            { Tile.SouthWest, new() { (0, 1), (-1, 0) } },
            // South and east
            { Tile.SouthEast, new() { (0, 1), (1, 0) } },
            // No options
            { Tile.Ground, new() },
            // Start has all options
            { Tile.Start, new() { (0, 1), (0, -1), (-1, 0), (1, 0) } }
        };

        public Point Start = (0, 0);
        public Tile StartTile = Tile.Start;
        public string[] Grid;

        /// <summary>
        /// Track our loop tiles
        /// </summary>
        public List<Point> Loop = new();

        public Day10() : base(10, 2023, "Pipe Maze")
        {
            // DebugInput = @"FF7FSF7F7F7F7F7F---7
            //                L|LJ||||||||||||F--J
            //                FL-7LJLJ||||||LJL-77
            //                F--JF--7||LJLJ7F7FJ-
            //                L---JF-JLJ.||-FJLJJ7
            //                |F|F-JF---7F7-L7L|7|
            //                |FFJF7L7F-JF7|JL---7
            //                7-L-JL7||F7|L7F-7F7|
            //                L.L7LFJ|||||FJL7||LJ
            //                L7JLJL-JLJLJL--JLJ.L";

            // DebugInput = @".F----7F7F7F7F-7....
            //                .|F--7||||||||FJ....
            //                .||.FJ||||||||L7....
            //                FJL7L7LJLJ||LJ.L-7..
            //                L--J.L7...LJS7F-7L7.
            //                ....F-J..F7FJ|L7L7L7
            //                ....L7.F7||L7|.L7L7|
            //                .....|FJLJ|FJ|F7|.LJ
            //                ....FJL-7.||.||||...
            //                ....L---J.LJ.LJLJ...";

            // Find our starting point
            Grid = Input.SplitByNewline(true).ToArray();

            if (TileMap[Grid[0][0]] != Tile.Start)
                for(int y=0; y<Grid.Length && Start == (0, 0); y++)
                    for(int x=0; x<Grid[y].Length && Start == (0, 0); x++)
                        if (TileMap[Grid[y][x]] == Tile.Start)
                            Start = (x, y);

            // Now identify what the start tile is
            // Look at neighboring tiles and see if they can move into Start
            var north = GetTile((Start.x, Start.y - 1));
            var canNorth = Directions[north].Contains((0, 1));

            var east = GetTile((Start.x + 1, Start.y));
            var canEast = Directions[east].Contains((-1, 0));

            var west = GetTile((Start.x - 1, Start.y));
            var canWest = Directions[west].Contains((1, 0));

            if (canNorth)
            {
                if (canEast)
                {
                    StartTile = Tile.NorthEast;
                }
                else if (canWest)
                {
                    StartTile = Tile.NorthWest;
                }
                else
                {
                    StartTile = Tile.Vertical;
                }
            }
            else if (canEast && canWest)
            {
                StartTile = Tile.Horizontal;
            }
            else if (canEast)
            {
                StartTile = Tile.SouthEast;
            }
            else
            {
                StartTile = Tile.SouthWest;
            }

            // Replace the start tile
            Grid[Start.y] = Grid[Start.y].Replace('S', TileMap.First(kvp => kvp.Value == StartTile).Key);
        }

        public Point[] GetMoves(Point point)
        {
            var points = new List<Point>();

            foreach (var direction in Directions[GetTile(point)])
                points.Add(point.Add(direction));

            return points.ToArray();
        }

        public char GetGridPoint(Point point) => point.y >= 0 && point.y < Grid.Length && point.x >= 0 && point.x < Grid[point.y].Length ? Grid[point.y][point.x] : '.';
        public Tile GetTile(Point point) => TileMap[GetGridPoint(point)];

        protected override string? SolvePartOne()
        {
            int distance = 0;
            Loop.Clear();

            // Make our first move
            var pos = Start;

            do
            {
                // For each pos, get our moves
                // Check this against lastPos
                // Choose to move to the one that is not lastPos
                var move = GetMoves(pos).First(m => Loop.Count == 0 || m != Loop[^1]);
                Loop.Add(pos);
                pos = move;
                distance++;
            } while (pos != Start);

            return (distance / 2).ToString();
        }

        private string GetColumn(Point point, bool up = true)
        {
            string ret = string.Empty;

            for(int y=point.y + (up ? -1 : 1); y>=0 && y<Grid.Length; )
            {
                if (up)
                {
                    ret = string.Concat(Grid[y][point.x], ret);
                    y--;
                }
                else
                {
                    ret += Grid[y][point.x];
                    y++;
                }
            }

            return ret;
        }

        protected override string? SolvePartTwo()
        {
            // Find all tiles that are not a part of the loop
            // Took a hint from the megathread to use the even-odd rule
            // And we need to consider that F-7 and L-J traveling horizontally and
            // Vertically:
            // 7     F
            // | and |
            // J     L
            // do not count as crossing a pipe
            // Valid combinations:
            // Horizontal: |, F---J, L---7
            // Vertical: -
            // 7     F
            // | and |
            // L and J
            int insideCount = 0;

            // I can try to use regular expressions to do the magic stuff
            var horizRegex = new Regex(@"\||F\-*J|L\-*7");
            var vertRegex = new Regex(@"\-|7\|*L|F\|*J");

            // For a proper count, we need to clear out the junk pieces
            // Anything not in the loop needs to be reset to Ground
            var resetGrid = new List<string>();
            for (int y = 0; y < Grid.Length; y++)
            {
                var str = string.Empty;

                for (int x = 0; x < Grid[y].Length; x++)
                    if (Loop.Contains((x, y)))
                        str += Grid[y][x];
                    else
                        str += '.';

                resetGrid.Add(str);
            }

            // Reset our grid
            Grid = resetGrid.ToArray();

            // For each direction, count the number of matches
            // If all directions are odd, then we are inside
            for(int y=1; y<Grid.Length-1; y++)
            {
                for(int x=1; x<Grid[y].Length-1; x++)
                {
                    if (Grid[y][x] != '.') continue;

                    if (
                        horizRegex.Matches(Grid[y].Substring(0, x)).Count % 2 == 1
                        &&
                        horizRegex.Matches(Grid[y].Substring(x + 1)).Count % 2 == 1
                        &&
                        vertRegex.Matches(GetColumn((x, y), true)).Count % 2 == 1
                        &&
                        vertRegex.Matches(GetColumn((x, y), false)).Count % 2 == 1
                    )
                        insideCount++;
                }
            }

            return insideCount.ToString();
        }
    }
}

