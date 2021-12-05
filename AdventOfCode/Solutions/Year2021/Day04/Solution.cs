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
            // This tracks if this has been solved or not
            private bool solved = false;

            public List<Tile> tiles { get; set; } = new List<Tile>();

            public bool IsWinner()
            {
                // This will only have to calculate up until the first time it was solved
                solved = solved ||
                    // Reduced the code down to be a little more dynamic
                    Enumerable
                        .Range(0, 5)
                        // This will return true if any of the conditions are true (a row or column is filled)
                        .Any(index =>
                        {
                            return
                            // Check rows with index starting at index*5
                            (tiles[(index * 5)].marked && tiles[(index * 5) + 1].marked && tiles[(index * 5) + 2].marked && tiles[(index * 5) + 3].marked && tiles[(index * 5) + 4].marked)
                            ||
                            // Check columns
                            (tiles[index].marked && tiles[index + 5].marked && tiles[index + 10].marked && tiles[index + 15].marked && tiles[index + 20].marked);
                        });

                return solved;
            }

            public void Reset() => this.solved = false;

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
            // Reset the called numbers and boards
            this.tiles.ForEach(tile => tile.marked = false);
            this.boards.ForEach(board => board.Reset());

            // Go through each called value, mark the tile, check for winners
            foreach(var called in this.calledTiles)
            {
                var tile = this.tiles.FirstOrDefault(tile => tile.value == called);

                if (tile == default)
                    throw new InvalidOperationException();

                // Start by predeterminig if this is the last board
                // Now that we save the winning status when won, this shouldn't be _too_ inefficient
                var unwon = this.boards.Where(board => !board.IsWinner()).ToList();

                // Mark this tile
                tile.marked = true;

                // Now recheck!
                if (unwon.Count == 1 && unwon[0].IsWinner())
                {
                    // Found the last winner!
                    Console.WriteLine($"Last Winning Board: {this.boards.IndexOf(unwon[0])}");

                    return unwon[0].GetScore(called).ToString();
                }
            }

            return null;
        }
    }
}

#nullable restore
