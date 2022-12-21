using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2022
{

    class Day20 : ASolution
    {
        private List<int> indexes = new();
        private List<int> values = new();

        public Day20() : base(20, 2022, "Grove Positioning System")
        {
            ReadList();
        }

        private void ReadList()
        {
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
            
            var zeroIndex = indexes.IndexOf(values.IndexOf(0));
            var v1000 = values[indexes[(zeroIndex + 1000) % values.Count]];
            var v2000 = values[indexes[(zeroIndex + 2000) % values.Count]];
            var v3000 = values[indexes[(zeroIndex + 3000) % values.Count]];

            return (v1000 + v2000 + v3000).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

