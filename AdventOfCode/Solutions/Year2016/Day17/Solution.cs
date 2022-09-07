using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{

    class Day17 : ASolution
    {

        public Day17() : base(17, 2016, "Two Steps Forward")
        {

        }

        // Tracking complete paths (we need to enumerate everything with this code)
        public List<string> completePaths = new List<string>();

        public Dictionary<string, (int x, int y)> GetDirections((int x, int y) pt, string currentPath)
        {
            var neighbors = new Dictionary<string, (int x, int y)>();

            // Get the MD5 hash of our current room
            var md5 = Utilities.MD5HashString(currentPath);

            // These are what are considered 'open'
            var open = "bcdef";

            // Up
            if (pt.y > 0 && open.Contains(md5[0]))
                neighbors.Add(currentPath + 'U', (pt.x, pt.y - 1));

            // Down
            if (pt.y < 3 && open.Contains(md5[1]))
                neighbors.Add(currentPath + 'D', (pt.x, pt.y + 1));

            // Left
            if (pt.x > 0 && open.Contains(md5[2]))
                neighbors.Add(currentPath + 'L', (pt.x - 1, pt.y));

            // Right
            if (pt.x < 3 && open.Contains(md5[3]))
                neighbors.Add(currentPath + 'R', (pt.x + 1, pt.y));

            return neighbors;
        }

        public class node
        {
            public (int x, int y) pos = (0, 0);
            public string path = string.Empty;
            public int gScore = Int32.MaxValue;
        }

        // Based on: https://en.wikipedia.org/wiki/A*_search_algorithm
        public string AStar((int x, int y) start, (int x, int y) goal, string startPath)
        {
            // This is the list of nodes we need to search
            var openSet = new HashSet<node>() { new node { path = startPath, gScore = Int32.MaxValue - 1 } };

            do
            {
                // Get the next node to work on
                var min = openSet.Min(node => node.gScore);
                var currentNode = openSet.FirstOrDefault(node => node.gScore == min);

                if (currentNode == default)
                    throw new Exception("Unable to find next node.");

                // Work on this node
                openSet.Remove(currentNode);

                // Removed the short-circuit code so that we could cound steps more
                if (currentNode.pos == goal)
                {
                    // Found the vault
                    this.completePaths.Add(currentNode.path.Replace(startPath, ""));
                    continue;
                }

                // We know the gScore to get to a neighbor is gScore[currentNode] + 1
                var tgScore = currentNode.gScore - 1;

                // Get possible neighbors
                var neighbors = GetDirections(currentNode.pos, currentNode.path);

                foreach(var n in neighbors)
                {
                    // We track all paths (may backtrack), no if statement here
                    var tempNode = new node() { pos = n.Value, path = n.Key, gScore = tgScore };

                    // HashSet prevents duplicates
                    openSet.Add(tempNode);
                }
            } while (openSet.Count > 0);

            return this.completePaths.OrderBy(l => l.Length).FirstOrDefault() ?? string.Empty;
        }

        protected override string SolvePartOne()
        {
            return AStar((0, 0), (3, 3), Input);
        }

        protected override string SolvePartTwo()
        {
            return this.completePaths.OrderByDescending(l => l.Length).FirstOrDefault()?.Length.ToString() ?? string.Empty;
        }
    }
}
