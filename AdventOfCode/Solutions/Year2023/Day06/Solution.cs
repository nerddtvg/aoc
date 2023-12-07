using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2023
{
    using Race = (ulong time, ulong distance);

    class Day06 : ASolution
    {
        public List<Race> races;
        public Race part2;

        public Day06() : base(06, 2023, "Wait For It")
        {
            var split = new Regex(" +");

            var tTime = split.Split(Input.SplitByNewline()[0]).Skip(1).Select(t => ulong.Parse(t)).ToArray();
            var tDistance = split.Split(Input.SplitByNewline()[1]).Skip(1).Select(t => ulong.Parse(t)).ToArray();

            // Combine them
            races = new();
            Enumerable.Range(0, tTime.Length).ForEach(idx => races.Add((tTime[idx], tDistance[idx])));

            var input = Input.Replace(" ", "").SplitByNewline();
            part2 = (ulong.Parse(input[0].Split(":")[1]), ulong.Parse(input[1].Split(":")[1]));
        }

        private ulong CountWins(Race race)
        {
            ulong winCounts = 0;

            for(ulong time = 1; time < race.time - 1; time++)
            {
                if ((race.time - time) * time > race.distance)
                    winCounts++;

                // Check if we have hit the other side of our arc
                else if (winCounts > 0)
                    break;
            }

            return winCounts;
        }

        protected override string? SolvePartOne()
        {
            return races.Aggregate((ulong)1, (agg, race) => agg * CountWins(race)).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return CountWins(part2).ToString();
        }
    }
}

