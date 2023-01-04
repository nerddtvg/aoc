using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2022
{

    class Day19 : ASolution
    {
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
            public (int ore, int clay, int obsidian, int geode) bots;

            /// <summary>
            /// How many of each resource do we have?
            /// </summary>
            public (int ore, int clay, int obsidian, int geode) resources;

            /// <summary>
            /// Tacking what robots to skip
            /// </summary>
            public string[] skipped = Array.Empty<string>();

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
                bots = (1, 0, 0, 0);

                // And no resources
                resources = (0, 0, 0, 0);
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
            DebugInput = @"Blueprint 1: Each ore robot costs 4 ore. Each clay robot costs 2 ore. Each obsidian robot costs 3 ore and 14 clay. Each geode robot costs 2 ore and 7 obsidian.
            Blueprint 2: Each ore robot costs 2 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 8 clay. Each geode robot costs 3 ore and 12 obsidian.";

            ReadBlueprints(Input);
        }

        private void ReadBlueprints(string input)
        {
            blueprints = input.SplitByNewline(true).Select(b => new Blueprint(b)).ToList();
        }

        private void FindMaximum()
        {
            // How long can we run?
            const int maxMinute = 24;

            // Hold our current states
            Queue<(Blueprint blueprint, int minute)> queue = new();

            // First thing we do is load up all of the blueprints into the queue
            blueprints.ForEach(b => queue.Enqueue((b, 0)));
            blueprints.ForEach(b => blueprintMax[b.id] = 0);

            while(queue.Count > 0)
            {
                (var blueprint, var minute) = queue.Dequeue();

                // If we are done...
                if (minute > maxMinute)
                {
                    blueprintMax[blueprint.id] = Math.Max(blueprintMax[blueprint.id], blueprint.resources.geode);
                    continue;
                }

                // Keeping track of what we can make each time will reduce our steps down the road
                // If we can make an Ore robot here, but save resources, don't make an Ore robot the next time around
                var skip = new List<string>();

                // Otherwise, we have the ability to select a bot to make or not
                // Let's find all possible bots that we can make this minute and queue those as new states
                if (!blueprint.skipped.Contains("geode") && blueprint.costsGeode.ore <= blueprint.resources.ore && blueprint.costsGeode.clay <= blueprint.resources.clay && blueprint.costsGeode.obsidian <= blueprint.resources.obsidian)
                {
                    // If we can build a geode, don't bother building something else
                    var newBlueprint = blueprint.Clone();

                    newBlueprint.resources.ore += newBlueprint.bots.ore - newBlueprint.costsGeode.ore;
                    newBlueprint.resources.clay += newBlueprint.bots.clay - newBlueprint.costsGeode.clay;
                    newBlueprint.resources.obsidian += newBlueprint.bots.obsidian - newBlueprint.costsGeode.obsidian;
                    newBlueprint.resources.geode += newBlueprint.bots.geode;

                    newBlueprint.bots.geode++;

                    newBlueprint.skipped = Array.Empty<string>();

                    queue.Enqueue((newBlueprint, minute + 1));

                    skip.Add("geode");
                }
                else
                {
                    if (!blueprint.skipped.Contains("ore") && blueprint.costsOre.ore <= blueprint.resources.ore && blueprint.costsOre.clay <= blueprint.resources.clay && blueprint.costsOre.obsidian <= blueprint.resources.obsidian)
                    {
                        var newBlueprint = blueprint.Clone();

                        newBlueprint.resources.ore += newBlueprint.bots.ore - newBlueprint.costsOre.ore;
                        newBlueprint.resources.clay += newBlueprint.bots.clay - newBlueprint.costsOre.clay;
                        newBlueprint.resources.obsidian += newBlueprint.bots.obsidian - newBlueprint.costsOre.obsidian;
                        newBlueprint.resources.geode += newBlueprint.bots.geode;

                        newBlueprint.bots.ore++;

                        newBlueprint.skipped = Array.Empty<string>();

                        queue.Enqueue((newBlueprint, minute + 1));

                        skip.Add("ore");
                    }

                    if (!blueprint.skipped.Contains("clay") && blueprint.costsClay.ore <= blueprint.resources.ore && blueprint.costsClay.clay <= blueprint.resources.clay && blueprint.costsClay.obsidian <= blueprint.resources.obsidian)
                    {
                        var newBlueprint = blueprint.Clone();

                        newBlueprint.resources.ore += newBlueprint.bots.ore - newBlueprint.costsClay.ore;
                        newBlueprint.resources.clay += newBlueprint.bots.clay - newBlueprint.costsClay.clay;
                        newBlueprint.resources.obsidian += newBlueprint.bots.obsidian - newBlueprint.costsClay.obsidian;
                        newBlueprint.resources.geode += newBlueprint.bots.geode;

                        newBlueprint.bots.clay++;

                        newBlueprint.skipped = Array.Empty<string>();

                        queue.Enqueue((newBlueprint, minute + 1));

                        skip.Add("clay");
                    }

                    if (!blueprint.skipped.Contains("obsidian") && blueprint.costsObsidian.ore <= blueprint.resources.ore && blueprint.costsObsidian.clay <= blueprint.resources.clay && blueprint.costsObsidian.obsidian <= blueprint.resources.obsidian)
                    {
                        var newBlueprint = blueprint.Clone();

                        newBlueprint.resources.ore += newBlueprint.bots.ore - newBlueprint.costsObsidian.ore;
                        newBlueprint.resources.clay += newBlueprint.bots.clay - newBlueprint.costsObsidian.clay;
                        newBlueprint.resources.obsidian += newBlueprint.bots.obsidian - newBlueprint.costsObsidian.obsidian;
                        newBlueprint.resources.geode += newBlueprint.bots.geode;

                        newBlueprint.bots.obsidian++;

                        newBlueprint.skipped = Array.Empty<string>();

                        queue.Enqueue((newBlueprint, minute + 1));

                        skip.Add("obsidian");
                    }
                }

                // Add a state of doing nothing (saving resources)
                blueprint.resources.ore += blueprint.bots.ore;
                blueprint.resources.clay += blueprint.bots.clay;
                blueprint.resources.obsidian += blueprint.bots.obsidian;
                blueprint.resources.geode += blueprint.bots.geode;
                blueprint.skipped = skip.ToArray();

                queue.Enqueue((blueprint, minute + 1));
            }
        }

        protected override string? SolvePartOne()
        {
            FindMaximum();
            return blueprintMax.Sum(b => b.Key * b.Value).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

