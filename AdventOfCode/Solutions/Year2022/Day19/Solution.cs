using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2022
{

    class Day19 : ASolution
    {
        public enum BlueprintType
        {
            ore,
            clay,
            obsidian,
            geode
        }
        /// <summary>
        /// The definition of a blueprint
        /// </summary>
        public struct Blueprint
        {
            /// <summary>
            /// Blueprint ID
            /// </summary>
            public int id;

            /// <summary>
            /// How much does each ore robot cost?
            /// </summary>
            public (int ore, int clay, int obsidian) costsOre;

            /// <summary>
            /// How much does each clay robot cost?
            /// </summary>
            public (int ore, int clay, int obsidian) costsClay;

            /// <summary>
            /// How much does each obsidian robot cost?
            /// </summary>
            public (int ore, int clay, int obsidian) costsObsidian;

            /// <summary>
            /// How much does each geode robot cost?
            /// </summary>
            public (int ore, int clay, int obsidian) costsGeode;

            /// <summary>
            /// How many of each robot do we have?
            /// </summary>
            public (int ore, int clay, int obsidian) bots;

            /// <summary>
            /// How many of each resource do we have?
            /// </summary>
            public (int ore, int clay, int obsidian, int geode) resources;

            /// <summary>
            /// Maximum desired values (allowing us to produce one of each robot each turn)
            /// </summary>
            public (int ore, int clay, int obsidian) maximums;

            /// <summary>
            /// Provides a simple hash to prevent duplicate efforts
            /// </summary>
            public string GetIdString(int minute)
            {
                return $"{id}-{bots.ore}-{bots.clay}-{bots.obsidian}-{resources.ore}-{resources.clay}-{resources.obsidian}-{resources.geode}-{minute}";
            }
        }

        private Blueprint parseBlueprint(string input)
        {
            var pattern = new Regex(@"Blueprint (?<id>[0-9]+): Each ore robot costs (?<ore>[0-9]+) ore. Each clay robot costs (?<clay>[0-9]+) ore. Each obsidian robot costs (?<obsore>[0-9]+) ore and (?<obsclay>[0-9]+) clay. Each geode robot costs (?<geodeore>[0-9]+) ore and (?<geodeobs>[0-9]+) obsidian.");

            var match = pattern.Match(input);

            if (!match.Success)
                throw new Exception();

            Blueprint blueprint = new()
            {
                id = Convert.ToInt32(match.Groups["id"].Value),
                costsOre = (ore: Convert.ToInt32(match.Groups["ore"].Value), 0, 0),
                costsClay = (ore: Convert.ToInt32(match.Groups["clay"].Value), 0, 0),
                costsObsidian = (ore: Convert.ToInt32(match.Groups["obsore"].Value), clay: Convert.ToInt32(match.Groups["obsclay"].Value), 0),
                costsGeode = (ore: Convert.ToInt32(match.Groups["geodeore"].Value), clay: 0, obsidian: Convert.ToInt32(match.Groups["geodeobs"].Value)),

                // We always start with one ore bot
                bots = (1, 0, 0),

                // And no resources
                resources = (0, 0, 0, 0)
            };

            // Determine our ultimate maximums for each bot
            // Another shortcut to prevent us from making dozens of ore when that's useless
            blueprint.maximums = (ore: Math.Max(Math.Max(blueprint.costsOre.ore, blueprint.costsObsidian.ore), Math.Max(blueprint.costsGeode.ore, blueprint.costsClay.ore)), clay: blueprint.costsObsidian.clay, obsidian: blueprint.costsGeode.obsidian);

            return blueprint;
        }

        /// <summary>
        /// Holds all of our input blueprints
        /// </summary>
        public List<Blueprint> blueprints = new();

        /// <summary>
        /// Holds each blueprint's maximum output
        /// </summary>
        public Dictionary<int, int> blueprintMax = new();

        public Day19() : base(19, 2022, "Not Enough Minerals")
        {
            // DebugInput = @"Blueprint 1: Each ore robot costs 4 ore. Each clay robot costs 2 ore. Each obsidian robot costs 3 ore and 14 clay. Each geode robot costs 2 ore and 7 obsidian.
            // Blueprint 2: Each ore robot costs 2 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 8 clay. Each geode robot costs 3 ore and 12 obsidian.";

            ReadBlueprints(Input);
        }

        private void ReadBlueprints(string input)
        {
            blueprints = input.SplitByNewline(true).Select(b => parseBlueprint(b)).ToList();
        }

        /// <summary>
        /// Find the maximum for each blueprint within the allowed timeframe
        /// </summary>
        /// <param name="maxMinute">Maximum number of minutes to run</param>
        private void FindMaximum(int maxMinute)
        {
            // Hold our current states
            Queue<(Blueprint blueprint, int minute)> queue = new();

            // Don't duplicate tests
            HashSet<string> seen = new();

            // First thing we do is load up all of the blueprints into the queue
            blueprints.ForEach(b => queue.Enqueue((b, 1)));
            blueprints.ForEach(b => blueprintMax[b.id] = 0);

            while (queue.Count > 0)
            {
                (var blueprint, var minute) = queue.Dequeue();

                if (minute >= maxMinute)
                    continue;

                // Skip this blueprint if we have been here before
                var hash = blueprint.GetIdString(minute);
                if (seen.Contains(hash))
                    continue;

                seen.Add(hash);

                // /u/yossi_peti made an interesting comment:
                // Instead of running the search minute by minute, I searched over the options of "what type of robot
                // should I build next?" This allows you to reduce the search space by jumping over multiple minutes at once.

                // If we look at the robots to build, we can skip minutes at a time
                // Do this by looking at each bot and determining:
                // a. Can I build it now? If yes, do that and enqueue it
                // b. How long until I can build it? Enqueue that time plus resources

                // Determine how long until we can build a geode bot
                // Skip ahead and do that
                // Otherwise, if we can build it now, the timeCost will be zero or less
                Blueprint newBlueprint;
                int timeCost = 0;

                // Order of operations:
                // We start at the top of the minute
                // Determine what/if we can build
                // Increase timeCost by one for all actions
                // If timeCost is under zero, we can build now so set that to zero
                // * This prevents rollback in time and bad resource counts
                // Increase resources by timeCost
                // * Because we need to account for the minute we are currently in
                //   so that when we start the next minute, the resources are correct
                // Increase the bot count after the resources step
                // Enqueue for the next run (minute + timeCost) to get to the next designated minute

                newBlueprint = blueprint.Clone();

                timeCost = 1 + new int[] { 0, (int)Math.Ceiling((newBlueprint.costsOre.ore - newBlueprint.resources.ore) / (double)newBlueprint.bots.ore) }.Max();
                if (timeCost + minute <= maxMinute)
                {
                    newBlueprint.resources.ore -= newBlueprint.costsOre.ore;
                    newBlueprint.resources.clay -= newBlueprint.costsOre.clay;
                    newBlueprint.resources.obsidian -= newBlueprint.costsOre.obsidian;

                    newBlueprint.resources.ore += newBlueprint.bots.ore * timeCost;
                    newBlueprint.resources.clay += newBlueprint.bots.clay * timeCost;
                    newBlueprint.resources.obsidian += newBlueprint.bots.obsidian * timeCost;

                    newBlueprint.bots.ore++;

                    queue.Enqueue((newBlueprint, minute + timeCost));
                }

                newBlueprint = blueprint.Clone();

                timeCost = 1 + new int[] { 0, (int)Math.Ceiling((newBlueprint.costsClay.ore - newBlueprint.resources.ore) / (double)newBlueprint.bots.ore) }.Max();
                if (timeCost + minute <= maxMinute)
                {
                    newBlueprint.resources.ore -= newBlueprint.costsClay.ore;
                    newBlueprint.resources.clay -= newBlueprint.costsClay.clay;
                    newBlueprint.resources.obsidian -= newBlueprint.costsClay.obsidian;

                    newBlueprint.resources.ore += newBlueprint.bots.ore * timeCost;
                    newBlueprint.resources.clay += newBlueprint.bots.clay * timeCost;
                    newBlueprint.resources.obsidian += newBlueprint.bots.obsidian * timeCost;

                    newBlueprint.bots.clay++;

                    queue.Enqueue((newBlueprint, minute + timeCost));
                }

                // Only look at this if we have a clay bot
                if (blueprint.bots.clay > 0)
                {
                    newBlueprint = blueprint.Clone();

                    timeCost = 1 + new int[] { 0, (int)Math.Ceiling((newBlueprint.costsObsidian.ore - newBlueprint.resources.ore) / (double)newBlueprint.bots.ore), (int)Math.Ceiling((newBlueprint.costsObsidian.clay - newBlueprint.resources.clay) / (double)newBlueprint.bots.clay) }.Max();
                    if (timeCost + minute <= maxMinute)
                    {
                        newBlueprint.resources.ore -= newBlueprint.costsObsidian.ore;
                        newBlueprint.resources.clay -= newBlueprint.costsObsidian.clay;
                        newBlueprint.resources.obsidian -= newBlueprint.costsObsidian.obsidian;

                        newBlueprint.resources.ore += newBlueprint.bots.ore * timeCost;
                        newBlueprint.resources.clay += newBlueprint.bots.clay * timeCost;
                        newBlueprint.resources.obsidian += newBlueprint.bots.obsidian * timeCost;

                        newBlueprint.bots.obsidian++;

                        queue.Enqueue((newBlueprint, minute + timeCost));
                    }
                }

                // Only look at this if we have an obsidian bot
                if (blueprint.bots.obsidian > 0)
                {
                    newBlueprint = blueprint.Clone();

                    timeCost = 1 + new int[] { 0, (int)Math.Ceiling((newBlueprint.costsGeode.ore - newBlueprint.resources.ore) / (double)newBlueprint.bots.ore), (int)Math.Ceiling((newBlueprint.costsGeode.clay - newBlueprint.resources.clay) / (double)newBlueprint.bots.clay), (int)Math.Ceiling((newBlueprint.costsGeode.obsidian - newBlueprint.resources.obsidian) / (double)newBlueprint.bots.obsidian) }.Max();
                    if (timeCost + minute <= maxMinute)
                    {
                        // If we can build a geode, just count those geodes now
                        // Then if/when we exit early, the count is good
                        newBlueprint.resources.geode += maxMinute - minute - timeCost + 1;

                        // Save this possible max
                        blueprintMax[blueprint.id] = Math.Max(blueprintMax[blueprint.id], newBlueprint.resources.geode);

                        newBlueprint.resources.ore -= newBlueprint.costsGeode.ore;
                        newBlueprint.resources.clay -= newBlueprint.costsGeode.clay;
                        newBlueprint.resources.obsidian -= newBlueprint.costsGeode.obsidian;

                        newBlueprint.resources.ore += newBlueprint.bots.ore * timeCost;
                        newBlueprint.resources.clay += newBlueprint.bots.clay * timeCost;
                        newBlueprint.resources.obsidian += newBlueprint.bots.obsidian * timeCost;

                        queue.Enqueue((newBlueprint, minute + timeCost));
                    }
                }
            }
        }

        protected override string? SolvePartOne()
        {
            FindMaximum(24);
            return blueprintMax.Sum(b => b.Key * b.Value).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
            // Only read the first three
            blueprints = blueprints.Take(3).ToList();
            FindMaximum(32);
            return blueprintMax.Sum(b => b.Key * b.Value).ToString();
        }
    }
}
