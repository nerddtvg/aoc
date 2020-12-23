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

        public CupGame(string input) => Init(input.ToIntArray());

        public CupGame(int[] input) => Init(input);

        private void Init(int[] input) {
            // Initialize
            this.cups = new LinkedList<int>();
            
            // Load in order
            foreach(var i in input)
                this.cups.AddLast(i);
            
            // Set the current cup to the start
            this.currentCup = this.cups.First;

            // Get the highest value for later
            this.highest = this.cups.Max(a => a);
        }

        public void playRound() {
            // Get our hand
            List<int> hand = new List<int>();
            
            // Get and remove three nodes
            for(int i=0; i<3; i++) {
                var tNode = getNextNode(this.currentCup);
                hand.Add(tNode.Value);
                this.cups.Remove(tNode);
            }

            int destination = this.currentCup.Value - 1;
            LinkedListNode<int> destinationCup = null;

            while(destinationCup == null) {
                // Loop through the list of cups to find the destination value
                // If it is not found, reduce the destination value
                // If destination value < 1, loop to the highest value of cup
                destinationCup = this.cups.Find(destination--);

                if (destinationCup == null) {
                    // Not found
                    // We've already reduced destination
                    if (destination < 1) destination = this.highest;
                }
            }

            // Now we have a destination cup
            // Place our hand back down (we reverse the order so it is placed back correctly)
            hand.Reverse();
            foreach(var i in hand)
                this.cups.AddAfter(destinationCup, i);
            
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
            // DebugInput = "389125467";
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
            game = new CupGame(Input);

            // Get a stopwatch ready!
            var sw = new System.Diagnostics.Stopwatch();

            Console.WriteLine($"Part 2 Started");

            // Need to increase the cup count to 1 million (1000000)
            sw.Start();
            for(int i=game.highest+1; i<=1000000; i++)
                game.cups.AddLast(i);
            sw.Stop();

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
