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
        public bool overlap {get;set;}
    }

    class FabricTile {
        public int x {get; set;}
        public int y {get; set;}
        public int count {get; set;}
    }

    class Day03 : ASolution
    {
        List<SantaFabric> claims = new List<SantaFabric>();
        List<FabricTile> tiles = new List<FabricTile>();

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
                // Reduce the w,h by 1 so we don't go into the next square
                claim.x2 = claim.x + claim.w - 1;
                claim.y2 = claim.y + claim.h - 1;

                claims.Add(claim);
            });
            
            // Now we need to count our tiles
            int minX = claims.Min(a => a.x);
            int minY = claims.Min(a => a.y);
            int maxX = claims.Max(a => a.x2);
            int maxY = claims.Max(a => a.y2);

            // Allows us to draw the overlaps for a nice visual
            bool draw = false;

            if (draw) Console.WriteLine($"Min X: {minX}");
            if (draw) Console.WriteLine($"Min Y: {minY}");

            for(int y=minY; y<=maxY; y++) {
                if (draw)
                    Console.Write(y.ToString("0000") + ": ");
                
                for(int x=minX; x<=maxX; x++) {
                    var tile = new FabricTile() {
                        x = x,
                        y = y
                    };

                    // Find all claims that overlap this point
                    List<SantaFabric> claimsOverlap = claims.Where(a => 
                        a.x <= x
                        &&
                        a.y <= y
                        &&
                        x <= a.x2
                        &&
                        y <= a.y2
                    ).ToList();

                    // If we have a list...
                    if (claimsOverlap != null) {
                        // How many claims are on this point?
                        tile.count = claimsOverlap.Count;

                        // If we have overlap, ensure the claims state it
                        if (claimsOverlap.Count > 1)
                            claimsOverlap.ForEach(a => a.overlap = true);
                    } else {
                        tile.count = 0;
                    }

                    tiles.Add(tile);

                    // Draw this if desired
                    if (draw)
                        Console.Write(tile.count > 1 ? "#" : (tile.count == 1 ? "O" : "."));
                }

                if (draw)
                    Console.WriteLine();
            }
        }

        protected override string SolvePartOne()
        {
            return tiles.Count(a => a.count > 1).ToString();;
        }

        protected override string SolvePartTwo()
        {
            return claims.Where(a => a.overlap == false).First()?.id.ToString();
        }
    }
}
