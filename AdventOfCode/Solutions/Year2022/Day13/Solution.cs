using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Diagnostics;
using System.Text.Json.Nodes;

namespace AdventOfCode.Solutions.Year2022
{

    class Day13 : ASolution
    {

        public Day13() : base(13, 2022, "Distress Signal")
        {
            var examples = new Dictionary<string, bool>()
            {
                {
                    @"[1,1,3,1,1]
                    [1,1,5,1,1]",
                    true
                },

                {
                    @"[[1],[2,3,4]]
                    [[1],4]",
                    true
                },

                {
                    @"[9]
                    [[8,7,6]]",
                    false
                },

                {
                    @"[[4,4],4,4]
                    [[4,4],4,4,4]",
                    true
                },

                {
                    @"[7,7,7,7]
                    [7,7,7]",
                    false
                },

                {
                    @"[]
                    [3]",
                    true
                },

                {
                    @"[[[]]]
                    [[]]",
                    false
                },

                {
                    @"[1,[2,[3,[4,[5,6,7]]]],8,9]
                    [1,[2,[3,[4,[5,6,0]]]],8,9]",
                    false
                }
            };

            foreach(var kvp in examples)
            {
                var lines = kvp.Key.SplitByNewline(true);
                var a = ParseLine(lines[0]);
                var b = ParseLine(lines[1]);
                var result = CompareOrder(a, b);
                Debug.Assert(Debug.Equals(result, kvp.Value), $"Input:\n{lines[0]}\n{lines[1]}\n\nExpected: {kvp.Value}\nActual: {result}");
            }
        }

        private static JsonNode ParseLine(string line)
        {
            return JsonArray.Parse(line) ?? throw new Exception();
        }

        private static void ConvertToArray(JsonNode? parent, int index)
        {
            if (parent == default)
                throw new Exception();

            // Change this node from Number to Array with a single number inside
            if (parent[index] is JsonValue value)
            {
                // Convert to an array
                var array = JsonArray.Parse($"[{value.ToJsonString()}]");

                // Assign via parent (to keep the DOM correct)
                parent[index] = array;
            }
            else
            {
                throw new Exception();
            }
        }

        private static int GetIndex(JsonNode child)
        {
            if (child.Parent is JsonArray parent)
                for (int i = 0; i < parent.Count; i++)
                    if (parent[i] == child)
                        return i;

            throw new Exception();
        }

        /// <summary>
        /// Determine if "left" <paramref name="a" /> is in the right order with "right" <paramref name="b" />
        /// </summary>
        /// <returns><c>true</c> if ordered correctlly; otherwise, <c>false</c></returns>
        public static bool? CompareOrder(JsonNode a, JsonNode b)
        {
            if (a is JsonValue valueA && b is JsonValue valueB)
            {
                // Compare two integers, A should be less than or equal to B
                var vA = valueA.GetValue<int>();
                var vB = valueB.GetValue<int>();

                // If we are equal, we move on to the next integer
                if (vA == vB)
                    return null;

                return valueA.GetValue<int>() < valueB.GetValue<int>();
            }

            // If only one is not an Array, then we need to convert it
            if (a is JsonValue)
            {
                var parent = a.Parent;
                var index = GetIndex(a);
                ConvertToArray(parent, index);
                a = parent?[index] ?? throw new Exception();
            }

            if (b is JsonValue)
            {
                var parent = b.Parent;
                var index = GetIndex(b);
                ConvertToArray(parent, index);
                b = parent?[index] ?? throw new Exception();
            }

            // Now compare two array values
            // If both values are lists, compare each item
            // If left runs out first => right orer
            // If right runs out first => wrong over
            // If no decision, move on
            if (a is JsonArray arrayA && b is JsonArray arrayB)
            {
                var maxIndex = Math.Min(arrayA.Count, arrayB.Count);

                for (int i = 0; i < maxIndex; i++)
                {
                    // Check if these are in the right order
                    // If we do not get a value back, then we move on
                    var compare = CompareOrder(arrayA[i] ?? throw new Exception(), arrayB[i] ?? throw new Exception());

                    if (compare.HasValue)
                        return compare.Value;
                }

                // We've compared all of our options up to this point, now we're either out of
                // items or one array is 
                if (arrayA.Count < arrayB.Count)
                    return true;
                if (arrayB.Count < arrayA.Count)
                    return false;

                // No decision, return null
                return null;
            }

            return false;
        }

        protected override string? SolvePartOne()
        {
            int index = 1;
            int returnValue = 0;

            foreach(var group in Input.SplitByBlankLine())
            {
                // Parse the values
                var a = ParseLine(group[0]);
                var b = ParseLine(group[1]);

                // Compare them
                var result = CompareOrder(a, b);

                // If in order, count this index
                if (result ?? false)
                    returnValue += index;

                index++;
            }

            return returnValue.ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Add two packets to the Input:
            // [[2]]
            // [[6]]
            // Order all packets together
            // Find indices of the two packets
            // Multiply the sum
            var input = Input.SplitByNewline(true)
                .Append("[[2]]")
                .Append("[[6]]")
                .Select(item => ParseLine(item))
                .OrderBy(item => item, new OrderItems())
                .ToArray();

            // Getting our indices!
            var returnValue = 1;

            var pattern = new Regex(@"^\[+(2|6)\]+$");

            for(var i = 0; i < input.Length; i++)
            {
                // These could have been extended, so match any number of brackets
                if (pattern.IsMatch(input[i].ToJsonString()))
                    returnValue *= (i + 1);
            }

            return returnValue.ToString();
        }

        private class OrderItems : IComparer<JsonNode>
        {
            int IComparer<JsonNode>.Compare(JsonNode? a, JsonNode? b)
            {
                if (a == default || b == default)
                    throw new Exception();

                var result = Day13.CompareOrder(a, b);

                // Equal somehow
                if (!result.HasValue)
                    return 0;

                // In order, so a<b
                if (result.Value)
                    return -1;

                // Out of order
                return 1;
            }
        }
    }
}

