using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{

    class Day08 : ASolution
    {
        int max_x = 49;
        int max_y = 5;

        public Dictionary<(int x, int y), bool> grid;

        public Day08() : base(08, 2016, "")
        {

        }

        private void ResetGrid()
        {
            this.grid = new Dictionary<(int x, int y), bool>();

            for (int x = 0; x <= max_x; x++)
                for (int y = 0; y <= max_y; y++)
                {
                    this.grid[(x, y)] = false;
                }
        }

        private void ProcessLine(string line)
        {
            var rotate_regex = new Regex(@"^rotate (row y|column x)=([0-9]+) by ([0-9]+)$");
            var rect_regex = new Regex(@"^rect ([0-9]+)x([0-9]+)$");

            var rotate = rotate_regex.Match(line);
            var rect = rect_regex.Match(line);

            if (rotate.Success)
            {
                int index = 0;
                (int x, int y) move = (0, 0);
                
                index = Int32.Parse(rotate.Groups[2].Value);

                var oldList = new Dictionary<(int x, int y), bool>();
                var newList = new Dictionary<(int x, int y), bool>();

                if (rotate.Groups[1].Value.Substring(0, 1) == "r")
                {
                    // Row
                    move.x = Int32.Parse(rotate.Groups[3].Value);

                    while(move.x > max_x)
                    {
                        move.x -= max_x;
                    }

                    this.grid.Where(g => g.Key.y == index).ToList().ForEach(kvp => oldList.Add(kvp.Key, kvp.Value));
                }
                else
                {
                    // Column
                    move.y = Int32.Parse(rotate.Groups[3].Value);

                    while(move.y > max_y)
                    {
                        move.y -= max_y;
                    }

                    this.grid.Where(g => g.Key.x == index).ToList().ForEach(kvp => oldList.Add(kvp.Key, kvp.Value));
                }

                // Now we get the new values
                foreach(var key in oldList.Keys)
                {
                    (int x, int y) k2 = key.Add(move);

                    k2.x = k2.x % (max_x + 1);
                    k2.y = k2.y % (max_y + 1);

                    newList[k2] = oldList[key];
                }

                // Change all of the values in grid
                foreach(var kvp in newList)
                {
                    this.grid[kvp.Key] = kvp.Value;
                }
            }

            if (rect.Success)
            {
                var x = Int32.Parse(rect.Groups[1].Value)-1;
                var y = Int32.Parse(rect.Groups[2].Value)-1;

                // Turn on everything from (0,0) to the width/height specified
                foreach(var key in (0,0).GetPointsBetweenInclusive((x, y)))
                {
                    this.grid[key] = true;
                }
            }
        }

        private void RunInput()
        {
            foreach(var line in Input.SplitByNewline(true))
                ProcessLine(line);
        }

        private void PrintGrid()
        {
            for (int y = 0; y <= max_y; y++)
            {
                for (int x = 0; x <= max_x; x++)
                {
                    Console.Write(this.grid[(x, y)] ? '#' : ' ');
                }
                Console.WriteLine();
            }
        }

        protected override string SolvePartOne()
        {
            ResetGrid();
            RunInput();
            return this.grid.Count(g => g.Value).ToString();
        }

        protected override string SolvePartTwo()
        {
            PrintGrid();
            return "See Console Output";
        }
    }
}
