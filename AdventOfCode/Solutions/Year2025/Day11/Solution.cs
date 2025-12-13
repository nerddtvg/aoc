using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2025
{

    class Day11 : ASolution
    {
        private readonly Dictionary<string, HashSet<string>> nodes;
        private readonly Dictionary<string, HashSet<string>> reverseNodes = [];

        public Day11() : base(11, 2025, "Reactor")
        {
            // DebugInput = @"aaa: you hhh
            //     you: bbb ccc
            //     bbb: ddd eee
            //     ccc: ddd eee fff
            //     ddd: ggg
            //     eee: out
            //     fff: out
            //     ggg: out
            //     hhh: ccc fff iii
            //     iii: out";

            nodes = Input.SplitByNewline(true, true)
                .Select(line =>
                {
                    // Each node is 3 letters making this easier
                    var split = line.Split(' ');
                    return (key: split[0][..3], value: new HashSet<string>(split[1..]));
                })
                .ToDictionary(kvp => kvp.key, kvp => kvp.value);

            nodes.ForEach(kvp =>
            {
                kvp.Value.ForEach(startingNode =>
                {
                    if (!reverseNodes.ContainsKey(startingNode))
                        reverseNodes[startingNode] = [];

                    reverseNodes[startingNode].Add(kvp.Key);
                });
            });
        }

        protected override string? SolvePartOne()
        {
            var pathCount = 0;
            var stack = new Queue<(string node, string path)>(reverseNodes["out"].Select(dest => (dest, $"out|{dest}")));

            while (stack.TryDequeue(out var result))
            {
                // Continue until empty
                if (reverseNodes.TryGetValue(result.node, out var newNodes))
                    foreach (var newNode in newNodes)
                    {
                        if (newNode == "you")
                        {
                            pathCount++;
                        }
                        else
                        {
                            stack.Enqueue((newNode, $"{result.path}|{newNode}"));
                        }
                    }
            }
            
            return pathCount.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

