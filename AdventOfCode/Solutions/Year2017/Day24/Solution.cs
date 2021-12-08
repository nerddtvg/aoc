using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2017
{

    class Day24 : ASolution
    {
        public class Bridge
        {
            public int[] ports;

            public Bridge(string line) => ports = line.Split('/').Select(num => Int32.Parse(num)).ToArray();
        }

        public List<Bridge> allBridgePorts;
        public (int length, int strength)[] allCalcs;

        public Day24() : base(24, 2017, "")
        {
            allBridgePorts = Input.SplitByNewline().Select(line => new Bridge(line)).ToList();

            allCalcs = GetAllBridgeLengthStrengths(0, new List<Bridge>(), allBridgePorts).ToArray();
        }

        protected override string? SolvePartOne()
        {
            return allCalcs.Max(br => br.strength).ToString();
        }

        public IEnumerable<(int length, int strength)> GetAllBridgeLengthStrengths(int currentPort, List<Bridge> currentBridges, List<Bridge> ports)
        {
            // First return the current score
            yield return (currentBridges.Count, currentBridges.Sum(b => b.ports.Sum()));

            // Find all possibles
            var possible = ports.Where(p => p.ports.Contains(currentPort)).ToList();

            if (possible == default)
                yield break;

            // For each of the possible next steps, return them
            foreach(var temp in possible)
            {
                var newBridge = currentBridges.Append(temp).ToList();
                var newPort = currentPort;

                // If the new port has mismatched A and B, find the other one
                if (temp.ports[0] != temp.ports[1])
                    newPort = temp.ports.First(p => newPort != p);

                var newPortList = new List<Bridge>(ports);
                var removeIndex = newPortList.IndexOf(temp);
                newPortList.RemoveAt(removeIndex);

                foreach(var val in GetAllBridgeLengthStrengths(newPort, newBridge, newPortList))
                {
                    yield return val;
                }
            }
        }

        protected override string? SolvePartTwo()
        {
            var longest = allCalcs.Max(br => br.length);

            return allCalcs.Where(br => br.length == longest).Max(br => br.strength).ToString();
        }
    }
}

#nullable restore
