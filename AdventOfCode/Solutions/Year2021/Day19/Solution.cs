using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

using System.Linq;


// This is based on https://github.com/zedrdave/advent_of_code/blob/master/2021/19/__main__.py
// https://old.reddit.com/r/adventofcode/comments/rjpf7f/2021_day_19_solutions/hp8pkmb/
// This uses Matrix mathematics so we imported MathNet.Numerics

namespace AdventOfCode.Solutions.Year2021
{

    class Day19 : ASolution
    {
        // These are the rotation matrices how we rotate each sensor
        private Matrix<double> rotx = Matrix<double>.Build.DenseOfArray(new double[,] { { 1, 0, 0 }, { 0, 0, -1 }, { 0, 1, 0 } });
        private Matrix<double> roty = Matrix<double>.Build.DenseOfArray(new double[,] { { 0, 0, 1 }, { 0, 1, 0 }, { -1, 0, 0 } });
        private Matrix<double> rotz = Matrix<double>.Build.DenseOfArray(new double[,] { { 0, -1, 0 }, { 1, 0, 0 }, { 0, 0, 1 } });

        // Our 24 rotation matrices
        private List<Matrix<double>> rotations;

        // Our list of scanners
        // index 0: scanner
        // index 1: beacons
        // index 2: x,y,z
        private double[][][] scanners;

        // Our solved solutions
        private List<double[][]> aligned_scanners = new List<double[][]>();
        private List<double[]> scanner_positions = new List<double[]>();

        public Day19() : base(19, 2021, "Beacon Scanner")
        {
            scanners = Input.SplitByBlankLine().Select(
                lines => lines.Skip(1).Select(line => line.Split(",").Select(val => double.Parse(val)).ToArray()).ToArray()
            ).ToArray();

            // Build our rotations list
            rotations = Enumerable
                .Range(0, 4)
                .SelectMany(i => {
                    var rot1 = rotx.Power(i);

                    return Enumerable
                        .Range(0, 4)
                        .Select(i2 => roty.Power(i2))
                        .Union(new Matrix<double>[] { rotz.Power(1), rotz.Power(3) })
                        .Select(rotyz => rot1 * rotyz);
                }).ToList();
        }

        private uint BeaconDistance(double[] beaconA, double[] beaconB)
        {
            // If they're equal, this is zero
            if (beaconA == beaconB) return 0;

            return (uint)Math.Abs(beaconA[0] - beaconB[0]) + (uint)Math.Abs(beaconA[1] - beaconB[1]) + (uint)Math.Abs(beaconA[2] - beaconB[2]);
        }

        private (bool success, double[][] aligned_scanner, double[] scanner_pos) align(double[][] reference_scanner, double[][] scanner)
        {
            // Find if we have at least 12 beacons that have matching distances to 12 beacons in the reference scanner
            // This is (apparently) enough to determine we have overlap
            var matches = reference_scanner.SelectMany(beaconA =>
            {
                // First get all of the distances in reference_scanner (caches for later)
                var reference_distances = reference_scanner.Select(beaconA1 => BeaconDistance(beaconA, beaconA1)).ToArray();

                return scanner
                    .Where(beaconB => scanner.Select(beaconB1 => BeaconDistance(beaconB, beaconB1)).Intersect(reference_distances).Count() >= 12)
                    .Select(beaconB => (beaconA, beaconB));
            }).ToArray();

            // If we have no matches, don't continue
            if (matches.Length == 0)
                return (false, new double[][] { }, new double[] { });

            Matrix<double>? found_rotation = null;

            foreach(var rotation in this.rotations)
            {
                // Find the first rotation that works
                // We do this by finding all of matches.BeaconB @ rotation, then subtracting each BeaconA point
                // All of the values should be equal to each other
                var temp = matches.Select(pair => {
                    var rotatedB = Vector<double>.Build.Dense(pair.beaconB) * rotation;

                    return Vector<double>.Build.Dense(pair.beaconA) - rotatedB;
                }).Distinct().ToArray();

                // If we only have one (all are aligned and same distance)
                // we found our rotation
                if (temp.Length == 1)
                {
                    found_rotation = rotation;
                    break;
                }
            }

            // If we have no rotation, we have an error
            if (found_rotation == null)
                throw new Exception();

            // Figure out off offset position
            var scanner_pos = (Vector<double>.Build.Dense(matches[0].beaconA) - (Vector<double>.Build.Dense(matches[0].beaconB) * found_rotation));

            // Realign all of scanner's beacons to the new position/rotation
            var realigned_scanner = scanner.Select(beaconB => ((Vector<double>.Build.Dense(beaconB) * found_rotation) + scanner_pos).ToArray()).ToArray();

            return (true, realigned_scanner, scanner_pos.ToArray());
        }

        protected override string? SolvePartOne()
        {
            // Work through our list of scanners and continue to find matches/alignments
            // until none remain
            this.aligned_scanners = new List<double[][]>() { scanners[0] };

            // The list of other scanners to deal with
            var other_scanners = new Queue<double[][]>(scanners.Skip(1));

            // Tracking the scanner positions (index her matches the aligned_scanners index)
            this.scanner_positions = new List<double[]>() { new double[] { 0, 0, 0 } };

            while(other_scanners.Count > 0)
            {
                bool found = false;

                var scanner = other_scanners.Dequeue();

                // Go through all of the known scanners and try to align based on those
                foreach(var reference_scanner in this.aligned_scanners)
                {
                    var ret = align(reference_scanner, scanner);

                    if (ret.success)
                    {
                        // Found a match, add the aligned positions to the list
                        this.aligned_scanners.Add(ret.aligned_scanner);
                        this.scanner_positions.Add(ret.scanner_pos);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    // Re-add to the queue to process again later
                    other_scanners.Enqueue(scanner);
                }
            }

            // HashSet and Distinct() don't work on arrays
            // So we have to convert to tuples
            return this.aligned_scanners
                .SelectMany(scannerBeacons => scannerBeacons.Select(beacon => (beacon[0], beacon[1], beacon[2])))
                .Distinct()
                .Count()
                .ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Get the two furthest apart scanners
            return this.scanner_positions
                .Max(scannerA =>
                    this.scanner_positions.Max(scannerB => BeaconDistance(scannerA, scannerB))
                ).ToString();
        }
    }
}

