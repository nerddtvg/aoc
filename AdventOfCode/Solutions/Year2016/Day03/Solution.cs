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
        private List<(int x, int y, int z)> triangles2 = new List<(int x, int y, int z)>();

        public Day03() : base(03, 2016, "")
        {
            // Process the input with regex
            var regex = new Regex(@"([0-9]+)\s+([0-9]+)\s+([0-9]+)");
            int lineCount = 0;

            (int x, int y, int z) tri1 = (0, 0, 0);
            (int x, int y, int z) tri2 = (0, 0, 0);
            (int x, int y, int z) tri3 = (0, 0, 0);

            foreach (var line in Input.SplitByNewline(true))
            {
                var match = regex.Match(line);

                if (match.Success)
                {
                    triangles.Add((Int32.Parse(match.Groups[1].Value), Int32.Parse(match.Groups[2].Value), Int32.Parse(match.Groups[3].Value)));

                    // Part 2 has them split by columns in rows of 3, so we need to track them
                    switch(lineCount)
                    {
                        case 0:
                            tri1.x = Int32.Parse(match.Groups[1].Value);
                            tri2.x = Int32.Parse(match.Groups[2].Value);
                            tri3.x = Int32.Parse(match.Groups[3].Value);
                            break;

                        case 1:
                            tri1.y = Int32.Parse(match.Groups[1].Value);
                            tri2.y = Int32.Parse(match.Groups[2].Value);
                            tri3.y = Int32.Parse(match.Groups[3].Value);
                            break;

                        case 2:
                            tri1.z = Int32.Parse(match.Groups[1].Value);
                            tri2.z = Int32.Parse(match.Groups[2].Value);
                            tri3.z = Int32.Parse(match.Groups[3].Value);

                            triangles2.Add(tri1);
                            triangles2.Add(tri2);
                            triangles2.Add(tri3);
                            break;
                    }
                    
                    lineCount = (lineCount + 1) % 3;
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
            return triangles2.Count(t => IsValidTriangle(t)).ToString();
        }
    }
}
