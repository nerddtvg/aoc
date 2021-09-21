using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{
    class Day04Room
    {
        public string name { get; set; }
        public int sector { get; set; }
        public string checksum { get; set; }
    }

    class Day04 : ASolution
    {
        private List<Day04Room> rooms = new List<Day04Room>();

        public Day04() : base(04, 2016, "")
        {
            // Parse each room first
            var regex = new Regex(@"^([a-z0-9\-]+)-([0-9]+)\[([a-z]+)\]$");

            foreach(var line in Input.SplitByNewline())
            {
                var match = regex.Match(line);

                if (match.Success)
                {
                    rooms.Add(new Day04Room()
                    {
                        name = match.Groups[1].Value,
                        sector = Int32.Parse(match.Groups[2].Value),
                        checksum = match.Groups[3].Value
                    });
                }
            }
        }

        private bool IsValidRoom(Day04Room room)
        {
            return room.checksum.Equals(
                room
                    .name
                    .Where(c => c != '-')
                    .GroupBy(c => c)
                    .OrderByDescending(c => c.Count())
                    .ThenBy(c => c.Key)
                    .Take(5)
                    .Select(c => c.Key)
                    .JoinAsString()
            );
        }

        protected override string SolvePartOne()
        {
            return this.rooms.Where(r => IsValidRoom(r)).Sum(r => r.sector).ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
