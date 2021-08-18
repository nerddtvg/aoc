using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2015
{

    class Day18 : ASolution
    {
        private Dictionary<(int x, int y), bool> _grid = new Dictionary<(int x, int y), bool>();

        private int minX = 0;
        private int minY = 0;
        private int maxX = 99;
        private int maxY = 99;

        public Day18() : base(18, 2015, "")
        {
            LoadInput();
        }

        private void LoadInput()
        {
            int y = 0;

            // Load the initial grid
            foreach(var line in Input.SplitByNewline())
            {
                for (var x = 0; x < line.Length; x++)
                {
                    this._grid[(x, y)] = line.Substring(x, 1) == "#";
                }

                y++;
            }
        }

        private bool GetNewState((int x, int y) pos, int part = 1)
        {
            // A light which is on stays on when 2 or 3 neighbors are on, and turns off otherwise.
            // A light which is off turns on if exactly 3 neighbors are on, and stays off otherwise.
            var neighbors = GetNeighbors(pos, part);
            var neighborsOn = neighbors.Count(a => a);

            // In part 2, all four corners are always on
            if (part == 2)
            {
                if ((pos.x == minX || pos.x == maxX) && (pos.y == minY || pos.y == maxY))
                {
                    return true;
                }
            }

            return
                (
                    this._grid[pos] && (neighborsOn == 2 || neighborsOn == 3)
                )
                ||
                (
                    !this._grid[pos] && neighborsOn == 3
                );
        }

        private List<bool> GetNeighbors((int x, int y) pos, int part = 1)
        {
            var ret = new List<bool>();
            for (var x = -1; x <= 1; x++)
                for (var y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;

                    // In part 2, all four corners are always on
                    if (part == 2)
                    {
                        if ((pos.x + x == minX || pos.x + x == maxX) && (pos.y + y == minY || pos.y + y == maxY))
                        {
                            ret.Add(true);
                            continue;
                        }
                    }

                    if (this._grid.ContainsKey((pos.x+x, pos.y+y)))
                        ret.Add(this._grid[(pos.x+x, pos.y+y)]);
                }

            return ret;
        }

        private Dictionary<(int x, int y), bool> CycleGrid(int part = 1)
        {
            var newGrid = new Dictionary<(int x, int y), bool>();

            for (int x = minX; x <= maxX; x++)
                for (int y = minY; y <= maxY; y++)
                {
                    newGrid[(x, y)] = GetNewState((x, y), part);
                }

            return newGrid;
        }

        protected override string SolvePartOne()
        {
            for (int step = 0; step < 100; step++)
            {
                this._grid = CycleGrid();
            }

            return this._grid.Count(a => a.Value).ToString();
        }

        protected override string SolvePartTwo()
        {
            // Reload our grid
            LoadInput();
            
            for (int step = 0; step < 100; step++)
            {
                this._grid = CycleGrid(2);
            }

            return this._grid.Count(a => a.Value).ToString();
        }
    }
}
