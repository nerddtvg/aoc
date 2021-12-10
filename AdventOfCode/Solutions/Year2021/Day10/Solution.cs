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
                            score += this.values[ch];

                            // Part 1 ends at the first one
                            if (part == 1)
                                return this.values[ch];
                        }
                        break;
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
            return null;
        }
    }
}

#nullable restore
