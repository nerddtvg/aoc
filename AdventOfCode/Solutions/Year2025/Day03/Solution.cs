using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Diagnostics;
using System.Numerics;
using System.Collections;


namespace AdventOfCode.Solutions.Year2025
{

    class Day03 : ASolution
    {

        public Day03() : base(03, 2025, "Lobby")
        {
            var tests = new[]
            {
                ("987654321111111", 98),
                ("811111111111119", 89),
                ("234234234234278", 78),
                ("818181911112111", 92)
            };

            foreach ((var line, var expected) in tests)
            {
                var result = MaxJoltage(line);
                Debug.Assert(result == expected, $"Line '{line}' returned '{result}', expected '{expected}'");
            }

            var tests2 = new[]
            {
                ("987654321111111", 987654321111),
                ("811111111111119", 811111111119),
                ("234234234234278", 434234234278),
                ("818181911112111", 888911112111)
            };

            foreach ((var line, BigInteger expected) in tests2)
            {
                var result = MaxJoltage2(line);
                Debug.Assert(result == expected, $"Line '{line}' returned '{result}', expected '{expected}'");
            }
        }

        protected override string? SolvePartOne()
        {
            // Time  : 00:00:00.0048054
            return Input.SplitByNewline().Sum(MaxJoltage).ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Time  : 00:00:00.0051798
            return Input.SplitByNewline().SumBigInteger(MaxJoltage2).ToString();
        }

        private int MaxJoltage(string line)
        {
            // Search LTR and find the highest value + index
            // Then search LTR index+1 to find the next highest value
            var ret = 0;
            var max = line[0];
            var maxI = 0;

            for (int i = 1; i < line.Length - 1; i++)
            {
                if (line[i] > max)
                {
                    max = line[i];
                    maxI = i;
                }
            }

            // Convert char int to int without parse
            ret += (max - 48) * 10;

            max = line[maxI + 1];
            for (int i = maxI + 2; i < line.Length; i++)
            {
                if (line[i] > max)
                {
                    max = line[i];
                }
            }

            ret += max - 48;

            return ret;
        }

        private BigInteger? MaxJoltage2(string line)
        {
            int desiredLength = 12;

            // Work with a sliding window
            // 0 to length-desiredLength
            // Find maximum value, slide window, try again
            var ret = string.Empty;
            for (int left = 0; left < line.Length && ret.Length < desiredLength; left++)
            {
                int right = line.Length - desiredLength + ret.Length;

                // Get the substring to check
                var part = line[left..(right + 1)];

                // Find the maximum character and its first position
                var max = part.Max();
                var idx = part.IndexOf(max);

                left += idx;
                ret += max;
            }

            return BigInteger.Parse(ret);

            // BELOW: Previous attempts with multiple iterations
            // I went to the subreddit for a hint because each attempt I had kept failing

            /*
            // Get all  of our characters and positions in the string
            var charIdx = line.Select((c, idx) => (c, idx)).GroupBy(obj => obj.c, obj => obj.idx).ToDictionary(grp => grp.Key, grp => new SortedSet<int>(grp));
            var includedIndex = new SortedSet<int>();

            // Go through each value, highest to lowest and 'activate' the index
            while (includedIndex.Count < 12)
            {
                for (var c = '9'; c > '0' && includedIndex.Count < 12; c--)
                {
                    if (charIdx.TryGetValue(c, out var value))
                    {
                        // First, highest character match => Add all entries
                        if (includedIndex.Count == 0)
                        {
                            value.ForEach(idx => includedIndex.Add(idx));
                            charIdx.Remove(c);
                            continue;
                        }

                        // Now we want to add all of these values IF the index is less than the lowest index in the set
                        // If the lowestIdx is within the last 12, then we can ignore it
                        var lowestIdx = includedIndex.Count < 12 ? 0 : includedIndex.First();

                        // Send ToArray() to ensure we can maniuplate the original list inside the loop
                        foreach (var idx in value.OrderDescending().ToArray())
                        {
                            if (idx < lowestIdx || includedIndex.Count == 12)
                            {
                                break;
                            }

                            includedIndex.Add(idx);
                            value.Remove(idx);
                        }

                        if (value.Count == 0)
                            charIdx.Remove(c);
                    }
                }
            }

            return BigInteger.Parse(includedIndex.Select(i => line[i]).JoinAsString());

            var sorted = new Queue<char>(line.ToCharArray().OrderDescending());

            while (sorted.Count > 0 && includedIndex.Count < 12)
            {
                var searchC = sorted.Dequeue();

                for (int i = 0; i < line.Length; i++)
                {
                    if (!includedIndex.Contains(i) && line[i] == searchC)
                    {
                        includedIndex.Add(i);
                        break;
                    }
                }
            }

            return BigInteger.Parse(includedIndex.Select(i => line[i]).JoinAsString());
            */
        }
    }
}
