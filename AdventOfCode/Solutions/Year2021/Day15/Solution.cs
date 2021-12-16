using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2021
{

    class Day15 : ASolution
    {
        private Dictionary<(int x, int y), int> grid = new Dictionary<(int x, int y), int>();

        private int maxX = 0;
        private int maxY = 0;

        public Day15() : base(15, 2021, "Chiton")
        {
//             DebugInput = @"199111
// 199191
// 111191
// 999991";

            // Read in the grid
            foreach(var line in Input.SplitByNewline().Select((line, idx) => (line, idx)))
            {
                var arr = line.line.ToIntArray();
                for (int x = 0; x < arr.Length; x++)
                    this.grid[(x, line.idx)] = arr[x];

                this.maxX = Math.Max(arr.Length - 1, this.maxX);
                this.maxY = Math.Max(line.idx, this.maxY);
            }
        }

        protected override string? SolvePartOne()
        {
            return AStar((0, 0), (this.maxX, this.maxY)).ToString();
        }

        protected override string? SolvePartTwo()
        {
            ExpandGrid();
            return AStar((0, 0), (this.maxX, this.maxY)).ToString();
        }

        public List<(int x, int y)> GetNeighbors((int x, int y) pt)
        {
            var neighbors = new List<(int x, int y)>();

            if (pt.x > 0)
                neighbors.Add((pt.x - 1, pt.y));

            if (pt.y > 0)
                neighbors.Add((pt.x, pt.y - 1));

            if (pt.x < maxX)
                neighbors.Add((pt.x + 1, pt.y));

            if (pt.y < maxY)
                neighbors.Add((pt.x, pt.y + 1));

            return neighbors;
        }

        // Based on: https://en.wikipedia.org/wiki/A*_search_algorithm
        public int AStar((int x, int y) start, (int x, int y) goal)
        {
            // This is the list of nodes we need to search
            var openSet = new PriorityQueue<(int x, int y), int>();
            openSet.Enqueue(start, 0);

            // A Dictionary of the node that we cameFrom Value to reach the node Key
            var cameFrom = new Dictionary<(int x, int y), (int x, int y)>();

            // gScore is the known shortest path from start to Key, other values are assumed infinity
            var gScore = new Dictionary<(int x, int y), int>() { { start, 0 } };

            do
            {
                // Get the next node to work on
                var currentNode = openSet.Dequeue();

                // Removed the short-circuit code so that we could cound steps more
                if (currentNode == goal)
                {
                    // Found the shortest possible route
                    return gScore[currentNode];
                }
                
                // Get possible neighbors
                // Then we get each of the possible moves because there could be multiple moves to each tile
                // That function will also provide a cost of moving to that tile
                foreach(var move in GetNeighbors(currentNode))
                {
                    // Each move costs us the risk score to get there
                    var tgScore = gScore[currentNode] + this.grid[move];

                    if (!gScore.ContainsKey(move) || tgScore < gScore[move])
                    {
                        gScore[move] = tgScore;
                        cameFrom[move] = currentNode;

                        // Add this to our queue
                        openSet.Enqueue(move, tgScore);
                    }
                }
            } while (openSet.Count > 0);

            return 0;
        }

        private void ExpandGrid()
        {
            // From zero to maxX/maxY
            var tileWidth = this.maxX + 1;
            var tileHeight = this.maxY + 1;

            var newGrid = this.grid;

            // We're going to start by expanding to the right and then work down
            for (int y = 0; y < tileHeight; y++)
            {
                for (int x = tileWidth; x < tileWidth * 5; x++)
                {
                    newGrid[(x, y)] = newGrid[(x - tileWidth, y)] + 1;

                    if (newGrid[(x, y)] > 9)
                        newGrid[(x, y)] = 1;
                }
            }

            // Now let's go down...........
            for (int y = tileHeight; y < tileHeight * 5; y++)
            {
                for (int x = 0; x < tileWidth * 5; x++)
                {
                    newGrid[(x, y)] = newGrid[(x, y - tileHeight)] + 1;

                    if (newGrid[(x, y)] > 9)
                        newGrid[(x, y)] = 1;
                }
            }

            // Update our max values
            this.maxX = (tileWidth * 5) - 1;
            this.maxY = (tileHeight * 5) - 1;
        }
    }
}

#nullable restore
