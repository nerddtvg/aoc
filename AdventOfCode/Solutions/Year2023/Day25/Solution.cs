using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using QuikGraph;


namespace AdventOfCode.Solutions.Year2023
{
    class Day25 : ASolution
    {
        private Dictionary<string, Dictionary<string, int>> connected = new();

        public Day25() : base(25, 2023, "Snowverload")
        {
            // DebugInput = @"jqt: rhn xhk nvd
            //                rsh: frs pzl lsr
            //                xhk: hfx
            //                cmg: qnr nvd lhk bvb
            //                rhn: xhk bvb hfx
            //                bvb: xhk hfx
            //                pzl: lsr hfx nvd
            //                qnr: nvd
            //                ntq: jqt hfx bvb xhk
            //                nvd: lhk
            //                lsr: lhk
            //                rzs: qnr cmg lsr rsh
            //                frs: qnr lhk lsr";

            foreach(var line in Input.SplitByNewline(shouldTrim: true))
            {
                var split = line.Split(':', StringSplitOptions.TrimEntries);
                var children = split[1].Split(' ');

                if (!connected.ContainsKey(split[0]))
                    connected[split[0]] = new(children.ToDictionary(c => c, c => 1));
                else
                    children.ForEach(child => connected[split[0]][child] = 1);

                foreach (var child in children)
                    if (!connected.ContainsKey(child))
                        connected[child] = new() { { split[0], 1 } };
                    else
                        connected[child][split[0]] = 1;
            }
        }

        protected override string? SolvePartOne()
        {
            // Tried to implement mincut as shown from Thomas Jungblut
            // This is a very slow algorithm given our data structures

            // Save the original count of nodes
            var nodeCount = connected.Keys.Count;

            // https://blog.thomasjungblut.com/graph/mincut/mincut/
            var currentPartition = new HashSet<string>();
            HashSet<string> currentBestPartition = default!;
            (string sNode, string tNode, int weight) currentBestCut = default;

            while(connected.Count > 1)
            {
                var cutOfThePhase = MaximumAdjSearch(connected);

                if (currentBestCut == default || cutOfThePhase.weight < currentBestCut.weight)
                {
                    currentBestCut = cutOfThePhase;
                    currentBestPartition = new HashSet<string>(currentPartition)
                    {
                        cutOfThePhase.tNode
                    };
                }

                currentPartition.Add(cutOfThePhase.tNode);

                // Merge S and T nodes, T goes away
                // If there is a common node that both S and T connect to, we need to identify it
                var commonNode = connected[cutOfThePhase.tNode].Select(edge => edge.Key)
                    .Intersect(connected[cutOfThePhase.sNode].Select(edge => edge.Key))
                    .ToList();

                foreach (var commonNodeName in commonNode)
                {
                    // Found one, need to combine the weights
                    // Increase sNode <=> commonNodeName with the value from tNode <=> commonNodeName
                    connected[cutOfThePhase.sNode][commonNodeName] += connected[cutOfThePhase.tNode][commonNodeName];
                }

                // Now we remove tNode completely
                connected[cutOfThePhase.tNode].Keys.ForEach(key => connected[key].Remove(cutOfThePhase.tNode));
                connected.Remove(cutOfThePhase.tNode);
            }

            return ((nodeCount - currentBestPartition.Count) * currentBestPartition.Count).ToString();
        }

        private (string sNode, string tNode, int weight) MaximumAdjSearch(Dictionary<string, Dictionary<string, int>> graph)
        {
            var start = graph.Keys.First();
            var foundSet = new List<string>() { start };
            var cutWeight = new List<int>();
            var candidates = new HashSet<string>(graph.Keys);
            candidates.Remove(start);

            while(candidates.Count > 0)
            {
                var maxNextVertex = string.Empty;
                var maxWeight = int.MinValue;

                foreach(var next in candidates)
                {
                    int weightSum = 0;

                    foreach(var s in foundSet)
                    {
                        if (graph[next].TryGetValue(s, out int sWeight))
                        {
                            weightSum += sWeight;
                        }
                    }

                    if (weightSum > maxWeight)
                    {
                        maxNextVertex = next;
                        maxWeight = weightSum;
                    }
                }

                candidates.Remove(maxNextVertex);
                foundSet.Add(maxNextVertex);
                cutWeight.Add(maxWeight);
            }

            return (foundSet[^2], foundSet[^1], cutWeight[^1]);
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

