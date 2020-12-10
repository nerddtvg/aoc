using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2019
{

    class HullTile {
        public int x {get;set;}
        public int y {get;set;}
        public int color {get;set;}
    }

    class Day11 : ASolution
    {

        public List<HullTile> tiles {get;set;}

        public Day11() : base(11, 2019, "")
        {
            this.tiles = new List<HullTile>();
        }

        protected override string SolvePartOne()
        {
            Intcode intcode = new Intcode(Input, 2);
            //intcode.Run();

            // Traverse the hull
            // If a tile exists, return the color appropriately
            // If not, it hasn't been painted and it is black
            int x = 0;
            int y = 0;
            int dir = 0; // 0 == up, 1 == right, 2 == down, 3 == left
            while(intcode.State == State.Waiting) {
                // Each run is:
                // Send tile color input
                // Run
                // Retrieve new? color
                // Run
                // Retrieve direction
                // Change direction and update x/y

                IEnumerable<HullTile> search = tiles.Where(t => t.x == x && t.y == y);

                HullTile t;

                if (search.Count() == 0) {
                    t = new HullTile() { x = x, y = y, color = 0};
                } else {
                    t = search.First();

                    // Remove the tile so we can update it
                    tiles.Remove(t);
                }

                // Send the color value
                intcode.SetInput(t.color);
                intcode.Run();

                // Get new color value
                t.color = Convert.ToInt32(intcode.output_register);

                // Add to the list
                this.tiles.Add(t);

                // Continue to get the direction
                intcode.Run();
                
                // Change direction
                if (intcode.output_register.Dequeue() == 0) {
                    dir -= 1;
                } else {
                    dir += 1;
                }

                // Correct the value
                if (dir < 0) dir += 4;
                dir %= 4;

                switch(dir) {
                    case 0:
                        y -= 1;
                        break;
                        
                    case 1:
                        x += 1;
                        break;
                        
                    case 2:
                        y += 1;
                        break;
                        
                    case 3:
                        x -= 1;
                        break;
                }

                // Move on!
            }

            return this.tiles.Count.ToString();
        }

        protected override string SolvePartTwo()
        {
            // Starts on a single white panel
            this.tiles = new List<HullTile>();

            Intcode intcode = new Intcode(Input, 2);
            //intcode.Run();

            // Traverse the hull
            // If a tile exists, return the color appropriately
            // If not, it hasn't been painted and it is black
            int x = 0;
            int y = 0;
            int dir = 0; // 0 == up, 1 == right, 2 == down, 3 == left

            bool first = true;

            while(intcode.State == State.Waiting) {
                // Each run is:
                // Send tile color input
                // Run
                // Retrieve new? color
                // Run
                // Retrieve direction
                // Change direction and update x/y

                IEnumerable<HullTile> search = tiles.Where(t => t.x == x && t.y == y);

                HullTile t;

                if (search.Count() == 0) {
                    t = new HullTile() { x = x, y = y, color = 0};
                } else {
                    t = search.First();

                    // Remove the tile so we can update it
                    tiles.Remove(t);
                }

                if (first) {
                    t.color = 1;
                    first = false;
                }

                // Send the color value
                intcode.SetInput(t.color);
                intcode.Run();

                // Get new color value
                t.color = Convert.ToInt32(intcode.output_register);

                // Add to the list
                this.tiles.Add(t);

                // Continue to get the direction
                intcode.Run();
                
                // Change direction
                if (intcode.output_register.Dequeue() == 0) {
                    dir -= 1;
                } else {
                    dir += 1;
                }

                // Correct the value
                if (dir < 0) dir += 4;
                dir %= 4;

                switch(dir) {
                    case 0:
                        y -= 1;
                        break;
                        
                    case 1:
                        x += 1;
                        break;
                        
                    case 2:
                        y += 1;
                        break;
                        
                    case 3:
                        x -= 1;
                        break;
                }

                // Move on!
            }

            string output = "\n";

            int sy = tiles.Min(a => a.y);
            int ey = tiles.Max(a => a.y);

            int sx = tiles.Min(a => a.x);
            int ex = tiles.Max(a => a.x);

            for(y=sy; y<=ey; y++) {
                for(x=sx; x<=ex; x++) {
                    HullTile tile = tiles.FirstOrDefault(a => a.x == x && a.y == y);

                    if (tile == null || tile.color == 0) {
                        output += " ";
                    } else {
                        output += "#";
                    }
                }

                output += "\n";
            }

            return output;
        }
    }
}
