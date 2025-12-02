using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2025
{
    partial class Day02 : ASolution
    {

        public Day02() : base(02, 2025, "Gift Shop")
        {
            // DebugInput = "11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124";
        }

        protected override string? SolvePartOne()
        {
            ulong solution = 0;

            // Brute force method, check each value
            Input.Trim().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ForEach(line =>
            {
                var start_end = line.Split('-');
                ulong start = ulong.Parse(start_end[0]);
                ulong end = ulong.Parse(start_end[1]);

                for(var i = start; i<=end; i++)
                {
                    var s = i.ToString();

                    if (s.Length % 2 == 0)
                    {
                        // Can only evenly split an even count of digits
                        if (s[0..(s.Length/2)] == s[(s.Length/2)..])
                        {
                            solution += i;
                        }
                    }
                }
            });

            // Time  : 00:00:00.0819624
            return solution.ToString();
        }

        protected override string? SolvePartTwo()
        {
            ulong solution = 0;

            var regex = PatternMatch();

            // Brute force method, check each value
            Input.Trim().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ForEach(line =>
            {
                var start_end = line.Split('-');
                ulong start = ulong.Parse(start_end[0]);
                ulong end = ulong.Parse(start_end[1]);

                for (var i = start; i <= end; i++)
                {
                    var s = i.ToString();

                    if (regex.IsMatch(i.ToString()))
                        solution += i;
                }
            });

            // Time  : 00:00:01.6094041
            return solution.ToString();
        }

        [GeneratedRegex(@"^([0-9]+)\1+$")]
        private static partial Regex PatternMatch();
    }
}

