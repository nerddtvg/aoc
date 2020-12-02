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
            for(int y=0; y<100; y++) {
                for(int x=0; x<100; x++) {
                    Intcode intcode = new Intcode(Input, 2);
                    intcode.SetInput(x);
                    intcode.SetInput(y);
                    intcode.Run();

                    // Gather the output
                    map.Add(new BeamTile() { x = x, y = y, state = Convert.ToInt32(intcode.output_register) });
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
            BeamTile fr = map.Where(a => a.state == 1).OrderBy(a => a.x).First();
            
            // Find the furthest left/bottom point in the tractor beam
            BeamTile fl = map.Where(a => a.state == 1).OrderBy(a => a.y).ThenByDescending(a => a.x).First();

            return null;
        }
    }
}
