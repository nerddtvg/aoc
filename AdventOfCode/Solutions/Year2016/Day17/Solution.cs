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

        public Dictionary<string, (int x, int y)> GetDirections((int x, int y) pt, string currentPath)
        {
            var neighbors = new Dictionary<string, (int x, int y)>();

            // Get the MD5 hash of our current room
            var md5 = Utilities.MD5HashString(currentPath);

            // These are what are considered 'open'
            var open = "bcdef";

            // Up
            if (pt.y >= 0 && open.Contains(md5[0]))
                neighbors.Add(currentPath + 'U', (pt.x, pt.y - 1));

            // Down
            if (pt.y <= 2 && open.Contains(md5[1]))
                neighbors.Add(currentPath + 'D', (pt.x, pt.y + 1));

            // Left
            if (pt.x >= 0 && open.Contains(md5[2]))
                neighbors.Add(currentPath + 'L', (pt.x - 1, pt.y));

            // Right
            if (pt.x <= 2 && open.Contains(md5[3]))
                neighbors.Add(currentPath + 'R', (pt.x + 1, pt.y));

            return neighbors;
        }

        // Based on: https://en.wikipedia.org/wiki/A*_search_algorithm
        public string AStar((int x, int y) start, (int x, int y) goal, string startPath)
        {
            // This is the list of nodes we need to search
            var openSet = new HashSet<(int x, int y)>() { start };

            // A Dictionary of the node that we cameFrom Value to reach the node Key
            var cameFrom = new Dictionary<(int x, int y), string>() { { start, startPath } };

            // gScore is the known shortest path from start to Key, other values are assumed infinity
            var gScore = new Dictionary<(int x, int y), int>() { { start, 0 } };

            do
            {
                // Get the next node to work on
                var min = gScore.Where(kvp => openSet.Contains(kvp.Key)).Min(kvp => kvp.Value);
                var currentNode = openSet.FirstOrDefault(pt => gScore[pt] == min);

                if (currentNode == default && currentNode != (0, 0))
                    throw new Exception("Unable to find next node.");

                // Current path
                var currentPath = cameFrom[currentNode];

                // Removed the short-circuit code so that we could cound steps more
                if (currentNode == goal)
                {
                    // Found the vault
                    return currentPath;
                }

                // Work on this node
                openSet.Remove(currentNode);

                // We know the gScore to get to a neighbor is gScore[currentNode] + 1
                var tgScore = gScore[currentNode] + 1;

                // Get possible neighbors
                foreach(var n in GetDirections(currentNode, currentPath))
                {
                    // We track all paths (may backtrack), no if statement here
                    //if (!gScore.ContainsKey(n.Value) || tgScore < gScore[n])
                    //{
                        // This is a shorter path to that node
                        cameFrom[n.Value] = n.Key;
                        gScore[n.Value] = tgScore;

                        // HashSet prevents duplicates
                        openSet.Add(n.Value);
                    //}
                }
            } while (openSet.Count > 0);

            // Moved here so we could accomodate part 2
            /* if (cameFrom.ContainsKey(goal))
            {
            
                // Go backwards from here
                var ret = new List<(int x, int y)>();

                var currentNode = goal;

                // Prevent an accidental infinite loop
                int max = 0;

                do
                {
                    ret.Add(cameFrom[currentNode]);
                    currentNode = cameFrom[currentNode];
                } while (currentNode != start && max++ < 1000);

                return ret;
            } */

            return string.Empty;
        }

        protected override string SolvePartOne()
        {
            return AStar((0, 0), (3, 3), Input);
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
