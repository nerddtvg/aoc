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

        private bool IsValid(string input, int[] counts)
        {
            // Check that we have the correct number and length of groups of '#'
            var regStr = @"^\.*";
            counts.ForEach((count, idx) => regStr += @"#{" + count.ToString() + @"}\." + (idx == counts.Length - 1 ? "*" : "+"));
            regStr += "$";

            return new Regex(regStr).IsMatch(input);
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
                .Sum(line => GetStrings(line.chars).Count(str => IsValid(str, line.counts)))
                .ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

