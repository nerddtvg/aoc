using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{
    class Food
    {
        public int id { get; set; }
        public List<string> ingredients { get; set; } = new();
        public List<string> allergens { get; set; } = new();
    }

    class Day21 : ASolution
    {
        List<Food> foods = new List<Food>();
        Dictionary<string, List<string>> ingredientAllergens = new Dictionary<string, List<string>>();
        List<string> allergens = new List<string>();

        public Day21() : base(21, 2020, "")
        {
            int i = 0;

            foreach (string line in Input.SplitByNewline(true))
            {
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
            foreach (var allergen in this.allergens)
            {
                // Find all of the foods that have this
                var tFoods = this.foods.Where(a => a.allergens.Contains(allergen)).ToList();

                // Find all the common ingredients
                // Starting list
                var tIngredients = tFoods[0].ingredients;

                // Find all that match
                foreach (var tFood in tFoods)
                {
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
            // Now we need to figure out what ingredient has what allergen
            // We will work to reduce the ingredientAllergens list to do this

            while (true)
            {
                bool removed = false;

                // Get a list of all ingredientAllergens that have only one ingredient (known match)
                var ingredients = this.ingredientAllergens.Where(a => a.Value.Count == 1).Select(a => a.Value[0]);

                // Remove any from the lists
                foreach (var ing in ingredients)
                    foreach (var kvp in this.ingredientAllergens)
                    {
                        // Only remove if we have more than one listed
                        if (kvp.Value.Count == 1) continue;

                        bool tRemoved = this.ingredientAllergens[kvp.Key].Remove(ing);

                        // Save this if we removed anything
                        removed = removed || tRemoved;
                    }

                if (!removed) break;
            }

            // Blank line for easier reading
            Console.WriteLine("");

            foreach (var kvp in this.ingredientAllergens)
                Console.WriteLine($"Allergen: {kvp.Key}, Ingredients: {string.Join(", ", kvp.Value)}");

            // Now we return a string of ingredients sorted by allergen alphabetically
            List<string> output = new List<string>();
            foreach (var key in this.ingredientAllergens.Keys.OrderBy(a => a))
                output.Add(this.ingredientAllergens[key][0]);

            return string.Join(",", output);
        }
    }
}
