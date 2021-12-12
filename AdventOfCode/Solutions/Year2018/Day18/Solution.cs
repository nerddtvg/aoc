using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2018
{

    class Day18 : ASolution
    {
        public enum LumberState
        {
            None,
            Open,
            Tree,
            Lumber
        }

        private Dictionary<(int x, int y), LumberState> grid = new Dictionary<(int x, int y), LumberState>();

        public Day18() : base(18, 2018, "Settlers of The North Pole")
        {
            Reset();
        }

        private void Reset()
        {
            int y = 0;
            foreach(var line in Input.Trim().SplitByNewline())
            {
                int x = 0;
                foreach(var ch in line.ToCharArray())
                {
                    switch(ch)
                    {
                        case '.':
                            this.grid[(x++, y)] = LumberState.Open;
                            break;
                            
                        case '|':
                            this.grid[(x++, y)] = LumberState.Tree;
                            break;
                            
                        case '#':
                            this.grid[(x++, y)] = LumberState.Lumber;
                            break;
                    }
                }

                y++;
            }
        }

        public LumberState GetNewState((int x, int y) pt)
        {
            var thisPt = GetPoint(pt);
            var neigh = GetNeighborCount(pt);

            if (thisPt == LumberState.Open && neigh.tree >= 3)
                return LumberState.Tree;

            if (thisPt == LumberState.Tree && neigh.lumber >= 3)
                return LumberState.Lumber;

            if (thisPt == LumberState.Lumber)
            {
                if (neigh.lumber >= 1 && neigh.tree >= 1)
                    return LumberState.Lumber;

                return LumberState.Open;
            }

            return thisPt;
        }

        public (int open, int tree, int lumber) GetNeighborCount((int x, int y) pt)
        {
            var neigh = new List<LumberState>() {
                GetPoint((pt.x - 1, pt.y - 1)),
                GetPoint((pt.x, pt.y - 1)),
                GetPoint((pt.x + 1, pt.y - 1)),
                GetPoint((pt.x - 1, pt.y)),
                GetPoint((pt.x + 1, pt.y)),
                GetPoint((pt.x - 1, pt.y + 1)),
                GetPoint((pt.x, pt.y + 1)),
                GetPoint((pt.x + 1, pt.y + 1))
            };

            return (neigh.Count(n => n == LumberState.Open), neigh.Count(n => n == LumberState.Tree), neigh.Count(n => n == LumberState.Lumber));
        }

        public LumberState GetPoint((int x, int y)pt)
        {
            if (this.grid.ContainsKey(pt))
                return this.grid[pt];

            return LumberState.None;
        }

        private void RunRound()
        {
            var newGrid = new Dictionary<(int x, int y), LumberState>();

            foreach(var k in this.grid.Keys)
            {
                newGrid[k] = GetNewState(k);
            }

            this.grid = newGrid;
        }

        protected override string? SolvePartOne()
        {
            Utilities.Repeat(() => RunRound(), 10);

            return (this.grid.Count(pt => pt.Value == LumberState.Lumber) * this.grid.Count(pt => pt.Value == LumberState.Tree)).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
