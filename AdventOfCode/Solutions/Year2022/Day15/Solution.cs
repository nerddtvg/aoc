using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Diagnostics;

namespace AdventOfCode.Solutions.Year2022
{

    class Day15 : ASolution
    {

        public Day15() : base(15, 2022, "Beacon Exclusion Zone")
        {
            var example = @"Sensor at x=2, y=18: closest beacon is at x=-2, y=15
Sensor at x=9, y=16: closest beacon is at x=10, y=16
Sensor at x=13, y=2: closest beacon is at x=15, y=3
Sensor at x=12, y=14: closest beacon is at x=10, y=16
Sensor at x=10, y=20: closest beacon is at x=10, y=16
Sensor at x=14, y=17: closest beacon is at x=10, y=16
Sensor at x=8, y=7: closest beacon is at x=2, y=10
Sensor at x=2, y=0: closest beacon is at x=2, y=10
Sensor at x=0, y=11: closest beacon is at x=2, y=10
Sensor at x=20, y=14: closest beacon is at x=25, y=17
Sensor at x=17, y=20: closest beacon is at x=21, y=22
Sensor at x=16, y=7: closest beacon is at x=15, y=3
Sensor at x=14, y=3: closest beacon is at x=15, y=3
Sensor at x=20, y=1: closest beacon is at x=15, y=3";

            var sensors = LoadSensors(example);
            var count = CountSensors(sensors, 10);

            Debug.Assert(Debug.Equals(count, 26), $"Expected: 26\nActual: {count}");
        }

        private int CountSensors(List<Sensor> sensors, int y)
        {
            // Go across this y row
            // From x = Min(x - distance) to Max(x + distance)
            // Determine if x,y is within the distance to any sensor
            var minX = (int)sensors.Min(s => s.x - s.distance);
            var maxX = (int)sensors.Max(s => s.x + s.distance);

            int count = 0;

            for (var x = minX; x <= maxX; x++)
            {
                if (sensors.Any(s => (s.beaconX, s.beaconY) != (x, y) && (s.x, s.y).ManhattanDistance((x, y)) <= s.distance))
                    count++;
            }

            return count;
        }

        private List<Sensor> LoadSensors(string input)
        {
            var pattern = new Regex(@"^Sensor at x=([0-9\-]+), y=([0-9\-]+): closest beacon is at x=([0-9\-]+), y=([0-9\-]+)$");

            return input.SplitByNewline(true)
                .Select(line =>
                {
                    var match = pattern.Match(line);

                    if (!match.Success)
                        throw new Exception();

                    return new Sensor()
                    {
                        x = Int32.Parse(match.Groups[1].Value),
                        y = Int32.Parse(match.Groups[2].Value),
                        beaconX = Int32.Parse(match.Groups[3].Value),
                        beaconY = Int32.Parse(match.Groups[4].Value),
                        distance = (Int32.Parse(match.Groups[1].Value), Int32.Parse(match.Groups[2].Value)).ManhattanDistance((Int32.Parse(match.Groups[3].Value), Int32.Parse(match.Groups[4].Value)))
                    };
                })
                .ToList();
        }

        protected override string? SolvePartOne()
        {
            var sensors = LoadSensors(Input);
            var count = CountSensors(sensors, 2000000);

            return count.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }

        struct Sensor
        {
            public int x;
            public int y;
            public int beaconX;
            public int beaconY;
            public uint distance;
        }
    }
}

