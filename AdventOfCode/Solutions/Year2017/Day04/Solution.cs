using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2017
{

    class Day04 : ASolution
    {

        public Day04() : base(04, 2017, "")
        {

        }

        public bool IsValidPassPhrase(string phase) =>
            !phase.Split(' ', StringSplitOptions.RemoveEmptyEntries).GroupBy(word => word).Any(grp => grp.Count() > 1);

        public bool IsValidPassPhrase2(string phase) =>
            !phase.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(word => word.ToCharArray().OrderBy(c => c).JoinAsString()).GroupBy(word => word).Any(grp => grp.Count() > 1);

        protected override string? SolvePartOne()
        {
            return Input.SplitByNewline(true).Count(line => IsValidPassPhrase(line)).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return Input.SplitByNewline(true).Count(line => IsValidPassPhrase2(line)).ToString();
        }
    }
}

#nullable restore
