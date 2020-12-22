using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{
    class CombatGame {
        private Queue<int> player1 = new Queue<int>();
        private Queue<int> player2 = new Queue<int>();

        List<string> rounds = new List<string>();

        public void LoadPlayers(string input) {
            int playerC = 1;

            player1.Clear();
            player2.Clear();

            foreach(var player in input.SplitByBlankLine(true)) {
                    foreach(var line in player) {
                        try {
                            if (playerC == 1)
                                player1.Enqueue(Int32.Parse(line));
                            else {
                                player2.Enqueue(Int32.Parse(line));
                            }
                        } catch {
                            // Skips the invalid player line
                        }
                    }

                playerC++;
            }
        }

        private void PlayClassicGame() {
            int p1 = player1.Dequeue();
            int p2 = player2.Dequeue();

            if (p1 > p2) {
                player1.Enqueue(p1);
                player1.Enqueue(p2);
            } else {
                player2.Enqueue(p2);
                player2.Enqueue(p1);
            }
        }

        public int PlayRecursiveGame() {
            // Returns 1 for player 1 winning, 2 for player 2
            while(player1.Count > 0 && player2.Count > 0) {
                // First we need to determine if this has been played before
                string key = player1.First().ToString("00") + player2.First().ToString("00");
                if (rounds.Contains(key)) {
                    // Player 1 wins!
                    return 1;
                }

                int p1,p2;
                p1 = player1.Dequeue();
                p2 = player2.Dequeue();

                //Console.WriteLine();
                //Console.WriteLine($"Player 1's Deck: {string.Join(", ", player1)}");
                //Console.WriteLine($"Player 2's Deck: {string.Join(", ", player2)}");

                // Now add it to the previous round remembering list
                rounds.Add(key);

                // If we have at least as many cards in our decks as the top cards for each player (account for popping one off)
                // New game of recursive combat
                int winner;
                if (player1.Count >= p1 && player2.Count >= p2) {
                    CombatGame game2 = new CombatGame();
                    game2.LoadPlayers(string.Join("\n", player1) + "\n\n" + string.Join("\n", player2));

                    winner = game2.PlayRecursiveGame();
                } else {
                    // If not any of the above, the winner has the higher card
                    if (p1 > p2) {
                        winner = 1;
                    } else {
                        winner = 2;
                    }
                }
                    
                if (winner == 1) {
                    player1.Enqueue(p1);
                    player1.Enqueue(p2);
                } else if (winner == 2) {
                    player2.Enqueue(p2);
                    player2.Enqueue(p1);
                }
            }

            return (player1.Count > 0 ? 1 : 2);
        }

        public int PlayFullGame(int part=1) {
            if (part == 1) {
                // Classic game
                while(player1.Count > 0 && player2.Count > 0) {PlayClassicGame();}
            } else {
                PlayRecursiveGame();
            }

            // Winner?
            var player = player1.Count > 0 ? player1 : player2;

            // Total
            int total = 0;
            int i = player.Count;

            while(player.Count > 0) {
                total += i-- * player.Dequeue();
            }

            return total;
        }
    }

    class Day22 : ASolution
    {
        CombatGame game = new CombatGame();

        public Day22() : base(22, 2020, "")
        {
            /** /
            DebugInput = @"
            Player 1:
            9
            2
            6
            3
            1

            Player 2:
            5
            8
            4
            7
            10";
            /**/
        }

        protected override string SolvePartOne()
        {
            game.LoadPlayers(Input);
            return game.PlayFullGame(1).ToString();
        }

        protected override string SolvePartTwo()
        {
            game.LoadPlayers(Input);
            return game.PlayFullGame(2).ToString();
        }
    }
}
