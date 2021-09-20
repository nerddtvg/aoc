using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

// This one was way above my paygrade so I went to the Megathread for help
// https://old.reddit.com/r/adventofcode/comments/3xspyl/day_22_solutions/
// Based some of the answer on /u/LincolnA's response:
// https://old.reddit.com/r/adventofcode/comments/3xspyl/day_22_solutions/cy7l1bq/

namespace AdventOfCode.Solutions.Year2015
{
    // Make this a value-type with struct
    struct Day22Spell
    {
        public string name { get; set; }
        public int cost { get; set; }
        public int damage { get; set; }
        public int heal { get; set; }
        public int armor { get; set; }
        public int mana { get; set; }
        public int duration { get; set; }
    }

    class Day22 : ASolution
    {
        private Day22Spell[] allSpells = new Day22Spell[5]
        {
            new Day22Spell
            {
                name = "Magic Missle",
                cost = 53,
                damage = 4,
                heal = 0,
                armor = 0,
                mana = 0,
                duration = 1
            },
            new Day22Spell
            {
                name = "Drain",
                cost = 73,
                damage = 2,
                heal = 2,
                armor = 0,
                mana = 0,
                duration = 1
            },
            new Day22Spell
            {
                name = "Shield",
                cost = 113,
                damage = 0,
                heal = 0,
                armor = 7,
                mana = 0,
                duration = 6
            },
            new Day22Spell
            {
                name = "Poison",
                cost = 173,
                damage = 3,
                heal = 0,
                armor = 0,
                mana = 0,
                duration = 6
            },
            new Day22Spell
            {
                name = "Recharge",
                cost = 229,
                damage = 0,
                heal = 0,
                armor = 0,
                mana = 101,
                duration = 5
            }
        };

        public int MinDepth = Int32.MaxValue;

        public Day22() : base(22, 2015, "")
        {

        }

        /// <summary>
        /// Play a turn in the game
        /// </summary>
        /// <param name="part">Puzzle part (1 or 2</param>
        /// <param name="myTurn"><c>true</c> if this is the player's turn; otherwise, <c>false</c></param>
        /// <param name="spent">How much mana the player has spent this far</param>
        /// <param name="hp">How many hit points the player has</param>
        /// <param name="mana">How much mana the player has</param>
        /// <param name="spells">Currently active spells</param>
        /// <param name="bossHp">How many hit points the boss has</param>
        /// <param name="bossDamage">How much damage one hit from the boss takes</param>
        private void playTurn(int part, bool myTurn, int spent, int hp, int mana, Day22Spell[] spells, int bossHp, int bossDamage)
        {
            // Check if we have breached a found depth already
            if (spent > MinDepth)
                return;

            // Applying effects
            mana = spells.Sum(s => s.mana) + mana;
            int damage = spells.Sum(s => s.damage);
            int armor = spells.Sum(s => s.armor);

            // Did the boss die?
            bossHp = bossHp - damage;

            if (bossHp <= 0)
            {
                // Boss died, check if we are a good new MinDepth
                MinDepth = Math.Min(MinDepth, spent);
                return;
            }

            // Decrement our spell durations
            for (int i = 0; i < spells.Length; i++)
            {
                spells[i].duration--;
            }

            // Filter out any expired spells
            var spellsList = spells.Where(s => s.duration > 0).ToList();

            if (myTurn)
            {
                int localHp = hp;

                // Find all spells we can cast (not currently in use and we can afford)
                var canCast = allSpells.Where(s => s.cost <= mana && !spells.Select(b => b.name).Contains(s.name)).ToList();

                // If we can't cast, we're dead
                if (canCast.Count == 0)
                    return;

                foreach(var spell in canCast)
                {
                    int castSpent = spent + spell.cost;
                    int castMana = mana - spell.cost;

                    // Figure out our next step based on this cast spell
                    int extraDamage = 0;
                    int castHeal = 0;
                    Day22Spell[] castSpells = spellsList.Append(spell).ToArray();
                    if (spell.duration == 1)
                    {
                        // This is instantaneous so let's calculate it now
                        extraDamage = spell.damage;
                        castHeal = spell.heal;
                        castSpells = spellsList.ToArray();
                    }

                    var castBossHp = bossHp - extraDamage;
                    if (castBossHp <= 0)
                    {
                        // Boss died on this hit
                        MinDepth = Math.Min(MinDepth, castSpent);
                        return;
                    }
                    else
                    {
                        // Move on to the next turn
                        playTurn(part, false, castSpent, localHp + castHeal, castMana, castSpells, castBossHp, bossDamage);
                    }
                }
            }
            else
            {
                // Boss turn
                int localBossDamage = Math.Max(bossDamage - armor, 1);
                int localHp = hp - localBossDamage;

                // Did we die?
                if (localHp <= 0)
                    return;

                // Play our turn next
                playTurn(part, true, spent, localHp, mana, spellsList.ToArray(), bossHp, bossDamage);
            }
        }

        protected override string SolvePartOne()
        {
            MinDepth = Int32.MaxValue;

            playTurn(1, true, 0, 50, 500, new Day22Spell[]{}, 51, 9);

            return MinDepth.ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
