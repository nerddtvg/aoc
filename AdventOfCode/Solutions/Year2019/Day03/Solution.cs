using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2019
{

    class Point {
        public int value { get; set; }
        public int[] steps { get; set; }
        public int dist { get; set; }
    }

    class Day03 : ASolution
    {
        private Dictionary<string, Point> grid;        

        public Day03() : base(03, 2019, "")
        {
            grid = new Dictionary<string, Point>();

            string[] lines = Input.SplitByNewline();

            MapLine(1, lines[0].Split(','));
            MapLine(2, lines[1].Split(','));
        }

        protected override string SolvePartOne()
        {
            int d = int.MaxValue;

            grid.Where(kvp => kvp.Value.value == 3).ToList().ForEach(kvp => {
                int x = int.Parse(kvp.Key.Split(';')[0]);
                int y = int.Parse(kvp.Key.Split(';')[1]);

                d = Math.Min(d, Math.Abs(x) + Math.Abs(y));
            });

            return d.ToString();
        }

        protected override string SolvePartTwo()
        {
            return grid.Where(kvp => kvp.Value.value == 3).Where(kvp => kvp.Value.dist == grid.Where(kvp => kvp.Value.value == 3).Min(kvp => kvp.Value.dist)).First().Value.dist.ToString();
        }

        private void MapLine(int id, string[] line) {
            int x = 0;
            int y = 0;
            int steps = 0;

            foreach(string l in line) {
                string dir = l.Substring(0, 1);
                int len = int.Parse(l.Substring(1));

                switch(dir) {
                    case "U":
                        for(int i=y+1; i <= y+len; i++) {
                            steps++;
                            setGridValue(id, x, i, steps);
                        }
                        
                        y += len;
                        
                        break;

                    case "D":
                        for(int i=y-1; i >= y-len; i--) {
                            steps++;
                            setGridValue(id, x, i, steps);
                        }
                        
                        y -= len;
                        
                        break;
                        
                    case "L":
                        for(int i=x-1; i >= x-len; i--) {
                            steps++;
                            setGridValue(id, i, y, steps);
                        }
                        
                        x -= len;
                        
                        break;
                        
                    case "R":
                        for(int i=x+1; i <= x+len; i++) {
                            steps++;
                            setGridValue(id, i, y, steps);
                        }
                        
                        x += len;

                        break;
                }
            }
        }

        private void setGridValue(int id, int x, int y, int steps) {
            string key = x.ToString() + ";" + y.ToString();

            if (grid.ContainsKey(key)) {
                if (grid[key].value != id) {
                    grid[key].value = 3;
                    grid[key].steps[id-1] = steps;
                    grid[key].dist = grid[key].steps[0] + grid[key].steps[1];
                }
            } else {
                grid[key] = new Point() { value = id };
                grid[key].steps = new int[2];
                grid[key].steps[id-1] = steps;
            }
        }
    }
}
