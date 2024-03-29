using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Diagnostics.CodeAnalysis;


namespace AdventOfCode.Solutions.Year2021
{

    class Day23 : ASolution
    {
        State part1;
        State part1example;
        State part2;
        State part2example;

        public Day23() : base(23, 2021, "Amphipod")
        {
            // Our initial state
            this.part1 = new State()
            {
                pods = new Amphipod[8]
                {
                    new Amphipod() { pod = 'A', index = 1, x = 4, y = 1, moveCost = 1 },
                    new Amphipod() { pod = 'A', index = 2, x = 8, y = 2, moveCost = 1 },
                    new Amphipod() { pod = 'B', index = 1, x = 2, y = 2, moveCost = 10 },
                    new Amphipod() { pod = 'B', index = 2, x = 6, y = 2, moveCost = 10 },
                    new Amphipod() { pod = 'C', index = 1, x = 4, y = 2, moveCost = 100 },
                    new Amphipod() { pod = 'C', index = 2, x = 6, y = 1, moveCost = 100 },
                    new Amphipod() { pod = 'D', index = 1, x = 2, y = 1, moveCost = 1000 },
                    new Amphipod() { pod = 'D', index = 2, x = 8, y = 1, moveCost = 1000 }
                }
            };

            this.part1example = new State()
            {
                pods = new Amphipod[8]
                {
                    new Amphipod() { pod = 'A', index = 1, x = 2, y = 2, moveCost = 1 },
                    new Amphipod() { pod = 'A', index = 2, x = 8, y = 2, moveCost = 1 },
                    new Amphipod() { pod = 'B', index = 1, x = 2, y = 1, moveCost = 10 },
                    new Amphipod() { pod = 'B', index = 2, x = 6, y = 1, moveCost = 10 },
                    new Amphipod() { pod = 'C', index = 1, x = 4, y = 1, moveCost = 100 },
                    new Amphipod() { pod = 'C', index = 2, x = 6, y = 2, moveCost = 100 },
                    new Amphipod() { pod = 'D', index = 1, x = 4, y = 2, moveCost = 1000 },
                    new Amphipod() { pod = 'D', index = 2, x = 8, y = 1, moveCost = 1000 }
                }
            };

            this.part2 = new State()
            {
                pods = new Amphipod[16]
                {
                    new Amphipod() { pod = 'A', index = 1, x = 4, y = 1, maxY = 4, moveCost = 1 },
                    new Amphipod() { pod = 'A', index = 2, x = 8, y = 4, maxY = 4, moveCost = 1 },
                    new Amphipod() { pod = 'B', index = 1, x = 2, y = 4, maxY = 4, moveCost = 10 },
                    new Amphipod() { pod = 'B', index = 2, x = 6, y = 4, maxY = 4, moveCost = 10 },
                    new Amphipod() { pod = 'C', index = 1, x = 4, y = 4, maxY = 4, moveCost = 100 },
                    new Amphipod() { pod = 'C', index = 2, x = 6, y = 1, maxY = 4, moveCost = 100 },
                    new Amphipod() { pod = 'D', index = 1, x = 2, y = 1, maxY = 4, moveCost = 1000 },
                    new Amphipod() { pod = 'D', index = 2, x = 8, y = 1, maxY = 4, moveCost = 1000 },

                    new Amphipod() { pod = 'A', index = 3, x = 6, y = 3, maxY = 4, moveCost = 1 },
                    new Amphipod() { pod = 'A', index = 4, x = 8, y = 2, maxY = 4, moveCost = 1 },
                    new Amphipod() { pod = 'B', index = 3, x = 4, y = 3, maxY = 4, moveCost = 10 },
                    new Amphipod() { pod = 'B', index = 4, x = 6, y = 2, maxY = 4, moveCost = 10 },
                    new Amphipod() { pod = 'C', index = 3, x = 4, y = 2, maxY = 4, moveCost = 100 },
                    new Amphipod() { pod = 'C', index = 4, x = 8, y = 3, maxY = 4, moveCost = 100 },
                    new Amphipod() { pod = 'D', index = 3, x = 2, y = 2, maxY = 4, moveCost = 1000 },
                    new Amphipod() { pod = 'D', index = 4, x = 2, y = 3, maxY = 4, moveCost = 1000 }
                }
            };

            this.part2example = new State()
            {
                pods = new Amphipod[16]
                {
                    new Amphipod() { pod = 'A', index = 1, x = 2, y = 4, maxY = 4, moveCost = 1 },
                    new Amphipod() { pod = 'A', index = 2, x = 8, y = 4, maxY = 4, moveCost = 1 },
                    new Amphipod() { pod = 'B', index = 1, x = 2, y = 1, maxY = 4, moveCost = 10 },
                    new Amphipod() { pod = 'B', index = 2, x = 6, y = 1, maxY = 4, moveCost = 10 },
                    new Amphipod() { pod = 'C', index = 1, x = 4, y = 1, maxY = 4, moveCost = 100 },
                    new Amphipod() { pod = 'C', index = 2, x = 6, y = 4, maxY = 4, moveCost = 100 },
                    new Amphipod() { pod = 'D', index = 1, x = 4, y = 4, maxY = 4, moveCost = 1000 },
                    new Amphipod() { pod = 'D', index = 2, x = 8, y = 1, maxY = 4, moveCost = 1000 },

                    new Amphipod() { pod = 'A', index = 3, x = 6, y = 3, maxY = 4, moveCost = 1 },
                    new Amphipod() { pod = 'A', index = 4, x = 8, y = 2, maxY = 4, moveCost = 1 },
                    new Amphipod() { pod = 'B', index = 3, x = 4, y = 3, maxY = 4, moveCost = 10 },
                    new Amphipod() { pod = 'B', index = 4, x = 6, y = 2, maxY = 4, moveCost = 10 },
                    new Amphipod() { pod = 'C', index = 3, x = 4, y = 2, maxY = 4, moveCost = 100 },
                    new Amphipod() { pod = 'C', index = 4, x = 8, y = 3, maxY = 4, moveCost = 100 },
                    new Amphipod() { pod = 'D', index = 3, x = 2, y = 2, maxY = 4, moveCost = 1000 },
                    new Amphipod() { pod = 'D', index = 4, x = 2, y = 3, maxY = 4, moveCost = 1000 }
                }
            };
        }

