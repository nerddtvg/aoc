using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2021
{

    class Day11 : ASolution
    {
        public Dictionary<(int x, int y), uint> octos = new Dictionary<(int x, int y), uint>();

        public uint part1Flashes = 0;

        public Day11() : base(11, 2021, "Dumbo Octopus")
        {
//             DebugInput = @"5483143223
// 2745854711
// 5264556173
// 6141336146
// 6357385478
// 4167524645
// 2176841721
// 6882881134
// 4846848554
// 5283751526";

            Reset();
        }

        private void Reset()
        {
            this.octos.Clear();

            this.part1Flashes = 0;

            int y = 0;
            foreach(var line in Input.Trim().SplitByNewline())
            {
                int x = 0;
                foreach(uint c in line.ToIntArray())
                {
                    this.octos.Add((x, y), c);

                    x++;
                }
                y++;
            }
        }

        private void RunRound()
        {
            var newOctos = new Dictionary<(int x, int y), uint>();

            var keys = this.octos.Keys.ToList();

            // First, increase everyone's power by one
            // We go through and generate a new set of octos
            // to prevent overwriting a value we need to reference
            keys.ForEach(key => newOctos[key] = this.octos[key] + 1);

            // Cascading flashing
            // Start with a set of octos above 9
            var octoFlashed = new HashSet<(int x, int y)>();
            var newFlashed = 0;
            do
            {
                // Find anyone that flashed that we haven't tracked before
                var thisLoop = newOctos
                    .Where(kvp => kvp.Value > 9 && !octoFlashed.Contains(kvp.Key))
                    .Select(kvp => kvp.Key)
                    .ToList();

                // Continue the loop again
                newFlashed = thisLoop.Count;
                this.part1Flashes += (uint) thisLoop.Count;

                // Go through each neighbor and increase their value
                thisLoop.ForEach(pt =>
                {
                    octoFlashed.Add(pt);
                    IncreaseNeighbors(pt, ref newOctos);
                });
            } while (newFlashed > 0);

            // Reset any above 9 to 0
            keys.ForEach(key => newOctos[key] = newOctos[key] > 9 ? 0 : newOctos[key]);

            // Update the dictionary 
            this.octos = newOctos;
        }

        public void IncreaseNeighbors((int x, int y) pt, ref Dictionary<(int x, int y), uint> octos)
        {
            // Go through each of the x,y points and increase the neighbors
            for (int dy = -1; dy <= 1; dy++)
                for (int dx = -1; dx <= 1; dx++)
                {
                    var newPt = (pt.x + dx, pt.y + dy);
                    if (dx == 0 && dy == 0 || !octos.ContainsKey(newPt))
                        continue;

                    octos[newPt]++;
                }
        }

        private void WriteGrid()
        {
            // 10x10 grid
            for (int y = 0; y < 10; y++)
                Console.WriteLine(string.Join(" ", this.octos.Where(kvp => kvp.Key.y == y).OrderBy(kvp => kvp.Key.x).Select(kvp => kvp.Value.ToString("0"))));
                
            Console.WriteLine();
        }

        protected override string? SolvePartOne()
        {
            Utilities.Repeat(() =>{
                RunRound();
            }, 100);

            return this.part1Flashes.ToString();
        }

        protected override string? SolvePartTwo()
        {
            int i = 0;

            // Find the round where everything is zero
            for (i = 101; i < Int32.MaxValue; i++)
            {
                RunRound();

                // Using any here should prevent traversing the entire values list each time
                // Since Any should stop at the first true value
                if (!this.octos.Any(kvp => kvp.Value > 0))
                    break;
            }

            return i.ToString();
        }
    }
}

#nullable restore
