using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

using System.Diagnostics;


namespace AdventOfCode.Solutions.Year2018
{

    class Day22 : ASolution
    {
        public enum CaveType
        {
            Rocky,
            Wet,
            Narrow
        }

        [Flags]
        public enum Equiped : uint
        {
            None = 0,
            Climbing = 1,
            Torch = 2,
            Neither = 4
        }

        private int caveDepth = 0;

        private (int x, int y) goal = (10, 10);

        private Dictionary<(int x, int y), int> erosionLevels = new Dictionary<(int x, int y), int>();
        private Dictionary<(int x, int y), int> geologicIndices = new Dictionary<(int x, int y), int>();

        public Day22() : base(22, 2018, "Mode Maze")
        {
//             DebugInput = @"depth: 510
// target: 10,10";

            // Just going to simplify this without regex
            var vals = Input.SplitByNewline().Select(line => line.Split(" ", 2)[1].Trim()).ToArray();

            this.caveDepth = Int32.Parse(vals[0]);
            this.goal = (Int32.Parse(vals[1].Split(',')[0].Trim()), Int32.Parse(vals[1].Split(',')[1].Trim()));

            if (!string.IsNullOrEmpty(DebugInput))
            {
                Debug.Assert(GetGeologicIndex((0, 0)) == 0);
                Debug.Assert(GetGeologicIndex((1, 0)) == 16807);
                Debug.Assert(GetGeologicIndex((0, 1)) == 48271);
                Debug.Assert(GetGeologicIndex((1, 1)) == 145722555);
                Debug.Assert(GetGeologicIndex((10, 10)) == 0);

                Debug.Assert(GetErosionLevel((0, 0)) == 510);
                Debug.Assert(GetErosionLevel((1, 0)) == 17317);
                Debug.Assert(GetErosionLevel((0, 1)) == 8415);
                Debug.Assert(GetErosionLevel((1, 1)) == 1805);
                Debug.Assert(GetErosionLevel((10, 10)) == 510);
            }
        }

        public CaveType GetTileType((int x, int y) pt)
        {
            var er = GetErosionLevel(pt);

            if (er % 3 == 0)
                return CaveType.Rocky;
            
            if (er % 3 == 1)
                return CaveType.Wet;

            return CaveType.Narrow;
        }

        public int GetErosionLevel((int x, int y) pt)
        {
            // Caching to speed things up!
            if (!this.erosionLevels.ContainsKey(pt))
                this.erosionLevels[pt] = ((GetGeologicIndex(pt) + this.caveDepth) % 20183);

            return this.erosionLevels[pt];
        }

        public int GetGeologicIndex((int x, int y) pt)
        {
            if (pt == (0, 0) || pt == this.goal)
                return 0;

            if (pt.y == 0)
                return 16807 * pt.x;

            if (pt.x == 0)
                return 48271 * pt.y;

            // Caching to speed things up!
            if (!this.geologicIndices.ContainsKey(pt))
                this.geologicIndices[pt] = GetErosionLevel((pt.x - 1, pt.y)) * GetErosionLevel((pt.x, pt.y - 1));

            return this.geologicIndices[pt];
        }

        public int GetRiskLevel((int x, int y) pt)
        {
            var type = GetTileType(pt);

            if (type == CaveType.Narrow)
                return 2;

            if (type == CaveType.Wet)
                return 1;

            return 0;
        }

        protected override string? SolvePartOne()
        {
            var risk = 0;
            for (int y = 0; y <= this.goal.y; y++)
            {
                for (int x = 0; x <= this.goal.x; x++)
                {
                    risk += GetRiskLevel((x, y));
                }
            }

            return risk.ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Despite the DebugInput working perfectly
            // The final answer is wrong
            // Actual answer: 999, given answers: 995 or 1000
            return AStar((0, 0), this.goal).ToString();
        }

        public struct CaveState
        {
            public (int x, int y) pos;
            public Equiped equiped;
            public int cost;

            public CaveState()
            {
                pos = (Int32.MaxValue, Int32.MaxValue);
                equiped = Equiped.None;
                cost = Int32.MaxValue;
            }

            public CaveState(int _cost, Equiped _equiped, (int x, int y) _pos)
            {
                cost = _cost;
                equiped = _equiped;
                pos = _pos;
            }
        }

        public List<(int x, int y)> GetNeighbors((int x, int y) pt)
        {
            var neighbors = new List<(int x, int y)>();

            if (pt.x > 0)
                neighbors.Add((pt.x - 1, pt.y));

            if (pt.y > 0)
                neighbors.Add((pt.x, pt.y - 1));

            neighbors.Add((pt.x + 1, pt.y));
            neighbors.Add((pt.x, pt.y + 1));

            return neighbors;
        }

