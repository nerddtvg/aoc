using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{
    class SatTileVariant {
        public List<List<string>> tile {get;set;}
        public List<string> edges {get;set;}

        public SatTileVariant(string[] input) {
            // Input is an array of strings (one per line of the tile)
            this.tile = input.Select(a => a.Select(b => b.ToString()).ToList()).ToList();

            // Set the edges (top, right, bottom, left)
            this.edges = new List<string>();
            this.edges.Add(input[0]);
            this.edges.Add(string.Join("", input.Select(a => a.Substring(a.Length-1, 1))));
            this.edges.Add(input[9]);
            this.edges.Add(string.Join("", input.Select(a => a.Substring(0, 1))));
        }
    }

    class SatTile {
        public int id {get;set;}
        public List<List<string>> tile {get;set;}

        public int x {get;set;}
        public int y {get;set;}
        public bool corner {get;set;}

        public bool searched {get;set;}

        // Let's keep a list of all rotated and flipped tile variants
        // This will make matching/string manipulation easier later
        public List<SatTileVariant> variants {get;set;}

        // A quick list of our edges
        public List<string> edges {get;set;}

        public override string ToString()
        {
            return $"{this.id} [{(this.x == Int32.MinValue ? "unk" : this.x)}, {(this.y == Int32.MinValue ? "unk" : this.y)}]: {string.Join(", ", this.variants[0].edges)}";
        }

        public SatTile(string[] input) {
            this.x = Int32.MinValue;
            this.y = Int32.MinValue;
            this.corner = false;
            this.searched = false;

            // Get the Tile ID
            this.id = Int32.Parse(input[0].Split(" ", StringSplitOptions.TrimEntries)[1].Replace(":", ""));

            // The rest of the input...
            input = input.Skip(1).ToArray();
            this.tile = input.Select(a => a.Select(b => b.ToString()).ToList()).ToList();

            // Set our edges for part 1
            this.edges = new List<string>();
            this.edges.Add(input[0]);
            this.edges.Add(string.Join("", input.Select(a => a.Substring(a.Length-1, 1))));
            this.edges.Add(input[9]);
            this.edges.Add(string.Join("", input.Select(a => a.Substring(0, 1))));

            // Get all variants
            this.variants = new List<SatTileVariant>();

            for(int flip=0; flip<2; flip++) {
                for(int rotate=0; rotate<4; rotate++) {
                    // Load the variant
                    var v = (string[]) input.Clone();

                    // We need to work on this
                    // Flip all of the lines around
                    if (flip > 0)
                        for(int i=0; i<v.Length; i++)
                            v[i] = v[i].Reverse();
                    
                    // Now we need to (maybe) rotate
                    for(int i=0; i<rotate; i++)
                        v = rotateArray(v);

                    this.variants.Add(new SatTileVariant(v));
                }
            }
        }

        public List<string> getTileWithoutBorder() =>
            // This returns the tile string without the borders (1 pixel on each side)
            this.tile.Skip(1).Take(8).Select(a => string.Join("", a.Skip(1).Take(8))).ToList();

        public string[] rotateArray(string[] input) {
            // Take the input array and rotate it 90 degrees
            string[] outArr = new string[input.Length];

            // Rotating 90 degrees is "simply" reading bottom to top, left to right
            for(int line=0; line<input[0].Length; line++) {
                // line is the index for outArr
                outArr[line] = "";

                // line is also the 'x' value in input
                for(int y=input.Length-1; y>=0; y--)
                    outArr[line] += input[y].Substring(line, 1);
            }

            return outArr;
        }

        public List<List<string>> rotateArray(List<List<string>> input) =>
            rotateArray(
                input.Select(a => string.Join("", a)).ToArray()
            ).Select(a => a.Select(a => a.ToString()).ToList()).ToList();

        public void FindNeighbors(ref List<SatTile> tiles) {
            // Avoiding stack issues
            if (this.searched) return;
            this.searched = true;

            //Console.WriteLine($"FindNeighbors: {this.id}, {this.x}, {this.y}");

            // We need to look for each of our neighbors (up, right, down, left)
            // Find up
            FindNeighbor(ref tiles, 0);

            // Find right
            FindNeighbor(ref tiles, 1);

            // Find down
            FindNeighbor(ref tiles, 2);

            // Find left
            FindNeighbor(ref tiles, 3);
        }

        private void FindNeighbor(ref List<SatTile> tiles, int direction) {
            int newX = this.x + (direction % 2 == 0 ? 0 : (direction == 1 ? 1 : -1));
            int newY = this.y + (direction % 2 == 0 ? (direction == 0 ? -1 : 1) : 0);

            // Already done
            if (tiles.Count(a => a.x == newX && a.y == newY) > 0) return;

            // Find a tile that has a variant matches our desired edge
            // Since we have every variant of each tile, we should be able to find one where the side we want is opposite our current side
            int newDirection = (direction + 2) % 4;

            var newTile = tiles.Where(a => a.searched == false && a.variants.Select(b => b.edges[newDirection]).Contains(this.edges[direction])).FirstOrDefault();

            if (newTile != null) {
                // Found it!

                // Get the specific variant
                var variant = newTile.variants.First(a => a.edges[newDirection] == this.edges[direction]);

                // Replace this tile information to account for the new rotation/flip information
                newTile.tile = variant.tile;
                newTile.edges = variant.edges;

                // Clear the variants to prevent double-matching later
                newTile.variants.Clear();
                
                // Set the new x,y
                newTile.x = newX;
                newTile.y = newY;

                // Find neighbors!
                newTile.FindNeighbors(ref tiles);
            }
        }
    }

    class Day20 : ASolution
    {
        List<SatTile> tiles = new List<SatTile>();
        List<string> allEdges;

        public Day20() : base(20, 2020, "")
        {
            // Debug Input
            /** /
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
            /**/

            // Load the tiles
            foreach(var tile in Input.SplitByBlankLine(true))
                tiles.Add(new SatTile(tile));

            // Go through and find the corners
            tiles.ForEach(tile => {
                // If a tile only matches on two sides, it's a corner
            
                // Get all edges that are not this one
                // Get the reverse of each to match against flips
                var allEdges = tiles.Where(a => a.id != tile.id).SelectMany(a => a.edges.Union(a.edges.Select(a => a.Reverse()))).Distinct().ToList();
                var matches = tile.edges.Intersect(allEdges);
                var count = matches.Count();

                //Console.WriteLine($"{tile.id}: {count}");

                // Check if we only have two matching
                if (count == 2)
                    tile.corner = true;
            });
        }

        protected override string SolvePartOne()
        {
            return tiles.Where(a => a.corner == true).Aggregate((long) 1, (x, y) => x * (long) y.id).ToString();
        }

        protected override string SolvePartTwo()
        {
            // We need to count the '#' that are not a part of the sea monsters
            // Sea monsters are 15 '#' characters
            /*
                              # 
            #    ##    ##    ###
             #  #  #  #  #  #   
            */
            // We need to remove the edges from each tile before stitching them together
            
            // First we get the tiles in order
            // Start in a corner
            var corner = tiles.First(a => a.corner == true);
            corner.x = 0;
            corner.y = 0;
            corner.FindNeighbors(ref tiles);

            // Let's go through and put all of these together into one large image
            int minX = tiles.Where(a => a.searched).Min(a => a.x);
            int maxX = tiles.Where(a => a.searched).Max(a => a.x);
            int minY = tiles.Where(a => a.searched).Min(a => a.y);
            int maxY = tiles.Where(a => a.searched).Max(a => a.y);

            Dictionary<int, string> image = new Dictionary<int, string>();

            for(int y=minY; y<=maxY; y++)
                for(int x=minX; x<=maxX; x++) {
                    var tile = tiles.FirstOrDefault(a => a.x == x && a.y == y);
                    List<string> tileString;

                    if (tile != null) {
                        tileString = tile.getTileWithoutBorder();
                    } else {
                        throw new Exception($"Ended up with a missing tile: X: {x}, Y: {y}");
                    }

                    // Go through the image and add this on to each line
                    int b = (y - minY);     // What "line" are we one, jumps of 10
                    
                    for(int i=0; i<tileString.Count; i++) {
                        int key = (b * 10) + i;

                        if(image.ContainsKey(key)) {
                            image[key] += tileString[i];
                        } else {
                            image[key] = tileString[i];
                        }
                    }
                }
            
            // Get the image in array form (so we can work with it again)
            // 2D array: [y][x]
            var imageArr = image.OrderBy(a => a.Key).Select(a => a.Value.Select(a => a.ToString()).ToList()).ToList();

            // Now we have an array of strings that form our image
            // We need to go through and find all of the sea monsters
            // Then replace those with 'O' so we don't double-count
            // Be sure to rotate the image to find all of them
            
            // We define an array of 'monster' dx,dy of what is expected
            /*
                        1111111111
              01234567890123456789
             +--------------------+
            0|                  # |0
             +--------------------+
             |          1111111111|
             |01234567890123456789|
             +--------------------+
            1|#    ##    ##    ###|1
             +--------------------+
             |          1111111111|
             |01234567890123456789|
             +--------------------+
            2| #  #  #  #  #  #   |2
             +--------------------+
            */
            List<(int dy, int dx)> monster = new List<(int dy, int dx)>() {
                (0, 18),
                (1, 0),
                (1, 5),
                (1, 6),
                (1, 11),
                (1, 12),
                (1, 17),
                (1, 18),
                (1, 19),
                (2, 1),
                (2, 4),
                (2, 7),
                (2, 10),
                (2, 13),
                (2, 16)
            };

            // Helpers for loops later
            int monsterWidth = 20;
            int monsterHeight = 3;

            // Print the starting image
            Console.WriteLine("\n" + string.Join("\n", imageArr));
            Console.WriteLine();

            int monsters = 0;
            for(int flip=0; flip<2; flip++) {
                for(int rotate=0; rotate<4; rotate++) {
                    // We need to go through and find all instances where the '#' lines up to a monster
                    for(int y=0; y<imageArr.Count-monsterHeight+1; y++) {
                        for(int x=0; x<imageArr[0].Count-monsterWidth+1; x++) {
                            // If we have the same number of '#' as the monster array, then we have a monster!
                            if (monster.Count(a => imageArr[y+a.dy][x+a.dx] == "#") == monster.Count) {
                                monsters++;

                                // Change all of them to 'O' to avoid double counting
                                monster.ForEach(a => imageArr[y+a.dy][x+a.dx] = "O");
                            }
                        }
                    }

                    // If we found monsters, we're in the right position, break out
                    if (monsters > 0) break;

                    // Using the helper to rotate the image
                    imageArr = this.tiles[0].rotateArray(imageArr);
                }

                // If we found monsters, we're in the right position, break out
                if (monsters > 0) break;

                // Need to flip the image
                imageArr.ForEach(a => a.Reverse());
            }

            // Print the final image
            Console.WriteLine("\n" + string.Join("\n", imageArr.Select(a => string.Join("", a))));

            Console.WriteLine($"Monsters: {monsters}");

            return imageArr.Sum(a => a.Count(b => b == "#")).ToString();
        }
    }
}
