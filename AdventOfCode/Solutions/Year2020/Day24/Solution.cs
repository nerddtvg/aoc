using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2020
{
    // We're going to use a basic x,y system but offset it
    // So odd rows will actually be 1/2 block to the left of even rows
    // http://playtechs.blogspot.com/2007/04/hex-grids.html
    class HexTile {
        public bool isBlack {get;set;}

        public HexTile() {
            // Starts off white
            this.isBlack = false;
        }
    }

    class Day24 : ASolution
    {
        // false = white
        // true = black
        Dictionary<(int x, int y), bool> tiles = new Dictionary<(int x, int y), bool>();

        public Day24() : base(24, 2020, "")
        {
            // We need to go through each line and process it
            foreach(var line in Input.SplitByNewline()) {
                // Everything starts at 0,0
                (int x, int y) pos = (0, 0);

                // Figure out the position
                foreach(var dir in readLine(line))
                    pos = getXY(pos, dir);
                
                // Now we can set the tile
                setTile(pos);
            }
        }

        private void setTile((int x, int y) pos) {
            if (tiles.ContainsKey(pos))
                tiles[pos] = !tiles[pos];
            else
                // This tile isn't set already, assume it's white
                // And now we're setting it means it is black
                tiles[pos] = true;
        }

        private bool getTile((int x, int y) pos) {
            if (tiles.ContainsKey(pos))
                return tiles[pos];
            else
                // This tile isn't set already, assume it's white
                return false;
        }

        private List<string> readLine(string line) {
            // Read the instructions via regex
            var instr = new Regex("^(e|se|sw|w|nw|ne)+$");
            var matches = instr.Matches(line);

            if (matches.Count != 1 || matches[0].Groups.Count != 2) return new List<string>();

            return matches[0].Groups[1].Captures.Select(a => a.Value).ToList();
        }

        private (int x, int y) getXY((int x, int y) pos, string dir) {
            // Takes a direction and returns a new x,y
            switch(dir) {
                case "e":
                    return (pos.x+1, pos.y);
                    
                case "sw":
                    return (pos.x-1, pos.y-1);
                    
                case "se":
                    return (pos.x+1, pos.y-1);
                    
                case "w":
                    return (pos.x-1, pos.y);
                    
                case "nw":
                    return (pos.x-1, pos.y+1);
                    
                case "ne":
                    return (pos.x+1, pos.y+1);
                
                default: throw new Exception($"Invalid direction: {dir}");
            }
        }

        protected override string SolvePartOne()
        {
            return this.tiles.Count(a => a.Value).ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
