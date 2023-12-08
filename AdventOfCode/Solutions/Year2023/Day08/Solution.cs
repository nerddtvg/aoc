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

        const string startNode = "AAA";
        const string endNode = "ZZZ";

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

        private int FindStepsToEnd()
        {
            var step = 0;

            // Track our node and instruction count
            var currentNode = startNode;
            var currentIdx = 0;

            do
            {
                if (instructions[currentIdx++] == 'L')
                    currentNode = map[currentNode].L;
                else
                    currentNode = map[currentNode].R;

                currentIdx %= instructions.Length;
                step++;
            } while (currentNode != endNode);

            return step;
        }

        protected override string? SolvePartOne()
        {
            return FindStepsToEnd().ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

