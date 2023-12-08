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
            /// Tacking what robots to skip
            /// </summary>
            public BlueprintType[] skipped = Array.Empty<BlueprintType>();

            public Blueprint(string input)
            {
                var pattern = new Regex(@"Blueprint (?<id>[0-9]+): Each ore robot costs (?<ore>[0-9]+) ore. Each clay robot costs (?<clay>[0-9]+) ore. Each obsidian robot costs (?<obsore>[0-9]+) ore and (?<obsclay>[0-9]+) clay. Each geode robot costs (?<geodeore>[0-9]+) ore and (?<geodeobs>[0-9]+) obsidian.");

                var match = pattern.Match(input);

                if (!match.Success)
                    throw new Exception();

                id = Convert.ToInt32(match.Groups["id"].Value);
                costsOre = (ore: Convert.ToInt32(match.Groups["ore"].Value), 0, 0);
                costsClay = (ore: Convert.ToInt32(match.Groups["clay"].Value), 0, 0);
                costsObsidian = (ore: Convert.ToInt32(match.Groups["obsore"].Value), clay: Convert.ToInt32(match.Groups["obsclay"].Value), 0);
                costsGeode = (ore: Convert.ToInt32(match.Groups["geodeore"].Value), clay: 0, obsidian: Convert.ToInt32(match.Groups["geodeobs"].Value));

                // We always start with one ore bot
                bots = (1, 0, 0);

                // And no resources
                resources = (0, 0, 0, 0);

                // Determine our ultimate maximums for each bot
                // Another shortcut to prevent us from making dozens of ore when that's useless
                maximums = (ore: Math.Max(Math.Max(costsOre.ore, costsObsidian.ore), Math.Max(costsGeode.ore, costsClay.ore)), clay: costsObsidian.clay, obsidian: costsGeode.obsidian);
            }

            /// <summary>
            /// Simplify the resource increase
            /// </summary>
            public void IncreaseResources()
            {
                this.resources.ore += this.bots.ore;
                this.resources.clay += this.bots.clay;
                this.resources.obsidian += this.bots.obsidian;
            }

            /// <summary>
            /// Provides a simple hash to prevent duplicate efforts
            /// </summary>
            public string GetIdString(int minute)
            {
                return $"{id}-{bots.ore}-{bots.clay}-{bots.obsidian}-{resources.ore}-{resources.clay}-{resources.obsidian}-{resources.geode}-{minute}";
            }
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
            blueprints = input.SplitByNewline(true).Select(b => new Blueprint(b)).ToList();
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

            while(queue.Count > 0)
            {
                (var blueprint, var minute) = queue.Dequeue();

                // Skip this blueprint if we have been here before
                var hash = blueprint.GetIdString(minute);

                if (seen.Contains(hash))
                    continue;

                seen.Add(hash);

                // If we are done...
                if (minute > maxMinute)
                {
                    blueprintMax[blueprint.id] = Math.Max(blueprintMax[blueprint.id], blueprint.resources.geode);
                    continue;
                }

                // Keeping track of what we can make each time will reduce our steps down the road
                // If we can make an Ore robot here, but save resources, don't make an Ore robot the next time around
                var skip = new HashSet<BlueprintType>();

                // Otherwise, we have the ability to select a bot to make or not
                // Let's find all possible bots that we can make this minute and queue those as new states
                // This only applies if we are over 1 minute remainig (otherwise the bot won't be done in time)
                if (minute < maxMinute)
                {
                    if (!blueprint.skipped.Contains(BlueprintType.geode) && blueprint.costsGeode.ore <= blueprint.resources.ore && blueprint.costsGeode.clay <= blueprint.resources.clay && blueprint.costsGeode.obsidian <= blueprint.resources.obsidian)
                    {
                        // If we can build a geode, don't bother building something else
                        var newBlueprint = blueprint.Clone();

                        // If we can build a geode, just count those geodes now
                        newBlueprint.resources.geode += maxMinute - minute;

                        newBlueprint.IncreaseResources();

                        newBlueprint.resources.ore -= newBlueprint.costsGeode.ore;
                        newBlueprint.resources.clay -= newBlueprint.costsGeode.clay;
                        newBlueprint.resources.obsidian -= newBlueprint.costsGeode.obsidian;

                        newBlueprint.skipped = Array.Empty<BlueprintType>();

                        queue.Enqueue((newBlueprint, minute + 1));

                        skip.Add(BlueprintType.geode);
                    }
                    else
                    {
                        if (!blueprint.skipped.Contains(BlueprintType.ore) && blueprint.bots.ore < blueprint.maximums.ore && blueprint.costsOre.ore <= blueprint.resources.ore && blueprint.costsOre.clay <= blueprint.resources.clay && blueprint.costsOre.obsidian <= blueprint.resources.obsidian)
                        {
                            var newBlueprint = blueprint.Clone();

                            newBlueprint.IncreaseResources();

                            newBlueprint.resources.ore -= newBlueprint.costsOre.ore;
                            newBlueprint.resources.clay -= newBlueprint.costsOre.clay;
                            newBlueprint.resources.obsidian -= newBlueprint.costsOre.obsidian;

                            newBlueprint.bots.ore++;

                            newBlueprint.skipped = Array.Empty<BlueprintType>();

                            queue.Enqueue((newBlueprint, minute + 1));

                            skip.Add(BlueprintType.ore);
                        }

                        if (!blueprint.skipped.Contains(BlueprintType.clay) && blueprint.bots.clay < blueprint.maximums.clay && blueprint.costsClay.ore <= blueprint.resources.ore && blueprint.costsClay.clay <= blueprint.resources.clay && blueprint.costsClay.obsidian <= blueprint.resources.obsidian)
                        {
                            var newBlueprint = blueprint.Clone();

                            newBlueprint.IncreaseResources();

                            newBlueprint.resources.ore -= newBlueprint.costsClay.ore;
                            newBlueprint.resources.clay -= newBlueprint.costsClay.clay;
                            newBlueprint.resources.obsidian -= newBlueprint.costsClay.obsidian;

                            newBlueprint.bots.clay++;

                            newBlueprint.skipped = Array.Empty<BlueprintType>();

                            queue.Enqueue((newBlueprint, minute + 1));

                            skip.Add(BlueprintType.clay);
                        }

                        if (!blueprint.skipped.Contains(BlueprintType.obsidian) && blueprint.bots.obsidian < blueprint.maximums.obsidian && blueprint.costsObsidian.ore <= blueprint.resources.ore && blueprint.costsObsidian.clay <= blueprint.resources.clay && blueprint.costsObsidian.obsidian <= blueprint.resources.obsidian)
                        {
                            var newBlueprint = blueprint.Clone();

                            newBlueprint.IncreaseResources();

                            newBlueprint.resources.ore -= newBlueprint.costsObsidian.ore;
                            newBlueprint.resources.clay -= newBlueprint.costsObsidian.clay;
                            newBlueprint.resources.obsidian -= newBlueprint.costsObsidian.obsidian;

                            newBlueprint.bots.obsidian++;

                            newBlueprint.skipped = Array.Empty<BlueprintType>();

                            queue.Enqueue((newBlueprint, minute + 1));

                            skip.Add(BlueprintType.obsidian);
                        }
                    }
                }

                // Add a state of doing nothing (saving resources)
                blueprint.IncreaseResources();
                blueprint.skipped = skip.ToArray();

                queue.Enqueue((blueprint, minute + 1));
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
