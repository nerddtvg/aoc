using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2023
{

    class Day12 : ASolution
    {
        (char[] chars, int[] counts)[] lines;

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
                .Select(row => (chars: row[0].ToCharArray(), counts: row[1].Split(',').Select(d => int.Parse(d)).ToArray()))
                .ToArray();
        }

        private string BuildRegex(int[] counts)
        {
            var regStr = @"^\.*";
            counts.ForEach((count, idx) => regStr += @"#{" + count.ToString() + @"}\." + (idx == counts.Length - 1 ? "*" : "+"));
            regStr += "$";

            return regStr;
        }

        private IEnumerable<string> GetStrings(char[] chars)
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
                    foreach (var suffix in GetStrings(chars.Skip(1).ToArray()))
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

        private int CountValids(char[] chars, int[] counts)
        {
            // Do this only once per line will save cycles
            var regex = new Regex(BuildRegex(counts));

            return GetStrings(chars).Count(regex.IsMatch);
        }

        protected override string? SolvePartOne()
        {
            // lines.ForEach(line =>
            // {
            //     Console.WriteLine($"  {line.chars.JoinAsString()}:  {string.Join(",", line.counts)}");
            //     GetStrings(line.chars).ForEach(str =>
            //     {
            //         if (IsValid(str, line.counts))
            //             Console.WriteLine($"* {str}");
            //         else
            //             Console.WriteLine($"  {str}");
            //     });
            //     Console.WriteLine($"");
            // });

            return lines
                .Sum(line => CountValids(line.chars, line.counts))
                .ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Figured this would be too long/complex to run
            // It was and didn't complete after 15 minutes
            // Need to actually refactor this into something logical
            
            // Expand the lines
            lines = lines.Select(line =>
            {
                int duplicationCount = 5;

                var chars = (char[])Array.CreateInstance(typeof(char), line.chars.Length * duplicationCount);
                var counts = (int[])Array.CreateInstance(typeof(int), line.chars.Length * duplicationCount);
                for (int i = 0; i < duplicationCount; i++)
                {
                    Array.Copy(line.chars, 0, chars, i * line.chars.Length, line.chars.Length);
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

