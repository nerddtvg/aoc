using System;
using System.Collections.Generic;
using System.Text;

using System.Text.RegularExpressions;
using System.Linq;

namespace AdventOfCode.Solutions.Year2015
{

    class Day15 : ASolution
    {
        private Dictionary<string, (int capacity, int durability, int flavor, int texture, int calories)> _ingredients = new Dictionary<string, (int capacity, int durability, int flavor, int texture, int calories)>();

        public Day15() : base(15, 2015, "")
        {
            // Samples:
            // Butterscotch: capacity -1, durability -2, flavor 6, texture 3, calories 8
            // Cinnamon: capacity 2, durability 3, flavor -2, texture -1, calories 3
            var matches = Regex.Matches(Input, "([a-z]+): capacity ([-0-9]+), durability ([-0-9]+), flavor ([-0-9]+), texture ([-0-9]+), calories ([-0-9]+)", RegexOptions.IgnoreCase);

            foreach(Match match in matches)
            {
                this._ingredients[match.Groups[1].Value] = (Int32.Parse(match.Groups[2].Value), Int32.Parse(match.Groups[3].Value), Int32.Parse(match.Groups[4].Value), Int32.Parse(match.Groups[5].Value), Int32.Parse(match.Groups[6].Value));
            }
        }

        private int GetScore(int[] amounts)
        {
            // We have an array of amounts for each ingredient to try and score
            (int capacity, int durability, int flavor, int texture, int calories) score = (0, 0, 0, 0, 0);

            var names = this._ingredients.Keys.ToArray();
            for (int i = 0; i < amounts.Length; i++)
            {
                // We look at each ingredient in our list and get our scores
                var tScore = GetIngredientScore(amounts[i], this._ingredients[names[i]]);

                score.capacity += tScore.capacity;
                score.durability += tScore.durability;
                score.flavor += tScore.flavor;
                score.texture += tScore.texture;
            }

            return (score.capacity <= 0 || score.durability <= 0 || score.flavor <= 0 || score.texture <= 0 ? 0 : score.capacity * score.durability * score.flavor * score.texture);
        }

        private (int capacity, int durability, int flavor, int texture, int calories) GetIngredientScore(int amount, (int capacity, int durability, int flavor, int texture, int calories) ingredient)
        {
            // Figure out the score for this
            return (amount * ingredient.capacity, amount * ingredient.durability, amount * ingredient.flavor, amount * ingredient.texture, ingredient.calories);
        }

        protected override string SolvePartOne()
        {
            // We need to determine how much for each ingredient to use
            var amounts = new List<int[]>();

            for (int a = 100; a >= 0; a--)
                for (int b = 100-a; b >= 0; b--)
                    for (int c = 100-b; c >= 0; c--)
                        for (int d = 100-c; d >= 0; d--)
                        {
                            if (a + b + c + d != 100) continue;
                            amounts.Add(new int[] { a, b, c, d });
                        }

            // Now that we have a list of possible amounts, figure out how highest score
            int maxScore = 0;
            foreach(var amount in amounts)
            {
                int tScore = GetScore(amount);

                maxScore = Math.Max(tScore, maxScore);
            }

            return maxScore.ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
