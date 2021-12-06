using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2017
{

    class Day12 : ASolution
    {
        public class Program
        {
            public int id { get; set; } = 0;
            public List<int> talksWith = new List<int>();
        }

        private List<Program> programs = new List<Program>();

        private HashSet<int> foundPrograms = new HashSet<int>();

        public Day12() : base(12, 2017, "Digital Plumber")
        {

        }

        private void Reset()
        {
            this.programs.Clear();
            this.foundPrograms.Clear();

            foreach(var line in Input.SplitByNewline())
            {
                var parts = line.Split("<->", 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                if (parts.Length != 2)
                    throw new InvalidOperationException(line);

                var p1 = Int32.Parse(parts[0]);
                var p2 = parts[1].ToIntArray(",");

                // Create this list
                var p1Program = FindProgram(p1);
                p1Program.talksWith.AddRange(p2);

                foreach(var tP2 in p2)
                {
                    // Get the referenced programs and link them
                    var thisP2 = FindProgram(tP2);
                    thisP2.talksWith.Add(p1);
                }
            }
        }

        // First we can find/add a program
        private Program FindProgram(int id)
        {
            var program = this.programs.FirstOrDefault(p => p.id == id);

            if (program == default)
            {
                program = new Program() { id = id };
                this.programs.Add(program);
            }

            return program;
        }

        protected override string? SolvePartOne()
        {
            Reset();

            return FindGroup(0).ToString();
        }

        private int FindGroup(int baseId)
        {
            var found = new HashSet<int>();
            var thisRound = FindProgram(baseId).talksWith.ToList();

            do
            {
                var tempRound = new HashSet<int>();

                foreach (var id in thisRound)
                {
                    // Add the current round list
                    found.Add(id);

                    // Add to our global list
                    this.foundPrograms.Add(id);

                    // Now we need to check for all of the children this one talks to
                    FindProgram(id).talksWith.ForEach(pid => tempRound.Add(pid));
                }

                // Remove any we know
                thisRound = tempRound.Where(pid => !found.Contains(pid)).ToList();
            } while (thisRound.Count > 0);

            return found.Count;
        }

        protected override string? SolvePartTwo()
        {
            // Go through every id not found until its done
            var max = this.programs.Max(prog => prog.id);

            // We've already found one
            var foundCount = 1;

            for (int i = 1; i <= max; i++)
            {
                if (this.foundPrograms.Contains(i))
                    continue;

                FindGroup(i);
                foundCount++;
            }

            return foundCount.ToString();
        }
    }
}

#nullable restore
