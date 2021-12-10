using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2021
{

    class Day10 : ASolution
    {
        private Dictionary<char, uint> values = new Dictionary<char, uint>()
        {
            { ')', 3 },
            { ']', 57 },
            { '}', 1197 },
            { '>', 25137 }
        };

        private Dictionary<char, uint> valuesPt2 = new Dictionary<char, uint>()
        {
            // Matching the desired openings since that's what we track
            { '(', 1 },
            { '[', 2 },
            { '{', 3 },
            { '<', 4 }
        };

        private Dictionary<char, char> matches = new Dictionary<char, char>()
        {
            { '(', ')' },
            { '[', ']' },
            { '{', '}' },
            { '<', '>' }
        };

        private List<uint> incompleteScores = new List<uint>();

        public Day10() : base(10, 2021, "")
        {
//             DebugInput = @"[({(<(())[]>[[{[]{<()<>>
// [(()[<>])]({[<{<<[]>>(
// {([(<{}[<>[]}>{[]{[(<()>
// (((({<>}<{<{<>}{[]{[]{}
// [[<[([]))<([[{}[[()]]]
// [{[{({}]{}}([{[{{{}}([]
// {<[[]]>}<{[{[{[]{()[[[]
// [<(<(<(<{}))><([]([]()
// <{([([[(<>()){}]>(<<{{
// <{([{{}}[<[[[<>{}]]]>[]]";

            // DebugInput = "[[<[([]))<([[{}[[()]]]";
        }

        private uint CorruptedScore(string line)
        {
            var openings = new Stack<char>();

            foreach(var ch in line)
            {
                switch(ch)
                {
                    case '{':
                    case '(':
                    case '[':
                    case '<':
                        openings.Push(ch);
                        break;

                    case '}':
                    case ')':
                    case ']':
                    case '>':
                        if (this.matches[openings.Pop()] != ch)
                        {
                            // Part 1 ends at the first one
                            return this.values[ch];
                        }
                        break;
                }
            }

            // Part 2 needs to finish the incompleteness
            if (openings.Count > 0)
            {
                uint score = 0;

                while(openings.Count > 0)
                {
                    score = score * 5 + this.valuesPt2[openings.Pop()];
                }

                if (score > 0)
                    this.incompleteScores.Add(score);
            }

            return 0;
        }

        protected override string? SolvePartOne()
        {
            return Input.SplitByNewline().Sum(line => CorruptedScore(line)).ToString();
        }

        protected override string? SolvePartTwo()
        {
            this.incompleteScores = this.incompleteScores
                // Discount any that are 0 just in case
                .Where(score => score > 0)
                // Sort
                .OrderBy(score => score)
                .ToList();

            if (this.incompleteScores.Count % 2 == 0)
                throw new Exception("Even number of responses");

            // Find the middle (don't +1 here because we have a zero start array)
            return this.incompleteScores[(this.incompleteScores.Count / 2)].ToString();
        }
    }
}

#nullable restore
