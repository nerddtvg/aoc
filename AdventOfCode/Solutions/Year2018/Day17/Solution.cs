using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2018
{
    enum WaterTile
    {
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

        private int minY = 0;
        private int maxY = 0;
        private int minX = 0;
        private int maxX = 0;

        public Day17() : base(17, 2018, "Reservoir Research")
        {
//             DebugInput = @"x=495, y=2..7
// y=7, x=495..501
// x=501, y=3..7
// x=498, y=2..4
// x=506, y=1..2
// x=498, y=10..13
// x=504, y=10..13
// y=13, x=498..504";

            this.ReadInput();
        }

        private WaterTile GetTile((int x, int y) pos) =>
            this.tiles.ContainsKey(pos) ? this.tiles[pos] : WaterTile.Sand;

        private void ReadInput()
        {
            // Read the input
            this.tiles = new Dictionary<(int x, int y), WaterTile>();

            // Spring is at 500,0
            this.tiles[(500, 0)] = WaterTile.Spring;

            // Start the flowing at 500,1
            this.tiles[(500, 1)] = WaterTile.Flowing;

            foreach (var line in Input.SplitByNewline(true, true))
            {
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

                for (int i = min; i <= max; i++)
                {
                    // Change our dynamic value
                    if (staticVar == "x")
                        pos.y = i;
                    else
                        pos.x = i;

                    this.tiles[pos] = WaterTile.Clay;
                }
            }

            minY = this.tiles.Keys.Min(pos => pos.y);
            maxY = this.tiles.Keys.Max(pos => pos.y);
            minX = this.tiles.Keys.Min(pos => pos.x);
            maxX = this.tiles.Keys.Max(pos => pos.x);
        }

        private bool runFlowing()
        {
            // This will run one generation of flowing water
            // If we return true, we made changes

            // Foreach WaterTile.Flowing =>
            // Flow down then left+right
            var flowingKeys = this.tiles.Where(kvp => kvp.Value == WaterTile.Flowing).Select(kvp => kvp.Key).ToList();

            var ret = false;

            foreach(var flow in flowingKeys)
            {
                // Check that we can flow down
                var posY = (x: flow.x, y: flow.y + 1);

                // Don't go past the max depth
                if (posY.y > maxY)
                {
                    continue;
                }

                // We can flow down
                if (!this.tiles.ContainsKey(posY) || this.tiles[posY] == WaterTile.Sand)
                {
                    ret = true;
                    this.tiles[posY] = WaterTile.Flowing;
                    continue;
                }

                // We're already flowing down, skip
                if (this.tiles[posY] == WaterTile.Flowing)
                {
                    continue;
                }

                // If the tile below us is still or clay, move left+right, we become still
                if (this.tiles[posY] == WaterTile.Clay || this.tiles[posY] == WaterTile.Still)
                {
                    // Left and Right
                    var posX1 = (x: flow.x - 1, y: flow.y);
                    var posX2 = (x: flow.x + 1, y: flow.y);

                    // Move left, if sand or non-existent, it's flowing
                    while (GetTile(posX1) == WaterTile.Sand)
                    {
                        ret = true;
                        this.tiles[posX1] = WaterTile.Flowing;

                        // If the next tile below is Sand, stop moving
                        var posX1Y = (posX1.x, posX1.y + 1);
                        if (GetTile(posX1Y) == WaterTile.Sand)
                            break;
                            
                        posX1 = (x: posX1.x - 1, y: posX1.y);
                    }

                    // Move right, if sand or non-existent, it's flowing
                    while (GetTile(posX2) == WaterTile.Sand)
                    {
                        ret = true;
                        this.tiles[posX2] = WaterTile.Flowing;

                        // If the next tile below is Sand, stop moving
                        var posX2Y = (posX2.x, posX1.y + 1);
                        if (GetTile(posX2Y) == WaterTile.Sand)
                            break;

                        posX2 = (x: posX2.x + 1, y: posX2.y);
                    }

                    // If we have "flowed", continue
                    if (ret)
                        continue;

                    // Lastly, we need to detect any "rows" of flowing water blocked by clay
                    if (GetTile(posX1) == WaterTile.Clay || GetTile(posX2) == WaterTile.Clay)
                    {
                        // Determine if we have a line of flowing here
                        // bounded by clay on both sides
                        int clayX1 = flow.x;
                        int clayX2 = flow.x;

                        // Check left
                        do
                        {
                            if (GetTile((clayX1, flow.y)) == WaterTile.Clay || GetTile((clayX1, flow.y)) != WaterTile.Flowing || clayX1 < minX)
                                break;

                            clayX1--;
                        } while (true);

                        do
                        {
                            if (GetTile((clayX2, flow.y)) == WaterTile.Clay || GetTile((clayX2, flow.y)) != WaterTile.Flowing || clayX2 > maxX)
                                break;

                            clayX2++;
                        } while (true);

                        // If either of these are sand, we don't do anything
                        if (GetTile((clayX1, flow.y)) == WaterTile.Sand || GetTile((clayX2, flow.y)) == WaterTile.Sand)
                        {
                            continue;
                        }

                        // If this is out of bounds, skip
                        if (clayX1 >= minX && clayX2 <= maxX)
                        {
                            for (int x = clayX1 + 1; x < clayX2; x++)
                            {
                                this.tiles[(x, flow.y)] = WaterTile.Still;
                            }

                            ret = true;
                        }
                    }

                    continue;
                }
            }

            return ret;
        }

        private void PrintGrid()
        {
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    var tile = this.tiles.ContainsKey((x, y)) ? this.tiles[(x, y)] : WaterTile.Sand;
                    Console.Write(tileText[(int)tile]);
                }

                Console.WriteLine();
            }
            
            Console.WriteLine();
        }

        protected override string SolvePartOne()
        {
            while (this.runFlowing())
            {
                if (!string.IsNullOrEmpty(DebugInput))
                    PrintGrid();
            }
            
            // PrintGrid();

            // My answers are off by just a few, refactoring
            // 50828 = Too low
            // 50835 = Tow low
            // 50842 = Too high

            return this.tiles.Count(a => a.Value == WaterTile.Flowing || a.Value == WaterTile.Still).ToString();
        }

        protected override string SolvePartTwo()
        {
            return string.Empty;
        }
    }
}
