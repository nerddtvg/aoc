using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{
    class CupGame {
        public LinkedList<int> cups {get;set;}
        private LinkedListNode<int> currentCup {get;set;}
        public int highest {get;set;}

        // Keep a record of all nodes
        private Dictionary<int, LinkedListNode<int>> references {get;set;}

        public CupGame(string input, int padding=0) => Init(input.ToIntArray(), padding);

        public CupGame(int[] input, int padding=0) => Init(input, padding);

        private void Init(int[] input, int padding) {
            // Initialize
            this.cups = new LinkedList<int>();
            this.references = new Dictionary<int, LinkedListNode<int>>();
            
            // Load in order
            foreach(var i in input)
                this.addCup(i);
            
            // Set the current cup to the start
            this.currentCup = this.cups.First;

            // Get the highest value for later
            this.highest = this.cups.Max(a => a);

            // Now we will also pad this as requested
            for(int i=this.highest+1; i<=padding; i++)
                this.addCup(i);

            // Reset the highest
            this.highest = Math.Max(padding, this.highest);
        }

        private void addCup(int value) {
            this.cups.AddLast(value);
            this.references.Add(value, this.cups.Last);
        }

        public void playRound() {
            // Get our hand
            List<int> hand = new List<int>();
            
            // Get and remove three nodes
            for(int i=0; i<3; i++) {
                var tNode = getNextNode(this.currentCup);
                hand.Add(tNode.Value);

                // Remove from the lists
                this.references.Remove(tNode.Value);
                this.cups.Remove(tNode);
            }

            int destination = this.currentCup.Value - 1;
            LinkedListNode<int> destinationCup = null;

            while(destinationCup == null) {
                // Loop through the list of cups to find the destination value
                // If it is not found, reduce the destination value
                // If destination value < 1, loop to the highest value of cup
                if (this.references.ContainsKey(destination)) {
                    destinationCup = this.references[destination];
                }

                // Decrement
                destination--;

                if (destinationCup == null) {
                    // Not found
                    // We've already reduced destination
                    if (destination < 1) destination = this.highest;
                }
            }

            // Now we have a destination cup
            // Place our hand back down (we reverse the order so it is placed back correctly)
            hand.Reverse();
            foreach(var i in hand) {
                this.cups.AddAfter(destinationCup, i);
                this.references.Add(i, destinationCup.Next);
            }
            
            // New current cup is +1
            this.currentCup = getNextNode(this.currentCup);
        }

        // Help with the wrap around
        private LinkedListNode<int> getNextNode(LinkedListNode<int> node) {
            if (node == null)
                node = this.currentCup;
            
            // When we hit the last node, Next is null
            if (node.Next == null)
                return this.cups.First;
            else
                return node.Next;
        }

        public long getCupsAfter1() {
            // For Part 2 we only want the two cups immediately after '1'
            // Multiply them together
            var start = this.cups.Find(1);
            long output;

            start = getNextNode(start);
            output = start.Value;

            start = getNextNode(start);
            output *= start.Value;

            return output;
        }

        public override string ToString()
        {
            // This is how we will return our score
            // Start *after* the cup labeled 1 and put each number in place
            // Stop at the number *before* 1, so 1 does not appear
            var start = this.cups.Find(1);
            string output = "";

            do {
                // Get the next cup
                start = getNextNode(start);

                // Did we hit the end?
                if (start.Value == 1) break;

                output += start.Value.ToString();
            } while(true);

            return output;
        }
    }

    class Day23 : ASolution
    {
        CupGame game;

        public Day23() : base(23, 2020, "")
        {
            //DebugInput = "389125467";
        }

        protected override string SolvePartOne()
        {
            game = new CupGame(Input);
            for(int i=0; i<100; i++)
                game.playRound();

            return game.ToString();
        }

        protected override string SolvePartTwo()
        {
            // Load the initial game
            game = new CupGame(Input, 1000000);

            // Get a stopwatch ready!
            var sw = new System.Diagnostics.Stopwatch();

            Console.WriteLine($"Part 2 Started");

            Console.WriteLine($"Part 2 Loading: {new TimeSpan(sw.ElapsedTicks)}");

            // Now play the game ten million (10000000) times
            sw.Reset();
            sw.Start();
            for(int i=0; i<10000000; i++) {
                game.playRound();

                // Every 100,000 print time
                if (i > 0 && i % 100000 == 0)
                    Console.WriteLine($"Part 2 Round {i.ToString("N0")}: {new TimeSpan(sw.ElapsedTicks)}");
            }
            sw.Stop();

            Console.WriteLine($"Part 2 Calculation: {new TimeSpan(sw.ElapsedTicks)}");

            Console.WriteLine($"Part 2 Complete");
            
            // Now we only want the two cups immediately clockwise of cup 1
            return game.getCupsAfter1().ToString();
        }
    }
}
