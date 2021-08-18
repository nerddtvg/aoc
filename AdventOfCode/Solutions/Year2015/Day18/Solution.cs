using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2015
{

    class Day18 : ASolution
    {
        private Dictionary<(int x, int y), bool> _grid = new Dictionary<(int x, int y), bool>();

        public Day18() : base(18, 2015, "")
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

        private bool GetNewState((int x, int y) pos)
        {
            // A light which is on stays on when 2 or 3 neighbors are on, and turns off otherwise.
            // A light which is off turns on if exactly 3 neighbors are on, and stays off otherwise.
            var neighbors = GetNeighbors(pos);
            var neighborsOn = neighbors.Count(a => a);

            return
                (
                    this._grid[pos] && (neighborsOn == 2 || neighborsOn == 3)
                )
                ||
                (
                    !this._grid[pos] && neighborsOn == 3
                );
        }

        private List<bool> GetNeighbors((int x, int y) pos)
        {
            var ret = new List<bool>();
            for (var x = -1; x <= 1; x++)
                for (var y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    if (this._grid.ContainsKey((pos.x+x, pos.y+y)))
                        ret.Add(this._grid[(pos.x+x, pos.y+y)]);
                }

            return ret;
        }

        private Dictionary<(int x, int y), bool> CycleGrid()
        {
            var newGrid = new Dictionary<(int x, int y), bool>();

            for (int x = 0; x < 100; x++)
                for (int y = 0; y < 100; y++)
                {
                    newGrid[(x, y)] = GetNewState((x, y));
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
            return null;
        }
    }
}
