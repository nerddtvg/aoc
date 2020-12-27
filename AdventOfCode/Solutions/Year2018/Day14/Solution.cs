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

        public Day14() : base(14, 2018, "")
        {
            // Load the input into our linked list
            this.recipes.AddLast(3);
            this.recipes.AddLast(7);

            // Setup the elves
            this.elves.Add(recipes.First);
            this.elves.Add(recipes.Last);

            //DebugInput = "9";
            //DebugInput = "5";
            //DebugInput = "18";
            //DebugInput = "2018";
        }

        private LinkedListNode<int> getRecipeCount(LinkedListNode<int> node, int x=0) {
            for(int i=0; i<x; i++)
                if (node.Next == null)
                    node = node.List.First;
                else
                    node = node.Next;
            
            return node;
        }

        private void runRound() {
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
            foreach(var digit in digits)
                this.recipes.AddLast(digit);
            
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
                runRound();
            
            string ret = "";

            // Now get the recipies
            for(int i=0; i<10; i++)
                ret += this.recipes.ElementAt<int>(recipeCount+i).ToString();

            return ret;
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
