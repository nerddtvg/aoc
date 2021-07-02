using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2015
{

    class Day06 : ASolution
    {
        private Dictionary<(int x, int y), bool> grid = new Dictionary<(int x, int y), bool>();
        private Dictionary<(int x, int y), uint> grid2 = new Dictionary<(int x, int y), uint>();

        public Day06() : base(06, 2015, "")
        {
            // Initialize the lights
            InitializeGrid();
            
            foreach (var line in Input.SplitByNewline())
            {
                var parsed = ParseLine(line);

                if (parsed.HasValue)
                    switch(parsed.Value.action)
                    {
                        case "turn on":
                            TurnOn(parsed.Value.start, parsed.Value.end);
                            TurnOn2(parsed.Value.start, parsed.Value.end);
                            break;

                        case "turn off":
                            TurnOff(parsed.Value.start, parsed.Value.end);
                            TurnOff2(parsed.Value.start, parsed.Value.end);
                            break;

                        case "toggle":
                            Toggle(parsed.Value.start, parsed.Value.end);
                            Toggle2(parsed.Value.start, parsed.Value.end);
                            break;
                    }
            }
        }

        private void InitializeGrid()
        {
            grid.Clear();
            grid2.Clear();

            // Reset everything to off
            for (int x = 0; x <= 999; x++)
                for (int y = 0; y <= 999; y++)
                {
                    grid[(x, y)] = false;
                    grid2[(x, y)] = 0;
                }
        }

        private (string action, (int x, int y) start, (int x, int y) end)? ParseLine(string line)
        {
            var r = new Regex(@"^([a-z ]+) ([0-9]+),([0-9]+) through ([0-9]+),([0-9]+)$");

            var match = r.Match(line);
            
            if (!match.Success)
                return null;

            int x1, x2, y1, y2;

            x1 = Math.Min(Int32.Parse(match.Groups[2].Value), Int32.Parse(match.Groups[4].Value));
            x2 = Math.Max(Int32.Parse(match.Groups[2].Value), Int32.Parse(match.Groups[4].Value));
            
            y1 = Math.Min(Int32.Parse(match.Groups[3].Value), Int32.Parse(match.Groups[5].Value));
            y2 = Math.Max(Int32.Parse(match.Groups[3].Value), Int32.Parse(match.Groups[5].Value));

            return (match.Groups[1].Value, (x1, y1), (x2, y2));
        }

        private void TurnOn((int x, int y) start, (int x, int y) end) =>
            this.GetKeys(start, end).ForEach(a => this.grid[a] = true);

        private void TurnOff((int x, int y) start, (int x, int y) end) =>
            this.GetKeys(start, end).ForEach(a => this.grid[a] = false);

        private void Toggle((int x, int y) start, (int x, int y) end) =>
            this.GetKeys(start, end).ForEach(a => this.grid[a] = !this.grid[a]);

        private void TurnOn2((int x, int y) start, (int x, int y) end) =>
            this.GetKeys(start, end).ForEach(a => this.grid2[a]++);

        private void TurnOff2((int x, int y) start, (int x, int y) end) =>
            this.GetKeys(start, end).ForEach(a =>
            {
                if (this.grid2[a] > 0) this.grid2[a]--;
            });

        private void Toggle2((int x, int y) start, (int x, int y) end) =>
            this.GetKeys(start, end).ForEach(a => this.grid2[a] += 2);

        private List<(int x, int y)> GetKeys((int x, int y) start, (int x, int y) end)
        {
            var ret = new List<(int x, int y)>();

            for (var x = start.x; x <= end.x; x++)
                for (var y = start.y; y <= end.y; y++)
                    ret.Add((x, y));

            return ret;
        }

        protected override string SolvePartOne()
        {
            return this.grid.Count(a => a.Value).ToString();
        }

        protected override string SolvePartTwo()
        {
            return this.grid2.Sum(a => a.Value).ToString();
        }
    }
}
