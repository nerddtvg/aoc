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

        private string DeckId(Queue<int> deck) {
            string o = "";

            foreach(var d in deck)
                o += d.ToString("00");
            
            return o;
        }

        public int PlayRecursiveGame() {
            // Returns 1 for player 1 winning, 2 for player 2
            while(player1.Count > 0 && player2.Count > 0) {
                // First we need to determine if this has been played before
                string key = DeckId(player1) + "-" + DeckId(player2);
                if (this.rounds.Contains(key)) {
                    // Player 1 wins!
                    return 1;
                }

                int p1,p2;
                p1 = this.player1.Dequeue();
                p2 = this.player2.Dequeue();

                // Now add it to the previous round remembering list
                this.rounds.Add(key);

                // If we have at least as many cards in our decks as the top cards for each player (account for popping one off)
                // New game of recursive combat
                int winner;
                if (this.player1.Count >= p1 && this.player2.Count >= p2) {
                    CombatGame game2 = new CombatGame();

                    // New decks are ONLY the quantity of cards drawn (so if p1 == 3, only get 3 cards from the deck)
                    string deck = "";
                    for(int i=0; i<p1; i++)
                        deck += this.player1.ElementAt(i) + "\n";
                    
                    deck += "\n";
                    for(int i=0; i<p2; i++)
                        deck += this.player2.ElementAt(i) + "\n";

                    game2.LoadPlayers(deck.Trim());

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
                    this.player1.Enqueue(p1);
                    this.player1.Enqueue(p2);
                } else if (winner == 2) {
                    this.player2.Enqueue(p2);
                    this.player2.Enqueue(p1);
                }
            }

            return (this.player1.Count > 0 ? 1 : 2);
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
