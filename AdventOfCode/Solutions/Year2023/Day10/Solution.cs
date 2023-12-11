using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


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

        public Day10() : base(10, 2023, "Pipe Maze")
        {
            // Find our starting point
            Grid = Input.SplitByNewline().ToArray();

            if (TileMap[Grid[0][0]] != Tile.Start)
                for(int y=0; y<Grid.Length && Start == (0, 0); y++)
                    for(int x=0; x<Grid[y].Length && Start == (0, 0); x++)
                        if (TileMap[Grid[y][x]] == Tile.Start)
                            Start = (x, y);

            // Now identify what the start tile is
            // Look at neighboring tiles and see if they can move into Start
            var north = TileMap[Grid[Start.y - 1][Start.x]];
            var canNorth = Directions[north].Contains((0, 1));

            var east = TileMap[Grid[Start.y][Start.x + 1]];
            var canEast = Directions[east].Contains((-1, 0));

            var west = TileMap[Grid[Start.y][Start.x - 1]];
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
        }

        public Point[] GetMoves(Point point)
        {
            var points = new List<Point>();

            foreach (var direction in Directions[GetTile(point)])
                points.Add(point.Add(direction));

            return points.ToArray();
        }

        public char GetGridPoint(Point point) => Grid[point.y][point.x];
        public Tile GetTile(Point point) => TileMap[GetGridPoint(point)];

        protected override string? SolvePartOne()
        {
            int distance = 1;
            var lastPos = Start;

            // Make our first move
            var pos = Directions[GetTile(Start)].First().Add(Start);

            do
            {
                // For each pos, get our moves
                // Check this against lastPos
                // Choose to move to the one that is not lastPos
                var move = GetMoves(pos).First(m => m != lastPos);

                lastPos = pos;
                pos = move;
                distance++;
            } while (pos != Start);

            return (distance / 2).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