        protected override string? SolvePartOne()
        {
            // With updated Part 2 code, this answer is off by 2 with the example and input (+2 to the answer)
            return AStar(this.part1).ToString();
        }

        protected override string? SolvePartTwo()
        {
            this.part2.pods = this.part2.pods.SortPods();
            return AStar(this.part2).ToString();
        }

        public struct State
        {
            public State() { }

            // This defines where each amphipod is located
            // And the current cost
            public int cost = 0;

            public Amphipod[] pods = new Amphipod[0];

            public bool isSolved => pods.All(pod => pod.isSolved);

            internal string podsHash => string.Join("-", pods
                .OrderBy(pod => pod.pod)
                .ThenBy(pod => pod.x)
                .ThenBy(pod => pod.y)
                .SelectMany(pod => new int[] { pod.pod, pod.x, pod.y })
                .ToArray());

            public override int GetHashCode()
            {
                return podsHash.GetHashCode();
            }

            public override bool Equals([NotNullWhen(true)] object? obj)
            {
                // Check if the pods are equal
                if (obj == null || obj.GetType() != typeof(State))
                    return false;

                return Equals((State)obj);
            }

            public bool Equals(State obj)
            {
                // Ignore the costs, only care about the pods and their locations (position of A1 and A2 do not matter if swapped)
                return podsHash == obj.podsHash;
            }
        }

        public bool IsSolved(State state)
        {
            return state.pods.All(pod => pod.isSolved);
        }

