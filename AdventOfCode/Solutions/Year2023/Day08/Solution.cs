using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2023
{
    class Day08 : ASolution
    {
        private class Node
        {
            public string L = string.Empty;
            public string R = string.Empty;
        }

        private Dictionary<string, Node> map;
        private string instructions;

        public Day08() : base(08, 2023, "Haunted Wasteland")
        {
            var groups = Input.SplitByBlankLine();

            instructions = groups[0].JoinAsString();

            map = groups[1].Select(line =>
            {
                var matches = new Regex(@"^([A-Z]+) = \(([A-Z]+), ([A-Z]+)\)").Match(line);

                return new KeyValuePair<string, Node>(matches.Groups[1].Value, new Node() { L = matches.Groups[2].Value, R = matches.Groups[3].Value });
            }).ToDictionary();
        }

        protected override string? SolvePartOne()
        {
            var step = 0;

            // Track our node and instruction count
            var currentNode = "AAA";
            var currentIdx = 0;

            do
            {
                if (instructions[currentIdx++] == 'L')
                    currentNode = map[currentNode].L;
                else
                    currentNode = map[currentNode].R;

                currentIdx %= instructions.Length;
                step++;
            } while (currentNode != "ZZZ");

            return step.ToString();
        }

        protected override string? SolvePartTwo()
        {
            // First brute force method was taking too long
            // This indicates we probably need to do this as
            // a lowest common multiplier solution
            var step = 0;

            // Find all nodes that end in 'A'
            var currentNodes = map.Keys.Where(k => k[^1] == 'A').ToArray();
            var currentIdx = 0;

            // Track the last Z node we found for each path
            var lastZ = Enumerable.Repeat(0, currentNodes.Length).ToArray();

            // Track the distance between the Z nodes
            var distances = Enumerable.Repeat((double)0, currentNodes.Length).ToArray();

            do
            {
                var instruction = instructions[currentIdx++];

                currentIdx %= instructions.Length;
                step++;

                for (int i = 0; i<currentNodes.Length; i++)
                {
                    if (instruction == 'L')
                        currentNodes[i] = map[currentNodes[i]].L;
                    else
                        currentNodes[i] = map[currentNodes[i]].R;


                    // If this node ends in 'Z', we need to record if
                    if (currentNodes[i][^1] == 'Z')
                    {
                        if (lastZ[i] == 0)
                            lastZ[i] = step;
                        else if (distances[i] == 0)
                            distances[i] = (double)step - (double)lastZ[i];
                    }
                }

                // If we have found all distances, break out
            } while (distances.Any(d => d == 0));

            return Utilities.FindLCM(distances).ToString();
        }
    }
}

