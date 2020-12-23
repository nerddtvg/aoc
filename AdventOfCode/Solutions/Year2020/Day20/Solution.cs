using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{
    class SatTile {
        public int id {get;set;}
        public string tile {get;set;}
        public List<uint> edges {get;set;}

        public int x {get;set;}
        public int y {get;set;}
        public bool corner {get;set;}

        public bool searched {get;set;}

        public override string ToString()
        {
            return $"{this.id}: {string.Join(", ", this.edges)}";
        }

        public SatTile(string[] input) {
            this.edges = new List<uint>();

            this.x = Int32.MinValue;
            this.y = Int32.MinValue;
            this.corner = false;
            this.searched = false;

            // Get the Tile ID
            this.id = Int32.Parse(input[0].Split(" ", StringSplitOptions.TrimEntries)[1].Replace(":", ""));

            // The rest of the input...
            this.tile = string.Join("\n", input.Skip(1));

            // Set the edges (top, right, bottom, left)
            this.edges.Add(HashEdge(input[1]));
            this.edges.Add(HashEdge(string.Join("", input.Skip(1).Select(a => a.Substring(a.Length-1, 1)))));
            this.edges.Add(HashEdge(input[10]));
            this.edges.Add(HashEdge(string.Join("", input.Skip(1).Select(a => a.Substring(0, 1)))));
        }

        private uint HashEdge(string edge) {
            // Change all of the '.' to 0 and '#' to 1s
            // Then we can bitwise match against these values
            uint ret = 0b_0000000000;
            int pow = edge.Length;

            for(int i=0; i<edge.Length; i++)
                ret += edge.Substring(i, 1) == "#" ? (uint) Math.Pow(2, (pow-i)) : 0;
            
            return ret;
        }

        /*
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

            Console.WriteLine($"FindNeighbors: {this.id}, {this.x}, {this.y}");

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
            } else {
                Console.WriteLine($"No Neighbor: {this.id}, {this.x}, {this.y}, direction: {direction}");
            }
        }
        */
    }

    class Day20 : ASolution
    {
        List<SatTile> tiles = new List<SatTile>();
        List<string> allEdges;

        public Day20() : base(20, 2020, "")
        {
            // Debug Input
            DebugInput = @"
            Tile 2311:
            ..##.#..#.
            ##..#.....
            #...##..#.
            ####.#...#
            ##.##.###.
            ##...#.###
            .#.#.#..##
            ..#....#..
            ###...#.#.
            ..###..###

            Tile 1951:
            #.##...##.
            #.####...#
            .....#..##
            #...######
            .##.#....#
            .###.#####
            ###.##.##.
            .###....#.
            ..#.#..#.#
            #...##.#..

            Tile 1171:
            ####...##.
            #..##.#..#
            ##.#..#.#.
            .###.####.
            ..###.####
            .##....##.
            .#...####.
            #.##.####.
            ####..#...
            .....##...

            Tile 1427:
            ###.##.#..
            .#..#.##..
            .#.##.#..#
            #.#.#.##.#
            ....#...##
            ...##..##.
            ...#.#####
            .#.####.#.
            ..#..###.#
            ..##.#..#.

            Tile 1489:
            ##.#.#....
            ..##...#..
            .##..##...
            ..#...#...
            #####...#.
            #..#.#.#.#
            ...#.#.#..
            ##.#...##.
            ..##.##.##
            ###.##.#..

            Tile 2473:
            #....####.
            #..#.##...
            #.##..#...
            ######.#.#
            .#...#.#.#
            .#########
            .###.#..#.
            ########.#
            ##...##.#.
            ..###.#.#.

            Tile 2971:
            ..#.#....#
            #...###...
            #.#.###...
            ##.##..#..
            .#####..##
            .#..####.#
            #..#.#..#.
            ..####.###
            ..#.#.###.
            ...#.#.#.#

            Tile 2729:
            ...#.#.#.#
            ####.#....
            ..#.#.....
            ....#..#.#
            .##..##.#.
            .#.####...
            ####.#.#..
            ##.####...
            ##..#.##..
            #.##...##.

            Tile 3079:
            #.#.#####.
            .#..######
            ..#.......
            ######....
            ####.#..#.
            .#...#.##.
            #.#####.##
            ..#.###...
            ..#.......
            ..#.###...";

            // Load the tiles
            foreach(var tile in Input.SplitByBlankLine(true))
                tiles.Add(new SatTile(tile));

            // Go through and find the corners
            tiles.ForEach(tile => {
                // If a tile only matches on two sides, it's a corner
            
                // Get all edges that are not this one
                // Get the bitwise inverse to match against flips
                var allEdges = tiles.Where(a => a.id != tile.id).SelectMany(a => a.edges.Union(a.edges.Select(a => ~a))).Distinct().ToList();
                var matches = tile.edges.Intersect(allEdges);
                var count = matches.Count();

                Console.WriteLine($"{tile.id}: {count}");

                // Check if we only have two matching
                if (count == 2)
                    tile.corner = true;
            });

            // Just start with the first tile and let's go through the list
            //tiles[0].x = 0;
            //tiles[0].y = 0;
            //tiles[0].FindNeighbors(ref tiles);
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
