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
        int part1 = 0, part2 = 0;

        public Day02() : base(02, 2021, "Dive!")
        {
            // Generalized
            int ypos = 0;
            int xpos = 0;
            
            int ypos2 = 0;
            int xpos2 = 0;
            int aim2 = 0;

            // Changed to handle both processes at the same time
            foreach(var line in Input.SplitByNewline(true))
            {
                var str = line.Split(' ', 2);
                int val = Int32.Parse(str[1]);

                switch(str[0])
                {
                    case "forward":
                        // Part 1
                        xpos += val;
                        
                        // Part 2
                        xpos2 += val;
                        ypos2 += aim2 * val;
                        break;
                        
                    case "up":
                        // Part 1
                        ypos -= val;
                        
                        // Part 2
                        aim2 -= val;
                        break;
                        
                    case "down":
                        // Part 1
                        ypos += val;
                        
                        // Part 2
                        aim2 += val;
                        break;
                }
            }

            Console.WriteLine($"[Part 1] X Pos: {xpos}");
            Console.WriteLine($"[Part 1] Y Pos: {ypos}");
            
            Console.WriteLine($"[Part 2] X Pos: {xpos2}");
            Console.WriteLine($"[Part 2] Y Pos: {ypos2}");
            Console.WriteLine($"[Part 2]   Aim: {aim2}");

            // Save the answers
            this.part1 = xpos * ypos;
            this.part2 = xpos2 * ypos2;
        }

        protected override string? SolvePartOne()
        {
            return (this.part1).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return (this.part2).ToString();
        }
    }
}

#nullable restore
