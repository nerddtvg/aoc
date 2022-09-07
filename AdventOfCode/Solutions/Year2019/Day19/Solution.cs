using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2019
{

    class BeamTile {
        public int x {get;set;}
        public int y {get;set;}
        public int state {get;set;}
    }

    class Day19 : ASolution
    {
        List<BeamTile> map = new List<BeamTile>();

        public Day19() : base(19, 2019, "")
        {
            // Need to find Santa's ship (100x100 square inside the beam)
            for(int y=0; y<50; y++) {
                for(int x=0; x<50; x++) {
                    Intcode intcode = new Intcode(Input, 2);
                    intcode.SetInput(x);
                    intcode.SetInput(y);
                    intcode.Run();

                    // Gather the output
                    map.Add(new BeamTile() { x = x, y = y, state = Convert.ToInt32(intcode.output_register.Dequeue()) });
                }
            }
        }

        protected override string SolvePartOne()
        {
            // How many are in state == 1
            return map.Count(a => a.x < 50 && a.y < 50 && a.state == 1).ToString();
        }

        protected override string SolvePartTwo()
        {
            // Slopes didn't work, do this the "hard" way
            // Based on https://github.com/fdouw/AoC2019/blob/master/Day19/Day19.cs
            int prevStart = 0;      // Improve performance by ignoring the void below the beam
            int L = 99;             // 100x100 block means x and x+99 inclusive

            Intcode intcode;

            for (int y = L; y < 10_000; y++)
            {
                for (int x = prevStart; x < 10_000; x++)
                {
                    intcode = new Intcode(Input, 2);
                    intcode.SetInput(x);
                    intcode.SetInput(y);
                    intcode.Run();

                    if (intcode.output_register.Dequeue() == 1)
                    {
                        // prevStart = (x < y) ? x : y;
                        prevStart = x;

                        intcode = new Intcode(Input, 2);
                        intcode.SetInput(x + L);
                        intcode.SetInput(y - L);
                        intcode.Run();

                        if (intcode.output_register.Dequeue() == 1)
                        {
                            return (x * 10_000 + y - L).ToString();
                        }

                        break;
                    }
                }
            }

            return string.Empty;
        }
    }
}
