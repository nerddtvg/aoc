using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2015
{
    enum Day21ItemType
    {
        Weapon,
        Armor,
        Ring
    }

    class Day21Item
    {
        public string name { get; set; }
        public int cost { get; set; } = 0;
        public int damage { get; set; } = 0;
        public int armor { get; set; } = 0;

        public Day21Item(string name, int cost, int damage, int armor)
        {
            this.name = name;
            this.cost = cost;
            this.damage = damage;
            this.armor = armor;
        }
    }

    class Day21Combo
    {
        public List<Day21Item> items { get; set; } = new List<Day21Item>();
        public int cost { get; set; } = 0;

        public int damage => items.Sum(a => a.damage);
        public int armor => items.Sum(a => a.armor);
    }

    class Day21 : ASolution
    {
        private List<Day21Item> weapons = new List<Day21Item>()
        {
            new Day21Item("Dagger", 8, 4, 0),
            new Day21Item("Shortsword", 10, 5, 0),
            new Day21Item("Warhammer", 25, 6, 0),
            new Day21Item("Longsword", 40, 7, 0),
            new Day21Item("Greataxe", 74, 8, 0)
        };

        private List<Day21Item> armor = new List<Day21Item>()
        {
            new Day21Item("Leather", 13, 0, 1),
            new Day21Item("Chainmail", 31, 0, 2),
            new Day21Item("Splintmail", 53, 0, 3),
            new Day21Item("Bandedmail", 75, 0, 4),
            new Day21Item("Platemail", 102, 0, 5)
        };

        private List<Day21Item> rings = new List<Day21Item>()
        {
            new Day21Item("Damage +1", 25, 1, 0),
            new Day21Item("Damage +2", 50, 2, 0),
            new Day21Item("Damage +3", 100, 3, 0),
            new Day21Item("Defense +1", 20, 0, 1),
            new Day21Item("Defense +2", 40, 0, 2),
            new Day21Item("Defense +3", 80, 0, 3)
        };

        private List<Day21Combo> combos = new List<Day21Combo>();

        public Day21() : base(21, 2015, "")
        {

        }

        private void LoadCombos()
        {
            this.combos = new List<Day21Combo>();

            // We select:
            // 1 weapon
            // 0-1 armor
            // 0-2 rings

            // Selecting one weapon
            foreach(var thisWeapon in this.weapons)
            {
                // Make a list of possible armor (pad with empty)
                var armorList = this.armor.Append(default).ToList();

                foreach(var thisArmor in armorList)
                {
                    // Get a combination of no rings, one ring, and two rings
                    var ringList = this.rings.Select(a => new Day21Item[] { a })
                        .Union(this.rings.GetAllCombos(2).Select(a => a.ToArray()))
                        .Append(new Day21Item[] { })
                        .ToList();

                    foreach(var thisRing in ringList)
                    {
                        // We have a combination
                        var combo = new Day21Combo();

                        // Put them together
                        foreach(var item in thisRing.Select(a => a).Append(thisArmor).Append(thisWeapon))
                        {
                            if (item == default) continue;

                            combo.cost += item.cost;
                            combo.items.Add(item);
                        }

                        // Check that this is valid
                        // We have 100 hit points
                        // The input has: 104 HP, 8 damage, 1 armor
                        var playerMoves = MoveCount(100, combo.items.Sum(a => a.armor), 8);
                        var bossMoves = MoveCount(104, 1, combo.items.Sum(a => a.damage));

                        if (bossMoves > playerMoves)
                        {
                            continue;
                        }

                        // Add to our list
                        this.combos.Add(combo);
                    }
                }
            }
        }

        /// <summary>
        /// Since each attack is static, we can calculate how many moves this player gets before dying
        /// </summary>
        private int MoveCount(int defenderHitPoints, int defenderArmor, int attackerDamage)
        {
            return (int) Math.Round(((double)defenderHitPoints / Math.Max(attackerDamage - defenderArmor, 1)), MidpointRounding.ToPositiveInfinity);
        }

        protected override string SolvePartOne()
        {
            LoadCombos();
            return this.combos.OrderBy(a => a.cost).FirstOrDefault()?.cost.ToString() ?? string.Empty;
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
