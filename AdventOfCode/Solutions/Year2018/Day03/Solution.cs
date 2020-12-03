using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2018
{
    class SantaFabric {
        public int x {get; set;}
        public int y {get; set;}
        public int x2 {get; set;}
        public int y2 {get; set;}
        public int w {get; set;}
        public int h {get; set;}
        public int id {get;set;}
    }

    class Day03 : ASolution
    {
        List<SantaFabric> claims = new List<SantaFabric>();

        public Day03() : base(03, 2018, "")
        {
            // Claim examples:
            /*
             * #1 @ 1,3: 4x4
             * #2 @ 3,1: 4x4
             * #3 @ 5,5: 2x2
             * #id @ x,y: wxh
            */
            Input.SplitByNewline().ToList().ForEach(a => {
                string[] parts = a.Replace("#", "").Replace("@", "").Replace(":", "").Split(" ");

                // There is an extra space we have to account for
                string[] xy = parts[2].Split(",");
                string[] wh = parts[3].Split("x");

                SantaFabric claim = new SantaFabric();

                // Claim ID
                claim.id = Int32.Parse(parts[0]);

                // Claim x,y start
                claim.x = Int32.Parse(xy[0]);
                claim.y = Int32.Parse(xy[1]);

                // Claim width,height
                claim.w = Int32.Parse(wh[0]);
                claim.h = Int32.Parse(wh[1]);

                // Claim end points
                claim.x2 = claim.x + claim.w;
                claim.y2 = claim.y + claim.h;

                claims.Add(claim);
            });
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
