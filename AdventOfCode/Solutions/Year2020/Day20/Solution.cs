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

        public SatTile(string[] input) {
            this.edges = new List<string>();

            this.x = 0;
            this.y = 0;
            this.corner = false;

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
