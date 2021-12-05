using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2021
{

    class Day04 : ASolution
    {
        // A board will be a list of tiles in order going across row first, then down
        // A tile will be a class that has a value and marked/unmarked
        // Tiles will be shared between boards
        public class Tile
        {
            public int value { get; set; } = 0;
            public bool marked { get; set; } = false;
        }

        public class Board
        {
            public List<Tile> tiles { get; set; } = new List<Tile>();
        }

        // All tiles
        public List<Tile> tiles { get; set; } = new List<Tile>();

        // All boards
        public List<Board> boards { get; set; } = new List<Board>();

        // The order of being marked
        public List<int> calledTiles { get; set; } = new List<int>();

        public Day04() : base(04, 2021, "")
        {
            // Generate the list of tiles
            calledTiles = Input.SplitByNewline().First().ToIntArray(",").ToList();

            // Reset our tiles
            tiles = Enumerable.Range(0, 100).Select(val => new Tile() { value = val }).ToList();

            // Read in the boards
            boards = Input
                .SplitByBlankLine()
                // Skip the first line which is the called tiles
                .Skip(1)
                .Select(board =>
                    new Board()
                    {
                        // Compact the multi-line string, split it out into values, then find the appropriate tiles that match in order
                        tiles = string.Join(" ", string.Join(" ", board).Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)).ToIntArray(" ").Select(val => this.tiles.First(tile => tile.value == val)).ToList()
                    }
                ).ToList();
        }

        protected override string? SolvePartOne()
        {
            return null;
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
