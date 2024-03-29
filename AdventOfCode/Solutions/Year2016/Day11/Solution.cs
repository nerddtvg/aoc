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
        public const int elerium = 6;
        public const int dilithium = 7;

        // Hard coding the inputs because it will be easier than reading them
        public int[][] initial = new int[][]
        {
            new int[] { promethium, -promethium },
            new int[] { cobalt, curium, ruthenium, plutonium },
            new int[] { -cobalt, -curium, -ruthenium, -plutonium },
            new int[] { }
        };

        // Hard coding the inputs because it will be easier than reading them
        public int[][] initial2 = new int[][]
        {
            new int[] { promethium, -promethium, elerium, -elerium, dilithium, -dilithium },
            new int[] { cobalt, curium, ruthenium, plutonium },
            new int[] { -cobalt, -curium, -ruthenium, -plutonium },
            new int[] { }
        };

        public Day11() : base(11, 2016, "Radioisotope Thermoelectric Generators")
        {
            // Debug
            // this.initial = new int[][]
            // {
            //     new int[] { -promethium, -cobalt },
            //     new int[] { promethium },
            //     new int[] { cobalt },
            //     new int[] { }
            // };

            this.initial = this.initial.Select(floor => floor.OrderBy(val => val).ToArray()).ToArray();
            this.initial2 = this.initial2.Select(floor => floor.OrderBy(val => val).ToArray()).ToArray();
        }

        public bool FloorIsCorrect(int[] floor)
        {
            // Maybe the floor is empty or if it isn't, make sure everything cancels out or there is no extra generator
            if (floor.Length == 0)
                return true;

            // Remove every match
            var unMatched = floor.Where(val => !floor.Contains(-1 * val)).ToArray();

            // Nothing unmatched, or no chips, or chips + no rtgs
            var chipsLeft = unMatched.Count(val => val < 0);
            return unMatched.Length == 0 || (chipsLeft == 0 ^ floor.Count(val => val > 0) == 0);
        }

        public bool IsFinished(FloorState state)
        {
            return state.elevator == 3 && !Enumerable.Range(0, state.floors.Length - 1).Any(idx => state.floors[idx].Length > 0);
        }

        public class FloorState : IEquatable<FloorState>
        {
            public int elevator = 0;
            public int[][] floors = new int[][] { };

            public override int GetHashCode()
            {
                var values = new Dictionary<int, int>()
                {
                    { promethium, 1 },
                    { -promethium, 2 },
                    { cobalt, 4 },
                    { -cobalt, 8 },
                    { curium, 16 },
                    { -curium, 32 },
                    { ruthenium, 64 },
                    { -ruthenium, 128 },
                    { plutonium, 256 },
                    { -plutonium, 512 },
                    { elerium, 1024 },
                    { -elerium, 2048 },
                    { dilithium, 4096 },
                    { -dilithium, 8192 }
                };

                // We need to combine the elevator position and the items on each floor
                return elevator + floors.Select((floor, idx) => (int) Math.Pow(10, idx+1) * floor.Sum(val => values[val])).Sum();

                // This really messed us up
                //return floors.Select(x => x.GetHashCode()).Aggregate(elevator.GetHashCode(), (x, y) => x ^ y);
            }

            public override bool Equals(object? obj)
            {
                return obj is FloorState && Equals((FloorState) obj);
            }

            public bool Equals(FloorState? obj)
            {
                if (obj == default) return false;
                
                // Make sure we truly match
                return elevator == obj.elevator
                    && floors.Length == obj.floors.Length
                    && floors.Select((floor, idx) => floor.SequenceEqual(obj.floors[idx])).All(ret => ret == true);
            }
        }

        // Based on: https://en.wikipedia.org/wiki/A*_search_algorithm
        public int AStar(int[][] initialFloors)
        {
            var initialState = new FloorState() { elevator = 0, floors = initialFloors };

            // This is the list of elevator positions (int) and floors (int[][]) we need to search
            var openSet = new PriorityQueue<FloorState, int>();
            openSet.Enqueue(initialState, 0);

            // A Dictionary of the node that we cameFrom Value to reach the floor Key
            var cameFrom = new Dictionary<FloorState, FloorState>();

            // gScore is the known shortest path from start to Key, other values are assumed infinity
            var gScore = new Dictionary<FloorState, int>() { { initialState, 0 } };

            do
            {
                // Get the next node to work on
                var currentState = openSet.Dequeue();

                // Did we find the shortest path?
                if (IsFinished(currentState))
                {
                    // var states = new List<FloorState>();

                    // states.Add(currentState);
                    // var s = cameFrom[currentState];
                    // while (!s.Equals(initialState))
                    // {
                    //     states.Add(s);
                    //     s = cameFrom[s];
                    // }

                    // // Initial State
                    // states.Add(s);

                    // // REverse it
                    // states.Reverse();

                    // foreach(var state in states)
                    // {
                    //     for (int i = 3; i >= 0; i--)
                    //     {
                    //         Console.Write($"F{i + 1}: {(state.elevator == i ? 'E' : '.')}  ");

                    //         for (var g = 1; g <= 5; g++)
                    //         {
                    //             Console.Write($"{(state.floors[i].Contains(g) ? g.ToString() : ".")}  ");
                    //             Console.Write($"{(state.floors[i].Contains(-1*g) ? (-1*g).ToString() : ". ")}  ");
                    //         }

                    //         Console.WriteLine();
                    //     }

                    //     Console.WriteLine();
                    // }

                    return gScore[currentState];
                }

                // Work on this node
                //openSet.Remove(currentState);

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
                var moves = (currentState.floors[currentState.elevator].Length >= 2 ? currentState.floors[currentState.elevator].GetAllCombos(r: 2).Select(lst => lst.ToList()) : new List<List<int>>())
                    .Union(currentState.floors[currentState.elevator].Select(val => new List<int>() { val }))
                    .ToList();

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
                        if (dir == 1 && move.Count == 1 && movedChipsUp && currentState.floors[currentState.elevator].Contains(-1 * move[0]))
                            continue;

                        var newFloors = currentState.floors.Select(floor => (int[])floor.Clone()).ToArray();

                        // Remove the item(s) we're moving
                        newFloors[currentState.elevator] = newFloors[currentState.elevator].Where(val => !move.Contains(val)).OrderBy(val => val).ToArray();

                        // Add the items we're moving
                        newFloors[currentState.elevator + dir] = newFloors[currentState.elevator + dir].Union(move).OrderBy(val => val).ToArray();

                        // Is this a valid combination?
                        if (!FloorIsCorrect(newFloors[currentState.elevator]) || !FloorIsCorrect(newFloors[currentState.elevator + dir]))
                            continue;

                        // Setup our new floor state
                        FloorState newState = new FloorState() { elevator = currentState.elevator + dir, floors = newFloors };

                        if (!gScore.ContainsKey(newState) || tgScore < gScore[newState])
                        {
                            cameFrom[newState] = currentState;
                            gScore[newState] = tgScore;

                            // Add to our search list with our priority score
                            // Had to tweak this down from *10 to *5 in order to not miss a valid option
                            openSet.Enqueue(newState, tgScore - (newFloors[3].Length * 5));

                            // Track that we've moved one down
                            movedOneDown = movedOneDown || (move.Count == 1 && dir == -1);

                            // Track that we had a pair and we are moving only that chip up
                            movedChipsUp = movedChipsUp || (dir == 1 && move.Count == 1 && newFloors[currentState.elevator].Contains(-1 * move[0]));
                        }
                    }
                }

                // Let's do some sanity checks
                /* if (openSet.Count > 1000)
                {
                    // Search openSet for matching states assuming the hashing is failing
                    var keys = gScore.Keys;
                    var found = false;

                    for (int i = 0; i < keys.Count - 1 && !found; i++)
                    {
                        for (int q = i + 1; q < keys.Count && !found; q++)
                        {
                            var scoreA = keys.ElementAt(i);
                            var scoreB = keys.ElementAt(q);

                            if (scoreA.elevator == scoreB.elevator
                            && scoreA.floors[0].SequenceEqual(scoreB.floors[0])
                            && scoreA.floors[1].SequenceEqual(scoreB.floors[1])
                            && scoreA.floors[2].SequenceEqual(scoreB.floors[2])
                            && scoreA.floors[3].SequenceEqual(scoreB.floors[3]))
                            {
                                Console.WriteLine("Found match:");
                                Console.WriteLine($"Index: {i} / {q}");;
                                Console.WriteLine($"Score A: {scoreA.GetHashCode()}");
                                Console.WriteLine($"Score B: {scoreB.GetHashCode()}");
                            }
                        }
                    }
                } */
            } while (openSet.Count > 0);

            return 0;
        }

        protected override string SolvePartOne()
        {
            // This takes under 1 second
            return AStar(this.initial).ToString();
        }

        protected override string SolvePartTwo()
        {
            // This takes about 45 seconds
            return AStar(this.initial2).ToString();
        }
    }
}
