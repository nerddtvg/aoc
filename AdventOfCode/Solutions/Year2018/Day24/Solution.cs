using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Diagnostics;

namespace AdventOfCode.Solutions.Year2018
{

    class Day24 : ASolution
    {
        public Day24() : base(24, 2018, "Immune System Simulator 20XX")
        {
            var examples = new Dictionary<string, int>()
            {
                {
                    @"Immune System:
                    17 units each with 5390 hit points (weak to radiation, bludgeoning) with an attack that does 4507 fire damage at initiative 2
                    989 units each with 1274 hit points (immune to fire; weak to bludgeoning, slashing) with an attack that does 25 slashing damage at initiative 3

                    Infection:
                    801 units each with 4706 hit points (weak to radiation) with an attack that does 116 bludgeoning damage at initiative 1
                    4485 units each with 2961 hit points (immune to radiation; weak to fire, cold) with an attack that does 12 slashing damage at initiative 4",
                    5216
                }
            };

            // Check each example against the logic
            foreach(var example in examples)
            {
                var units = PlayGame(ReadGame(example.Key));
                Debug.Assert(Debug.Equals(units, example.Value), $"Expected: {example.Value}\nActual: {units}");
            }
        }

        protected override string? SolvePartOne()
        {
            var game = ReadGame(Input);
            return PlayGame(game).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }

        private Game ReadGame(string input)
        {
            var game = new Game();

            var groups = input.SplitByBlankLine(true);

            game.Immune = groups[0].Skip(1).Select(line => ReadGroup(line)).ToList();
            game.Infection = groups[1].Skip(1).Select(line => ReadGroup(line)).ToList();

            return game;
        }

        private Group ReadGroup(string line)
        {
            // Weakness and immunity may be in any order
            var pattern = new Regex(@"^(?<units>[0-9]+) units each with (?<hitpoints>[0-9]+) hit points (?<subquery>\(.+\) )?with .* attack that does (?<attackpower>[0-9]+) (?<attacktype>[a-z]+) damage at initiative (?<initiative>[0-9]+)$");

            if (!pattern.IsMatch(line))
                throw new Exception();

            var match = pattern.Match(line);

            // 1:1 matches that exist in each
            var group = new Group()
            {
                UnitCount = Int32.Parse(match.Groups["units"].Value),
                UnitHitPoints = Int32.Parse(match.Groups["hitpoints"].Value),
                AttackPower = Int32.Parse(match.Groups["attackpower"].Value),
                AttackType = Enum.Parse<AttackType>(match.Groups["attacktype"].Value, true),
                Initiative = Int32.Parse(match.Groups["initiative"].Value),
            };

            if (match.Groups.ContainsKey("subquery") && match.Groups["subquery"].Value.Length > 0)
            {
                var weaknesses = new Regex(@"weak to ([a-z, ]+)").Match(match.Groups["subquery"].Value);
                var immunity = new Regex(@"immune to ([a-z, ]+)").Match(match.Groups["subquery"].Value);

                if (weaknesses.Groups.Count > 1)
                    group.Weakness = weaknesses
                        .Groups[1]
                        .Value
                        .Split(',', options: StringSplitOptions.TrimEntries).Select(w => Enum.Parse<AttackType>(w, true)).ToArray();

                if (immunity.Groups.Count > 1)
                    group.Immune = immunity
                        .Groups[1]
                        .Value
                        .Split(',', options: StringSplitOptions.TrimEntries).Select(w => Enum.Parse<AttackType>(w, true)).ToArray();
            }

            return group;
        }

        /// <summary>
        /// Run through the given <paramref name="game" />
        /// </summary>
        private int PlayGame(Game game)
        {
            // We reuse these a lot
            var inProgress = true;
            var cImmune = () => game.Immune.Count(group => group.IsActive);
            var cInfection = () => game.Infection.Count(group => group.IsActive);

            do
            {
                // Get our targets
                var attacks = SelectTargets(game);

                // Order by initiative descending
                // Then attack
                foreach (var attack in attacks.OrderByDescending(attack => attack.Attacker.Initiative))
                {
                    AttackUnits(attack.Attacker, attack.Defender);

                    if (cImmune() == 0 || cInfection() == 0)
                    {
                        inProgress = false;
                        break;
                    }
                }
            } while (inProgress);

            var survivors = (cImmune() > 0 ? game.Immune : game.Infection).Where(group => group.IsActive).ToList();

            return survivors.Sum(s => s.UnitCount);
        }

