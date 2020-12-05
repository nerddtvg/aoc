using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2018
{
    class MarbleGame {
        public List<int> bag {get;set;}
        public SortedList<int, int> board{get;set;}
        public int currentMarbleKey {get;set;}
    }

    class MarblePlayer {
        public List<int> bag {get;set;}
        public int score {get;set;}
    }

    class Day09 : ASolution
    {
        MarbleGame game = new MarbleGame();
        List<MarblePlayer> players = new List<MarblePlayer>();
        int playerCount = 0;

        bool draw = false;

        public Day09() : base(09, 2018, "")
        {
            Dictionary<string, int> tests = new Dictionary<string, int>() {
                {"9 players; last marble is worth 25 points", 32},
                {"10 players; last marble is worth 1618 points", 8317},
                {"13 players; last marble is worth 7999 points", 146373},
                {"17 players; last marble is worth 1104 points", 2764},
                {"21 players; last marble is worth 6111 points", 54718},
                {"30 players; last marble is worth 5807 points", 37305}
            };

            // Validate these tests
            foreach(var kvp in tests) {
                Console.Write($"[TEST] {kvp.Key}: Expected {kvp.Value.ToString()}, Received: ");
                RunGame(kvp.Key);
                Console.WriteLine($"{players.OrderByDescending(a => a.score).Select(a => a.score.ToString()).First()}");
            }
        }

        private void RunGame(string input) {
            game = new MarbleGame();
            players = new List<MarblePlayer>();
            playerCount = 0;

            // Sample input: ## players; last marble is worth ### points
            string[] parts = input.Split(" ");

            // Set our player count
            playerCount = Int32.Parse(parts[0]);

            // Setup the player scores
            foreach(int player in Enumerable.Range(0, playerCount))
                players.Add(new MarblePlayer() { bag = new List<int>(), score = 0 });

            // We know how many players and marbles we have now, start loading
            // We get the last marble value so we need 1 extra step
            game.bag = Enumerable.Range(0, Int32.Parse(parts[6])+1).ToList();
            game.board = new SortedList<int, int>();
            game.currentMarbleKey = -1;

            // Offset to account for increment in loop
            int playerTurn = -1;

            // Run the game
            while(game.bag.Count > 0) {
                // If this is the first move, place the first marble and carry on
                if (game.currentMarbleKey == -1) {
                    InsertMarble(0, 0);
                    game.currentMarbleKey = 0;
                    printBoard(playerTurn, 0);
                    continue;
                }

                int oldKey = game.currentMarbleKey;

                // Whose turn? Playing the 0 marble does not count
                playerTurn = ++playerTurn % players.Count;

                int nextMarbleValue = game.bag.First();

                if (nextMarbleValue % 23 != 0) {
                    // Get the position to add this marble into
                    // Between 1 and 2 marbles clockwise means this position will be +2
                    int newKey = getClockwiseKey(2);

                    // We need to check if this is a special case
                    if (newKey == 0)
                        // This should go at the end of the line and not at position 0
                        newKey = game.board.Count;

                    // Add the marble
                    InsertMarble(newKey, nextMarbleValue);

                    // Note the new currentMarbleKey
                    game.currentMarbleKey = newKey;
                } else {
                    // People get points!
                    // First the player gets the next ball in line
                    players[playerTurn].score += nextMarbleValue;
                    players[playerTurn].bag.Add(nextMarbleValue);

                    // Remove this marble from the open bag
                    game.bag.Remove(nextMarbleValue);

                    // Now we also remove the marble 7 spaces counter clockwise
                    int removeKey = getCounterClockwiseKey(7);
                    int newKey = getCounterClockwiseKey(6);
                    int newValue = game.board[newKey];
                    int removedValue = RemoveMarble(removeKey);

                    // Add the removed marble to the player's stash
                    players[playerTurn].score += removedValue;
                    players[playerTurn].bag.Add(removedValue);

                    // Get the new current marble by value (saved earlier)
                    foreach(var i in game.board)
                        if (i.Value == newValue) game.currentMarbleKey = i.Key;
                }

                printBoard(playerTurn, game.currentMarbleKey);
            }
        }

        private void printBoard(int playerTurn, int oldKey) {
            // Write the Line
            if (draw) {
                playerTurn++;

                Console.Write("[" + (playerTurn == -1 ? "--" : playerTurn.ToString("00")) + "] ");
                foreach(var c in game.board) {
                    if (c.Key == oldKey) Console.Write("(");
                    else Console.Write(" ");

                    string v = "   " + c.Value.ToString();
                    Console.Write(v.Substring(v.Length-2));

                    if (c.Key == oldKey) Console.Write(")");
                    else Console.Write(" ");
                }
                Console.WriteLine();
            }
        }

        public int getClockwiseKey(int count, int current) => (current + count) % game.board.Count;

        public int getClockwiseKey(int count) => (game.currentMarbleKey + count) % game.board.Count;

        public int getCounterClockwiseKey(int count) {
            // We need to handle negatives
            int newKey = game.currentMarbleKey - count;
            
            while(newKey < 0)
                newKey += game.board.Count;
            
            return newKey;
        }

        public int RemoveMarble(int index) {
            int marbleValue = game.board[index];

            // We need to remove/shift all of the values down one
            foreach(int key in Enumerable.Range(index, game.board.Count-1-index)) {
                // Starting at the end, move the index we have up one
                game.board[key] = game.board[key+1];
            }

            // Remove the last one in the list because it's useless now
            game.board.RemoveAt(game.board.Count-1);

            return marbleValue;
        }

        public void InsertMarble(int index, int value) {
            // We do this because SortedList does not have an insert+offset option
            if (game.board.Count != index) {
                // We need to bump the rest of the values
                foreach(int key in Enumerable.Range(index, game.board.Count-index).Reverse()) {
                    // Starting at the end, move the index we have up one
                    game.board[key+1] = game.board[key];
                }
            }

            // Remove this marble from the bag
            game.bag.Remove(value);
            game.bag.Sort();

            // Now adding is easy
            game.board[index] = value;
        }

        protected override string SolvePartOne()
        {
            // Run the actual game
            RunGame(Input);
            
            return players.OrderByDescending(a => a.score).Select(a => a.score.ToString()).First();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
