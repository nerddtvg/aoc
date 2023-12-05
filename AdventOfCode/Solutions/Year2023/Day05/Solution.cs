using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


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

        public Day05() : base(05, 2023, "If You Give A Seed A Fertilizer")
        {
//             DebugInput = @"seeds: 79 14 55 13

// seed-to-soil map:
// 50 98 2
// 52 50 48

// soil-to-fertilizer map:
// 0 15 37
// 37 52 2
// 39 0 15

// fertilizer-to-water map:
// 49 53 8
// 0 11 42
// 42 0 7
// 57 7 4

// water-to-light map:
// 88 18 7
// 18 25 70

// light-to-temperature map:
// 45 77 23
// 81 45 19
// 68 64 13

// temperature-to-humidity map:
// 0 69 1
// 1 0 69

// humidity-to-location map:
// 60 56 37
// 56 93 4";

            // Load the desired seed indices
            var groups = Input.SplitByBlankLine();
            seeds = new Regex(@"\d+").Matches(groups[0][0]).Select(digit => uint.Parse(digit.Value)).ToList();

            maps = new();

            // Load the maps
            foreach(var order in Enum.GetValues<TypeOrder>().SkipLast(1))
            {
                // Find the "{key}-to..." section and parse it
                var section = groups.First(grp => grp[0].StartsWith($"{order}-"));

                for(int q=1; q<section.Length; q++)
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
            foreach (var order in Enum.GetValues<TypeOrder>().Skip(1))
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
            try
            {
                // Wrapped in a try/catch because ValueTuple won't return null for default
                var item = maps.First(map => map.destName == orderToFind && map.sourceIdx <= sourceValue && sourceValue < (map.sourceIdx + map.count));

                // Calculate the offset from the start -> count
                var offset = sourceValue - item.sourceIdx;

                // Return the destIdx with offset
                return item.destIdx + offset;
            }
            catch { }

            return sourceValue;
        }

        protected override string? SolvePartOne()
        {
            return seeds.Min(MapSeedLocation).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

