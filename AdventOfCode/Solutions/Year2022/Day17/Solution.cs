using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2022
{

    class Day17 : ASolution
    {
        /// <summary>
        /// Holds the rocks in order of them falling
        /// </summary>
        public Queue<RockType> rocks = new();

        /// <summary>
        /// Holds our input for air jet directions
        /// </summary>
        public Queue<char> airJet = new();

        public Day17() : base(17, 2022, "Pyroclastic Flow")
        {
            DebugInput = @">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>";

            // Load the queues
            rocks.Enqueue(RockType.Horiz);
            rocks.Enqueue(RockType.Plus);
            rocks.Enqueue(RockType.Bracket);
            rocks.Enqueue(RockType.Vertical);
            rocks.Enqueue(RockType.Box);

            // And our input
            Input.ToCharArray().ToList().ForEach(c => airJet.Enqueue(c));
        }

        protected override string? SolvePartOne()
        {
            return string.Empty;
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }

        public enum RockType
        {
            Horiz,
            Plus,
            Bracket,
            Vertical,
            Box
        }
    }
}

