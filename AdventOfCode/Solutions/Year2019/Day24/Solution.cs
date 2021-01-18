using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2019
{
    class Day24 : ASolution
    {
        // Part 1
        Dictionary<(int x, int y), bool> tiles = new Dictionary<(int x, int y), bool>();

        // Part 2
        Dictionary<(int x, int y, int z), bool> tilesZ = new Dictionary<(int x, int y, int z), bool>();

        List<ulong> history = new List<ulong>();

        // Part 1
        public bool getValue((int x, int y) pos) =>
            this.tiles.ContainsKey(pos) ? this.tiles[pos] : false;

        // Part 2
        public bool getValue((int x, int y, int z) pos) =>
            this.tilesZ.ContainsKey(pos) ? this.tilesZ[pos] : false;

        public List<bool> getNeighbors((int x, int y) pos) =>
            new List<bool>() {
                getValue((pos.x, pos.y-1)),
                getValue((pos.x-1, pos.y)),
                getValue((pos.x+1, pos.y)),
                getValue((pos.x, pos.y+1))
            };
        
        public bool getNewValue(bool current, (int x, int y) pos) =>
            getNewValue(current, getNeighbors(pos));
        
        public bool getNewValue(bool current, List<bool> neighbors) =>
            (
                (current && neighbors.Count(a => a) == 1)
                ||
                (!current && (neighbors.Count(a => a) == 1 || neighbors.Count(a => a) == 2))
            );

        public Day24() : base(24, 2019, "")
        {
            /**/
            DebugInput = @"
            ....#
            #..#.
            #..##
            ..#..
            #....";
            /**/

            // Read the input
            int x,y;

            y = 0;
            foreach(string line in Input.SplitByNewline(true, true)) {
                x = 0;
                foreach(var c in line) {
                    this.tiles.Add((x, y), (c == '#'));
                    this.tilesZ.Add((x, y, 0), (c == '#'));

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

        private void runGeneration2() {
            // Need a new list
            Dictionary<(int x, int y, int z), bool> newTiles = new Dictionary<(int x, int y, int z), bool>();

            // We need to search up and down 1 level from our min and max depths
            int minZ = this.tilesZ.Min(a => a.Key.z)-1;
            int maxZ = this.tilesZ.Max(a => a.Key.z)+1;
            
            // We have to search this manually so we can keep track of where we are in the cube to determine neighbors
            for(int z=minZ; z<=maxZ; z++)
                for(int i=0; i<25; i++) {
                    // We are on tile 'i' of level 'z'
                    // We have to search other levels for all tiles EXCEPT: 7, 9, 17, 19
                    // We skip tile 12 because that's another level
                    if (i == 12) continue;

                    (int x, int y, int z) pos = (i % 5, i / 5, z);
                    
                    // Get our local neighbors
                    // Anything outside the range will return false
                    var neighbors = new List<bool>() {
                        getValue((pos.x, pos.y-1, pos.z)),
                        getValue((pos.x-1, pos.y, pos.z)),
                        getValue((pos.x+1, pos.y, pos.z)),
                        getValue((pos.x, pos.y+1, pos.z))
                    };

                    // If we are on the top row, get depth up one
                    if (i / 5 == 0)
                        neighbors.Add(getValue((2, 1, pos.z+1)));

                    // If we are on the bottom row, get depth up one
                    if (i / 5 == 4)
                        neighbors.Add(getValue((2, 3, pos.z+1)));

                    // If we are on the left column, get depth up one
                    if (i % 5 == 0)
                        neighbors.Add(getValue((1, 2, pos.z+1)));

                    // If we are on the right column, get depth up one
                    if (i % 5 == 4)
                        neighbors.Add(getValue((3, 2, pos.z+1)));
                    
                    // Now for the hard part: The internals down one level
                    if (i == 7) {
                        neighbors.Add(getValue((0, 0, pos.z-1)));
                        neighbors.Add(getValue((1, 0, pos.z-1)));
                        neighbors.Add(getValue((2, 0, pos.z-1)));
                        neighbors.Add(getValue((3, 0, pos.z-1)));
                        neighbors.Add(getValue((4, 0, pos.z-1)));
                    } else if (i == 11) {
                        neighbors.Add(getValue((0, 0, pos.z-1)));
                        neighbors.Add(getValue((0, 1, pos.z-1)));
                        neighbors.Add(getValue((0, 2, pos.z-1)));
                        neighbors.Add(getValue((0, 3, pos.z-1)));
                        neighbors.Add(getValue((0, 4, pos.z-1)));
                    } else if (i == 14) {
                        neighbors.Add(getValue((4, 0, pos.z-1)));
                        neighbors.Add(getValue((4, 1, pos.z-1)));
                        neighbors.Add(getValue((4, 2, pos.z-1)));
                        neighbors.Add(getValue((4, 3, pos.z-1)));
                        neighbors.Add(getValue((4, 4, pos.z-1)));
                    } else if (i == 18) {
                        neighbors.Add(getValue((0, 4, pos.z-1)));
                        neighbors.Add(getValue((1, 4, pos.z-1)));
                        neighbors.Add(getValue((2, 4, pos.z-1)));
                        neighbors.Add(getValue((3, 4, pos.z-1)));
                        neighbors.Add(getValue((4, 4, pos.z-1)));
                    }

                    // Now we have all of the possible neighbors, let's find the new value
                    newTiles[pos] = getNewValue(getValue(pos), neighbors);
                }

            this.tilesZ = newTiles;
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
            for(int i=0; i<10; i++)
                this.runGeneration2();
            
            return this.tilesZ.Count(a => a.Value == true).ToString();
        }
    }
}
