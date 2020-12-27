using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2018
{

    class Day14 : ASolution
    {
        LinkedList<int> recipes = new LinkedList<int>();
        List<LinkedListNode<int>> elves = new List<LinkedListNode<int>>();
        List<LinkedListNode<int>> found = new List<LinkedListNode<int>>();

        public Day14() : base(14, 2018, "")
        {
            //DebugInput = "9";
            //DebugInput = "5";
            //DebugInput = "18";
            //DebugInput = "2018";

            //DebugInput = "51589";
            //DebugInput = "01245";
            //DebugInput = "92510";
            //DebugInput = "59414";

            this.LoadInput();
        }

        private void LoadInput() {
            // Load the input into our linked list
            this.recipes.Clear();
            this.recipes.AddLast(3);
            this.recipes.AddLast(7);

            // Setup the elves
            this.elves.Clear();
            this.elves.Add(recipes.First);
            this.elves.Add(recipes.Last);

            // Remove the old entries
            this.found.Clear();
        }

        private LinkedListNode<int> getRecipeCount(LinkedListNode<int> node, int x=0) {
            for(int i=0; i<x; i++)
                if (node.Next == null)
                    node = node.List.First;
                else
                    node = node.Next;
            
            return node;
        }

        private void runRound(string firstChar = "") {
            // First thing we do is sum up all of the elves
            // Should never be higher than 18 (0-9 * 2)
            int sum = this.elves.Sum(a => a.Value);

            // Now we need to add all of the digits
            List<int> digits = new List<int>();

            // We have a tens place
            if (sum >= 10)
                digits.Add(1);

            // Add the ones place
            digits.Add(sum % 10);

            // Add them
            foreach(var digit in digits) {
                var t = this.recipes.AddLast(digit);

                // Save instances of our first character to ease in finding things later
                if (!string.IsNullOrWhiteSpace(firstChar) && digit.ToString() == firstChar)
                    this.found.Add(t);
            }
            
            // Now find the elves next nodes...
            List<LinkedListNode<int>> tElves = new List<LinkedListNode<int>>();
            this.elves.ForEach(e => {
                int i = e.Value + 1;
                tElves.Add(this.getRecipeCount(e, i));
            });

            // Replace the values
            this.elves = tElves;
        }

        protected override string SolvePartOne()
        {
            // We need to make as many recipes as our puzzle input says + 10
            int recipeCount = Int32.Parse(Input);
            while(this.recipes.Count < recipeCount + 10)
                this.runRound();
            
            string ret = "";

            // Now get the recipies
            for(int i=0; i<10; i++)
                ret += this.recipes.ElementAt<int>(recipeCount+i).ToString();

            return ret;
        }

        protected override string SolvePartTwo()
        {
            this.LoadInput();

            // Run through the rounds
            // Look for instances of our puzzle input
            while(true) {
                this.runRound(Input.Substring(0, 1));

                // Look to see if our Puzzle input appears anywhere
                // We know where our first character lives throughout the puzzle, we should find it
                // Remove any that don't match to save time later
                List<LinkedListNode<int>> remove = new List<LinkedListNode<int>>();

                for(int i=0; i<this.found.Count; i++) {
                    // Get the next 5 digits and check them
                    string temp = "";
                    var startNode = this.found[i];
                    var node = startNode;

                    for(int q=0; node != null && q<Input.Length; q++) {
                        temp += node.Value.ToString();
                        node = node.Next;
                    }

                    // If we had a null entry, we hit the end of the list before the length is right
                    if (temp.Length < Input.Length) continue;
                    
                    if (temp == Input) {
                        // Found it!
                        // Unfortunately we don't have an indexing solution for C# LinkedLists so we loop this...
                        var tempNode = startNode;
                        int count = 0;

                        while(tempNode.Previous != null) {
                            count++;
                            tempNode = tempNode.Previous;
                        }

                        return count.ToString();
                    }

                    // Not found, remove this from our found list
                    remove.Add(startNode);
                }

                // Got a list of "found" entries we don't care about anymore
                remove.ForEach(a => this.found.Remove(a));
            }

            return null;
        }
    }
}
