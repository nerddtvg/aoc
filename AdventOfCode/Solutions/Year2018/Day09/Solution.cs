using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2018
{
    class MarbleGame {
        public List<ulong> bag {get;set;}
        public LinkedList<ulong> board{get;set;}
        public LinkedListNode<ulong> currentMarble {get;set;}
    }

    class MarblePlayer {
        public ulong score {get;set;}
    }

    class Day09 : ASolution
    {
        MarbleGame game = new MarbleGame();
        List<MarblePlayer> players = new List<MarblePlayer>();
        int playerCount = 0;

        bool draw = false;

        public Day09() : base(09, 2018, "")
        {
            Dictionary<string, ulong> tests = new Dictionary<string, ulong>() {
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
                RunGame(ParseGame(kvp.Key));
                Console.WriteLine($"{players.OrderByDescending(a => a.score).Select(a => a.score.ToString()).First()}");
            }
        }

        private (int playerCount, int finalMarble) ParseGame(string input) {
            // Sample input: ## players; last marble is worth ### points
            string[] parts = input.Split(" ");

            return (Int32.Parse(parts[0]), Int32.Parse(parts[6]));
        }

        private void RunGame((int playerCount, int finalMarble) input) {
            game = new MarbleGame();
            players = new List<MarblePlayer>();
            playerCount = 0;

            // Set our player count
            playerCount = input.playerCount;

            // Setup the player scores
            foreach(ulong player in Enumerable.Range(0, playerCount))
                players.Add(new MarblePlayer() { score = 0 });

            // We know how many players and marbles we have now, start loading
            // We get the last marble value so we need 1 extra step
            game.bag = Enumerable.Range(0, input.finalMarble+1).Select(a => Convert.ToUInt64(a)).ToList();
            game.board = new LinkedList<ulong>();

            // Offset to account for increment in loop
            int playerTurn = -1;

            // Run the game
            while(game.bag.Count > 0) {
                // If this is the first move, place the first marble and carry on
                if (game.board.Count == 0) {
                    InsertMarble(0, 0);
                    game.currentMarble = game.board.Find(0);
                    printBoard(playerTurn, game.currentMarble);
                    continue;
                }

                // Whose turn? Playing the 0 marble does not count
                playerTurn = ++playerTurn % players.Count;

                ulong nextMarbleValue = game.bag.First();

                if (nextMarbleValue % 23 != 0) {
                    // Get the position to add this marble into
                    // Between 1 and 2 marbles clockwise means this position will be +2
                    var after = getNode(1);

                    // Add the marble after
                    game.currentMarble = InsertMarble(after, nextMarbleValue);
                } else {
                    // People get points!
                    // First the player gets the next ball in line
                    players[playerTurn].score += nextMarbleValue;

                    // Remove this marble from the open bag
                    game.bag.Remove(nextMarbleValue);

                    // Get the values to remove and the next currentMarble
                    // New position is 6 counter-clockwise, removing 7 counter-clockwise (one more)
                    game.currentMarble = getNode(-6);
                    LinkedListNode<ulong> removeMarble = getNode(-1);

                    // Add the removed marble to the player's stash
                    players[playerTurn].score += removeMarble.Value;
                    
                    // Remove the node
                    game.board.Remove(removeMarble);
                }

                printBoard(playerTurn, game.currentMarble);
            }
        }

        private LinkedListNode<ulong> getNode(int step) {
            LinkedListNode<ulong> temp = game.currentMarble;

            // How many to go?
            int count = Math.Abs(step);
            
            if (step < 0)
                for(int i=0; i<count; i++)
                    if (temp.Previous == null)
                        temp = game.board.Last;
                    else
                        temp = temp.Previous;
            else
                for(int i=0; i<count; i++)
                    if (temp.Next == null)
                        temp = game.board.First;
                    else
                        temp = temp.Next;

            return temp;
        }

        private void printBoard(int playerTurn, LinkedListNode<ulong> currentMarble) {
            // Write the Line
            if (draw) {
                playerTurn++;

                Console.Write("[" + (playerTurn == -1 ? "--" : playerTurn.ToString("00")) + "] ");
                foreach(var c in game.board) {
                    if (c == currentMarble.Value) Console.Write("(");
                    else Console.Write(" ");

                    string v = "   " + c.ToString();
                    Console.Write(v.Substring(v.Length-2));

                    if (c == currentMarble.Value) Console.Write(")");
                    else Console.Write(" ");
                }
                Console.WriteLine();
            }
        }

        public LinkedListNode<ulong> InsertMarble(LinkedListNode<ulong> after, ulong value) {
            // Remove this marble from the bag
            game.bag.Remove(value);
            game.bag.Sort();

            // Now adding is easy (null means it is the first)
            if (after == null)
                if (game.board.Count == 0)
                    return game.board.AddFirst(value);
                else
                    return game.board.AddLast(value);
            else
                return game.board.AddAfter(after, value);
        }

        public LinkedListNode<ulong> InsertMarble(ulong after, ulong value) {
            return InsertMarble(game.board.Find(after), value);
        }

        protected override string SolvePartOne()
        {
            // Run the actual game
            RunGame(ParseGame(Input));

            return players.OrderByDescending(a => a.score).Select(a => a.score.ToString()).First();
        }

        protected override string SolvePartTwo()
        {
            var input = ParseGame(Input);

            // Part 2 is much longer
            input.finalMarble *= 100;
            RunGame(input);

            return players.OrderByDescending(a => a.score).Select(a => a.score.ToString()).First();
        }
    }
}
