using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

using System.Linq;

namespace AdventOfCode.Solutions.Year2015
{

    class Day24 : ASolution
    {
        private List<int> packageWeights { get; set; } = new();

        public Day24() : base(24, 2015, "")
        {

        }

        private void LoadPackages()
        {
            this.packageWeights = new List<int>();

            foreach(var line in Input.SplitByNewline())
            {
                this.packageWeights.Add(int.Parse(line));
            }

            this.packageWeights = this.packageWeights.OrderByDescending(a => a).ToList();
        }

        // I realized we don't care about groups 2 and 3, we only need the smallest possible combination that makes 1/3rd the total weight
        private BigInteger MinQES(int groupCount = 3)
        {
            // The weight has to be 1/3rd of the total for each group
            var weight = (int) (this.packageWeights.Sum() / groupCount);

            BigInteger minQES = BigInteger.Zero;

            for (int i = 2; i < this.packageWeights.Count; i++)
            {
                foreach (var perm in this.packageWeights.GetAllCombos(i).Where(a => a.Sum() == weight))
                {
                    // Valid combo
                    var tempQES = perm.Select(i => new BigInteger(i)).Aggregate((x, y) => x * y);

                    // Set our known minimum
                    minQES = minQES == BigInteger.Zero ? tempQES : BigInteger.Min(minQES, tempQES);

                    // Break early:
                    // While we can check every combination, we're making
                    // an assumption (based on others' experience) that
                    // the first combo is our answer
                    return minQES;
                }
            }

            return minQES;
        }

        protected override string SolvePartOne()
        {
            LoadPackages();

            var sd = DateTime.Now;
            System.Console.WriteLine("Part 1:");
            System.Console.WriteLine($"Start: {DateTime.Now}");

            var ret = MinQES();

            System.Console.WriteLine($"End: {DateTime.Now}");
            System.Console.WriteLine($"Time Spent: {DateTime.Now-sd}");

            return ret.ToString();
        }

        protected override string SolvePartTwo()
        {
            LoadPackages();

            var sd = DateTime.Now;
            System.Console.WriteLine("Part 2:");
            System.Console.WriteLine($"Start: {DateTime.Now}");

            var ret = MinQES(4);

            System.Console.WriteLine($"End: {DateTime.Now}");
            System.Console.WriteLine($"Time Spent: {DateTime.Now-sd}");

            return ret.ToString();
        }
    }
}
