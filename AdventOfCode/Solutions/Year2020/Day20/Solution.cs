using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{
    class SatTile {
        public int id {get;set;}
        public string tile {get;set;}
        public List<string> edges {get;set;}

        public int x {get;set;}
        public int y {get;set;}
        public bool corner {get;set;}

        public bool searched {get;set;}

        public SatTile(string[] input) {
            this.edges = new List<string>();

            this.x = Int32.MinValue;
            this.y = Int32.MinValue;
            this.corner = false;
            this.searched = false;

            // Get the Tile ID
            this.id = Int32.Parse(input[0].Split(" ", StringSplitOptions.TrimEntries)[1].Replace(":", ""));

            // The rest of the input...
            this.tile = string.Join("\n", input.Skip(1));

            // Set the edges (top, right, bottom, left)
            this.edges.Add(input[1]);
            this.edges.Add(string.Join("", input.Skip(1).Select(a => a.Substring(a.Length-1, 1))));
            this.edges.Add(input[10]);
            this.edges.Add(string.Join("", input.Skip(1).Select(a => a.Substring(0, 1))));
        }

        public void RotateTile(int count=0) {
            // We will rotate the tile X times 90 degrees
            while(count < 0) count += 4;
            count = count % 4;

            // If this is 0, we don't do anything
            if (count == 0) return;

            // Re-order the edges (top, right, bottom, left)
            List<string> tEdges = new List<string>();
            for(int i=count; i<this.edges.Count; i++)
                tEdges.Add(this.edges[i]);

            for(int i=0; i<count; i++)
                tEdges.Add(this.edges[i]);
            
            this.edges = tEdges;

            // Trim out the newlines because they will trip up the math
            this.tile = this.tile.Replace("\n", "");
            string tTile = "";
            if (count == 1) {
                // Bottom to top, left to right
                for(int x=0; x<10; x++) {
                    for(int y=9; y>=0; y--)
                        tTile += this.tile.Substring((y*10)+x, 1);
                    
                    tTile += "\n";
                }
            } else if (count == 2) {
                // Right to left, bottom to top
                for(int y=9; y>=0; y--) {
                    for(int x=9; x>=0; x--)
                        tTile += this.tile.Substring((y*10)+x, 1);
                    
                    tTile += "\n";
                }
            } else if (count == 3) {
                // Bottom to top, right to left
                for(int x=9; x>=0; x--) {
                    for(int y=9; y>=0; y--)
                        tTile += this.tile.Substring((y*10)+x, 1);
                    
                    tTile += "\n";
                }
            }

            this.tile = tTile;
        }

        public void FindNeighbors(ref List<SatTile> tiles) {
            // Avoiding stack issues
            if (this.searched) return;
            this.searched = true;

            // We need to look for each of our neighbors (up, right, down, left)
            // Find up
            FindNeighbor(ref tiles, 0);

            // Find right
            FindNeighbor(ref tiles, 1);

            // Find down
            FindNeighbor(ref tiles, 2);

            // Find left
            FindNeighbor(ref tiles, 3);

            // Now we need to find the neighbor's neighbors
            //tiles.Where(a => a.x == this.x && a.y == this.y-1).FirstOrDefault()?.FindNeighbors(ref tiles);
            //tiles.Where(a => a.x == this.x && a.y == this.y+1).FirstOrDefault()?.FindNeighbors(ref tiles);
            //tiles.Where(a => a.x == this.x-1 && a.y == this.y).FirstOrDefault()?.FindNeighbors(ref tiles);
            //tiles.Where(a => a.x == this.x+1 && a.y == this.y).FirstOrDefault()?.FindNeighbors(ref tiles);
        }

        private void FindNeighbor(ref List<SatTile> tiles, int direction) {
            int newX = this.x + (direction % 2 == 0 ? 0 : (direction == 1 ? 1 : -1));
            int newY = this.y + (direction % 2 == 0 ? (direction == 0 ? -1 : 1) : 0);

            // Already done
            if (tiles.Count(a => a.x == newX && a.y == newY) > 0) return;

            // Find a tile that matches our desired edge
            var dir = tiles.Where(a => a.searched == false && a.edges.Contains(this.edges[direction])).FirstOrDefault();

            if (dir != null) {
                // Found it!
                // Figure out the direction we need to rotate (if any)
                for(int i=0; i<dir.edges.Count; i++)
                    if (dir.edges[i] == this.edges[direction]) {
                        dir.RotateTile((direction+2)-i);
                        break;
                    }
                
                dir.x = newX;
                dir.y = newY;
                dir.FindNeighbors(ref tiles);
            }
        }
    }

    class Day20 : ASolution
    {
        List<SatTile> tiles = new List<SatTile>();
        List<string> allEdges;

        public Day20() : base(20, 2020, "")
        {
            // Load the tiles
            foreach(var tile in Input.SplitByBlankLine(true))
                tiles.Add(new SatTile(tile));
            
            // Get all edges
            allEdges = tiles.SelectMany(a => a.edges).ToList();

            // Just start with the first tile and let's go through the list
            tiles[0].x = 0;
            tiles[0].y = 0;
            tiles[0].FindNeighbors(ref tiles);
        }

        protected override string SolvePartOne()
        {
            return tiles.Where(a => a.corner == true).Count().ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
