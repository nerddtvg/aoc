using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

using System.Diagnostics;

#nullable enable

namespace AdventOfCode.Solutions.Year2018
{

    class Day22 : ASolution
    {
        public enum CaveType
        {
            Rocky,
            Wet,
            Narrow
        }

        private int caveDepth = 0;

        private (int x, int y) goal = (10, 10);

        private Dictionary<(int x, int y), int> erosionLevels = new Dictionary<(int x, int y), int>();
        private Dictionary<(int x, int y), int> geologicIndices = new Dictionary<(int x, int y), int>();

        public Day22() : base(22, 2018, "Mode Maze")
        {
//             DebugInput = @"depth: 510
// target: 10,10";

            // Just going to simplify this without regex
            var vals = Input.SplitByNewline().Select(line => line.Split(" ", 2)[1].Trim()).ToArray();

            this.caveDepth = Int32.Parse(vals[0]);
            this.goal = (Int32.Parse(vals[1].Split(',')[0].Trim()), Int32.Parse(vals[1].Split(',')[1].Trim()));

            if (!string.IsNullOrEmpty(DebugInput))
            {
                Debug.Assert(GetGeologicIndex((0, 0)) == 0);
                Debug.Assert(GetGeologicIndex((1, 0)) == 16807);
                Debug.Assert(GetGeologicIndex((0, 1)) == 48271);
                Debug.Assert(GetGeologicIndex((1, 1)) == 145722555);
                Debug.Assert(GetGeologicIndex((10, 10)) == 0);

                Debug.Assert(GetErosionLevel((0, 0)) == 510);
                Debug.Assert(GetErosionLevel((1, 0)) == 17317);
                Debug.Assert(GetErosionLevel((0, 1)) == 8415);
                Debug.Assert(GetErosionLevel((1, 1)) == 1805);
                Debug.Assert(GetErosionLevel((10, 10)) == 510);
            }
        }

        public CaveType GetTileType((int x, int y) pt)
        {
            var er = GetErosionLevel(pt);

            if (er % 3 == 0)
                return CaveType.Rocky;
            
            if (er % 3 == 1)
                return CaveType.Wet;

            return CaveType.Narrow;
        }

        public int GetErosionLevel((int x, int y) pt)
        {
            // Caching to speed things up!
            if (!this.erosionLevels.ContainsKey(pt))
                this.erosionLevels[pt] = ((GetGeologicIndex(pt) + this.caveDepth) % 20183);

            return this.erosionLevels[pt];
        }

        public int GetGeologicIndex((int x, int y) pt)
        {
            if (pt == (0, 0) || pt == this.goal)
                return 0;

            if (pt.y == 0)
                return 16807 * pt.x;

            if (pt.x == 0)
                return 48271 * pt.y;

            // Caching to speed things up!
            if (!this.geologicIndices.ContainsKey(pt))
                this.geologicIndices[pt] = GetErosionLevel((pt.x - 1, pt.y)) * GetErosionLevel((pt.x, pt.y - 1));

            return this.geologicIndices[pt];
        }

        public int GetRiskLevel((int x, int y) pt)
        {
            var type = GetTileType(pt);

            if (type == CaveType.Narrow)
                return 2;

            if (type == CaveType.Wet)
                return 1;

            return 0;
        }

        protected override string? SolvePartOne()
        {
            var risk = 0;
            for (int y = 0; y <= this.goal.y; y++)
            {
                for (int x = 0; x <= this.goal.x; x++)
                {
                    risk += GetRiskLevel((x, y));
                }
            }

            return risk.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
