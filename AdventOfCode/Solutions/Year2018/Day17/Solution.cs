using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2018
{
    enum WaterTile {
        Sand,
        Clay,
        Flowing,
        Still,
        Spring
    }

    class Day17 : ASolution
    {
        Dictionary<(int x, int y), WaterTile> tiles = new Dictionary<(int x, int y), WaterTile>();

        public readonly string[] tileText = new string[] { ".", "#", "|", "~", "+" };

        public Day17() : base(17, 2018, "")
        {
            this.ReadInput();
        }

        private WaterTile GetTile((int x, int y) pos) =>
            this.tiles.ContainsKey(pos) ? this.tiles[pos] : WaterTile.Sand;

        private void ReadInput() {
            // Read the input
            this.tiles = new Dictionary<(int x, int y), WaterTile>();

            // Spring is at 500,0
            this.tiles[(500, 0)] = WaterTile.Spring;

            // Start the flowing at 500,1
            this.tiles[(500, 1)] = WaterTile.Flowing;

            foreach(var line in Input.SplitByNewline(true, true)) {
                var staticVar = line.Substring(0, 1);
                var staticVal = Int32.Parse(line.Split(",")[0].Split("=", StringSplitOptions.TrimEntries)[1]);
                var range = line.Split(",")[1].Split("=")[1].Split("..", StringSplitOptions.TrimEntries);

                var min = Int32.Parse(range[0]);
                var max = Int32.Parse(range[1]);

                // Where are we working?
                (int x, int y) pos = (0, 0);

                if (staticVar == "x")
                    pos.x = staticVal;
                else
                    pos.y = staticVal;

                for(int i=min; i<=max; i++) {
                    // Change our dynamic value
                    if (staticVar == "x")
                        pos.y = i;
                    else
                        pos.x = i;
                    
                    this.tiles[pos] = WaterTile.Clay;
                }
            }
        }

        private bool runFlowing() {
            // This will run one generation of flowing water
            // If we return true, we made changes

            return false;
        }

        protected override string SolvePartOne()
        {
            while(this.runFlowing()) {}

            return this.tiles.Count(a => a.Value == WaterTile.Flowing || a.Value == WaterTile.Still).ToString();
        }

        protected override string SolvePartTwo()
        {
            return string.Empty;
        }
    }
}
