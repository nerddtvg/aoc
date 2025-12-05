using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2025
{

    class Day05 : ASolution
    {
        private (ulong a, ulong b)[] ranges;
        private ulong[] ingredients;

        public Day05() : base(05, 2025, "Cafeteria")
        {
            var split = Input.SplitByBlankLine(true);

            ranges = [.. split[0].Select(line => { var t = line.Split('-'); return (ulong.Parse(t[0]), ulong.Parse(t[1])); })];
            ingredients = [.. split[1].Select(ulong.Parse)];
        }

        private bool IsFresh(ulong ingredient)
        {
            return ranges.Any(range => range.a <= ingredient && ingredient <= range.b);
        }

        protected override string? SolvePartOne()
        {
            return ingredients.Count(IsFresh).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

