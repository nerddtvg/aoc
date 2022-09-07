using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

using System.Numerics;


namespace AdventOfCode.Solutions.Year2017
{

    class Day20 : ASolution
    {
        public class Particle
        {
            public List<long> position;
            public List<long> velocity;
            public List<long> acceleration;

            public int index;

            public Particle(string line, int index)
            {
                this.index = index;
                var matches = Regex.Match(line, "p=< *([\\-0-9]+), *([\\-0-9]+), *([\\-0-9]+)>, v=< *([\\-0-9]+), *([\\-0-9]+), *([\\-0-9]+)>, a=< *([\\-0-9]+), *([\\-0-9]+), *([\\-0-9]+)>");

                this.position = new List<long>()
                {
                    Int32.Parse(matches.Groups[1].Value),
                    Int32.Parse(matches.Groups[2].Value),
                    Int32.Parse(matches.Groups[3].Value)
                };

                this.velocity = new List<long>()
                {
                    Int32.Parse(matches.Groups[4].Value),
                    Int32.Parse(matches.Groups[5].Value),
                    Int32.Parse(matches.Groups[6].Value)
                };

                this.acceleration = new List<long>()
                {
                    Int32.Parse(matches.Groups[7].Value),
                    Int32.Parse(matches.Groups[8].Value),
                    Int32.Parse(matches.Groups[9].Value)
                };
            }

            public void Update()
            {
                this.velocity[0] += this.acceleration[0];
                this.velocity[1] += this.acceleration[1];
                this.velocity[2] += this.acceleration[2];
                
                this.position[0] += this.velocity[0];
                this.position[1] += this.velocity[1];
                this.position[2] += this.velocity[2];
            }

            public ulong DistanceToZero() => (ulong) Math.Abs(this.position[0]) + (ulong) Math.Abs(this.position[1]) + (ulong) Math.Abs(this.position[2]);
        }

        public List<Particle> particles = new List<Particle>();

        public Day20() : base(20, 2017, "Particle Swarm")
        {
            // DebugInput = "p=< 3,0,0>, v=< 2,0,0>, a=<-1,0,0>\np=< 4,0,0>, v=< 0,0,0>, a=<-2,0,0>";

            Reset();
        }

        private void Reset()
        {
            int index = 0;

            this.particles = Input.SplitByNewline().Select(p => new Particle(p, index++)).ToList();
        }

        protected override string? SolvePartOne()
        {
            // This takes about 8 seconds, not great

            // Run this ~100,000 times to see if we get a good answer
            Utilities.Repeat(() =>
            {
                this.particles.ForEach(p => p.Update());
            }, 100000);

            return this.particles.OrderBy(p => p.DistanceToZero()).First().index.ToString();
        }

        protected override string? SolvePartTwo()
        {
            Reset();

            // This takes about 12 seconds, not great

            // Run this ~100,000 times to see if we get a good answer
            Utilities.Repeat(() =>
            {
                // Short-circuit sort of
                if (this.particles.Count == 1) return;

                this.particles.ForEach(p => p.Update());

                var collided = this.particles
                    // Find which have matching positions
                    .GroupBy(p => (p.position[0], p.position[1], p.position[2]))
                    .Where(grp => grp.Count() > 1)
                    // Select all of those in those positions
                    .SelectMany(grp => grp.ToArray())
                    // Get the list
                    .ToList();

                // Remove them
                collided.ForEach(deadP => this.particles.Remove(deadP));
            }, 100000);

            return this.particles.Count.ToString();
        }
    }
}

