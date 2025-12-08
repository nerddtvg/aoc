using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Numerics;


namespace AdventOfCode.Solutions.Year2025
{

    class Day08 : ASolution
    {
        private readonly HashSet<(int a, int b, int c)> points;
        private readonly Dictionary<double, (int a, int b, int c)[]> pairs;

        private readonly List<HashSet<(int a, int b, int c)>> groups = [];

        private (int a, int b, int c)[] linkedPoints = [];

        public Day08() : base(08, 2025, "Playground")
        {
            // DebugInput = @"162,817,812
            //     57,618,57
            //     906,360,560
            //     592,479,940
            //     352,342,300
            //     466,668,158
            //     542,29,236
            //     431,825,988
            //     739,650,466
            //     52,470,668
            //     216,146,977
            //     819,987,18
            //     117,168,530
            //     805,96,715
            //     346,949,466
            //     970,615,88
            //     941,993,340
            //     862,61,35
            //     984,92,344
            //     425,690,689";

            points = [.. Input.SplitByNewline(true, true)
                .Select(line =>
                {
                    var t = line.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();
                    return (t[0], t[1], t[2]);
                })];

            // This assumes no overlapping distances between distinct pairs of points
            pairs = points
                .GetAllCombos(2)
                .Select(ePair => { var pairs = ePair.ToArray(); return (dist: pairs[0].Distance(pairs[1]), pairs); })
                .ToDictionary(itm => itm.dist, itm => itm.pairs);
        }

        protected override string? SolvePartOne()
        {
            // Difference with debug input
            int count = points.Count > 20 ? 1000 : 10;

            GroupTogether(count);

            // A lot happens in the initializer
            // Time  : 00:00:00.0704138
            return groups
                .Select(grp => grp.Count)
                .OrderDescending()
                .Take(3)
                .Aggregate(BigInteger.One, (a, b) => a * b)
                .ToString();
        }

        private void GroupTogether(int count = int.MaxValue)
        {
            // Go through the list of distances from shortest to (max count)
            // Identify if one or both of those items are in a circuit
            // If so, that circuit is now extended (or combined)
            // If not, make a new circuit

            // Sending through ToArray so we can remove entries
            foreach(var kvp in pairs.OrderBy(kvp => kvp.Key).Take(count).ToArray())
            {
                var found = -1;
                var pair = kvp.Value;

                var removeGroups = new List<int>();

                int idx = 0;
                foreach (var grp in groups)
                {
                    if (grp.Contains(pair[0]) || grp.Contains(pair[1]))
                    {
                        // If this is the first, append it
                        if (found < 0)
                        {
                            // Save for later
                            found = idx;

                            groups[idx].Add(pair[0]);
                            groups[idx].Add(pair[1]);
                        }
                        else
                        {
                            // Connecting two groups together!
                            groups[idx].ForEach(grp_itm => groups[found].Add(grp_itm));
                            removeGroups.Add(idx);
                        }
                    }

                    // For Part 2: If this made a single circuit from all points, then we are done
                    if (groups[idx].Count == points.Count)
                    {
                        linkedPoints = pair;
                        break;
                    }

                    idx++;
                }

                // Break out early for Part 2
                if (linkedPoints.Length > 0)
                    break;

                if (found < 0)
                {
                    // Not found, new circuit
                    groups.Add([.. pair]);
                }

                // Pass this descending and via ToArray to ensure we can manipulate the original list
                removeGroups.OrderDescending().ToArray().ForEach(groups.RemoveAt);

                pairs.Remove(kvp.Key);
            }
        }

        protected override string? SolvePartTwo()
        {
            GroupTogether();

            if (linkedPoints.Length != 2)
                throw new Exception("Unable to complete Part 2");

            return ((BigInteger)linkedPoints[0].a * (BigInteger)linkedPoints[1].a).ToString();
        }
    }
}