        // Based on: https://en.wikipedia.org/wiki/A*_search_algorithm
        public int AStar((int x, int y) start, (int x, int y) goal)
        {
            // This is the list of nodes we need to search
            var openSet = new PriorityQueue<CaveState, int>();
            openSet.Enqueue(new CaveState(0, Equiped.Torch, start), 0);

            // A Dictionary of the node that we cameFrom Value to reach the node Key
            var cameFrom = new Dictionary<(int x, int y), CaveState>();

            // gScore is the known shortest path from start to Key, other values are assumed infinity
            var gScore = new Dictionary<((int x, int y), Equiped equiped), int>() { { (start, Equiped.Torch), 0 } };

            // Shortest so far
            var shortest = Int32.MaxValue;

            do
            {
                // Get the next node to work on
                var currentNode = openSet.Dequeue();

                // Removed the short-circuit code so that we could cound steps more
                if (currentNode.pos == goal)
                {
                    // We may need to find more paths before the shortest is found
                    // This is thanks to how the scoring is achieved
                    shortest = Math.Min(shortest, currentNode.cost);

                    // Using this debug, I found two answers 995 and 1000
                    // Neither was correct, the answer is 999 but that is never found
                    Console.WriteLine($"Possible Answer: {currentNode.cost}");
                    continue;
                }
                
                // Get possible neighbors
                // Then we get each of the possible moves because there could be multiple moves to each tile
                // That function will also provide a cost of moving to that tile
                foreach(var move in GetNeighbors(currentNode.pos).SelectMany(node => CalcCost(currentNode, node, goal)))
                {
                    // Added +10 to get some padding for debug
                    if (move.cost < (shortest == Int32.MaxValue ? shortest : shortest + 10) && (!gScore.ContainsKey((move.pos, move.equiped)) || move.cost < gScore[(move.pos, move.equiped)]))
                    {
                        // This is a shorter path to that node and the same equipment
                        cameFrom[move.pos] = currentNode;
                        gScore[(move.pos, move.equiped)] = move.cost;

                        openSet.Enqueue(move, move.cost);
                    }
                }
            } while (openSet.Count > 0);

            return shortest;
        }

        public IEnumerable<CaveState> CalcCost(CaveState state, (int x, int y) newTile, (int x, int y) goal)
        {
            // The cost of moving from one tile (state.pos) to newTile starts with 1 minute to move
            var cost = state.cost + 1;

            var type = GetTileType(newTile);

            // The last tile, we must be equiped with a torch
            if (newTile == goal)
            {
                if (state.equiped == Equiped.Torch)
                    yield return new CaveState(cost, state.equiped, newTile);
                else
                    yield return new CaveState(cost + 7, Equiped.Torch, newTile);
            }
            else if (type == CaveType.Rocky)
            {
                if ((state.equiped & (Equiped.Climbing | Equiped.Torch)) != Equiped.None)
                {
                    // Valid as-is
                    yield return new CaveState(cost, state.equiped, newTile);

                    // Or change it...
                    yield return new CaveState(cost + 7, state.equiped ^ (Equiped.Climbing | Equiped.Torch), newTile);
                }
                else
                {
                    // We have two possibilities
                    // Either equip the climbing or torch, cost is 7 minutes each
                    yield return new CaveState(cost + 7, Equiped.Climbing, newTile);
                    yield return new CaveState(cost + 7, Equiped.Torch, newTile);
                }
            }
            else if (type == CaveType.Wet)
            {
                if ((state.equiped & (Equiped.Climbing | Equiped.Neither)) != Equiped.None)
                {
                    // Valid as-is
                    yield return new CaveState(cost, state.equiped, newTile);

                    // Or change it...
                    yield return new CaveState(cost + 7, state.equiped ^ (Equiped.Climbing | Equiped.Neither), newTile);
                }
                else
                {
                    // We have two possibilities
                    // Either equip the climbing or neither, cost is 7 minutes each
                    yield return new CaveState(cost + 7, Equiped.Climbing, newTile);
                    yield return new CaveState(cost + 7, Equiped.Neither, newTile);
                }
            }
            else if (type == CaveType.Narrow)
            {
                if ((state.equiped & (Equiped.Torch | Equiped.Neither)) != Equiped.None)
                {
                    // Valid as-is
                    yield return new CaveState(cost, state.equiped, newTile);

                    // Or change it...
                    yield return new CaveState(cost + 7, state.equiped ^ (Equiped.Torch | Equiped.Neither), newTile);
                }
                else
                {
                    // We have two possibilities
                    // Either equip the torch or neither, cost is 7 minutes each
                    yield return new CaveState(cost + 7, Equiped.Torch, newTile);
                    yield return new CaveState(cost + 7, Equiped.Neither, newTile);
                }
            }
        }
    }
}

