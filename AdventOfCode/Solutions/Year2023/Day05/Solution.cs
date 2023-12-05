using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.ComponentModel;


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
            // Load the desired seed indices
            var groups = Input.SplitByBlankLine();
            seeds = new Regex(@"\d+").Matches(groups[0][0]).Select(digit => uint.Parse(digit.Value)).ToList();

            maps = new();

            // Load the maps
            foreach(var order in Enum.GetValues<TypeOrder>().SkipLast(1))
            {
                // Find the "{key}-to..." section and parse it
                var section = groups.First(grp => grp[0].StartsWith($"{order}-"));

                for(uint q=1; q<section.Length; q++)
                {
                    // Parse the digits again
                    var digits = new Regex(@"\d+").Matches(section[q]).Select(digit => uint.Parse(digit.Value)).ToList();

                    maps.Add((order, order + 1, digits[1], digits[0], digits[2]));
                }
            }
        }

        private uint MapSeedLocation(uint seed)
        {
            uint value = seed;

            // We need to find each of the next locations
            for(TypeOrder order = (TypeOrder)1; (int)order < TypeOrderCount; order++)
                value = FindNextValue(order, value);

            return value;
        }

        /// <summary>
        /// Find the value of <paramref name="orderToFind"/>
        /// </summary>
        private uint FindNextValue(TypeOrder orderToFind, uint sourceValue)
        {
            // Find the source map that matches
            // the range of sourceIdx <= sourceValue < (sourceIdx + count)
            // If it is undefined, then the value is sourceValue
            // "Any source numbers that aren't mapped correspond to the same destination number. So, seed number 10 corresponds to soil number 10."
            Func<TypeMap, bool> conditional = map => map.destName == orderToFind && map.sourceIdx <= sourceValue && sourceValue < (map.sourceIdx + map.count);

            if (!maps.Any(conditional))
                return sourceValue;

            // Switched to Any to kick out bad values due to the excess debug loggin on exceptions
            var item = maps.First(map => map.destName == orderToFind && map.sourceIdx <= sourceValue && sourceValue < (map.sourceIdx + map.count));

            // Calculate the offset from the start -> count
            var offset = sourceValue - item.sourceIdx;

            // Return the destIdx with offset
            return item.destIdx + offset;
        }

        protected override string? SolvePartOne()
        {
            return seeds.Min(MapSeedLocation).ToString();
        }

        protected override string? SolvePartTwo()
        {
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

