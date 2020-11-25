using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2019
{
    class AsteroidPoint {
        public int x { get; set; }
        public int y { get; set; }
        public int asteroid { get; set; }
        public double angle { get; set; }
        public double distance { get; set; }

        public void calcAngle(AsteroidPoint source) {
            this.angle = GetAngle(source);

            if (source.x == this.x) {
                this.distance = Math.Abs(this.y - source.y);
            } else {
                this.distance = Math.Sqrt((Math.Pow((this.y - source.y) * 1.0, 2) + Math.Pow(this.x - source.x, 2)));
            }
        }

        public double GetAngle(AsteroidPoint source) {
            // Get the angle from the source point to this one
            // Need to convert to doubles before the slope calc
            double dy = (this.y - source.y) * 1.0;
            double dx = (this.x - source.x) * 1.0;

            // Convert to degrees and offset so 0 degrees is up
            // Swap the dy,dx parameters and rotate it
            double atan = Math.Atan2(dy, dx) * (180/Math.PI) + 180 + 270;

            if (atan >= 360)
                atan -= 360;

            return atan;
        }
    }

    class Day10 : ASolution
    {

        public List<AsteroidPoint> map { get; set; }

        public Day10() : base(10, 2019, "")
        {
            this.map = new List<AsteroidPoint>();

            int x = 0;
            int y = 0;

            Input.SplitByNewline().ToList().ForEach(r => {
                r.ToCharArray().ToList().ForEach(c => {
                    if (c == '#') {
                        this.map.Add(new AsteroidPoint() { x = x, y = y, asteroid = 1 });
                    }

                    x++;
                });

                y++;
                x = 0;
            });
        }

        protected override string SolvePartOne()
        {
            // For each asteroid, we need to find out how many it can see
            int x = 0;
            int y = 0;
            int max = 0;

            this.map.ForEach(r => {
                // Make sure this is an asteroid
                if (r.asteroid != 1) return;

                // Set the angles
                this.map.ForEach(b => b.calcAngle(r));

                // Get a list of all asteroids based on their slope/angle to our point
                int c = this.map.Where(t => (t.x != r.x || t.y != r.y) && t.asteroid == 1).Select(t => t.angle).Distinct().Count();

                if (c > max) {
                    max = c;
                    x = r.x;
                    y = r.y;
                }
            });

            return $"Max: {max.ToString()}, x: {x.ToString()}, y: {y.ToString()}";
        }

        protected override string SolvePartTwo()
        {
            // Get the starting point from part 1
            AsteroidPoint source = this.map.Where(t => t.x == 28 && t.y == 29).First();

            // Remove from the list
            //this.map.Remove(source);

            // Set the angles
            this.map.ForEach(b => b.calcAngle(source));

            // List of angles to process
            List<double> angles = this.map.Where(t => t.asteroid == 1).Select(t => t.angle).OrderBy(t => t).Distinct().ToList();

            // We need to rotate through and figure out what to do with our lives
            int vaporized = 0;
            int maxVaporized = 200;
            AsteroidPoint temp = null;

            do {
                int i = 0;

                do {
                    // Is there a point to remove?
                    var t = this.map.Where(t => (t.x != source.x || t.y != source.y) && t.angle == angles[i] && t.asteroid == 1);

                    if (t.Count() > 0) {
                        temp = t.OrderBy(d => d.distance).First();
                        temp.asteroid = 0;

                        vaporized++;
                    }

                    i++;
                } while (i < angles.Count && vaporized < maxVaporized);
            } while (vaporized < maxVaporized);

            return $"Value: {((temp.x * 100) + temp.y).ToString("0000")}, x: {temp.x.ToString()}, y: {temp.y.ToString()}";
        }
    }
}
