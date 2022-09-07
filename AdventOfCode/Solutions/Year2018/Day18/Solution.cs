using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


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

        private string GridToString()
        {
            // Grid size is 0,0 to 49,49
            return Enumerable.Range(0, 50).Select(y => Enumerable.Range(0, 50).Select(x => this.grid[(x, y)] == LumberState.Open ? '.' : (this.grid[(x, y)] == LumberState.Tree ? '|' : '#')).JoinAsString()).JoinAsString();
        }

        protected override string? SolvePartTwo()
        {
            // 1000000000: Brute force is too slow
            // There should be a repeating pattern, we just need to find it
            Reset();

            var states = new Dictionary<string, int>() { { GridToString(), 0 } };
            int first = 0;
            int count = 1;

            // Console.WriteLine($"{count.ToString("D5")}: {this.grid.Count(pt => pt.Value == LumberState.Open).ToString("D5")} {this.grid.Count(pt => pt.Value == LumberState.Tree).ToString("D5")} {this.grid.Count(pt => pt.Value == LumberState.Lumber).ToString("D5")}");
            for (count = 1; count < 100000; count++)
            {
                RunRound();
                // Console.WriteLine($"{count.ToString("D5")}: {this.grid.Count(pt => pt.Value == LumberState.Open).ToString("D5")} {this.grid.Count(pt => pt.Value == LumberState.Tree).ToString("D5")} {this.grid.Count(pt => pt.Value == LumberState.Lumber).ToString("D5")}");

                // Find if this existed before
                var key = GridToString();
                if (states.ContainsKey(key))
                {
                    first = states[key];
                    Console.WriteLine("Found Old State:");
                    Console.WriteLine($"First: {first}");
                    Console.WriteLine($"Second: {count}");

                    break;
                }

                states[key] = count;
            }

            // So now we have a first and second time something was found
            int difference = count - first;

            // Where do we land in this sequence?
            var mod = (1000000000 - first) % difference;

            // Then we "simply" need to find the state at that point
            var knownState = states.First(kvp => kvp.Value == first + mod).Key;

            // Count the '|' and '#'
            return (knownState.Count(ch => ch == '|') * knownState.Count(ch => ch == '#')).ToString();
        }
    }
}

