using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Threading.Tasks.Dataflow;


namespace AdventOfCode.Solutions.Year2023
{

    class Day04 : ASolution
    {

        public Day04() : base(04, 2023, "Scratchcards")
        {

        }

        protected override string? SolvePartOne()
        {
            // If we make the assumption that no number will appear twice
            // on a line besides being a matching number, this can be done
            // relatively quickly by finding all digits and then finding
            // pairs
            return Input.SplitByNewline()
                // Get the "winning numbers | card" portion
                .Select(line => line.Split(":", 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[1])
                .Select(card =>
                {
                    var matches = new Regex(@"\d+")
                        // Finds all the of digits
                        .Matches(card)
                        // Group the digits together
                        .GroupBy(digit => digit.Value)
                        // Find the keys where there are 2 matches
                        .Where(group => group.Count() == 2)
                        // Total the number of keys
                        .Count();

                    // Find our score
                    if (matches == 0) return 0;

                    // 1 match => 1
                    // 2 matches => 2
                    // 3 matches => 4
                    // 4 matches => 8
                    // ...
                    return Math.Pow(2, matches - 1);
                })
                .Sum()
                .ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Empty counts
            int[] cardCounts = Enumerable.Repeat(0, 200).ToArray();

            int cardIdx = 0;

            // For each card, find the number of matches
            // Then math it out!
            Input.SplitByNewline()
                // Get the "winning numbers | card" portion
                .ToList().ForEach(line =>
                {
                    // cardIdx is one off
                    cardIdx++;

                    // We may have zero or more of these cards
                    // defined by copies, so don't set the count
                    // to one, we need to increment it
                    cardCounts[cardIdx]++;

                    var matches = new Regex(@"\d+")
                        // Finds all the of digits
                        .Matches(line.Split(":", 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[1])
                        // Group the digits together
                        .GroupBy(digit => digit.Value)
                        // Find the keys where there are 2 matches
                        .Where(group => group.Count() == 2)
                        // Total the number of keys
                        .Count();

                    // For each of our matches, we increment up
                    // to that number of cards after for "copies"
                    for(var i = cardIdx + 1; matches > 0; matches--, i++)
                    {
                        // We need to increment by the number of
                        // cardCounts for this card
                        cardCounts[i] += cardCounts[cardIdx];
                    }
                });

            return cardCounts.Sum().ToString();
        }
    }
}

