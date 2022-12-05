using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Diagnostics;

namespace AdventOfCode.Solutions.Year2022
{

    class Day05 : ASolution
    {
        private Dictionary<int, Stack<char>> stacks = new();

        public Day05() : base(05, 2022, "Supply Stacks")
        {
            var example = @"    [D]    
[N] [C]    
[Z] [M] [P]
 1   2   3 

move 1 from 2 to 1
move 3 from 1 to 3
move 2 from 2 to 1
move 1 from 1 to 2";

            ResetStacks(example);
            RunInstructions(example);
            var output = GetTops();
            Debug.Assert(Debug.Equals(output, "CMZ"), $"Expected: CMZ\nActual: {output}");
        }

        private void ResetStacks(string input)
        {
            stacks = new();

            // Stacks are in group 1
            // Instructions in group 2
            var groups = input.SplitByBlankLine();

            // Stack characters in 1, 5, 9, etc.
            // char index == ((stack - 1) * 4) + 1
            foreach (var line in groups[0])
            {
                for (int stack = 1; stack <= 9; stack++)
                {
                    var cIndex = ((stack - 1) * 4) + 1;

                    // Make sure we don't end early
                    if (cIndex >= line.Length)
                        continue;

                    if (!stacks.ContainsKey(stack))
                        stacks[stack] = new();

                    char c = line[((stack - 1) * 4) + 1];

                    // Make sure this is not the index row
                    if (65 <= c && c <= 90)
                    {
                        stacks[stack].Push(c);
                    }
                }
            }

            // Need to do a quick reverse of the stacks (LIFO)
            foreach(var kvp in stacks)
            {
                var order = kvp.Value.ToList();
                kvp.Value.Clear();
                order.ForEach(c => kvp.Value.Push(c));
            }
        }

        private void PerformLine(string line)
        {
            // Line in format: move 1 from 2 to 1
            var parser = new Regex("^move ([0-9]+) from ([0-9]+) to ([0-9]+)$");

            if (!parser.IsMatch(line))
                return;

            var matches = parser.Matches(line);

            // count = matches[1]
            // from = matches[2]
            // to = matches[3]
            var count = Int32.Parse(matches[0].Groups[1].Value);
            var from = Int32.Parse(matches[0].Groups[2].Value);
            var to = Int32.Parse(matches[0].Groups[3].Value);

            if (!stacks.ContainsKey(from))
                throw new Exception();

            if (!stacks.ContainsKey(to))
                throw new Exception();

            // Move the items
            int taken = 0;
            while(stacks[from].Count > 0 && taken < count)
            {
                stacks[to].Push(stacks[from].Pop());
                taken++;
            }
        }

        private void RunInstructions(string input)
        {
            // Stacks are in group 1
            // Instructions in group 2
            foreach(var line in input.SplitByBlankLine()[1])
                PerformLine(line);
        }

        private string GetTops()
        {
            // Get the tops of the stacks
            return stacks
                .Select(kvp => kvp.Value.Count > 0 ? kvp.Value.Peek().ToString() : "")
                .JoinAsString();
        }

        protected override string? SolvePartOne()
        {
            ResetStacks(Input);
            RunInstructions(Input);
            return GetTops();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

