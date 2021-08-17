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

        private int CountChildren(JsonElement root)
        {
            // For part 2, we need to count all integers included in this object
            // UNLESS the object has the value "red" anywhere
            int ret = 0;

            if (root.ValueKind == JsonValueKind.Array)
            {
                foreach(var el in root.EnumerateArray())
                {
                    ret += CountChildren(el);
                }
            }
            else if (root.ValueKind == JsonValueKind.Object)
            {
                // Look to see if we have "red" as a value anywhere, if so, skip it
                foreach(var el in root.EnumerateObject())
                {
                    // For each element...
                    if (el.Value.ValueKind == JsonValueKind.String && string.Equals("red", el.Value.GetString(), StringComparison.InvariantCultureIgnoreCase))
                        return 0;

                    // Otherwise, add to our return
                    ret += CountChildren(el.Value);
                }
            }
            else if (root.ValueKind == JsonValueKind.Number)
            {
                return root.GetInt32();
            }

            return ret;
        }

        protected override string SolvePartTwo()
        {
            return CountChildren((JsonElement) this._doc).ToString();
        }
    }
}
