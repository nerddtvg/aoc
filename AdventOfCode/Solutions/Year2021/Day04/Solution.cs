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

            public bool IsWinner() =>
                // Any row
                (tiles[0].marked && tiles[1].marked && tiles[2].marked && tiles[3].marked && tiles[4].marked)
                ||
                (tiles[5].marked && tiles[6].marked && tiles[7].marked && tiles[8].marked && tiles[9].marked)
                ||
                (tiles[10].marked && tiles[11].marked && tiles[12].marked && tiles[13].marked && tiles[14].marked)
                ||
                (tiles[15].marked && tiles[16].marked && tiles[17].marked && tiles[18].marked && tiles[19].marked)
                ||
                (tiles[20].marked && tiles[21].marked && tiles[22].marked && tiles[23].marked && tiles[24].marked)
                ||
                // Any column
                (tiles[0].marked && tiles[5].marked && tiles[10].marked && tiles[15].marked && tiles[20].marked)
                ||
                (tiles[1].marked && tiles[6].marked && tiles[11].marked && tiles[16].marked && tiles[21].marked)
                ||
                (tiles[2].marked && tiles[7].marked && tiles[12].marked && tiles[17].marked && tiles[22].marked)
                ||
                (tiles[3].marked && tiles[8].marked && tiles[13].marked && tiles[18].marked && tiles[23].marked)
                ||
                (tiles[4].marked && tiles[9].marked && tiles[14].marked && tiles[19].marked && tiles[24].marked);

            public int GetScore(int multipler) => multipler * this.tiles.Where(tile => !tile.marked).Sum(tile => tile.value);
        }

        // All tiles
        public List<Tile> tiles { get; set; } = new List<Tile>();

        // All boards
        public List<Board> boards { get; set; } = new List<Board>();

        // The order of being marked
        public List<int> calledTiles { get; set; } = new List<int>();

        public Day04() : base(04, 2021, "Giant Squid")
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
            // Go through each called value, mark the tile, check for winners
            foreach(var called in this.calledTiles)
            {
                var tile = this.tiles.FirstOrDefault(tile => tile.value == called);

                if (tile == default)
                    throw new InvalidOperationException();

                tile.marked = true;

                // Check for a winner
                foreach(var board in this.boards)
                {
                    if (board.IsWinner())
                    {
                        // Found a winner!
                        Console.WriteLine($"Winning Board: {this.boards.IndexOf(board)}");

                        return board.GetScore(called).ToString();
                    }
                }
            }

            return null;
        }

        protected override string? SolvePartTwo()
        {
            // Reset the called numbers
            this.tiles.ForEach(tile => tile.marked = false);

            // Track the won boards
            var wonBoards = new HashSet<int>();

            // Go through each called value, mark the tile, check for winners
            foreach(var called in this.calledTiles)
            {
                var tile = this.tiles.FirstOrDefault(tile => tile.value == called);

                if (tile == default)
                    throw new InvalidOperationException();

                tile.marked = true;

                for (int i = 0; i < this.boards.Count; i++)
                {
                    // Skip if already known
                    if (wonBoards.Contains(i))
                        continue;

                    if (this.boards[i].IsWinner())
                    {
                        wonBoards.Add(i);

                        if (wonBoards.Count == this.boards.Count)
                        {
                            // Found the last winner!
                            Console.WriteLine($"Last Winning Board: {i}");

                            return this.boards[i].GetScore(called).ToString();
                        }
                    }
                }
            }

            return null;
        }
    }
}

#nullable restore
