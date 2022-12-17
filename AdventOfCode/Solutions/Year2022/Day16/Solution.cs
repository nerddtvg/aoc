using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2022
{

    class Day16 : ASolution
    {

        public Day16() : base(16, 2022, "Proboscidea Volcanium")
        {
            DebugInput = @"Valve AA has flow rate=0; tunnels lead to valves DD, II, BB
            Valve BB has flow rate=13; tunnels lead to valves CC, AA
            Valve CC has flow rate=2; tunnels lead to valves DD, BB
            Valve DD has flow rate=20; tunnels lead to valves CC, AA, EE
            Valve EE has flow rate=3; tunnels lead to valves FF, DD
            Valve FF has flow rate=0; tunnels lead to valves EE, GG
            Valve GG has flow rate=0; tunnels lead to valves FF, HH
            Valve HH has flow rate=22; tunnel leads to valve GG
            Valve II has flow rate=0; tunnels lead to valves AA, JJ
            Valve JJ has flow rate=21; tunnel leads to valve II";
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
                connections = match.Groups[3].Value.Split(',', StringSplitOptions.TrimEntries).ToArray()
            };
        }

        private Valve[] ReadValves(string input)
        {
            // Take our input lines and generate a graph
            return input.SplitByNewline(true)
                .Select(line => ReadValve(line))
                .ToArray();
        }

        /// <summary>
        /// We manipulate the state so these classes no longer match up, so this finds the right valve from our current state
        /// </summary>
        public Valve[] GetVertices(Valve currentValve, Valve[] state)
        {
            var currentId = currentValve.id;

            return currentValve.connections.Select(c => state.First(v => v.id == c)).ToArray();

        }

        /// <summary>
        /// Provides a list of all possible flow rates found.
        /// </summary>
        /// <param name="currentTime">How many minutes have passed to this point.</param>
        /// <param name="currentFlow">Our current flow amount.</param>
        /// <param name="currentValve">What valve we are currently on.</param>
        /// <param name="state">Valve states</param>
        public IEnumerable<int> GetMaxFlow(int timeLeft, int currentFlow, Valve currentValve, Valve[] state)
        {
            // Shortcut if we have somehow hit all of them
            // If we only have 1 minute left, only stay if we're not opened
            if (timeLeft <= 0 || (timeLeft == 1 && (currentValve.opened || currentValve.flowRate == 0)) || state.All(v => v.opened || v.flowRate == 0))
            {
                if (currentFlow > 0)
                    yield return currentFlow;
                yield break;
            }

            var outVertices = Array.Empty<Valve>();

            // Go through the options if we don't open this valve and simply move on
            if (timeLeft > 1)
            {
                outVertices = GetVertices(currentValve, state);

                foreach (var move in outVertices)
                {
                    // No state changes
                    foreach (var result in GetMaxFlow(timeLeft - 1, currentFlow, move, state))
                    {
                        yield return result;
                    }
                }
            }

            // And then if we do open the valve (if relevant)
            if (!currentValve.opened && currentValve.flowRate > 0)
            {
                // New value!
                timeLeft--;
                currentFlow += timeLeft * currentValve.flowRate;

                if (timeLeft <= 1)
                {
                    // Out of time
                    if (currentFlow > 0)
                        yield return currentFlow;
                    yield break;
                }

                if (outVertices.Length == 0)
                    outVertices = GetVertices(currentValve, state);
                
                // State change here
                var newValve = currentValve with {
                    opened = true
                };

                var newState = state.Where(v => v.id != newValve.id).Append(newValve).ToArray();

                foreach (var move in outVertices)
                {
                    foreach (var result in GetMaxFlow(timeLeft - 1, currentFlow, move, newState))
                    {
                        yield return result;
                    }
                }
            }
        }

        protected override string? SolvePartOne()
        {
            var valves = ReadValves(Input);

            // We're going to check every possibe combination
            var start = valves.First(v => v.id == "AA");
            var maxFlows = GetMaxFlow(30, 0, start, valves).ToList();

            return maxFlows.ToString();
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
            public string[] connections;
        }
    }
}

