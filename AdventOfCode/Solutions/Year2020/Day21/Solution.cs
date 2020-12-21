using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{
    class Food {
        public int id {get;set;}
        public List<string> ingredients {get;set;}
        public List<string> allergens {get;set;}
    }

    class Day21 : ASolution
    {
        List<Food> foods = new List<Food>();
        Dictionary<string, List<string>> ingredientAllergens = new Dictionary<string, List<string>>();
        List<string> allergens = new List<string>();

        public Day21() : base(21, 2020, "")
        {
            int i=0;

            foreach(string line in Input.SplitByNewline(true)) {
                // Ingredients: parts[0]
                // Allergens: parts[1]
                var parts = line.Split("(", StringSplitOptions.TrimEntries);

                var food = new Food();

                food.id = i++;
                food.ingredients = parts[0].Split(" ", StringSplitOptions.TrimEntries).ToList();
                food.allergens = parts[1].Replace("contains ", "").Replace(")", "").Split(",", StringSplitOptions.TrimEntries).ToList();

                // Add to our food list
                this.foods.Add(food);

                // Add to our allergen list
                this.allergens.AddRange(food.allergens);
                this.allergens = this.allergens.Distinct().ToList();
            }

            // Find out all of the allergens <-> ingredient mappings
            // For each food with allergen A listed:
            // * Find all common ingredients
            // * Remove ingredients from that list that are not listed in other foods
            foreach(var allergen in this.allergens) {
                // Find all of the foods that have this
                var tFoods = this.foods.Where(a => a.allergens.Contains(allergen)).ToList();

                // Find all the common ingredients
                // Starting list
                var tIngredients = tFoods[0].ingredients;

                // Find all that match
                foreach(var tFood in tFoods) {
                    tIngredients = tIngredients.Intersect(tFood.ingredients).ToList();
                }

                // Now go through the foods list and find any that have this ingredient but don't have this allergen
                /*
                List<string> remove = new List<string>();
                foreach(var tIngredient in tIngredients)
                    if (this.foods.Where(a => !a.allergens.Contains(allergen) && a.ingredients.Contains(tIngredient)).Count() > 0)
                        remove.Add(tIngredient);

                // Prune the list
                remove.ForEach(a => tIngredients.Remove(a));
                */

                Console.WriteLine($"Allergen: {allergen}, Ingredients: {string.Join(", ", tIngredients)}");
                this.ingredientAllergens.Add(allergen, tIngredients.ToList());
            }
        }

        protected override string SolvePartOne()
        {
            // Get a list of all allergens (not deduplicated)
            var ingredients = this.foods.SelectMany(a => a.ingredients).ToList();

            // Now remove all possible ingredients matched from earlier
            foreach (var kvp in this.ingredientAllergens)
                kvp.Value.ForEach(ing => ingredients.RemoveAll(a => a == ing));

            return ingredients.Count.ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
