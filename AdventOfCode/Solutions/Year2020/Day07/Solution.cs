using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;
#nullable enable

namespace AdventOfCode.Solutions.Year2020
{
    class OuterBag
    {
        public string color { get; set; } = string.Empty;
        public List<InnerBag> inner { get; set; } = new();
    }

    class InnerBag
    {
        public string color { get; set; } = string.Empty;
        public int qty { get; set; }
    }

    class Day07 : ASolution
    {
        List<OuterBag> rules = new List<OuterBag>();

        public Day07() : base(07, 2020, "")
        {
            foreach (string line in Input.SplitByNewline())
            {
                string[] parts = line.Split("bags contain");
                string[] bags = parts[1].Replace(".", "").Trim().Split(",");

                OuterBag t = new OuterBag();
                t.color = parts[0].Replace(" bags", "").Trim();
                t.inner = new List<InnerBag>();

                foreach (string inner in bags)
                {
                    if (inner.Trim() == "no other bags") break;

                    // First character is a qty, the rest is a bag
                    string[] innerBag = inner.Trim().Split(" ", 2);

                    t.inner.Add(new InnerBag() { qty = Int32.Parse(innerBag[0]), color = innerBag[1].Replace(" bags", "").Replace(" bag", "").Trim() });
                }

                rules.Add(t);
            }
        }

        protected override string SolvePartOne()
        {
            // Find everything that contains a shiny gold bag
            List<string> bags = rules.Where(a => a.inner.Select(a => a.color).Contains("shiny gold")).Select(a => a.color).Distinct().ToList();

            while (true)
            {
                // Save this for comparison
                int beforeCount = bags.Count;

                // Now find everything that contains bags we just found
                List<string> tBags = rules.Where(a => a.inner.Select(a => a.color).Intersect(bags).Count() > 0).Select(a => a.color).ToList();

                // Add to our list
                bags.AddRange(tBags);
                bags = bags.Distinct().ToList();

                if (beforeCount == bags.Count) break;
            }

            return bags.Count.ToString();
        }

        private int bagCountIB(InnerBag bag)
        {
            // Calculate based on the ruleset
            OuterBag? ob = rules.Where(a => a.color == bag.color).FirstOrDefault();
            if (ob != null) return bag.qty * bagCount(ob);

            return bag.qty;
        }

        private int bagCount(OuterBag bag)
        {
            // Need to process this rule including ourselve
            int count = 1;

            // How many inner bags?
            foreach (var ib in bag.inner)
                count += bagCountIB(ib);

            return count;
        }

        protected override string SolvePartTwo()
        {
            // Work down the tree from shiny gold to determine how many bags it must contain
            // Remove one from the total because we can't count shiny gold
            return (bagCount(rules.Where(a => a.color == "shiny gold").First()) - 1).ToString();
        }
    }
}
