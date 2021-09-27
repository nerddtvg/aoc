using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{

    class Day10 : ASolution
    {
        private Dictionary<int, List<int>> bots = new Dictionary<int, List<int>>();
        private Dictionary<int, List<int>> outputs = new Dictionary<int, List<int>>();
        private Dictionary<int, (string who, int index, string who2, int index2)> instructions = new Dictionary<int, (string who, int index, string who2, int index2)>();

        public Day10() : base(10, 2016, "")
        {
            ReadInput(Input);
            ProcessInput();
        }

        private void Reset()
        {
            this.bots = new Dictionary<int, List<int>>();
            this.outputs = new Dictionary<int, List<int>>();
            this.instructions = new Dictionary<int, (string who, int index, string who2, int index2)>();

            // We know that there are 209 bots and 20 outputs
            // Setting these to make it easier
            for (int i = 0; i <= 209; i++)
            {
                this.bots[i] = new List<int>();

                if (i <= 20)
                    this.outputs[i] = new List<int>();
            }
        }

        private void ReadInput(string input)
        {
            Reset();

            foreach(var line in input.SplitByNewline())
            {
                if (line.StartsWith("value"))
                {
                    // Bot just gets a value
                    var match = (new Regex(@"value ([0-9]+) goes to bot ([0-9]+)")).Match(line);

                    this.bots[Int32.Parse(match.Groups[2].Value)].Add(Int32.Parse(match.Groups[1].Value));
                }
                else
                {
                    // Instruction
                    var match = (new Regex(@"bot ([0-9]+) gives low to (output|bot) ([0-9]+) and high to (output|bot) ([0-9]+)")).Match(line);
                    this.instructions.Add(Int32.Parse(match.Groups[1].Value), (match.Groups[2].Value, Int32.Parse(match.Groups[3].Value), match.Groups[4].Value, Int32.Parse(match.Groups[5].Value)));
                }
            }
        }

        private void ProcessInput()
        {
            while(this.bots.Count > 0)
            {
                foreach(var bot in this.bots)
                {
                    // We will remove bots that are "finished"
                    if (bot.Value.Count == 2)
                    {
                        int min = Math.Min(bot.Value[0], bot.Value[1]);
                        int max = Math.Max(bot.Value[0], bot.Value[1]);

                        // Get rid of this bot
                        this.bots.Remove(bot.Key);

                        // Part 1:
                        if (min == 17 && max == 61)
                            Console.WriteLine($"Part 1: {bot.Key}");

                        if (this.instructions[bot.Key].who == "output")
                            this.outputs[this.instructions[bot.Key].index].Add(min);
                        else
                            this.bots[this.instructions[bot.Key].index].Add(min);

                        if (this.instructions[bot.Key].who2 == "output")
                            this.outputs[this.instructions[bot.Key].index2].Add(max);
                        else
                            this.bots[this.instructions[bot.Key].index2].Add(max);
                    }
                }
            }
        }

        protected override string SolvePartOne()
        {
            return "See Console Output";
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
