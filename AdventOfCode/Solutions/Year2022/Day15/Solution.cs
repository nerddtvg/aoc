using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Diagnostics;

using Microsoft.Z3;

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
            // Bringing over some of the solution from 2018 Day 23
            var sensors = LoadSensors(Input);

            // Helper because Z3 .NET doesn't expose an easy Abs function for us
            ArithExpr zabs(ArithExpr x) =>
                (ArithExpr)x.Context.MkITE(x.Context.MkGe(x, x.Context.MkInt(0)), x, x.Context.MkMul(x, x.Context.MkInt(-1)));

            // Create the z3Context we use to create objects
            var z3Context = new Microsoft.Z3.Context();

            // Define the integer variables that will be used during processing
            (var x, var y) = (z3Context.MkIntConst("x"), z3Context.MkIntConst("y"));

            // An array of booleans (1 or 0) that will determine if a specific point is valid
            var valid = Enumerable.Range(0, sensors.Count).Select(c => z3Context.MkIntConst($"valid_{c}")).ToArray();

            // A total summation of bots in range
            var range_count = z3Context.MkIntConst("sum");

            // The optimizer / solver instance
            var optimize = z3Context.MkOptimize();

            // For each bot...
            for (int i = 0; i < sensors.Count; i++)
            {
                // Determine if the bot is in range of the new x, y, z
                // If so, set in_ranges[i] to 1
                optimize.Add(
                    z3Context.MkEq(
                        valid[i],
                        z3Context.MkITE(
                            // We want to know if this location is valid
                            // That means the point must be GREATER THAN the distance to a sensor,
                            // otherwise it would have been detected
                            z3Context.MkGt(
                                z3Context.MkAdd(
                                    zabs(z3Context.MkSub(x, z3Context.MkInt(sensors[i].x))),
                                    zabs(z3Context.MkSub(y, z3Context.MkInt(sensors[i].y)))
                                ),
                                z3Context.MkReal(sensors[i].distance)
                            ),
                            z3Context.MkInt(1),
                            z3Context.MkInt(0)
                        )
                    )
                );
            }

            // Limit x and y values per Part 2 rules
            optimize.Add(z3Context.MkGe(x, z3Context.MkInt(0)));
            optimize.Add(z3Context.MkGe(y, z3Context.MkInt(0)));
            optimize.Add(z3Context.MkLe(x, z3Context.MkInt(4000000)));
            optimize.Add(z3Context.MkLe(y, z3Context.MkInt(4000000)));

            // range_count is the total of in_ranges[1] (1 if in range, 0 otherwise)
            // So this tells us the maximum number of bots in range
            optimize.Add(z3Context.MkEq(range_count, z3Context.MkAdd(valid)));

            // We want the highest range_count, and it should equal our sensor count
            var h1 = optimize.MkMaximize(range_count);

            if (optimize.Check() != Status.SATISFIABLE)
                throw new Exception();

            // Debug output:
            var xVal = Int128.Parse(optimize.Model.Consts.First(c => c.Key.Name is StringSymbol s && s.String == "x").Value.ToString());
            var yVal = Int128.Parse(optimize.Model.Consts.First(c => c.Key.Name is StringSymbol s && s.String == "y").Value.ToString());
            Console.WriteLine($"Position: ({xVal},{yVal})");
            Console.WriteLine($"Valid Sensors: {h1.Upper}");
            Console.WriteLine($"Sensor Count: {sensors.Count}");

            // The Math is good, but casting to uint wasn't enough
            return ((xVal * 4000000) + yVal).ToString();
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

