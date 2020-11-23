using System;
using System.Collections.Generic;
using System.Text;

// Source: https://www.reddit.com/r/adventofcode/comments/e5u5fv/2019_day_4_solutions/f9mpyrj/
using System.Linq;

namespace AdventOfCode.Solutions.Year2019
{

    class Day04 : ASolution
    {
        private int start;
        private int end;

        public Day04() : base(04, 2019, "")
        {
            start = int.Parse(Input.Split('-')[0]);
            end = int.Parse(Input.Split('-')[1]);
        }

        protected override string SolvePartOne()
        {
            // part 1
            int part1 = Enumerable.Range(start, end - start)
                .Select(p => p.ToString())
                .Where(p => string.Join("", p.OrderBy(d => d)) == p // ascending order
                            && p.Distinct().Count() < p.Length)		// at least one duplicate
                .Count();
            
            return part1.ToString();
        }

        protected override string SolvePartTwo()
        {
                
            // part 2
            int part2 = Enumerable.Range(start, end - start)
                .Select(p => p.ToString())
                .Where(p => string.Join("", p.OrderBy(d => d)) == p    // ascending order
                            && p.GroupBy(d=>d).Any(g=>g.Count() == 2)) // two digit group
                .Count();
            
            return part2.ToString();
        }
    }
}
