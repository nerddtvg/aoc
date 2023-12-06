using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;


namespace AdventOfCode.Solutions.Year2023
{
    using TypeMap = (TypeOrder sourceName, TypeOrder destName, uint sourceIdx, uint destIdx, uint count);

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
        public List<uint> seeds;
        public List<TypeMap> maps;

        const int TypeOrderCount = 8;

        public Day05() : base(05, 2023, "If You Give A Seed A Fertilizer")
        {
            DebugInput = @"seeds: 79 14 55 13

seed-to-soil map:
50 98 2
52 50 48

soil-to-fertilizer map:
0 15 37
37 52 2
39 0 15

fertilizer-to-water map:
49 53 8
0 11 42
42 0 7
57 7 4

water-to-light map:
88 18 7
18 25 70

light-to-temperature map:
45 77 23
81 45 19
68 64 13

temperature-to-humidity map:
0 69 1
1 0 69

humidity-to-location map:
60 56 37
56 93 4";

            // Load the desired seed indices
            var groups = Input.SplitByBlankLine();
            seeds = new Regex(@"\d+").Matches(groups[0][0]).Select(digit => uint.Parse(digit.Value)).ToList();

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
                    .Select(match => match.Groups.Values.Skip(1).Select(grp => uint.Parse(grp.Value)).ToArray())
                    .OrderBy(row => row[1])
                    .ToArray();

                // First step goes in as-is
                if (order == TypeOrder.seed)
                {
                    orderedDigits.ForEach(digits => maps.Add((order, order + 1, digits[1], digits[0], digits[2])));

                    // Fill in gaps at the top and bottom
                    // 0 => min(source)
                    // max(source) + count => uint.MaxValue
                    var min = orderedDigits.Min(digits => digits[1]);
                    var max = orderedDigits.Max(digits => digits[1]);
                    var maxCount = orderedDigits.First(digits => digits[1] == max)[2];
                    max += maxCount;

                    if (min > 0)
                        maps.Add((TypeOrder.seed, TypeOrder.soil, 0, 0, min));

                    if (max < uint.MaxValue)
                        maps.Add((TypeOrder.seed, TypeOrder.seed, max, max, uint.MaxValue - max));

                    continue;
                }

                // Now we have a full range list (0 => uint.MaxValue)
                // It should be easier to map the destinations now

                var tmpMaps = new List<TypeMap>();
                TypeMap defaultMap = (TypeOrder.seed, TypeOrder.seed, uint.MaxValue, uint.MaxValue, 0);

                // We need to work up each slice and figure out where each begins/ends
                var stepDestMap = maps.First(map => map.destIdx == 0);

                var orderedIdx = 0;

                // Pad the end of orderedDigits to include the full range as well
                var maxDigits = orderedDigits.Max(digits => digits[1]);
                var maxDigitsCount = orderedDigits.First(digits => digits[1] == maxDigits)[2];

                if (maxDigits + maxDigitsCount < uint.MaxValue)
                {
                    orderedDigits = orderedDigits.Append(new uint[] { maxDigits + maxDigitsCount, maxDigits + maxDigitsCount, uint.MaxValue - (maxDigits + maxDigitsCount) }).ToArray();
                }

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

                    if (stepDestIdx < stepSourceIdx)
                    {
                        // Gap at the top where the maps has mappings that orderedDigits doesn't
                        if (stepDestIdx + stepDestMap.count < stepSourceIdx)
                        {
                            // Directly copy this mapping changing the destination type
                            stepDestMap = stepDestMap.Clone();
                            stepDestMap.destName = order + 1;
                            tmpMaps.Add(stepDestMap);

                            // Move up to the next one
                            stepDestMap = maps
                                .Where(map => map.destIdx == (stepDestIdx + stepDestMap.count))
                                .DefaultIfEmpty(defaultMap)
                                .FirstOrDefault();

                            continue;
                        }

                        // We have a gap but we have to update the break point
                        tmpMaps.Add((TypeOrder.seed, order + 1, stepDestIdx, stepDestIdx, stepSourceIdx - stepDestIdx));

                        // Change the old breakpoint
                        stepDestMap.sourceIdx += stepSourceIdx - stepDestIdx;
                        stepDestMap.destIdx += stepSourceIdx - stepDestIdx;
                        stepDestMap.count -= stepSourceIdx - stepDestIdx;

                        if (stepDestMap.count == 0)
                            stepDestMap = maps.First(map => map.destIdx == stepSourceIdx);

                        continue;
                    }

                    // At this point we should always have equal values
                    Debug.Assert(stepDestIdx == stepSourceIdx, "Invalid condition");

                    // We have equal starts, find the lowest "count" step
                    var minStep = Math.Min(stepDestMap.count, stepSourceCount);

                    // Add the new map
                    tmpMaps.Add((TypeOrder.seed, order + 1, stepSourceIdx, orderedDigits[orderedIdx][0], minStep));

                    if (stepSourceIdx + minStep == uint.MaxValue)
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
                        stepDestMap = maps.First(map => map.destIdx == stepSourceIdx + minStep);

                    // Increment if needed
                    if (orderedDigits[orderedIdx][2] == 0)
                        orderedIdx++;
                } while (true);

                // We're done, save the new map
                maps = tmpMaps;

                Console.WriteLine($"seed 0 => {order+1} {maps.First(map => map.sourceIdx == 0).destIdx}");
            }
        }

        private uint MapSeedLocation(uint seed)
        {
            var map = maps.First(map => map.sourceIdx <= seed && seed <= map.sourceIdx + map.count);

            return map.destIdx + (seed - map.sourceIdx);
        }

        protected override string? SolvePartOne()
        {
            seeds.ForEach(seed => Console.WriteLine($"{seed}: {MapSeedLocation(seed)}"));

            return seeds.Min(MapSeedLocation).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
            // The original seeds array is a set of pairs
            // a b c d ... => (a, b), (c, d), ...
            // a is the start, b is the count
            // Get the seed locations listed
            return Enumerable.Range(0, seeds.Count / 2)
                .SelectMany(
                    idx => EnumerableExtensions.Range(seeds[idx * 2], seeds[(idx * 2) + 1])
                )
                .Select(MapSeedLocation)
                .Min()
                .ToString();
        }
    }
}

