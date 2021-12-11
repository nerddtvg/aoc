using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

// This is a path determining task
// I suck at path algorithms
// This will be based on someone else's solution from the mega thread
// https://old.reddit.com/r/adventofcode/comments/5hoia9/2016_day_11_solutions/

// Attempting: https://old.reddit.com/r/adventofcode/comments/5hoia9/2016_day_11_solutions/db1zbu0/
// This is an A* approach which we've already done in a few other days

namespace AdventOfCode.Solutions.Year2016
{

    class Day11 : ASolution
    {
        public const int promethium = 1;
        public const int cobalt = 2;
        public const int curium = 3;
        public const int ruthenium = 4;
        public const int plutonium = 5;

        // Hard coding the inputs because it will be easier than reading them
        public int[][] initial = new int[][]
        {
            new int[] { promethium, -promethium },
            new int[] { cobalt, curium, ruthenium, plutonium },
            new int[] { -cobalt, -curium, -ruthenium, -plutonium },
            new int[] { }
        };

        public Day11() : base(11, 2016, "Radioisotope Thermoelectric Generators")
        {

        }

        public bool FloorIsCorrect(int[] floor)
        {
            // Maybe the floor is empty or if it isn't, make sure everything cancels out or there is no extra generator
            if (floor.Length == 0 || floor.Count(val => val < 0) == floor.Length)
                return true;

            // Remove every match
            var unMatched = floor.Where(val => !floor.Contains(-1 * val)).ToArray();

            // Nothing unmatched, or we have only chips OR only generators left
            var chipsLeft = unMatched.Count(val => val < 0);
            return unMatched.Length == 0 || (chipsLeft == 0 ^ chipsLeft == unMatched.Length);
        }

        public bool IsFinished(FloorState state)
        {
            var maxIdx = state.floors.Length - 1;
            return state.floors.Where((val, idx) => idx != maxIdx).Sum(val => val.Length) == 0;
        }

        public class FloorState : IEquatable<FloorState>
        {
            public int elevator = 0;
            public int[][] floors = new int[][] { };

            public override int GetHashCode()
            {
                return elevator + floors.Select((floor, idx) => ((idx + 1) * 1000) + floor.Sum()).Sum();
            }

            public override bool Equals(object obj)
            {
                return obj is FloorState && Equals((FloorState) obj);
            }

            public bool Equals(FloorState obj)
            {
                // Make sure we truly match
                return elevator == obj.elevator
                    && floors.Length == obj.floors.Length
                    && floors.Select((floor, idx) => floor.OrderBy(val => val).SequenceEqual(obj.floors[idx].OrderBy(val => val))).All(ret => ret == true);
            }
        }

        // Based on: https://en.wikipedia.org/wiki/A*_search_algorithm
        public int AStar(int[][] initialFloors)
        {
            // This is the list of elevator positions (int) and floors (int[][]) we need to search
            var openSet = new HashSet<FloorState>() { new FloorState() { elevator = 0, floors = initialFloors } };

            // A Dictionary of the node that we cameFrom Value to reach the floor Key
            var cameFrom = new Dictionary<FloorState, FloorState>();

            // gScore is the known shortest path from start to Key, other values are assumed infinity
            var gScore = new Dictionary<FloorState, int>() { { new FloorState() { elevator = 0, floors = initialFloors }, 0 } };

            do
            {
                // Get the next node to work on
                var min = gScore.Where(kvp => openSet.Contains(kvp.Key)).Min(kvp => kvp.Value);
                var currentState = openSet.FirstOrDefault(pt => gScore[pt] == min);
                    
                // Did we find the shortest path?
                if (IsFinished(currentState))
                {
                    return gScore[currentState];
                }

                // Work on this node
                openSet.Remove(currentState);

                // We know the gScore to get to a neighbor is gScore[currentNode] + 1
                var tgScore = gScore[currentState] + 1;

                // Get all possible moves
                // We can either move one or two items from the floor we're on
                // We can also go up or down or both
                var dirs = new List<int>();
                if (currentState.elevator > 0)
                    dirs.Add(-1);
                
                if (currentState.elevator < initial.Length-1)
                    dirs.Add(1);

                // Get a list of moving two possible items, combine it with a list of just one item
                var moves = (currentState.floors[currentState.elevator].Length > 2 ? currentState.floors[currentState.elevator].GetAllCombos(r: 2).Select(lst => lst.ToList()) : new List<List<int>>())
                    .Union(currentState.floors[currentState.elevator].Select(val => new List<int>() { val }))
                    .ToList();

                var addToOpenState = new Stack<FloorState>();

                foreach (var dir in dirs)
                {
                    // Shortcut hints: https://old.reddit.com/r/adventofcode/comments/5hoia9/2016_day_11_solutions/db1v1ws/

                    // Don't move down if floors below are empty
                    if (dir == -1)
                    {
                        if (currentState.floors.Select((floor, idx) => idx < currentState.elevator ? floor.Length : 0).Sum() == 0)
                            continue;
                    }

                    // Use this to track if we have moved one item downstairs
                    // If we have, we skip forward to reduce extra work
                    var movedOneDown = false;

                    // Check if we have already moved a chip from a pair up
                    var movedChipsUp = false;

                    foreach (var move in moves)
                    {
                        if (dir == -1 && move.Count > 1 && movedOneDown)
                            continue;

                        // If we have two pairs of chips+gen on this floor
                        // And we have already chosen to only move a single chip up
                        // And here we are looking at another pair, ignore it
                        if (dir == 1 && move.Count == 1 && move[0] < 0 && movedChipsUp && currentState.floors[currentState.elevator].Contains(-1 * move[0]))
                            continue;

                        var newFloors = currentState.floors.Select(floor => (int[])floor.Clone()).ToArray();

                        // Remove the item(s) we're moving
                        newFloors[currentState.elevator] = newFloors[currentState.elevator].Where(val => !move.Contains(val)).ToArray();

                        // Add the items we're moving
                        newFloors[currentState.elevator + dir] = newFloors[currentState.elevator + dir].Union(move).ToArray();

                        // Is this a valid combination?
                        if (!FloorIsCorrect(newFloors[currentState.elevator]) || !FloorIsCorrect(newFloors[currentState.elevator + dir]))
                            continue;

                        // Setup our new floor state
                        var newState = new FloorState() { elevator = currentState.elevator + dir, floors = newFloors };

                        if (!gScore.ContainsKey(newState) || tgScore < gScore[newState])
                        {
                            cameFrom[newState] = currentState;
                            gScore[newState] = tgScore;

                            // Add to our search list
                            addToOpenState.Push(newState);

                            // Track that we've moved one down
                            movedOneDown = movedOneDown || (move.Count == 1 && dir == -1);

                            // Track that we had a pair and we are moving only that chip up
                            movedChipsUp = movedChipsUp || (dir == 1 && move.Count == 1 && move[0] < 0 && newFloors[currentState.elevator].Contains(-1 * move[0]));
                        }
                    }
                }
                
                // Add to open state in reverse order (stack) so we move up if possible first
                while(addToOpenState.Count > 0)
                {
                    openSet.Add(addToOpenState.Pop());
                }
            } while (openSet.Count > 0);

            return 0;
        }

        protected override string SolvePartOne()
        {
            return AStar(this.initial).ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