        /// <summary>
        /// Handle the attacks and calculating new unit counts
        /// </summary>
        private void AttackUnits(Group Attacker, Group Defender)
        {
            // If this group is out of commission, do nothing
            if (!Attacker.IsActive || !Defender.IsActive)
                return;

            var attackPower = GetAttackPower(Attacker, Defender);

            // Immune, do nothing
            if (attackPower == 0) return;

            // Figure out how many _whole_ units this kills using integer division, no partial units
            var unitsKilled = attackPower / Defender.UnitHitPoints;

            // Remove those units from service
            Defender.UnitCount -= unitsKilled;
            Defender.UnitCount = Math.Max(Defender.UnitCount, 0);
        }

        /// <summary>
        /// Target selection for the entire field
        /// </summary>
        private (Group Attacker, Group Defender)[] SelectTargets(Game game)
        {
            // Go down both sets of groups and figure out who is attacking who on each side
            // If a group is picked, remove it from the list
            var ret = new List<(Group Attacker, Group Defender)>();

            // Find the targets of each
            ret.AddRange(SelectTargets(game.Infection.Where(group => group.IsActive).ToList(), game.Immune.Where(group => group.IsActive).ToList()));
            ret.AddRange(SelectTargets(game.Immune.Where(group => group.IsActive).ToList(), game.Infection.Where(group => group.IsActive).ToList()));

            return ret.ToArray();
        }

        /// <summary>
        /// Target selection for one set of attackers against one set of defenders
        /// </summary>
        private (Group Attacker, Group Defender)[] SelectTargets(List<Group> Attackers, List<Group> Defenders)
        {
            // Go down both sets of groups and figure out who is attacking who on each side
            // If a group is picked, remove it from the list
            var ret = new List<(Group Attacker, Group Defender)>();

            foreach(var attacker in Attackers)
            {
                var defender = ChooseDefender(attacker, Defenders);

                if (defender != default)
                {
                    // Remove from the Defender list so it isn't chosen again
                    Defenders.Remove(defender);

                    // Add to our return array
                    ret.Add((attacker, defender));
                }
            }

            return ret.ToArray();
        }

        /// <summary>
        /// Calculate the effective power of <paramref name="Attacker" /> against <paramref name="Defender" />
        /// </summary>
        private int GetAttackPower(Group Attacker, Group Defender)
        {
            int multiplier = 1;

            if (Defender.Weakness.Contains(Attacker.AttackType))
                multiplier = 2;
            else if (Defender.Immune.Contains(Attacker.AttackType))
                multiplier = 0;

            return multiplier * Attacker.EffectivePower;
        }

        /// <summary>
        /// Order and choose the appropriate defender to attack
        /// </summary>
        private Group? ChooseDefender(Group Attacker, List<Group> Defenders)
        {
            if (Defenders.Count == 0)
                return default;

            return Defenders
                // First calculate the damage possible for easier sorting
                .Select(defender => (defender, power: GetAttackPower(Attacker, defender)))
                // Find the highest effective power
                .OrderByDescending(group => group.power)
                // Tie breaker: Defender with highest Effective Power
                .ThenByDescending(group => group.defender.EffectivePower)
                // Tie breaker: Defender with highest Initiative
                .ThenByDescending(group => group.defender.Initiative)
                .Select(group => group.defender)
                .FirstOrDefault();
        }

        public enum AttackType
        {
            Default,
            Cold,
            Fire,
            Bludgeoning,
            Radiation,
            Slashing
        }

        public struct Game
        {
            /// <summary>
            /// A list of the infection groups
            /// </summary>
            public List<Group> Infection;

            /// <summary>
            /// A list of the immune groups
            /// </summary>
            public List<Group> Immune;
        }

        /// <summary>
        /// Our attack and defense groups
        /// </summary>
        public class Group
        {
            /// <summary>
            /// How many units in this group
            /// </summary>
            public int UnitCount { get; set; }

            /// <summary>
            /// How many hit points of each unit
            /// </summary>
            public int UnitHitPoints { get; set; }

            /// <summary>
            /// Is this unit still fighting?
            /// </summary>
            public bool IsActive => UnitCount > 0;

            /// <summary>
            /// Effective Power used in ordering
            /// </summary>
            public int EffectivePower => UnitCount * AttackPower;

            /// <summary>
            /// How powerful is the attack?
            /// </summary>
            public int AttackPower { get; set; }

            /// <summary>
            /// Tie breakers
            /// </summary>
            public int Initiative { get; set; }

            /// <summary>
            /// This group's attack type
            /// </summary>
            public AttackType AttackType { get; set; }

            /// <summary>
            /// This group's weaknesses
            /// </summary>
            public AttackType[] Weakness { get; set; } = Array.Empty<AttackType>();

            /// <summary>
            /// This group's immunities
            /// </summary>
            public AttackType[] Immune { get; set; } = Array.Empty<AttackType>();
        }
    }
}

