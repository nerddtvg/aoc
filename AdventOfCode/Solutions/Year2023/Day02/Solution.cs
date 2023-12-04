using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2023
{

    class Day02 : ASolution
    {

        private List<Game> Games = new();
        private class Game
        {
            public int id;
            public List<GameRound> rounds = new();
        }
        private class GameRound
        {
            public int r = 0;
            public int b = 0;
            public int g = 0;
        }

        public Day02() : base(02, 2023, "Cube Conundrum")
        {
            // Read the inputs
            var gameRe = new Regex(@"^Game ([\d]+): (.+)$");
            var roundRe = new Regex(@"(\d+) (blue|red|green)(|, (\d+) (blue|red|green)(|, (\d+) (blue|red|green)))(; |$)");

            Games = Input.SplitByNewline()
                .Select(line => gameRe.Match(line))
                .Select(match => new Game
                {
                    id = int.Parse(match.Groups[1].Value),
                    rounds = roundRe.Matches(match.Groups[2].Value)
                        .Select(round => new GameRound
                        {
                            r = (round.Groups[2].Value == "red" ? int.Parse(round.Groups[1].Value) : 0) + (round.Groups[5].Value == "red" ? int.Parse(round.Groups[4].Value) : 0) + (round.Groups[8].Value == "red" ? int.Parse(round.Groups[7].Value) : 0),
                            g = (round.Groups[2].Value == "green" ? int.Parse(round.Groups[1].Value) : 0) + (round.Groups[5].Value == "green" ? int.Parse(round.Groups[4].Value) : 0) + (round.Groups[8].Value == "green" ? int.Parse(round.Groups[7].Value) : 0),
                            b = (round.Groups[2].Value == "blue" ? int.Parse(round.Groups[1].Value) : 0) + (round.Groups[5].Value == "blue" ? int.Parse(round.Groups[4].Value) : 0) + (round.Groups[8].Value == "blue" ? int.Parse(round.Groups[7].Value) : 0)
                        })
                        .ToList()
                }
                )
                .ToList();
        }

        /// <summary>
        /// Return the games that are possible with the provided maximum values
        /// </summary>
        private List<Game> FindPossibleGames(int maxR, int maxG, int maxB)
        {
            return Games.Where(game => game.rounds.All(round => round.r <= maxR && round.g <= maxG && round.b <= maxB)).ToList();
        }

        protected override string? SolvePartOne()
        {
            return FindPossibleGames(12, 13, 14).Sum(game => game.id).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return Games
                .Select(game => game.rounds.Max(round => (ulong)round.r) * game.rounds.Max(round => (ulong)round.b) * game.rounds.Max(round => (ulong)round.g))
                .Sum(s => s)
                .ToString();
        }
    }
}

