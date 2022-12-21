using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2022
{

    class Day20 : ASolution
    {
        private List<LinkedListNode<int>> nodes = new();
        private LinkedList<int> list = new();
        private LinkedListNode<int> nodeZero = default!;

        public Day20() : base(20, 2022, "Grove Positioning System")
        {
            // DebugInput = @"1
            //     2
            //     -3
            //     3
            //     -2
            //     0
            //     4";

            ReadList();
        }

        private void ReadList()
        {
            list.Clear();
            nodes.Clear();

            // Need to read in the full list, keeping the original order in "nodes"
            Input.SplitByNewline(true)
                .Select(line => Int32.Parse(line))
                .ToList()
                .ForEach(nodeValue =>
                {
                    list.AddLast(nodeValue);

                    // Just to make the compiler check happy
                    if (list.Last == default)
                        throw new Exception();

                    nodes.Add(list.Last);

                    if (nodeValue == 0)
                    {
                        nodeZero = list.Last;
                    }
                });
        }

        private void MixList()
        {
            foreach (var node in nodes)
            {
                // Don't move anything at zero
                if (node.Value == 0) continue;

                var newNode = node.GetNodeByStep(node.Value);

                list.Remove(node);

                if (node.Value < 0)
                {
                    list.AddBefore(newNode, node);
                }
                else
                {
                    list.AddAfter(newNode, node);
                }
            }
        }

        protected override string? SolvePartOne()
        {
            // Ominous start:
            // That's not the right answer; your answer is too low. Curiously, it's the right answer for someone else; you might be logged in to the wrong account or just unlucky.

            // Mix the list once
            MixList();

            var v1000 = nodeZero.GetNodeByStep(1000).Value;
            var v2000 = nodeZero.GetNodeByStep(2000).Value;
            var v3000 = nodeZero.GetNodeByStep(3000).Value;

            return (v1000 + v2000 + v3000).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

