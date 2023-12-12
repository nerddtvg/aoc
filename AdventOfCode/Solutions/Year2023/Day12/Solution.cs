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
                .Sum(line => CountValids(line.chars, line.counts))
                .ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Figured this would be too long/complex to run
            // It was and didn't complete after 15 minutes
            // Need to actually refactor this into something logical
            return string.Empty;

            // Expand the lines
            lines = lines.Select(line =>
            {
                int duplicationCount = 5;

                var chars = string.Empty;
                var counts = (int[])Array.CreateInstance(typeof(int), line.chars.Length * duplicationCount);
                for (int i = 0; i < duplicationCount; i++)
                {
                    chars += line.chars;
                    Array.Copy(line.counts, 0, counts, i * line.counts.Length, line.counts.Length);
                }

                return (chars, counts);
            }).ToArray();

            return lines
                .Sum(line => CountValids(line.chars, line.counts))
                .ToString();
        }
    }
}

