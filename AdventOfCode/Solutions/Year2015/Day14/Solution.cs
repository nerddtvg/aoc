using System;
using System.Collections.Generic;
using System.Text;

using System.Text.RegularExpressions;
using System.Linq;

namespace AdventOfCode.Solutions.Year2015
{

    class Day14 : ASolution
    {
        private Dictionary<string, (int speed, int flight_time, int rest_time, int cycle_time)> _reindeer = new Dictionary<string, (int speed, int flight_time, int rest_time, int cycle_time)>();

        public Day14() : base(14, 2015, "")
        {
            // Sample: Comet can fly 14 km/s for 10 seconds, but then must rest for 127 seconds.
            var matches = Regex.Matches(Input, "([a-z]+) can fly ([0-9]+) km/s for ([0-9]+) seconds, but then must rest for ([0-9]+) seconds.", RegexOptions.IgnoreCase);

            // Load the data
            foreach(Match match in matches)
            {
                this._reindeer[match.Groups[1].Value] = (Int32.Parse(match.Groups[2].Value), Int32.Parse(match.Groups[3].Value), Int32.Parse(match.Groups[4].Value), Int32.Parse(match.Groups[3].Value) + Int32.Parse(match.Groups[4].Value));
            }
        }

        private int GetReindeerDistance((int speed, int flight_time, int rest_time, int cycle_time) deer, int seconds)
        {
            int complete_cycles = deer.cycle_time > 0 ? seconds / deer.cycle_time : 0;
            int remainder = deer.cycle_time > 0 ? seconds % deer.cycle_time : 0;

            // Get the total complete_cycles * flight_time plus any additional flight_time the deer may have gone
            return (((complete_cycles * deer.flight_time) + (remainder > deer.flight_time ? deer.flight_time : remainder)) * deer.speed);
        }

        protected override string SolvePartOne()
        {
            // After exactly 2503 seconds, what distance has the winning reindeer traveled?
            // Need to determine how many cycles each reindeer has completed, then any extra
            int maxDistance = 0;
            int time = 2503;

            foreach(var deer in this._reindeer)
            {
                // Get how far this deer went
                int tDistance = GetReindeerDistance(deer.Value, time);

                maxDistance = Math.Max(maxDistance, tDistance);
            }

            return maxDistance.ToString();
        }

        protected override string SolvePartTwo()
        {
            var points = new Dictionary<string, int>();
            var deer_names = this._reindeer.Keys;

            // Preload the names
            foreach(var name in this._reindeer.Keys)
                points[name] = 0;

            // We should score for each deer for every second
            for (var i = 1; i <= 2503; i++)
            {
                int maxDistance = 0;
                string maxDeer = string.Empty;

                foreach(var deer in this._reindeer)
                {
                    // How far has this deer gone this time?
                    int tDistance = this.GetReindeerDistance(deer.Value, i);

                    maxDistance = Math.Max(tDistance, maxDistance);

                    // This deer is in the lead
                    if (maxDistance == tDistance)
                        maxDeer = deer.Key;
                }

                points[maxDeer]++;
            }

            int maxPoints = points.Max(a => a.Value);

            return maxPoints.ToString();
        }
    }
}
