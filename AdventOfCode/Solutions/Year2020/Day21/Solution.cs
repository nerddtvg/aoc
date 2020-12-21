using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{
    class Food {
        public List<string> ingredients {get;set;}
        public List<string> allergens {get;set;}
    }

    class Day21 : ASolution
    {
        List<Food> foods = new List<Food>();

        public Day21() : base(21, 2020, "")
        {
            foreach(string line in Input.SplitByNewline(true)) {
                // Ingredients: parts[0]
                // Allergens: parts[1]
                var parts = line.Split("(", StringSplitOptions.TrimEntries);

                var food = new Food();

                food.ingredients = parts[0].Split(" ", StringSplitOptions.TrimEntries).ToList();
                food.allergens = parts[1].Replace("contains ", "").Replace(")", "").Split(",", StringSplitOptions.TrimEntries).ToList();

                foods.Add(food);
            }
        }

        protected override string SolvePartOne()
        {
            return null;
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
