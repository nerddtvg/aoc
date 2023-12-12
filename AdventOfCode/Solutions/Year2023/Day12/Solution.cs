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

            lines = ParseInput(Input);
        }

        private (string chars, int[] counts)[] ParseInput(string input)
        {
            return input.SplitByNewline(true)
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
                    return CountValid(line.chars, line.counts, 0, 0, 0);
                })
                .ToString();
        }

        private Dictionary<(int iChars, int iCounts, int currentLength), ulong> cache = new();

        /// <param name="chars">The current pattern to check for matches</param>
        /// <param name="counts">The counts to look for</param>
        /// <param name="currentLength">The current block length</param>
        private ulong CountValid(string chars, int[] counts, int currentLength, int iChars, int iCounts)
        {
            // If we are in the same place, same count, and same currentLength
            // we may have a duplicate
            var cacheHash = (iChars, iCounts, currentLength);

            if (cache.TryGetValue(cacheHash, out ulong value))
                return value;

            // Check if we are at the end, if so we return 1 or 0 directly
            if (iChars == chars.Length)
            {
                if (iCounts == counts.Length && currentLength == 0)
                {
                    // Ended outside the string and no count, this was the end of the string
                    return 1;
                }
                else if (iCounts == counts.Length - 1 && counts[iCounts] == currentLength)
                {
                    // Last block, last character, we matched
                    return 1;
                }

                // Invalid match
                return 0;
            }

            ulong result = 0;

            // Check each possible character
            foreach (var c in new char[] { '.', '#' })
            {
                // If the character matches OR the character in question is '?'
                // proceed with checking
                if (chars[iChars] == '?' || chars[iChars] == c)
                {
                    // If this is a dot, we may need to reset our blocks
                    if (c == '.' && currentLength == 0)
                    {
                        // Beginning of string, so let's move forward and count from there
                        result += CountValid(chars, counts, 0, iChars + 1, iCounts);
                    }
                    else if (c == '.' && currentLength > 0 && iCounts < counts.Length && counts[iCounts] == currentLength)
                    {
                        // We have ended a block and it matches the expected length
                        // Move forward
                        result += CountValid(chars, counts, 0, iChars + 1, iCounts + 1);
                    }
                    else if (c == '#' && iCounts < counts.Length && currentLength < counts[iCounts])
                    {
                        // Increase the current block count
                        result += CountValid(chars, counts, currentLength + 1, iChars + 1, iCounts);
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
            lines = ParseInput(Input.SplitByNewline().Select(line =>
            {
                // Repeat each section 5 times
                var pattern = string.Empty;
                var count = string.Empty;
                var split = line.Split(' ');

                for (int i = 0; i < 5; i++)
                {
                    pattern += split[0] + "?";
                    count += split[1] + ",";
                }

                return $"{pattern[..^1]} {count[..^1]}\n";
            }).JoinAsString());

            return lines
                .Sum(line =>
                {
                    cache.Clear();
                    return CountValid(line.chars, line.counts, 0, 0, 0);
                })
                .ToString();
        }
    }
}

