using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{

    class Day03 : ASolution
    {
        private List<(int x, int y, int z)> triangles = new List<(int x, int y, int z)>();

        public Day03() : base(03, 2016, "")
        {
            // Process the input with regex
            var regex = new Regex(@"([0-9]+)\s+([0-9]+)\s+([0-9]+)");
            foreach (var line in Input.SplitByNewline(true))
            {
                var match = regex.Match(line);

                if (match.Success)
                {
                    triangles.Add((Int32.Parse(match.Groups[1].Value), Int32.Parse(match.Groups[2].Value), Int32.Parse(match.Groups[3].Value)));
                }
            }
        }

        private bool IsValidTriangle((int x, int y, int z) tri) =>
            (tri.x + tri.y) > tri.z && (tri.x + tri.z) > tri.y && (tri.y + tri.z) > tri.x;

        protected override string SolvePartOne()
        {
            return triangles.Count(t => IsValidTriangle(t)).ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
