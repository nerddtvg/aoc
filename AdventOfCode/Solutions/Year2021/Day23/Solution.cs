using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace AdventOfCode.Solutions.Year2021
{

    class Day23 : ASolution
    {
        State initial;

        public Day23() : base(23, 2021, "Amphipod")
        {
            // Our initial state
            this.initial = new State()
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
        }

        protected override string? SolvePartOne()
        {
            return AStar(this.initial.Clone()).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }

        public struct State
        {
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

        public struct Amphipod
        {
            // This defines a pod (location and move cost)
            public char pod;
            public int index;
            public int x;
            public int y;
            public int moveCost = 0;

            // We're in a room if we're not in the hallway
            public bool inRoom => y > 0;

            public bool isSolved => inRoom && x == podRoomX;

            // If each char is in its approprate room
            // A == 2
            // B == 4
            // C == 6
            // D == 8
            public int podRoomX => 2 + ((int)(pod - 'A') * 2);
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
            var moveIntoRoom = state.pods.Where(pod => !pod.inRoom && !state.pods.Any(pod2 => pod2.x == pod.podRoomX && pod.pod != pod2.pod)).ToList();

            // Those we can move into a hallway
            // The spot above must be empty
            var moveIntoHall = state.pods.Where(pod => pod.inRoom && !(pod.isSolved && pod.y == 2) && (pod.y == 1 || state.pods.Count(pod2 => pod2.x == pod.x && pod2.y == 1) == 0)).ToList();

            // Do each possible action
            foreach(var move in moveIntoRoom)
            {
                // Make sure we're not blocked (validation this is a good move)
                var diffX = Math.Abs(move.podRoomX - move.x);
                if (Enumerable.Range(Math.Min(move.x, move.podRoomX), diffX+1).Any(i => state.pods.Count(pod => !(pod.pod == move.pod && pod.index == move.index) && pod.x == move.x + i) > 0))
                    continue;

                // Find the appropriate room and step count
                // Start with finding out if the pair amphipod is already in the room, if so we only move 1 y down otherwise we move 2 to the bottom
                var steps = state.pods.Count(pod => pod.pod == move.pod && pod.index != move.index && pod.isSolved) == 1 ? 1 : 2;

                // Start our new pod
                var newPod = move.Clone();
                newPod.y = steps;
                newPod.x = move.podRoomX;

                // Then count the steps to this x
                steps += Math.Abs(diffX);

                // So now we have our step count, increase the cost
                var newState = state.Clone();
                var newPods = newState.pods.Where(pod => !(pod.pod == move.pod && pod.index == move.index)).Append(newPod).OrderBy(pod => pod.pod).ThenBy(pod => pod.index).ToArray();

                // Update!
                newState.pods = newPods;
                newState.cost += steps * newPod.moveCost;

                // Return this possible new state
                yield return newState;
            }

            // Do each possible action
            foreach(var move in moveIntoHall)
            {
                // Flags to find if have been blocked
                var blockedLeft = false;
                var blockedRight = false;

                // Make sure we are not blocked above
                if (state.pods.Count(pod => pod.x == move.x && pod.y != move.y - 1) == 1)
                    continue;

                // Start our step count as our y steps up out of the room
                int steps = move.y;

                // We have a start x (pod.x) and can go left (>=0) or right (<=10)
                // So let's start from x and move in each direction outwards
                for (int diffX = 1; diffX <= 8; diffX++)
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
                                if (state.pods.Count(pod => pod.x == newX && pod.y == 0) == 0)
                                {
                                    // Start our new pod
                                    var newPod = move.Clone();
                                    newPod.y = 0;
                                    newPod.x = newX;

                                    // Our new state
                                    var newState = state.Clone();
                                    var newPods = newState.pods.Where(pod => !(pod.pod == move.pod && pod.index == move.index)).Append(newPod).OrderBy(pod => pod.pod).ThenBy(pod => pod.index).ToArray();

                                    newState.pods = newPods;
                                    newState.cost += steps + diffX;

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
                                if (state.pods.Count(pod => pod.x == newX && pod.y == 0) == 0)
                                {
                                    // Start our new pod
                                    var newPod = move.Clone();
                                    newPod.y = 0;
                                    newPod.x = newX;

                                    // Our new state
                                    var newState = state.Clone();
                                    var newPods = newState.pods.Where(pod => !(pod.pod == move.pod && pod.index == move.index)).Append(newPod).OrderBy(pod => pod.pod).ThenBy(pod => pod.index).ToArray();

                                    newState.pods = newPods;
                                    newState.cost += steps + diffX;

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

            do
            {
                // Get the next node to work on
                var currentNode = openSet.Dequeue();

                // Debug: If all are in a room, break for checks
                if (currentNode.cost > 0 && currentNode.pods.All(pod => pod.inRoom))
                    System.Diagnostics.Debugger.Break();

                // Removed the short-circuit code so that we could cound steps more
                if (currentNode.isSolved)
                {
                    // Found the shortest possible route
                    return currentNode.cost;
                }
                
                // Get possible neighbors
                // Then we get each of the possible moves because there could be multiple moves to each tile
                // That function will also provide a cost of moving to that tile
                foreach(var move in GetPossibleStates(currentNode))
                {
                    // Our priority is going to simply be the number of steps to the appropriate room for any unsolved
                    // This is a really, really rough cost just to prioritize the lowest first
                    var tgScore = currentNode.cost + currentNode
                        .pods
                        .Where(pod => !pod.isSolved).Sum(pod => pod.moveCost * (pod.y + Math.Abs(pod.x - pod.podRoomX)));

                    if (!gScore.ContainsKey(move) || move.cost < gScore[move])
                    {
                        gScore[move] = move.cost;

                        // Add this to our queue
                        openSet.Enqueue(move, tgScore);
                    }
                }
            } while (openSet.Count > 0);

            return 0;
        }
    }
}

#nullable restore
