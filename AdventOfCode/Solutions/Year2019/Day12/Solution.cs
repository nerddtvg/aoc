using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2019
{
    class Moon {
        public long x { get; set; }
        public long y { get; set; }
        public long z { get; set; }

        public long dx { get; set; }
        public long dy { get; set; }
        public long dz { get; set; }

        public void ApplyVelocity() {
            this.x += this.dx;
            this.y += this.dy;
            this.z += this.dz;
        }

        public void ApplyGravity(Moon moon) {
            if (moon.x > this.x) this.dx += 1;
            if (moon.x < this.x) this.dx -= 1;
            
            if (moon.y > this.y) this.dy += 1;
            if (moon.y < this.y) this.dy -= 1;
            
            if (moon.z > this.z) this.dz += 1;
            if (moon.z < this.z) this.dz -= 1;
        }

        public long CalcEnergy() {
            return (Math.Abs(this.x) + Math.Abs(this.y) + Math.Abs(this.z)) * (Math.Abs(this.dx) + Math.Abs(this.dy) + Math.Abs(this.dz));
        }

        public bool Equals(Moon obj)
        {
            return (this.x == obj.x && this.y == obj.y && this.z == obj.z && this.dx == obj.dx && this.dy == obj.dy && this.dz == obj.dz);
        }
    }

    class Day12 : ASolution
    {
        List<Moon> moons = new List<Moon>();
        List<Moon> initialMoons = new List<Moon>();

        public Day12() : base(12, 2019, "")
        {

        }

        private void WriteMoon(Moon m) {
            Console.WriteLine($"pos=<x={m.x.ToString(" 000;-000;   0")}, y={m.y.ToString(" 000;-000;   0")}, z={m.z.ToString(" 000;-000;   0")}> vel=<x={m.dx.ToString(" 000;-000;   0")}, y={m.dy.ToString(" 000;-000;   0")}, z={m.dz.ToString(" 000;-000;   0")}>");
        }

        private List<Moon> ParseMoons() {
            List<Moon> ret = new List<Moon>();

            Input.SplitByNewline().ToList().ForEach(m => {
                // Sample: <x=0, y=0, z=0>
                if (string.IsNullOrWhiteSpace(m.Trim())) return;

                int[] pos = m.Trim().Replace("<", "").Replace(">", "").Replace(" ", "").Replace("x=", "").Replace("y=", "").Replace("z=", "").Split(',').Select(p => int.Parse(p)).ToArray();
                ret.Add(new Moon() { x = pos[0], y = pos[1], z = pos[2], dx = 0, dy = 0, dz = 0});
            });

            return ret;
        }

        protected override string SolvePartOne()
        {
            this.moons = ParseMoons();

            this.moons.ForEach(m => WriteMoon(m));
            Console.WriteLine("");

            int step = 0;
            while(step < 1000) {
                this.moons.ForEach(m1 => {
                    this.moons.ForEach(m2 => {
                        // Skip this moon
                        if (m1.Equals(m2)) return;

                        // Apply gravity
                        m1.ApplyGravity(m2);
                    });
                });
                
                // Apply velocity
                this.moons.ForEach(m => m.ApplyVelocity());

                step++;

                if (step % 100 == 0) {
                    Console.WriteLine($"After {step} steps:");
                    this.moons.ForEach(m => WriteMoon(m));
                    Console.WriteLine("");
                }
            }

            // Get total energy
            return this.moons.Sum(m => m.CalcEnergy()).ToString();
        }

        protected override string SolvePartTwo()
        {
            this.moons = ParseMoons();
            this.initialMoons = ParseMoons();

            Console.WriteLine("-- Part 2 --");
            this.moons.ForEach(m => WriteMoon(m));
            Console.WriteLine("");

            // See how many it takes
            ulong[] steps = new ulong[3] {0,0,0};

            ulong step = 0;
            int step_determined = 0;
            bool loop = true;

            while(loop && step < 1000000000) {
                // Calculate X first
                int i = 0;

                step++;

                this.moons.ForEach(m1 => {
                    this.moons.ForEach(m2 => {
                        // Skip this moon
                        if (m1.Equals(m2)) return;

                        // Apply gravity
                        m1.ApplyGravity(m2);
                    });
                });
                
                // Apply velocity
                this.moons.ForEach(m => m.ApplyVelocity());
                
                bool matches;
                if (steps[0] == 0) {
                    matches = true;
                    // Check if we have hit paydirt
                    for(i = 0; i < this.moons.Count; i++)
                        if (this.moons[i].x != this.initialMoons[i].x || this.moons[i].dx != 0) {
                            matches = false;
                            break;
                        }
                    
                    if (matches) {
                        steps[0] = step;
                        step_determined++;
                    }
                }

                if (steps[1] == 0) {
                    matches = true;
                    // Check if we have hit paydirt
                    for(i = 0; i < this.moons.Count; i++)
                        if (this.moons[i].y != this.initialMoons[i].y || this.moons[i].dy != 0) {
                            matches = false;
                            break;
                        }
                    
                    if (matches) {
                        steps[1] = step;
                        step_determined++;
                    }
                }

                if (steps[2] == 0) {
                    matches = true;
                    // Check if we have hit paydirt
                    for(i = 0; i < this.moons.Count; i++)
                        if (this.moons[i].z != this.initialMoons[i].z || this.moons[i].dz != 0) {
                            matches = false;
                            break;
                        }
                    
                    if (matches) {
                        steps[2] = step;
                        step_determined++;
                    }
                }

                // Check if we have all steps
                loop = (step_determined != 3);
            }

            // We have how many itterations of each it takes to get back to the starting point
            Console.WriteLine($"Steps [step]:");
            steps.ToList().ForEach(s => Console.Write($"{s}, "));
            Console.WriteLine();
            
            return determineLCM(steps[0], determineLCM(steps[1], steps[2])).ToString();
        }

        private static ulong determineLCM(ulong a, ulong b)
        {
            ulong num1, num2;
            if (a > b)
            {
                num1 = a; num2 = b;
            }
            else
            {
                num1 = b; num2 = a;
            }

            for (ulong i = 1; i < num2; i++)
            {
                ulong mult = num1 * i;
                if (mult % num2 == 0)
                {
                    return mult;
                }
            }
            return num1 * num2;
        }
    }
}
