using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace AdventOfCode.Solutions
{
    class SolutionCollector(int year, int[] days) : IEnumerable<ASolution>
    {
        public readonly ASolution[] Solutions = [.. LoadSolutions(year, days)];

        public ASolution GetSolution(int day)
        {
            try
            {
                return Solutions.Single(s => s.Day == day);
            }
            catch(InvalidOperationException)
            {
                return default!;
            }
        }

        public IEnumerator<ASolution> GetEnumerator()
        {
            return ((IEnumerable<ASolution>)Solutions).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static IEnumerable<ASolution> LoadSolutions(int year, int[] days)
        {
            if(days.Sum() == 0)
            {
                // Starting in 2025, the number of puzzles changed to 12
                days = [.. Enumerable.Range(1, year < 2025 ? 25 : 12)];
            }

            foreach(int day in days)
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();

                var solution = Type.GetType($"AdventOfCode.Solutions.Year{year}.Day{day:D2}");
                if(solution != null && Activator.CreateInstance(solution) is ASolution solutionCast)
                {
                    // Tracking our initialization performance
                    stopWatch.Stop();
                    solutionCast.LoadTime = stopWatch.Elapsed;

                    yield return solutionCast;
                }
            }
        }
    }
}
