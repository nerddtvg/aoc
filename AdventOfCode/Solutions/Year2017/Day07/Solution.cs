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
        }

        private List<Tower> towers = new List<Tower>();

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
                    }
                }
            }
        }

        protected override string? SolvePartOne()
        {
            ReadInput();

            return this.towers.First(tower => tower.parent == null).id;
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
