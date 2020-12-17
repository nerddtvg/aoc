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
            // Find the furthest right point in the tractor beam
            BeamTile fr = map.Where(a => a.state == 1).OrderByDescending(a => a.x).First();
            
            // Find the furthest left/bottom point in the tractor beam
            BeamTile fl = map.Where(a => a.state == 1).OrderByDescending(a => a.y).ThenBy(a => a.x).First();

            Decimal slope_r = (decimal) fr.x / fr.y;
            Decimal slope_l = (decimal) fl.x / fl.y;

            // We have line slopes now
            // y=mx+b right?
            // b = 0
            // We can loop through either x or y to find the rest
            decimal fl_1 = 0;
            decimal fr_1 = 0;
            decimal fl_2 = 0;
            decimal fr_2 = 0;
            int y;
            for(y=500; y<10000000; y++) {
                // For each y, find out:
                // fr line point (x,y)
                // Then find fl where y = y+100
                fr_1 = Math.Round(y / slope_r, 0);
                fl_1 = Math.Round((y+100) / slope_l, 0);

                // So if fl_1 <= fr_1 - 100, we found it
                if ((fr_1 - fl_1) < 100) continue;

                // We found it
                break;
            }

            // Point is fl_2, y
            return ((fl_1 * 10000) + y).ToString();
        }
    }
}
