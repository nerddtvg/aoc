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

        private uint CorruptedScore(string line, int part = 1)
        {
            var openings = new Stack<char>();

            uint score = 0;

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
                            // Mismatched
                            if (part == 1)
                                // Part 1 ends at the first one
                                return this.values[ch];
                            else
                                // Ignore for Part 1
                                return 0;
                        }
                        break;
                }
            }
            
            if (part == 1)
                return 0;

            // Part 2 needs to finish the incompleteness
            if (part == 2)
            {
                while(openings.Count > 0)
                {
                    score = score * 5 + this.valuesPt2[openings.Pop()];
                }
            }

            return score;
        }

        protected override string? SolvePartOne()
        {
            return Input.SplitByNewline().Sum(line => CorruptedScore(line)).ToString();
        }

        protected override string? SolvePartTwo()
        {
            var scores = Input.SplitByNewline()
                // Get the corrected scores
                .Select(line => CorruptedScore(line, 2))
                // Discount any that are 0 (part 1 answers)
                .Where(score => score > 0)
                // Sort
                .OrderBy(score => score)
                .ToList();

            if (scores.Count % 2 == 0)
                throw new Exception("Even number of responses");

            // Find the middle (don't +1 here because we have a zero start array)
            return scores[(scores.Count / 2)].ToString();
        }
    }
}

#nullable restore
