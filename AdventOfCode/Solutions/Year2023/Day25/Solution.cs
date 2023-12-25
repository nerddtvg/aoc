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
        UndirectedGraph<Node, NodeEdge> graph;

        private class Node
        {
            public string node;
        }

        private class NodeEdge : Edge<Node>
        {
            public int weight;

            public NodeEdge(Node source, Node target) : base(source, target) { }
        }

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

            graph = new UndirectedGraph<Node, NodeEdge>();

            foreach(var line in Input.SplitByNewline(shouldTrim: true))
            {
                var split = line.Split(':', StringSplitOptions.TrimEntries);
                var children = split[1].Split(' ');

                graph.AddVerticesAndEdgeRange(children.Select(child => new NodeEdge(new Node() { node = split[0] }, new Node() { node = child }) { weight = 1 }));
            }
        }

        protected override string? SolvePartOne()
        {
            // Tried to implement mincut as shown from Thomas Jungblut
            // This is a very slow algorithm given our data structures

            // Save the original count of nodes
            var nodeCount = graph.VertexCount;

            // https://blog.thomasjungblut.com/graph/mincut/mincut/
            var currentPartition = new HashSet<Node>();
            HashSet<Node> currentBestPartition = default!;
            (Node sNode, Node tNode, int weight) currentBestCut = default;

            while(graph.VertexCount > 1)
            {
                var cutOfThePhase = MaximumAdjSearch(graph);

                if (currentBestCut == default || cutOfThePhase.weight < currentBestCut.weight)
                {
                    currentBestCut = cutOfThePhase;
                    currentBestPartition = new HashSet<Node>(currentPartition)
                    {
                        cutOfThePhase.tNode
                    };
                }

                currentPartition.Add(cutOfThePhase.tNode);

                // Merge S and T nodes, T goes away
                // If there is a common node that both S and T connect to, we need to identify it
                var commonNodeList = graph.AdjacentVertices(cutOfThePhase.tNode)
                    .Intersect(graph.AdjacentVertices(cutOfThePhase.sNode))
                    .ToList();

                foreach (var commonNode in commonNodeList)
                {
                    // Found one, need to combine the weights
                    // Increase sNode <=> commonNodeName with the value from tNode <=> commonNodeName
                    graph.TryGetEdge(cutOfThePhase.sNode, commonNode, out var sEdge);
                    graph.TryGetEdge(cutOfThePhase.sNode, commonNode, out var tEdge);
                    sEdge.weight += tEdge.weight;
                }

                // Now we remove tNode completely
                graph.RemoveVertex(cutOfThePhase.tNode);
            }

            return ((nodeCount - currentBestPartition.Count) * currentBestPartition.Count).ToString();
        }

        private (Node sNode, Node tNode, int weight) MaximumAdjSearch(UndirectedGraph<Node, NodeEdge> graph)
        {
            var start = graph.Vertices.First();
            var foundSet = new List<Node>() { start };
            var cutWeight = new List<int>();
            var candidates = new HashSet<Node>(graph.Vertices);
            candidates.Remove(start);

            while(candidates.Count > 0)
            {
                Node maxNextVertex = default!;
                var maxWeight = int.MinValue;

                foreach(var next in candidates)
                {
                    int weightSum = 0;

                    foreach(var s in foundSet)
                    {
                        if (graph.TryGetEdge(next, s, out NodeEdge edge))
                        {
                            weightSum += edge.weight;
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

