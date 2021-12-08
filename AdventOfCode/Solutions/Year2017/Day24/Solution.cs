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

        public Day24() : base(24, 2017, "")
        {
            allBridgePorts = Input.SplitByNewline().Select(line => new Bridge(line)).ToList();
        }

        protected override string? SolvePartOne()
        {
            return GetAllBridgeStrengths(0, new List<Bridge>(), allBridgePorts).Max().ToString();
        }

        public IEnumerable<int> GetAllBridgeStrengths(int currentPort, List<Bridge> currentBridges, List<Bridge> ports)
        {
            // First return the current score
            yield return currentBridges.Sum(b => b.ports.Sum());

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

                foreach(var val in GetAllBridgeStrengths(newPort, newBridge, newPortList))
                {
                    yield return val;
                }
            }
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
