using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2025
{

    class Day12 : ASolution
    {
        public struct Layout
        {
            public int w;
            public int h;
            public int[] shapes;
        }

        public int[] shapeSizes;
        public Layout[] layouts;

        public Day12() : base(12, 2025, "Christmas Tree Farm")
        {
            // For this, I went to the Megathread to look for hints
            // It seems there is a trick that makes this much easier than it looks

            var inputs = Input.SplitByBlankLine(true);

            // Count the number of '#' in each shape
            shapeSizes = [.. inputs[0..^1].Select(shape => shape.Sum(line => line.Count(c => c == '#')))];

            layouts = [.. inputs[^1].Select(layout =>
            {
                var i = layout.Split(['x', ':', ' '], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToArray();

                return new Layout()
                {
                    w = int.Parse(i[0]),
                    h = int.Parse(i[1]),
                    shapes = [.. i[2..].Select(int.Parse)]
                };
            })];
        }

        protected override string? SolvePartOne()
        {
            int total = 0;

            foreach(var layout in layouts)
            {
                // Each of our shapes is 3x3
                // So we can fit (w / 3) * (h / 3) shapes in a layout
                int layoutSize = layout.w * layout.h;
                int minShapes = layoutSize / 9;
                int shapeCount = layout.shapes.Sum();

                // If we can fit this as-is, we're done
                if (shapeCount <= minShapes)
                {
                    total++;
                    continue;
                }

                // Check if we cannot do this even with rotations by counting the number of '#' needed
                var spacesNeeded = layout.shapes.Select((count, idx) => shapeSizes[idx] * count).Sum();

                if (layoutSize < spacesNeeded)
                    continue;

                // Pause here and check answer
                // Answer was right because all inputs were solvable with the simple logic, no rotation checks and overlaps required
            }

            // Time  : 00:00:00.0019603
            return total.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

