using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Diagnostics;

namespace AdventOfCode.Solutions.Year2022
{

    class Day06 : ASolution
    {

        public Day06() : base(06, 2022, "Tuning Trouble")
        {
            var part1Examples = new Dictionary<string, int>()
            {
                { "mjqjpqmgbljsphdztnvjfqwrcgsmlb", 7 },
                { "bvwbjplbgvbhsrlpgdmjqwftvncz", 5 },
                { "nppdvjthqldpwncqszvftbrmjlhg", 6 },
                { "nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", 10 },
                { "zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw", 11 },
            };

            foreach(var example in part1Examples)
            {
                var val = FirstMarker(example.Key);
                Debug.Assert(Debug.Equals(val, example.Value), $"Expected: {example.Value}\nActual: {val}");
            }

            var part2Examples = new Dictionary<string, int>()
            {
                { "mjqjpqmgbljsphdztnvjfqwrcgsmlb", 19 },
                { "bvwbjplbgvbhsrlpgdmjqwftvncz", 23 },
                { "nppdvjthqldpwncqszvftbrmjlhg", 23 },
                { "nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", 29 },
                { "zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw", 26 },
            };

            foreach (var example in part2Examples)
            {
                var val = FirstMarker(example.Key, 2);
                Debug.Assert(Debug.Equals(val, example.Value), $"Expected: {example.Value}\nActual: {val}");
            }
        }

        private int FirstMarker(string line, int part = 1)
        {
            // We want to find the first instance where 4 characters do not repeat
            // And return the index+1 of the last character
            var group = string.Empty;
            var offset = 0;
            var take = part == 1 ? 4 : 14;
            do
            {
                // Get our next group of 4
                group = line.Skip(offset++).Take(take).JoinAsString();

                if (group.Length < take)
                    break;

                // Determine if we have unique letters
                if (group.GroupBy(g => g).Count() == take)
                    return offset + (take - 1);
            } while (offset < line.Length);

            return 0;
        }

        protected override string? SolvePartOne()
        {
            return FirstMarker(Input).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return FirstMarker(Input, 2).ToString();
        }
    }
}

