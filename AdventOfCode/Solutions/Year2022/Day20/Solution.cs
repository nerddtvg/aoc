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

        private List<int> indexes = new();
        private List<int> values = new();

        public Day20() : base(20, 2022, "Grove Positioning System")
        {
            DebugInput = @"1
                2
                -3
                3
                -2
                0
                4";

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

            values = Input.SplitByNewline(true)
                .Select(line => Int32.Parse(line))
                .ToList();

            indexes = Enumerable.Range(0, values.Count).ToList();
        }

        private void MixList()
        {
            // Working off indexes
            // If we simply move around the indexes, it doesn't really matter
            // what the values are
            for (int i = 0; i < values.Count; i++)
            {
                // Get our value
                var shift = values[i];

                // Do nothing
                if (shift == 0)
                    continue;

                // If shift is less than zero, move one more to get the indexing right moving backwards
                // if (shift < 0)
                //     shift--;

                // Get our position in the array right now
                var index = indexes.IndexOf(i);

                // Negative modulo in C# issue
                while (shift < 0)
                    shift += indexes.Count - 1;

                var newPosition = (index + shift) % (indexes.Count - 1);

                // If we are not moving, do nothing
                if (newPosition == index)
                    continue;

                // Remove the old value (copy the list)
                indexes.RemoveAt(index);

                var newIndexes = new List<int>();

                if (newPosition == 0)
                {
                    newIndexes.Add(i);
                    newIndexes.AddRange(indexes);
                }
                else
                {
                    newIndexes.AddRange(indexes.Take(newPosition));
                    newIndexes.Add(i);

                    // Append an end if needed
                    if (newPosition < indexes.Count)
                    {
                        newIndexes.AddRange(indexes.Skip(newPosition));
                    }
                }

                indexes = newIndexes;

                // Now copy from 0 .. a-1, a+1 .. b-1, b+1 .. end
                // Where a and be are index and newPosition in order
                // var a = Math.Min(newPosition, index);
                // var b = Math.Max(index, newPosition);

                // var newIndexes = new List<int>();

                // if (a > 0)
                //     newIndexes.AddRange(indexes.Take(a));

                // if (a == newPosition)
                //     newIndexes.Add(i);

                // newIndexes.AddRange(indexes.Skip(a + 1).Take(b - a));

                // if (b == newPosition)
                //     newIndexes.Add(i);

                // if (b < indexes.Count - 1)
                //     newIndexes.AddRange(indexes.Skip(b + 1));

                // // Save
                // indexes = newIndexes;

                PrintList();
            }

            return;
            foreach (var node in nodes)
            {
                // Don't move anything at zero
                if (node.Value == 0)
                    continue;

                // If we get one more shift for move left,
                // then we only have to move right
                var shift = node.Value;
                if (shift < 0)
                    shift--;

                var newNode = node.GetNodeByStep(shift);

                // If we found ourself, do nothing
                if (newNode == node)
                    continue;

                list.Remove(node);
                list.AddAfter(newNode, node);
            }
        }

        private void PrintList()
        {
            foreach(var index in indexes)
            {
                Console.Write("{0,3:N0}, ", values[index]);
            }

            Console.WriteLine();
        }

        protected override string? SolvePartOne()
        {
            // Ominous start:
            // That's not the right answer; your answer is too low. Curiously, it's the right answer for someone else; you might be logged in to the wrong account or just unlucky.

            // Mix the list once
            MixList();

            // var node = list.First;
            // while(node != default)
            // {
            //     Console.WriteLine(node.Value);

            //     node = node.Next;
            // }

            // var v1000 = nodeZero.GetNodeByStep(1000).Value;
            // var v2000 = nodeZero.GetNodeByStep(2000).Value;
            // var v3000 = nodeZero.GetNodeByStep(3000).Value;

            var zeroIndex = indexes.IndexOf(values.IndexOf(0));
            var v1000 = values[(zeroIndex + 1000) % values.Count];
            var v2000 = values[(zeroIndex + 2000) % values.Count];
            var v3000 = values[(zeroIndex + 3000) % values.Count];

            return (v1000 + v2000 + v3000).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

