using System;
using System.Collections.Generic;
using System.Text;

using System.Text.Json;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2015
{

    class Day12 : ASolution
    {
        private object _doc;

        public Day12() : base(12, 2015, "")
        {
            this._doc = JsonSerializer.Deserialize<object>(Input);
        }

        protected override string SolvePartOne()
        {
            var matches = Regex.Matches(Input, "(-|)[0-9]+");

            // We just need to find all numbers in the string, nothing fancy
            return matches.Select(a => Int32.Parse(a.Value)).Sum().ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
