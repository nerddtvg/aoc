using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    class Day25 : ASolution
    {
        public List<int[]> keys = [];
        public List<int[]> locks = [];

        public Day25() : base(25, 2024, "Code Chronicle")
        {
            Input.SplitByBlankLine().ForEach(item =>
            {
                var itemChars = item.Select(line => line.ToCharArray()).ToArray();
                var itemCounts = Enumerable.Range(0, 5).Select(x => itemChars.GetColumn(x).Count(c => c == '#') - 1).ToArray();

                if (itemChars[0][0] == '#')
                    locks.Add(itemCounts);
                else
                    keys.Add(itemCounts);
            });
        }

        protected override string? SolvePartOne()
        {
            return locks.Sum(lockItem => keys.Count(key => Enumerable.Range(0, 5).All(x => lockItem[x] + key[x] < 6))).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

