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

        // Refactored like this: https://old.reddit.com/r/adventofcode/comments/a6wpup/2018_day_17_solutions/ebysx1y/

        private void RunFlowing((int x, int y) pos)
        {
            // Change this tile to flowing
            this.tiles[pos] = WaterTile.Flowing;

            (int x, int y) = (pos.x, pos.y);
            while (GetTile((x, y+1)) != WaterTile.Clay && GetTile((x, y+1)) != WaterTile.Still)
            {
                y++;

                // Bail out if above the max
                if (y > maxY)
                    return;

                // This is now flowing
                this.tiles[(x, y)] = WaterTile.Flowing;
            }

            do
            {
                bool downLeft = false;
                bool downRight = false;

                int rangeMinX;
                for (rangeMinX = x; rangeMinX >= 0; rangeMinX--)
                {
                    if (GetTile((rangeMinX, y + 1)) != WaterTile.Clay && GetTile((rangeMinX, y + 1)) != WaterTile.Still)
                    {
                        downLeft = true;
                        break;
                    }

                    this.tiles[(rangeMinX, y)] = WaterTile.Flowing;

                    if (GetTile((rangeMinX - 1, y)) == WaterTile.Clay || GetTile((rangeMinX - 1, y)) == WaterTile.Still)
                    {
                        break;
                    }
                }

                int rangeMaxX;
                for (rangeMaxX = x; rangeMaxX <= maxX; rangeMaxX++)
                {
                    if (GetTile((rangeMaxX, y + 1)) != WaterTile.Clay && GetTile((rangeMaxX, y + 1)) != WaterTile.Still)
                    {
                        downRight = true;
                        break;
                    }

                    this.tiles[(rangeMaxX, y)] = WaterTile.Flowing;

                    if (GetTile((rangeMaxX + 1, y)) == WaterTile.Clay || GetTile((rangeMaxX + 1, y)) == WaterTile.Still)
                    {
                        break;
                    }
                }

                // Expand
                if (downLeft)
                {
                    if (GetTile((rangeMinX, y)) != WaterTile.Flowing)
                        RunFlowing((rangeMinX, y));
                }
                
                if (downRight)
                {
                    if (GetTile((rangeMaxX, y)) != WaterTile.Flowing)
                        RunFlowing((rangeMaxX, y));
                }

                if (downLeft || downRight)
                    return;

                // Found a boundary, fill it up
                for (int ax = rangeMinX; ax <= rangeMaxX; ax++)
                {
                    this.tiles[(ax, y)] = WaterTile.Still;
                }

                y--;
            } while (true);
        }

        private void PrintGrid()
        {
            for (int y = 0; y <= maxY; y++)
            {
                for (int x = 0; x <= maxX; x++)
                {
                    Console.Write(tileText[(int)GetTile((x, y))]);
                }

                Console.WriteLine();
            }
            
            Console.WriteLine();
        }

        protected override string SolvePartOne()
        {
            RunFlowing((500, 1));

            PrintGrid();

            // My answers are off by just a few, refactoring
            // 50828 = Too low
            // 50835 = Tow low
            // 50838 = Answer but C# isn't getting that
            // 50842 = Too high

            return this.tiles.Count(a => a.Value == WaterTile.Flowing || a.Value == WaterTile.Still).ToString();
        }

        protected override string SolvePartTwo()
        {
            return this.tiles.Count(a => a.Value == WaterTile.Still).ToString();
        }
    }
}
