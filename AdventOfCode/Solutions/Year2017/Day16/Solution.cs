using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2017
{

    class Day16 : ASolution
    {
        // A list of programs at what positions
        private Dictionary<char, int> programs = new Dictionary<char, int>();

        private int count = 16;

        public Day16() : base(16, 2017, "Permutation Promenade")
        {
            // This is another algorithm simplification problem
            // I doubt brute forcing this will be efficient

            // Debugging
            // count = 5;
            // DebugInput = "s1,x3/4,pe/b";

            foreach(var ch in Enumerable.Range(0, count))
            {
                programs.Add((char)('a' + ch), ch);
            }
        }

        private void Dance(string move)
        {
            switch(move[0])
            {
                case 's':
                    // "Spin" by moving everything forward X places
                    var offset = Int32.Parse(move.Substring(1));
                    foreach(var p in this.programs.Keys)
                    {
                        this.programs[p] = (this.programs[p] + offset) % count;
                    }
                    break;

                case 'x':
                    var slots = move.Substring(1).Split('/').Select(v => Int32.Parse(v)).ToArray();
                    // Get the ID of position A
                    var a = this.programs.Where(p => p.Value == slots[0]).Select(p => p.Key).First();
                    var b = this.programs.Where(p => p.Value == slots[1]).Select(p => p.Key).First();

                    // Now swap!
                    var t = this.programs[a];
                    this.programs[a] = this.programs[b];
                    this.programs[b] = t;
                    break;

                case 'p':
                    // Get the programs to move
                    var progs = move.Substring(1).Split('/', 2).Select(ch => ch.ToCharArray().First()).ToArray();

                    // Now swap!
                    var u = this.programs[progs[0]];
                    this.programs[progs[0]] = this.programs[progs[1]];
                    this.programs[progs[1]] = u;
                    break;
            }
        }

        protected override string? SolvePartOne()
        {
            // This took 0.0276 seconds
            foreach(var move in Input.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                Dance(move);
            }

            return this.programs.OrderBy(prog => prog.Value).Select(prog => prog.Key).JoinAsString();
        }

        protected override string? SolvePartTwo()
        {
            // We know where the digits ended up after round 1
            // We just need to extrapolate that to 1 billion times
            var changes = new Dictionary<int, int>();
            foreach(var ch in Enumerable.Range(0, count))
            {
                // Get the difference to the original position
                changes.Add(ch, this.programs[(char)('a' + ch)] - ch);
            }

            // For each entry that is below zero, bring it back up
            foreach(var low in changes.Where(ch => ch.Value < 0).Select(ch => ch.Key))
            {
                // Should only need to go up one increment
                changes[low] += count;
            }

            // Find the LCM for this so we don't have to loop *that* many times
            // Because we know the shift in changes of each digit, we know that for every LCM loops
            // the digits return to their 'home' place, so we only need to loop a few times
            var lcm = Utilities.FindLCM(changes.Where(ch => ch.Value > 0).Select(ch => (double) ch.Value).ToArray());

            // Then we only need to complete a reminder of (1 billion / lcm) moves (minus one for what has been done)
            var toComplete = (1000000000 % (int) lcm) - 1;

            // We can now loop things 1 billion times by skipping the dances
            // Minus one because we already did it
            Utilities.Repeat(() =>
            {
                foreach (var move in Input.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    Dance(move);
                }
            }, toComplete);

            // Still this is incredibly slow

            return this.programs.OrderBy(prog => prog.Value).Select(prog => prog.Key).JoinAsString();
        }
    }
}

#nullable restore
