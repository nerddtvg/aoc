using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{

    class Day13 : ASolution
    {
        int favNumber;
        List<(int x, int y)> path = new List<(int x, int y)>();

        HashSet<(int x, int y)> fiftySteps = new HashSet<(int x, int y)>();

        public Day13() : base(13, 2016, "A Maze of Twisty Little Cubicles")
        {
            // DebugInput = "10";

            favNumber = Int32.Parse(Input);
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

            // Part 2: Include the start
            this.fiftySteps.Add(start);

            do
            {
                // Get the next node to work on
                var min = gScore.Where(kvp => openSet.Contains(kvp.Key)).Min(kvp => kvp.Value);
                var currentNode = openSet.FirstOrDefault(pt => gScore[pt] == min);

                if (currentNode == default && currentNode != (0, 0))
                    throw new Exception("Unable to find next node.");

                // Removed the short-circuit code so that we could cound steps more

                // Work on this node
                openSet.Remove(currentNode);

                // We know the gScore to get to a neighbor is gScore[currentNode] + 1
                var tgScore = gScore[currentNode] + 1;
                
                // Get possible neighbors
                foreach(var n in GetNeighbors(currentNode))
                {
                    if (!gScore.ContainsKey(n) || tgScore < gScore[n])
                    {
                        // Count <= 50 nodes
                        if (tgScore <= 50)
                            this.fiftySteps.Add(n);

                        // This is a shorter path to that node
                        cameFrom[n] = currentNode;
                        gScore[n] = tgScore;

                        // HashSet prevents duplicates
                        openSet.Add(n);
                    }
                }
            } while (openSet.Count > 0);

            // Moved here so we could accomodate part 2
            if (cameFrom.ContainsKey(goal))
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
            }

            return new List<(int x, int y)>();
        }

        public bool IsOpen(int x, int y)
        {
            if (x < 0 || y < 0) return false;

            // Do the math, convert to base 2, if the number of 1s is even, it is open
            return Convert.ToString((x * x + 3 * x + 2 * x * y + y + y * y + favNumber), 2)
                .Count(ch => ch == '1') % 2 == 0;
        }

        protected override string SolvePartOne()
        {
            // Debug
            // path = AStar((1, 1), (7, 4));

            path = AStar((1, 1), (31, 39));

            // Remove the start from the count
            return (path.Count - 1).ToString();
        }

        protected override string SolvePartTwo()
        {
            return this.fiftySteps.Count.ToString();
        }
    }
}
