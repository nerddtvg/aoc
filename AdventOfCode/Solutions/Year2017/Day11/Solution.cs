using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2017
{

    class Day11 : ASolution
    {
        private (int x, int y) pos = (0, 0);
        private uint MaxDistance = uint.MinValue;

        public Day11() : base(11, 2017, "Hex Ed")
        {
            this.pos = (0, 0);
        }

        // From 2020 Day 24
        private (int x, int y) getXY((int x, int y) pos, string dir) {
            // Takes a direction and returns a new x,y
            switch(dir) {
                case "n":
                    return (pos.x, pos.y+1);
                    
                case "nw":
                    return (pos.x-1, pos.y);
                    
                case "sw":
                    return (pos.x-1, pos.y-1);
                    
                case "s":
                    return (pos.x, pos.y-1);
                    
                case "se":
                    return (pos.x+1, pos.y);
                    
                case "ne":
                    return (pos.x+1, pos.y+1);
                
                default: throw new Exception($"Invalid direction: {dir}");
            }
        }

        protected override string? SolvePartOne()
        {
            foreach(var dir in Input.Split(","))
            {
                this.pos = getXY(this.pos, dir);

                this.MaxDistance = Math.Max(this.MaxDistance, (uint) this.pos.ManhattanDistance((0, 0)));
            }

            return this.pos.ManhattanDistance((0, 0)).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return this.MaxDistance.ToString();
        }
    }
}

#nullable restore
