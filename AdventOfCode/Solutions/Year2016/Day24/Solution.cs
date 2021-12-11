using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{

    class Day24 : ASolution
    {
        public char[][] grid = new char[][] { };
        public int width = 0;
        public int height = 0;
        public Dictionary<char, (int x, int y)> positions = new Dictionary<char, (int x, int y)>();

        public char maxNode = '0';
        public string shortestPath = string.Empty;

        // This tracks all possible paths
        public Dictionary<(char start, char end), List<(int x, int y)>> paths = new Dictionary<(char start, char end), List<(int x, int y)>>();

        public Day24() : base(24, 2016, "Air Duct Spelunking")
        {
            Reset();
        }

        private void Reset()
        {
            this.grid = Input.SplitByNewline().Select(line => line.Trim().ToCharArray()).ToArray();
            this.width = this.grid[0].Length;
            this.height = this.grid.Length;

            // Remove whitespace for an accurate count
            var reparsed = Input.Replace("\n", "").Replace("\r", "");
            this.positions = Enumerable
                .Range(0, 10)
                // Find each of the characters 0 through 9
                .Select(idx => new { Key = (char)('0' + idx), Value = reparsed.IndexOf((char)('0' + idx)) })
                // Make sure we found it
                .Where(obj => obj.Value >= 0)
                // Convert to our positions
                .ToDictionary(obj => obj.Key, obj => (obj.Value % this.width, obj.Value / this.width));

            // Find the highest number
            this.maxNode = this.positions.Keys.Max();
        }

        public List<(int x, int y)> GetNeighbors((int x, int y) pt)
        {
            var neighbors = new List<(int x, int y)>();

            if (IsOpen(pt.x - 1, pt.y))
                neighbors.Add((pt.x - 1, pt.y));

            if (IsOpen(pt.x + 1, pt.y))
                neighbors.Add((pt.x + 1, pt.y));

            if (IsOpen(pt.x, pt.y - 1))
                neighbors.Add((pt.x, pt.y - 1));

            if (IsOpen(pt.x, pt.y + 1))
                neighbors.Add((pt.x, pt.y + 1));

            return neighbors;
        }

        // Based on: https://en.wikipedia.org/wiki/A*_search_algorithm
        public List<(int x, int y)> AStar((int x, int y) start, (int x, int y) goal)
        {
            // This is the list of nodes we need to search
            var openSet = new HashSet<(int x, int y)>() { start };

            // A Dictionary of the node that we cameFrom Value to reach the node Key
            var cameFrom = new Dictionary<(int x, int y), (int x, int y)>();

            // gScore is the known shortest path from start to Key, other values are assumed infinity
            var gScore = new Dictionary<(int x, int y), int>() { { start, 0 } };

            do
            {
                // Get the next node to work on
                var min = gScore.Where(kvp => openSet.Contains(kvp.Key)).Min(kvp => kvp.Value);
                var currentNode = openSet.FirstOrDefault(pt => gScore[pt] == min);

                if (currentNode == default && currentNode != (0, 0))
                    throw new Exception("Unable to find next node.");
                    
                // Did we find the shortest path?
                if (currentNode == goal)
                {
                    // Go backwards from here
                    var ret = new List<(int x, int y)>();

                    // Prevent an accidental infinite loop
                    int max = 0;

                    do
                    {
                        ret.Add(cameFrom[currentNode]);
                        currentNode = cameFrom[currentNode];
                    } while (currentNode != start && max++ < 1000);

                    return ret;
                }

                // Work on this node
                openSet.Remove(currentNode);

                // We know the gScore to get to a neighbor is gScore[currentNode] + 1
                var tgScore = gScore[currentNode] + 1;
                
                // Get possible neighbors
                foreach(var n in GetNeighbors(currentNode))
                {
                    if (!gScore.ContainsKey(n) || tgScore < gScore[n])
                    {
                        // This is a shorter path to that node
                        cameFrom[n] = currentNode;
                        gScore[n] = tgScore;

                        // HashSet prevents duplicates
                        openSet.Add(n);
                    }
                }
            } while (openSet.Count > 0);

            return new List<(int x, int y)>();
        }

        public bool IsOpen(int x, int y)
        {
            if (x < 0 || y < 0 || x >= this.width || y >= this.height) return false;

            // It's not a wall!
            return this.grid[y][x] != '#';
        }

        protected override string SolvePartOne()
        {
            // We're going to simply check every possible route
            // 0 -> 1, 0 -> 2 ... 0 -> max
            // 1 -> 2, 1 -> 3 ... 1 -> max
            // 2 -> 3, ... max
            this.paths.Clear();

            for (char start = '0'; start < this.maxNode; start++)
            {
                for (char end = '1'; end <= this.maxNode; end++)
                {
                    if (start == end) continue;
                    
                    this.paths[(start, end)] = AStar(this.positions[start], this.positions[end]);
                }
            }

            // And now we find the minimum path forward
            var str = Enumerable.Range(0, (int)(this.maxNode - '0')).Select(ch => (char)(ch + '1')).JoinAsString();

            int steps = Int32.MaxValue;
            foreach(var perm in str.Permutations())
            {
                // Must always start at zero
                var thisPerm = new char[] { '0' }.Union(perm).ToArray();

                var thisLength = 0;
                for (int i = 0; i < thisPerm.Length - 1; i++)
                {
                    thisLength += this.paths[((char)Math.Min((int)thisPerm[i], (int)thisPerm[i + 1]), (char)Math.Max((int)thisPerm[i], (int)thisPerm[i + 1]))].Count;
                }

                // Is this our current minimum?
                if (thisLength < steps)
                {
                    steps = thisLength;
                    this.shortestPath = thisPerm.JoinAsString();
                }
            }

            return steps.ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
