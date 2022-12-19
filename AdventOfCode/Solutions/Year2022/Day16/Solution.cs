using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2022
{

    class Day16 : ASolution
    {
        private HashSet<int> maxFlows = new();
        int max = 0;

        public Day16() : base(16, 2022, "Proboscidea Volcanium")
        {
            // DebugInput = @"Valve AA has flow rate=0; tunnels lead to valves DD, II, BB
            // Valve BB has flow rate=13; tunnels lead to valves CC, AA
            // Valve CC has flow rate=2; tunnels lead to valves DD, BB
            // Valve DD has flow rate=20; tunnels lead to valves CC, AA, EE
            // Valve EE has flow rate=3; tunnels lead to valves FF, DD
            // Valve FF has flow rate=0; tunnels lead to valves EE, GG
            // Valve GG has flow rate=0; tunnels lead to valves FF, HH
            // Valve HH has flow rate=22; tunnel leads to valve GG
            // Valve II has flow rate=0; tunnels lead to valves AA, JJ
            // Valve JJ has flow rate=21; tunnel leads to valve II";
        }

        private Valve ReadValve(string line)
        {
            var pattern = new Regex(@"Valve ([A-Z]+) has flow rate=([0-9]+); tunnels? leads? to valves? ([A-Z ,]+)");

            var match = pattern.Match(line);

            if (!match.Success)
                throw new Exception();

            return new Valve()
            {
                id = match.Groups[1].Value,
                flowRate = Int32.Parse(match.Groups[2].Value),
                connections = new Dictionary<string, int>(match
                    .Groups[3]
                    .Value
                    .Split(',', StringSplitOptions.TrimEntries)
                    .Select(s => new KeyValuePair<string, int>(s, 1)))
            };
        }

        private Valve[] ReadValves(string input)
        {
            // Take our input lines and generate a graph
            var valves = input.SplitByNewline(true)
                .Select(line => ReadValve(line))
                .ToArray();

            // Let's reduce the graph removing meaningless zero use valves
            // Get all of the zero value nodes
            var zeroes = valves
                .Where(v => v.flowRate == 0 && v.id != "AA")
                .Select(v => v.id)
                .ToList();

            // For each, find its neighbors and remove this from the list
            foreach(var nodeToRemove in zeroes)
            {
                // Figure out who this connects to
                var newNeighbors = valves.First(v => v.id == nodeToRemove).connections;

                foreach(var a in newNeighbors)
                    foreach(var b in newNeighbors)
                    {
                        // No self-connections
                        if (a.Key == b.Key)
                            continue;

                        // Get the neighbors
                        var aValve = valves.First(v => v.id == a.Key);
                        var bValve = valves.First(v => v.id == b.Key);

                        aValve.connections[b.Key] = a.Value + b.Value;
                        bValve.connections[a.Key] = a.Value + b.Value;

                        aValve.connections.Remove(nodeToRemove);
                        bValve.connections.Remove(nodeToRemove);
                    }
            }

            // Now remove the nodes
            valves = valves.Where(v => !zeroes.Contains(v.id)).ToArray();

            // For the rest, let's find the shortest path between each
            // By doing this, we can avoid having to have open and closed paths below
            // because AA -> ## -> XY would already skip opening the valves
            var allValves = valves.Select(v => v.id).ToArray();

            foreach(var valveId in allValves)
            {
                var end = valves.First(v => v.id == valveId);

                foreach(var valve in valves)
                {
                    // Already have a path
                    if (valve.id == valveId || valve.connections.ContainsKey(valveId))
                        continue;

                    int minDistance = Int32.MaxValue;
                    GetShortestPath(valve, end, valves, Array.Empty<string>(), 0, ref minDistance);

                    valve.connections[valveId] = minDistance;
                    end.connections[valve.id] = minDistance;
                }
            }

            return valves;
        }

        /// <summary>
        /// Doing this as an interator causes us to search every path and no shortcuts, so using a ref param will help us determine if we should continue
        /// </summary>
        public void GetShortestPath(Valve start, Valve end, Valve[] valves, string[] visited, int depth, ref int minDistance)
        {
            // Add this to the list
            visited = visited.Append(start.id).ToArray();

            // Go through each connection and find a new path if possible
            foreach (var kvp in start.connections)
            {
                if (kvp.Key == end.id)
                {
                    minDistance = Math.Min(minDistance, depth + kvp.Value);
                    return;
                }

                // Otherwise we check each new connection found (if not in the state list)
                // Already visited there
                if (visited.Contains(kvp.Key))
                    continue;

                if (depth + kvp.Value > minDistance)
                    continue;

                GetShortestPath(valves.First(v => v.id == kvp.Key), end, valves, visited, depth + kvp.Value, ref minDistance);
            }
        }

        /// <summary>
        /// We manipulate the state so these classes no longer match up, so this finds the right valve from our current state
        /// </summary>
        public Valve[] GetVertices(Valve currentValve, Valve[] state)
        {
            var currentId = currentValve.id;

            return currentValve.connections.Select(kvp => state.First(v => v.id == kvp.Key)).ToArray();

        }

        /// <summary>
        /// Provides a list of all possible flow rates found.
        /// </summary>
        /// <param name="currentTime">How many minutes have passed to this point.</param>
        /// <param name="currentFlow">Our current flow amount.</param>
        /// <param name="currentValve">What valve we are currently on.</param>
        /// <param name="state">Valve states</param>
        public void GetMaxFlow(int timeLeft, int currentFlow, Valve currentValve, Valve[] state)
        {
            // Shortcut if we have somehow hit all of them
            // If we only have 1 minute left, no point in staying because even opening the valve does nothing
            if (timeLeft <= 1 || (timeLeft == 1 && (currentValve.opened || currentValve.flowRate == 0)) || state.All(v => v.opened || v.flowRate == 0))
            {
                if (currentFlow > 0)
                {
                    maxFlows.Add(currentFlow);
                    max = Math.Max(max, currentFlow);
                }

                return;
            }

            var outVertices = GetVertices(currentValve, state);

            // And then if we do open the valve (if relevant)
            if (!currentValve.opened && currentValve.flowRate > 0)
            {
                // New value!
                var newTimeLeft = timeLeft - 1;
                var newFlow = currentFlow + (newTimeLeft * currentValve.flowRate);

                if (newTimeLeft <= 1)
                {
                    // Out of time
                    if (newFlow > 0)
                    {
                        maxFlows.Add(newFlow);
                        max = Math.Max(max, newFlow);
                    }

                    return;
                }
                
                // State change here
                var newValve = currentValve with {
                    opened = true
                };

                var newState = state.Where(v => v.id != newValve.id).Append(newValve).ToArray();

                foreach (var move in outVertices)
                {
                    if (newTimeLeft < currentValve.connections[move.id])
                        continue;

                    GetMaxFlow(newTimeLeft - currentValve.connections[move.id], newFlow, move, newState);
                }
            }

            // What if we don't open a valve? (Only valid for "AA")
            if (currentValve.id == "AA")
                foreach (var move in outVertices)
                {
                    if (timeLeft < currentValve.connections[move.id])
                        continue;

                    // No state changes
                    GetMaxFlow(timeLeft - currentValve.connections[move.id], currentFlow, move, state);
                }
        }

        protected override string? SolvePartOne()
        {
            var valves = ReadValves(Input);

            // We're going to check every possibe combination
            var start = valves.First(v => v.id == "AA");
            maxFlows.Clear();
            GetMaxFlow(30, 0, start, valves);

            return maxFlows.Max().ToString();

            // Takes 1 hour 5 minutes
            // return "1701";
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }

        public struct Valve
        {
            public string id;
            public int flowRate;
            public bool opened;
            public Dictionary<string, int> connections;
        }
    }
}

