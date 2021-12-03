using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2017
{

    class Day07 : ASolution
    {
        private class Tower
        {
            public string id { get; set; } = string.Empty;
            public int weight { get; set; } = 0;
            public Tower? parent { get; set; } = null;
            public List<Tower> children { get; set; } = new List<Tower>();
            public int depth { get; set; } = 0;
        }

        private List<Tower> towers = new List<Tower>();

        private Tower root = new Tower();

        public Day07() : base(07, 2017, "")
        {

        }

        private Tower AddTower(string id)
        {
            // First, find if this exists already
            var tower = this.towers.FirstOrDefault(t => t.id == id);

            if (tower == default)
            {
                tower = new Tower()
                {
                    id = id
                };

                this.towers.Add(tower);
            }

            return tower;
        }

        private void ReadInput()
        {
            this.towers.Clear();

            foreach(var line in Input.SplitByNewline(true))
            {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                // First, find if this exists already
                var tower = AddTower(parts[0]);

                // Set the weight
                tower.weight = Int32.Parse(parts[1].Replace("(", "").Replace(")", ""));

                // If we have children
                if (parts.Length > 2)
                {
                    // Part 2 is '->', we skip that
                    for (int i = 3; i < parts.Length; i++)
                    {
                        var child = AddTower(parts[i].Replace(",", ""));

                        // Set the parent
                        child.parent = tower;

                        // Set the child
                        tower.children.Add(child);
                    }
                }
            }

            // Set our root for reference
            var tRoot = this.towers.FirstOrDefault(tower => tower.parent == null);

            if (tRoot == default)
                throw new InvalidOperationException();

            this.root = tRoot;
        }

        // Get all of the weights of the above towers
        private int GetWeight(Tower tower) => tower.weight + tower.children.Sum(t => GetWeight(t));

        protected override string? SolvePartOne()
        {
            ReadInput();

            return this.root.id;
        }

        protected override string? SolvePartTwo()
        {
            // Find the weights of all children of Part1
            // Find the one that is not the same
            // Go up the tower until we find the one unbalanced level
            var currentNode = this.root;

            do
            {
                // Find the weights at this level
                var weights = currentNode.children.Select(child => (child, GetWeight(child)));

                // Find which one doesn't match
                var max = weights.Max(child => child.Item2);
                var maxNode = weights.Where(child => child.Item2 == max).FirstOrDefault();

                if (maxNode == default)
                    throw new InvalidOperationException();

                // We have a maxNode, if it's children doen't have children, this is our answer
                if (maxNode.child.children.Sum(c => c.children.Count) == 0)
                {
                    var min = weights.Min(child => child.Item2);

                    return (maxNode.child.weight - (max - min)).ToString();
                }

                // Otherwise, move up this node
                currentNode = maxNode.child;
            } while (currentNode != null);

            return string.Empty;
        }
    }
}

#nullable restore
