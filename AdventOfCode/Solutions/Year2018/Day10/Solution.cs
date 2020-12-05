using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2018
{
    class SkyLight {
        public int x {get;set;}
        public int y {get;set;}
        public int dx {get;set;}
        public int dy {get;set;}

        public void RunVelocity() {
            this.x += this.dx;
            this.y += this.dy;
        }
    }

    class Day10 : ASolution
    {
        List<SkyLight> points;

        public Day10() : base(10, 2018, "")
        {
            ParseInput();
        }

        private void ParseInput() {
            // Sample input:
            // position=< 43742,  11005> velocity=<-4, -1>
            points = new List<SkyLight>();
            foreach(string line in Input.SplitByNewline())
                points.Add(new SkyLight() {
                    x = Int32.Parse(line.Substring(10, 6).Trim()),
                    y = Int32.Parse(line.Substring(18, 6).Trim()),
                    dx = Int32.Parse(line.Substring(36, 2).Trim()),
                    dy = Int32.Parse(line.Substring(40, 2).Trim()),
                });
        }

        private void DrawSky() {
            int minX = points.Min(a => a.x);
            int maxX = points.Max(a => a.x);
            int minY = points.Min(a => a.y);
            int maxY = points.Max(a => a.y);

            // Only draw if this max-min X is <= 1000
            if (maxX-minX > 100) {
                // Print bounding box info
                Console.WriteLine($"{maxX-minX}, {maxY-minY}");
                return;
            }

            for(int y=minY; y<=maxY; y++) {
                for(int x=minX; x<=maxX; x++) {
                    if (points.Count(a => a.x == x && a.y == y) == 0)
                        Console.Write(".");
                    else
                        Console.Write("#");
                }

                Console.WriteLine();
            }
        }

        protected override string SolvePartOne()
        {
            // Need to run this a lot of times and see what comes out
            // This was run until 1,000,000 seconds and we only printed when the bounding box width was <= 100
            // End second was: 10886

            for(int c=0; c<10887; c++) {
                Console.WriteLine($"After {c} seconds:");
                DrawSky();

                points.ForEach(a => a.RunVelocity());
            }

            return null;
        }

        protected override string SolvePartTwo()
        {
            return 10886.ToString();
        }
    }
}
