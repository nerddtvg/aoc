using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{

    class Day22 : ASolution
    {
        private class GridStorage
        {
            public int size = 0;
            public int used = 0;
            public int avail = 0;
            public int usePer = 0;
        }

        private Dictionary<(int x, int y), GridStorage> storage = new Dictionary<(int x, int y), GridStorage>();

        private int maxX = 0;
        private int maxY = 0;

        public Day22() : base(22, 2016, "Grid Computing")
        {
//             DebugInput = @"TEST
// Filesystem            Size  Used  Avail  Use%
// /dev/grid/node-x0-y0   10T    8T     2T   80%
// /dev/grid/node-x0-y1   11T    6T     5T   54%
// /dev/grid/node-x0-y2   32T   28T     4T   87%
// /dev/grid/node-x1-y0    9T    7T     2T   77%
// /dev/grid/node-x1-y1    8T    0T     8T    0%
// /dev/grid/node-x1-y2   11T    7T     4T   63%
// /dev/grid/node-x2-y0   10T    6T     4T   60%
// /dev/grid/node-x2-y1    9T    8T     1T   88%
// /dev/grid/node-x2-y2    9T    6T     3T   66%";

            Reset();
        }

        private void Reset()
        {
            this.storage.Clear();

            foreach(var line in Input.Trim().SplitByNewline().Skip(2))
            {
                var matches = Regex.Match(line, @"node-x([0-9]+)-y([0-9]+)\s+([0-9]+)T\s+([0-9]+)T\s+([0-9]+)T\s+([0-9]+)%");

                if (!matches.Success)
                    throw new Exception($"Invalid line: {line}");

                var x = Int32.Parse(matches.Groups[1].Value);
                var y = Int32.Parse(matches.Groups[2].Value);

                this.maxX = Math.Max(this.maxX, x);
                this.maxY = Math.Max(this.maxY, y);

                this.storage.Add((x, y), new GridStorage()
                {
                    size = Int32.Parse(matches.Groups[3].Value),
                    used = Int32.Parse(matches.Groups[4].Value),
                    avail = Int32.Parse(matches.Groups[5].Value),
                    usePer = Int32.Parse(matches.Groups[6].Value)
                });
            }
        }

        protected override string SolvePartOne()
        {
            var viable = 0;

            for (int node1X = 0; node1X <= this.maxX; node1X++)
            {
                for (int node1Y = 0; node1Y <= this.maxY; node1Y++)
                {
                    for (int node2X = 0; node2X <= this.maxX; node2X++)
                    {
                        for (int node2Y = 0; node2Y <= this.maxY; node2Y++)
                        {
                            if (node1X == node2X && node1Y == node2Y)
                                continue;

                            var node1 = this.storage[(node1X, node1Y)];
                            var node2 = this.storage[(node2X, node2Y)];

                            if (node1.used == 0)
                                continue;

                            if (node1.used <= node2.avail)
                                viable++;
                        }
                    }

                }
            }

            return viable.ToString();
        }

        protected override string SolvePartTwo()
        {
            // This can be solved visually
            // Move the hole to the top-right corner (x=34, y=0)
            // Then move that data to the top-left corner
            // Each move from top-right to top-left involves rotating the hole around which is 5 steps

            // This output mimics what /u/Turbosack showed
            // https://old.reddit.com/r/adventofcode/comments/5jor9q/2016_day_22_solutions/dbhvxkp/
            // for (int node1Y = 0; node1Y <= this.maxY; node1Y++)
            // {
            //     for (int node1X = 0; node1X <= this.maxX; node1X++)
            //     {
            //         if (this.storage[(node1X, node1Y)].used == 0)
            //         {
            //             Console.Write($"__/{this.storage[(node1X, node1Y)].size.ToString("00")}");
            //         }
            //         else if (this.storage[(node1X, node1Y)].size >= 100)
            //         {
            //             // Large nodes are walls to us
            //             Console.Write($"|/{this.storage[(node1X, node1Y)].size}");
            //         }
            //         else
            //         {
            //             Console.Write($"{this.storage[(node1X, node1Y)].used.ToString("00")}/{this.storage[(node1X, node1Y)].size.ToString("00")}");
            //         }

            //         Console.Write("  ");
            //     }

            //     Console.WriteLine();
            // }

            // After visually seeing how this is laid out
            // 21 moves from hole to top
            // 13 moves to position next to top-right
            // 33*5 moves to top-left plus 1 to fill the last box

            var node = this.storage.FirstOrDefault(node => node.Value.used == 0);

            var count = 0;

            // First we move up to the top
            int x = node.Key.x;
            int y = node.Key.y;
            for (; y > 0; y--)
            {
                // Did we hit a "wall"?
                var newXY = (x, y - 1);
                if (this.storage.First(node => node.Key == newXY).Value.size > 100)
                {
                    // Move left, come back to this y on the next loop
                    x--;
                    y++;
                }

                // Otherwise, move up and count it
                count++;
            }

            // Now move to the top-right minus 1 x
            count += this.maxX - 1 - x;
            // 34 at this point

            // Then determine ((this.maxX-1) * 5) + 1
            count += ((this.maxX-1) * 5) + 1;

            return count.ToString();
        }
    }
}
