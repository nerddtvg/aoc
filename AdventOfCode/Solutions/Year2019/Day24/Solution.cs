using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2019
{
    class Day24 : ASolution
    {
        Dictionary<(int x, int y), bool> tiles = new Dictionary<(int x, int y), bool>();
        List<ulong> history = new List<ulong>();

        public bool getValue((int x, int y) pos) =>
            this.tiles.ContainsKey(pos) ? this.tiles[pos] : false;

        public List<bool> getNeighbors((int x, int y) pos) =>
            new List<bool>() {
                getValue((pos.x, pos.y-1)),
                getValue((pos.x-1, pos.y)),
                getValue((pos.x+1, pos.y)),
                getValue((pos.x, pos.y+1))
            };
        
        public bool getNewValue(bool current, (int x, int y) pos) =>
            (
                (current && getNeighbors(pos).Count(a => a) == 1)
                ||
                (!current && (getNeighbors(pos).Count(a => a) == 1 || getNeighbors(pos).Count(a => a) == 2))
            );

        public Day24() : base(24, 2019, "")
        {
            /*
            DebugInput = @"
            ....#
            #..#.
            #..##
            ..#..
            #....";
            */

            // Read the input
            int x,y;

            y = 0;
            foreach(string line in Input.SplitByNewline(true, true)) {
                x = 0;
                foreach(var c in line) {
                    this.tiles.Add((x, y), (c == '#'));

                    x++;
                }

                y++;
            }

            // Add the first biodiversity value
            this.history.Add(this.getBiodiversity());
        }

        private ulong getBiodiversity(Dictionary<(int x, int y), bool> tempTiles = null) {
            ulong ret = 0;

            if (tempTiles == null)
                tempTiles = this.tiles;

            int maxX = tempTiles.Max(a => a.Key.x);
            int maxY = tempTiles.Max(a => a.Key.y);

            int power = 0;
            for(int y=0; y<=maxY; y++)
                for(int x=0; x<=maxX; power++,x++)
                    ret += (tempTiles[(x, y)] == true ? (ulong) Math.Pow(2, power) : 0);
            
            return ret;
        }

        private ulong runGeneration() {
            // Need a new list
            Dictionary<(int x, int y), bool> newTiles = new Dictionary<(int x, int y), bool>();

            foreach(var key in this.tiles.Keys) {
                newTiles[key] = this.getNewValue(this.tiles[key], key);
            }

            this.tiles = newTiles;

            var bd = this.getBiodiversity();
            ulong ret = 0;

            // Check before adding
            if (this.history.Contains(bd))
                ret = bd;
            
            // Now add to the history
            this.history.Add(bd);
            
            return ret;
        }

        protected override string SolvePartOne()
        {
            ulong bd = 0;

            while(bd == 0) {
                bd = this.runGeneration();
            }
            
            return bd.ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
