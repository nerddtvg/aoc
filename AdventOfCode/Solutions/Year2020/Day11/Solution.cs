using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;
using System.Collections;

namespace AdventOfCode.Solutions.Year2020
{
    enum WaitingSpotType {
        Empty,
        Occupied,
        Floor
    }

    class Day11 : ASolution
    {
        Dictionary<(int x, int y), WaitingSpotType> map;

        public Day11() : base(11, 2020, "")
        {
            int y = 0;
            int x;

            foreach(string line in Input.SplitByNewline()) {
                x = 0;

                foreach(char c in line) {
                    map[(c, y)] = (c == '.' ? WaitingSpotType.Floor : (c == 'L' ? WaitingSpotType.Empty : WaitingSpotType.Occupied));
                }

                y++;
            }
        }

        protected override string SolvePartOne()
        {
            return null;
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
