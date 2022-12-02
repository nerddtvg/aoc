using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2018
{

    class Day15 : ASolution
    {
        Dictionary<(int x, int y), TileType> grid = new();
        List<Unit> units = new();

        public Day15() : base(15, 2018, "Beverage Bandits")
        {
            ResetGrid();
        }

        private void ResetGrid()
        {
            int y = 0;
            int x = 0;

            grid = new();
            units = new();

            foreach(var line in Input.SplitByNewline())
            {
                x = 0;

                foreach(var c in line)
                {
                    if (c == '#')
                    {
                        grid[(x, y)] = TileType.Wall;
                    }
                    else
                    {
                        grid[(x, y)] = TileType.Open;

                        if (c == 'E' || c == 'G')
                        {
                            // New Elf or Goblin
                            units.Add(new Unit()
                            {
                                Type = c == 'E' ? UnitType.Elf : UnitType.Goblin,
                                x = x,
                                y = y
                            });
                        }
                    }

                    x++;
                }

                y++;
            }
        }

        protected override string? SolvePartOne()
        {
            var outcome = PlayGame();
            return outcome.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }

        private int PlayGame()
        {
            // Determine the units to play and the order
            var playableUnits = GetUnits();

            // Protection against endless loops
            int i = 0;
            int maxLoops = 10;

            // Play until there is only one unit type left
            while (this.units.Where(unit => unit.isAlive).Select(unit => unit.Type).Distinct().Count() == 2 && i++ < maxLoops)
            {
                foreach (var playUnit in playableUnits)
                {
                    PlayUnit(playUnit);
                }
            }

            // Return the Outcome = round count * HP remaining
            return i * units.Where(unit => unit.isAlive).Sum(unit => unit.HP);
        }

        private void PlayUnit(Unit unit)
        {
            // First double check if we're dead, in case we were killed mid-round
            if (!unit.isAlive) return;

            // Get units we can play
            var enemies = GetUnits(default, unit.Type == UnitType.Elf ? UnitType.Goblin : UnitType.Elf);

            // Anyone in range?
            var inRange = InRange(unit, enemies);

            if (inRange.Count == 0)
            {
                // Need to move
            }

            if (inRange.Count > 0)
            {
                // Need to fire
                // the adjacent target with the fewest hit points is selected; in a tie,
                // the adjacent target with the fewest hit points which is first in reading order is selected.
                var lowestHP = inRange.Min(unit => unit.HP);
                var target = GetUnits(inRange.Where(unit => unit.HP == lowestHP)).FirstOrDefault();

                // If we have a target, we attack!
                if (target != default)
                {
                    target.HP -= unit.AttackPower;
                    target.isAlive = (target.HP > 0);
                }
            }
        }

        /// <summary>
        /// Get all units in range already
        /// </summary>
        private List<Unit> InRange(Unit thisUnit, IEnumerable<Unit> units)
        {
            var retList = new List<Unit>();
            
            if (units.Any(unit => unit.x == thisUnit.x && unit.y == thisUnit.y - 1))
                retList.Add(units.First(unit => unit.x == thisUnit.x && unit.y == thisUnit.y - 1));

            if (units.Any(unit => unit.x == thisUnit.x && unit.y == thisUnit.y + 1))
                retList.Add(units.First(unit => unit.x == thisUnit.x && unit.y == thisUnit.y + 1));

            if (units.Any(unit => unit.x == thisUnit.x - 1 && unit.y == thisUnit.y))
                retList.Add(units.First(unit => unit.x == thisUnit.x - 1 && unit.y == thisUnit.y));

            if (units.Any(unit => unit.x == thisUnit.x + 1 && unit.y == thisUnit.y))
                retList.Add(units.First(unit => unit.x == thisUnit.x + 1 && unit.y == thisUnit.y));

            return retList;
        }

        /// <summary>
        /// Get a list of units in order
        /// </summary>
        /// <param name="units">A given list of units to process. Default: <see cref="Day15.units" /></param>
        /// <param name="type">A given type of unit to return. Defualt: <c>null</c></param>
        private List<Unit> GetUnits(IEnumerable<Unit>? units = default, UnitType? type = null)
        {
            return (units ?? this.units)
                // Start with alive
                .Where(unit => unit.isAlive)
                // Specify a type
                .Where(unit => type.HasValue ? unit.Type == type.Value : true)
                // Proper reading order
                .OrderBy(unit => unit.y)
                .ThenBy(unit => unit.x)
                .ToList();
        }

        enum UnitType
        {
            Elf,
            Goblin
        }

        enum TileType
        {
            Open,
            Wall,
            Elf,
            Goblin
        }

        /// <summary>
        /// Playable unit
        /// </summary>
        class Unit
        {
            /// <summary>
            /// Elf or Goblin
            /// </summary>
            public UnitType Type { get; set; }

            /// <summary>
            /// Pos X
            /// </summary>
            public int x { get; set; }

            /// <summary>
            /// Pos Y
            /// </summary>
            public int y { get; set; }

            /// <summary>
            /// Is this unit alive?
            /// </summary>
            public bool isAlive { get; set; } = true;

            /// <summary>
            /// Attack Power: 3
            /// </summary>
            public int AttackPower { get; set; } = 3;

            /// <summary>
            /// Hit Points: Starts at 200
            /// </summary>
            public int HP { get; set; } = 200;

            public override string ToString() => string.Format("({0,2:D},{1,2:D}) [{2,2:D}] {3}", x, y, HP, Type.ToString());
        }
    }
}