        public IEnumerable<State> GetPossibleStates(State state)
        {
            // Goes through each possible move and lists it out
            // Get everything that can move into a room:
            // Restriction: Room must be empty or only have the corresponding amphipod included
            // If we find another pod with the same x as podRoomX, and it isn't the right pod char, it's bad
            var moveIntoRoom = state.pods
                // Anything in a hallway
                .Where(pod => !pod.inRoom)
                // Where the room is completely open
                // or anything below is the same type
                .Where(pod =>
                    !state.pods.Any(pod2 => pod2.x == pod.podRoomX)
                    ||
                    state.pods.Where(pod2 => pod2.x == pod.podRoomX).All(pod2 => pod2.pod == pod.pod)
                )
                .ToList();

            // Those we can move into a hallway
            // The spot above must be empty
            var moveIntoHall = state.pods
                // Must be in a room
                .Where(pod => pod.inRoom)
                // Nothing is above
                .Where(pod => !state.pods.Any(pod2 => pod2.x == pod.x && pod2.y < pod.y))
                // Either is in the wrong room OR has the wrong types below
                .Where(pod =>
                    !pod.isSolved
                    ||
                    state.pods.Any(pod2 => pod2.pod != pod.pod && pod2.x == pod.x && pod2.y > pod.y)
                )
                .ToList();

            // Those we can move directly into their appropriate room
            // This applies if their destination room has 0 pods or pods that are the same
            var swapIntoRoom = moveIntoHall
                .Where(pod => !pod.isSolved && !state.pods.Any(pod2 => pod2.x == pod.podRoomX && pod2.pod != pod.pod))
                .ToList();

            // Do each possible action
            foreach (var move in moveIntoRoom.Union(swapIntoRoom))
            {
                // Make sure we're not blocked (validation this is a good move)
                var diffX = Math.Abs(move.podRoomX - move.x);
                if (state.pods.Any(pod => pod.y == 0 && pod.x > Math.Min(move.x, move.podRoomX) && pod.x < Math.Max(move.x, move.podRoomX)))
                    continue;

                // If someone is already in this room and not the correct pod, skip it
                if (state.pods.Any(pod => pod.x == move.podRoomX && pod.pod != move.pod))
                    continue;

                // Find the appropriate room and step count
                var steps = move.maxY - state.pods.Count(pod => pod.x == move.podRoomX);

                // Start our new pod
                var newPod = move.Clone();
                newPod.y = steps;
                newPod.x = move.podRoomX;

                // Then count the steps to this x
                steps += Math.Abs(diffX);

                // We may be in a different room, so add our current y value
                if (move.y > 0 && move.x != move.podRoomX)
                    steps += move.y;

                // So now we have our step count, increase the cost
                var newState = state.Clone();

                // Update!
                newState.pods = newState.pods.ReplacePod(newPod);
                newState.cost += steps * newPod.moveCost;

                // Return this possible new state
                yield return newState;
            }

            // Do each possible action
            foreach (var move in moveIntoHall)
            {
                // Flags to find if have been blocked
                var blockedLeft = false;
                var blockedRight = false;

                // Make sure we are not blocked above
                if (state.pods.Any(pod => pod.x == move.x && pod.y < move.y))
                    continue;

                // Start our step count as our y steps up out of the room
                int stepsY = move.y;

                // We have a start x (pod.x) and can go left (>=0) or right (<=10)
                // So let's start from x and move in each direction outwards
                for (int diffX = 1; diffX <= 8 && (!blockedLeft || !blockedRight); diffX++)
                {
                    // We can only move up to 8 in each direction
                    // And we can't stop above the room, so it is never zero

                    // Check left
                    if (!blockedLeft)
                    {
                        var newX = move.x - diffX;

                        if (newX >= 0)
                        {
                            // If this is an open space and not a door (2, 4, 6, 8)
                            // then it is a valid move
                            if (newX != 2 && newX != 4 && newX != 6 && newX != 8)
                            {
                                if (!state.pods.Any(pod => pod.x == newX && pod.y == 0))
                                {
                                    // Start our new pod
                                    var newPod = move.Clone();
                                    newPod.y = 0;
                                    newPod.x = newX;

                                    // Our new state
                                    var newState = state.Clone();
                                    newState.pods = newState.pods.ReplacePod(newPod);
                                    newState.cost += (stepsY + diffX) * move.moveCost;

                                    // Return this state and move on
                                    yield return newState;
                                }
                                else
                                {
                                    blockedLeft = true;
                                }
                            }
                        }
                    }

                    // Check right
                    if (!blockedRight)
                    {
                        var newX = move.x + diffX;

                        if (newX <= 10)
                        {
                            // If this is an open space and not a door (2, 4, 6, 8)
                            // then it is a valid move
                            if (newX != 2 && newX != 4 && newX != 6 && newX != 8)
                            {
                                if (!state.pods.Any(pod => pod.x == newX && pod.y == 0))
                                {
                                    // Start our new pod
                                    var newPod = move.Clone();
                                    newPod.y = 0;
                                    newPod.x = newX;

                                    // Our new state
                                    var newState = state.Clone();
                                    newState.pods = newState.pods.ReplacePod(newPod);
                                    newState.cost += (stepsY + diffX) * move.moveCost;

                                    // Return this state and move on
                                    yield return newState;
                                }
                                else
                                {
                                    blockedRight = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        // Based on: https://en.wikipedia.org/wiki/A*_search_algorithm
        public int AStar(State start)
        {
            // This is the list of nodes we need to search
            var openSet = new PriorityQueue<State, int>();
            openSet.Enqueue(start, 0);

            // A Dictionary of the node that we cameFrom Value to reach the node Key
            var cameFrom = new Dictionary<(int x, int y), (int x, int y)>();

            // gScore is the known shortest path from start to Key, other values are assumed infinity
            var gScore = new Dictionary<State, int>() { { start, 0 } };

            int minCost = int.MaxValue;

            do
            {
                // Get the next node to work on
                var currentNode = openSet.Dequeue();

                // Debug: If all are in a room, break for checks
                // if (currentNode.cost > 0 && currentNode.pods.All(pod => pod.inRoom))
                //     System.Diagnostics.Debugger.Break();

                // Removed the short-circuit code so that we could count steps more
                if (currentNode.isSolved)
                {
                    // Found the shortest possible route
                    minCost = Math.Min(minCost, currentNode.cost);
                }

                // Shortcut: If we have already found a shorter path, ignore this
                if (currentNode.cost >= minCost)
                    continue;

                // Get possible neighbors
                // Then we get each of the possible moves because there could be multiple moves to each tile
                // That function will also provide a cost of moving to that tile
                foreach (var move in GetPossibleStates(currentNode))
                {
                    // Our priority is going to simply be the number of steps to the appropriate room for any unsolved
                    // This is a really, really rough cost just to prioritize the lowest first
                    var tgScore = move.cost + currentNode
                        .pods
                        .Where(pod => !pod.isSolved).Sum(pod => pod.moveCost * (pod.y + Math.Abs(pod.x - pod.podRoomX)));

                    if (move.cost < minCost && (!gScore.ContainsKey(move) || move.cost < gScore[move]))
                    {
                        gScore[move] = move.cost;

                        // Add this to our queue
                        openSet.Enqueue(move, tgScore);
                    }
                }
            } while (openSet.Count > 0);

            return minCost;
        }
    }

    public struct Amphipod
    {
        public Amphipod() { }

        // This defines a pod (location and move cost)
        public char pod = '0';
        public int index = 0;
        public int x = 0;
        public int y = 0;
        public int moveCost = 0;
        public int maxY = 2;

        // We're in a room if we're not in the hallway
        public bool inRoom => y > 0;

        public bool isSolved => inRoom && x == podRoomX;

        // If each char is in its approprate room
        // A == 2
        // B == 4
        // C == 6
        // D == 8
        public int podRoomX => 2 + ((int)(pod - 'A') * 2);

        public override string ToString() => $"Pod: {pod}-{index}, ({x},{y})";
    }

    public static class AmphipodExtensions
    {
        public static Amphipod[] SortPods(this IEnumerable<Amphipod> pods)
        {
            return pods.OrderBy(pod => pod.pod).ThenBy(pod => pod.index).ToArray();
        }
        
        public static Amphipod[] ReplacePod(this IEnumerable<Amphipod> pods, Amphipod newPod)
        {
            return pods
                // Skip the new pod
                .Where(pod => !(pod.pod == newPod.pod && pod.index == newPod.index))
                // Append it
                .Append(newPod)
                // Sort
                .SortPods();
        }
    }
}

