using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{

    class Day15 : ASolution
    {
        public class Disk
        {
            public uint id = 0;
            public uint modulo = 0;      // Number of positions
            public uint position = 0;

            // Get the position this disk is in at a specific time
            // Each disk is offset by 1, we use that to determine how much further the disk has moved
            public uint GetPosition(uint time) =>
                (time + id + position) % modulo;
        }

        public List<Disk> disks;

        public Day15() : base(15, 2016, "Timing is Everything")
        {
            // DebugInput = "Disc #1 has 5 positions; at time=0, it is at position 4.\nDisc #2 has 2 positions; at time=0, it is at position 1.";

            this.disks = Input.SplitByNewline()
                .Select(line =>
                {
                    var match = Regex.Match(line, "Disc #([0-9]+) has ([0-9]+) positions; at time=([0-9]+), it is at position ([0-9]+).");

                    return new Disk()
                    {
                        id = UInt32.Parse(match.Groups[1].Value),
                        modulo = UInt32.Parse(match.Groups[2].Value),
                        position = UInt32.Parse(match.Groups[4].Value)
                    };
                })
                .ToList();
        }

        protected override string SolvePartOne()
        {
            uint time = 0;

            for (time = 0; time < uint.MaxValue; time++)
            {
                // Originally brute forced this because I did != instead of ==
                // When it is ==, it's the Chinese Remainder Theorem
                if (this.disks.All(disk => disk.GetPosition(time) == 0))
                    break;
            }

            return time.ToString();
        }

        protected override string SolvePartTwo()
        {
            // No debug here
            if (!string.IsNullOrEmpty(DebugInput))
                return null;

            // Add a new disk
            this.disks.Add(new Disk()
            {
                id = (uint) this.disks.Count +1,
                position = 0,
                modulo = 11
            });

            uint time = 0;

            for (time = 0; time < uint.MaxValue; time++)
            {
                // Originally brute forced this because I did != instead of ==
                // When it is ==, it's the Chinese Remainder Theorem
                if (this.disks.All(disk => disk.GetPosition(time) == 0))
                    break;
            }

            return time.ToString();
        }
    }
}
