using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2021
{

    class Day02 : ASolution
    {

        public Day02() : base(02, 2021, "")
        {

        }

        protected override string? SolvePartOne()
        {
            int ypos = 0;
            int xpos = 0;

            foreach(var line in Input.SplitByNewline(true))
            {
                var str = line.Split(' ', 2);
                int val = Int32.Parse(str[1]);

                switch(str[0])
                {
                    case "forward":
                        xpos += val;
                        break;
                        
                    case "up":
                        ypos -= val;
                        break;
                        
                    case "down":
                        ypos += val;
                        break;
                }
            }

            Console.WriteLine($"X Pos: {xpos}");
            Console.WriteLine($"Y Pos: {ypos}");

            return (xpos*ypos).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
