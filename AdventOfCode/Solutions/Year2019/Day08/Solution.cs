using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2019
{

    class Day08 : ASolution
    {

        public int height { get; set; }
        public int width { get; set; }
        public List<List<int>> layers { get; set; }

        public Day08() : base(08, 2019, "")
        {
            this.height = 6;
            this.width = 25;
            this.layers = Layers();
        }

        public List<List<int>> Layers(string? input = null)
        {
            if (string.IsNullOrEmpty(input)) input = Input;

            int lineSize = (this.height * this.width);

            return Enumerable.Range(0, input.Length / lineSize).Select(i => input.Substring(i * lineSize, lineSize).ToIntArray().ToList()).ToList();
        }

        protected override string SolvePartOne()
        {
            int minCount = layers.Select(b => b.Where(c => c == 0).Count()).Min();

            List<int> l = layers.Where(a => a.Where(b => b == 0).Count() == minCount).First();

            return (l.Count(a => a == 1) * l.Count(b => b == 2)).ToString();
        }

        protected override string SolvePartTwo()
        {
            List<int> final = new List<int>();

            for (int k = 0; k < (this.width * this.height); k++)
            {
                final.Add(layers.Select(a => a[k]).Where(a => a != 2).First());
            }

            // Print it
            string output = "\n";
            for (int y = 0; y < this.height; y++)
            {
                for (int x = 0; x < this.width; x++)
                {
                    int pixel = final[x + (y * this.width)];

                    if (pixel == 1)
                    {
                        output += "▓";
                    }
                    else
                    {
                        output += " ";
                    }
                }

                // New line
                output += "\n";
            }

            return output;
        }
    }
}
