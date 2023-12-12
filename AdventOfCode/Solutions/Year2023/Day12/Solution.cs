using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2023
{

    class Day12 : ASolution
    {
        (string chars, int[] counts)[] lines;

        public Day12() : base(12, 2023, "Hot Springs")
        {
            // DebugInput = @"???.### 1,1,3
            //                .??..??...?##. 1,1,3
            //                ?#?#?#?#?#?#?#? 1,3,1,6
            //                ????.#...#... 4,1,1
            //                ????.######..#####. 1,6,5
            //                ?###???????? 3,2,1";

            // DebugInput = @".?##???.#? 3,2
            //                ?.?#??#??..# 3,1,1
            //                ??..??#?#????????.# 1,1,1,1,4,1";

            // DebugInput = @".?##???.#? 3,2";
            // DebugInput = @"??.#? 2";

            lines = Input.SplitByNewline(true)
                .Select(line => line.Split(' '))
                .Select(row => (chars: row[0], counts: row[1].Split(',').Select(d => int.Parse(d)).ToArray()))
                .ToArray();
        }

        private string BuildRegex(int[] counts)
        {
            var regStr = @"^\.*";
            counts.ForEach((count, idx) => regStr += @"#{" + count.ToString() + @"}\." + (idx == counts.Length - 1 ? "*" : "+"));
            regStr += "$";

            return regStr;
        }

        private IEnumerable<string> GetStrings(string chars)
        {
            if (chars.Length > 0)
            {
                // Shortcut here
                if (chars.All(c => c != '?'))
                {
                    yield return chars.JoinAsString();
                }
                else
                {
                    foreach (var suffix in GetStrings(chars[1..]))
                    {
                        if (chars[0] == '?')
                        {
                            yield return $"#{suffix}";
                            yield return $".{suffix}";
                        }
                        else
                        {
                            yield return $"{chars[0]}{suffix}";
                        }
                    }
                }
            }
            else
            {
                yield return string.Empty;
            }
        }

        private int CountValids(string chars, int[] counts)
        {
            // Do this only once per line will save cycles
            var regex = new Regex(BuildRegex(counts));

            return GetStrings(chars).Count(regex.IsMatch);
        }

        protected override string? SolvePartOne()
        {
            return lines
                .Sum(line =>
                {
                    cache.Clear();
                    return CountValid(line.chars, line.counts, 0);
                })
                .ToString();
        }

        private Dictionary<(string chars, int[] counts, int currentLength), int> cache = new();

        /// <param name="chars">The current pattern to check for matches</param>
        /// <param name="counts">The counts to look for</param>
        /// <param name="currentLength">The current block length</param>
        private int CountValid(string chars, int[] counts, int currentLength)
        {
            var cacheHash = (chars, counts, currentLength);

            if (cache.TryGetValue(cacheHash, out int value))
                return value;

            // Check if we are at the end, if so we return 1 or 0 directly
            if (chars.Length == 0 || counts.Length == 0)
            {
                if (counts.Length == 0 && currentLength == 0 && (chars.Length == 0 || chars.All(c => c != '#')))
                {
                    // Ended with an empty or 'working' string, empty array, and no extra
                    return 1;
                }
                else if (counts.Length == 1 && counts[0] == currentLength)
                {
                    // Last block, last character, we matched
                    return 1;
                }
                
                // Invalid match
                return 0;
            }

            int result = 0;

            // Check each possible character
            foreach (var c in new char[] { '.', '#' })
            {
                // If the character matches OR the character in question is '?'
                // proceed with checking
                if (chars[0] == '?' || chars[0] == c)
                {
                    // If this is a dot, we may need to reset our blocks
                    if (c == '.' && currentLength == 0)
                    {
                        // Beginning of string, so let's move forward and count from there
                        result += CountValid(chars[1..], counts, 0);
                    }
                    else if (c == '.' && currentLength > 0 && counts[0] == currentLength)
                    {
                        // We have ended a block and it matches the expected length
                        // Move forward
                        result += CountValid(chars[1..], counts[1..], 0);
                    }
                    else if (c == '#' && currentLength < counts[0])
                    {
                        // Increase the current block count
                        result += CountValid(chars[1..], counts, currentLength + 1);
                    }

                    // If c == '.' and counts[0] != currentLength:
                    // This is an invalid block and we do not add it or move forward
                }
            }

            cache[cacheHash] = result;
            return result;
        }

        protected override string? SolvePartTwo()
        {
            // Figured this would be too long/complex to run
            // It was and didn't complete after 15 minutes
            // Need to actually refactor this into something logical

            // Took another hint from the megathread regarding the caching
            // I understand we need caching, but wasn't really sure _what_ we were caching
            // Specifically how to process the string in parts rather than as a whole

            // Hint from /u/jonathan_paulson's code
            // https://github.com/jonathanpaulson/AdventOfCode/blob/341185efbe64ce771a57aef7d2bd101d9ea09329/2023/12.py

            // Logic:
            // Take a line (chars + counts)
            // Move left to right on the chars and counts
            // Count each block of '#' or possible '#' and see if that matches
            // the current 'count'.
            // If it does, move on to the next block
            // If not, that is a miss
            // Cache these responses (remaining string + counts + result)
            // return string.Empty;

            // Expand the lines
            lines = lines.Select(line =>
            {
                int duplicationCount = 5;

                var chars = string.Empty;
                var counts = (int[])Array.CreateInstance(typeof(int), line.chars.Length * duplicationCount);
                for (int i = 0; i < duplicationCount; i++)
                {
                    chars += (i > 0 ? "?" : "") + line.chars;
                    Array.Copy(line.counts, 0, counts, i * line.counts.Length, line.counts.Length);
                }

                return (chars, counts);
            }).ToArray();

            return lines
                .Sum(line =>
                {
                    cache.Clear();
                    return CountValid(line.chars, line.counts, 0);
                })
                .ToString();
        }
    }
}

