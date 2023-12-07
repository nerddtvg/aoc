using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;


namespace AdventOfCode.Solutions.Year2023
{
    using TypeMap = (ulong sourceIdx, ulong destIdx, ulong count);

    public enum TypeOrder
    {
        seed,
        soil,
        fertilizer,
        water,
        light,
        temperature,
        humidity,
        location
    }

    class Day05 : ASolution
    {
        public List<ulong> seeds;
        public List<TypeMap> maps;

        const int TypeOrderCount = 8;

        // The range of numbers from the live data includes 4,294,967,296 which is uint+1
        const ulong MaxValue = (ulong)uint.MaxValue + 1;

        public Day05() : base(05, 2023, "If You Give A Seed A Fertilizer")
        {
            // Load the desired seed indices
            var groups = Input.SplitByBlankLine();
            seeds = new Regex(@"\d+").Matches(groups[0][0]).Select(digit => ulong.Parse(digit.Value)).ToList();

            maps = new();

            // Load the maps
            for (TypeOrder order = 0; (int)order < TypeOrderCount - 1; order++)
            {
                // Find the "{key}-to..." section and parse it
                var section = groups.First(grp => grp[0].StartsWith($"{order}-"));

                // We need to reorder the section by sourceIdx
                var regex = new Regex(@"^(\d+) (\d+) (\d+)$", RegexOptions.Multiline);

                var orderedDigits = regex
                    .Matches(string.Join('\n', section))
                    .Select(match => match.Groups.Values.Skip(1).Select(grp => ulong.Parse(grp.Value)).ToArray())
                    .OrderBy(row => row[1])
                    .ToArray();

                // Pad the start of orderedDigits to include 0 => min
                if (orderedDigits[0][1] > 0)
                    orderedDigits = orderedDigits.Prepend(new ulong[] { 0, 0, orderedDigits[0][1] }).ToArray();

                // Pad the end of orderedDigits to include the full range as well
                if (orderedDigits[^1][1] + orderedDigits[^1][2] < MaxValue)
                    orderedDigits = orderedDigits.Append(new ulong[] { orderedDigits[^1][1] + orderedDigits[^1][2], orderedDigits[^1][1] + orderedDigits[^1][2], MaxValue - (orderedDigits[^1][1] + orderedDigits[^1][2]) }).ToArray();

                // Reorder
                orderedDigits = orderedDigits.OrderBy(row => row[1]).ToArray();

                // In the sample data, there are no gaps in groups
                // But in the live data, there were gaps found in some
                // Let's go through and fix those now
                List<(ulong start, ulong count)> missing = new();
                for (int i = 0; i < orderedDigits.Length - 1; i++)
                {
                    if (orderedDigits[i][1] + orderedDigits[i][2] < orderedDigits[i + 1][1])
                        missing.Add((orderedDigits[i][1] + orderedDigits[i][2], orderedDigits[i + 1][1] - (orderedDigits[i][1] + orderedDigits[i][2])));
                }

                orderedDigits = orderedDigits
                    .Union(missing.Select(row => new ulong[] { row.start, row.start, row.count }))
                    .OrderBy(row => row[1])
                    .ToArray();

                // First step goes in as-is
                if (order == TypeOrder.seed)
                {
                    orderedDigits.ForEach(digits => maps.Add((digits[1], digits[0], digits[2])));

                    continue;
                }

                // Now we have a full range list (0 => MaxValue)
                // It should be easier to map the destinations now
                var tmpMaps = new List<TypeMap>();

                // We need to work up each slice and figure out where each begins/ends
                var stepDestMap = maps.First(map => map.destIdx == 0);

                var orderedIdx = 0;

                // We work our way up the slices and determine which is next by figuring out where
                // the Idx + Counts start/end
                do
                {
                    // Remove for future replace
                    maps.Remove(stepDestMap);

                    // Check if we break out
                    if (stepDestMap.count == 0 && orderedDigits.Length <= orderedIdx)
                        break;

                    var stepDestIdx = stepDestMap.destIdx;
                    var stepSourceIdx = orderedDigits[orderedIdx][1];
                    var stepSourceCount = orderedDigits[orderedIdx][2];

                    // At this point we should always have equal values
                    Debug.Assert(stepDestIdx == stepSourceIdx, "Invalid condition");

                    // We have equal starts, find the lowest "count" step
                    var minStep = Math.Min(stepDestMap.count, stepSourceCount);

                    // Add the new map
                    tmpMaps.Add((stepDestMap.sourceIdx, orderedDigits[orderedIdx][0], minStep));

                    if (stepSourceIdx + minStep == MaxValue)
                        break;

                    // Update the old map
                    stepDestMap.sourceIdx += minStep;
                    stepDestMap.destIdx += minStep;
                    stepDestMap.count -= minStep;

                    // Update the ordered start and count
                    orderedDigits[orderedIdx][0] += minStep;
                    orderedDigits[orderedIdx][1] += minStep;
                    orderedDigits[orderedIdx][2] -= minStep;

                    // Find the next if needed
                    if (stepDestMap.count == 0)
                        stepDestMap = maps.Single(map => map.destIdx == stepSourceIdx + minStep);

                    // Increment if needed
                    if (orderedDigits[orderedIdx][2] == 0)
                        orderedIdx++;
                } while (true);

                // We're done, save the new map
                maps = tmpMaps;
            }

            // To make this faster when searching, we order it descending by start value
            maps = maps.OrderByDescending(map => map.sourceIdx).ToList();
        }

        private ulong MapSeedLocation(ulong seed, ulong count = 1)
        {
            // Find all break points that may trigger for this range from seed to seed+count
            // The lowest location at all times will be the location for key
            // So we can return just that location from finalMaps
            var seedLocations = maps.Where(map => seed < map.sourceIdx && map.sourceIdx < seed+count)
                .Select(map => map.destIdx)
                .ToArray();

            // Find the seed itself
            var map = maps.First(map => map.sourceIdx <= seed);
            var seedLocation = map.destIdx + (seed - map.sourceIdx);

            return seedLocations.Append(seedLocation).Min();
        }

        protected override string? SolvePartOne()
        {
            return seeds.Min(seed => MapSeedLocation(seed)).ToString();
        }

        protected override string? SolvePartTwo()
        {
            // return string.Empty;
            // The original seeds array is a set of pairs
            // a b c d ... => (a, b), (c, d), ...
            // a is the start, b is the count
            // Get the seed locations listed
            return Enumerable.Range(0, seeds.Count / 2)
                .Select(idx => MapSeedLocation(seeds[idx * 2], seeds[(idx * 2) + 1]))
                .Min()
                .ToString();
        }
    }
}

