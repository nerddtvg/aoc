using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2019
{

    class Day22 : ASolution
    {
        private List<int> deck = null;

        public Day22() : base(22, 2019, "")
        {

        }

        private void resetDeck(int count=10007) {
            this.deck = new List<int>();

            for(int i=0; i<count; i++)
                this.deck.Add(i);
        }

        private void runOperation(string operation) {
            var parts = operation.Split(" ");

            if (operation == "deal into new stack") {
                this.deck.Reverse();
            } else if (parts[0] == "deal" && parts[1] == "with") {
                int index = Int32.Parse(parts[3]);
                int divisor = this.deck.Count / index;

                // Dealing out is fun
                var newList = new Dictionary<int, int>();

                // for each old position, we need to know the new index
                // it will be (old_pos * index) % this.deck.count
                for(int i=0; i<this.deck.Count; i++)
                    newList.Add((i * index) % this.deck.Count, this.deck[i]);
                
                // Save the new list
                this.deck = newList.OrderBy(a => a.Key).Select(a => a.Value).ToList();
            } else if (parts[0] == "cut") {
                int index = Int32.Parse(parts[1]);
                int start = 0;

                // Start from the back
                if (index < 0)
                    start = this.deck.Count + index;
                
                var removed = this.deck.GetRange(start, Math.Abs(index));
                this.deck.RemoveRange(start, Math.Abs(index));

                // If this was from the back, add it to the front
                if (start > 0)
                    this.deck.InsertRange(0, removed);
                else
                    this.deck.AddRange(removed);
            }
        }

        private int findCard(int v) {
            for(int i=0; i<this.deck.Count; i++)
                if (this.deck[i] == v)
                    return i;
            
            return -1;
        }

        protected override string SolvePartOne()
        {
            this.resetDeck(10007);

            foreach(var line in Input.SplitByNewline(true, true))
                this.runOperation(line);

            return findCard(2019).ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
