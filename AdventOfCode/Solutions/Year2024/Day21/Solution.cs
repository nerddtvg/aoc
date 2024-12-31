using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    class Day21 : ASolution
    {
        #region Button Paths
        // Hardcoding directions into to make it easier
        Dictionary<char, Dictionary<char, string>> distDirectional = new()
        {
            ['^'] = new()
            {
                ['^'] = "A",
                ['A'] = ">A",
                ['<'] = "v<A",
                ['v'] = "vA",
                ['>'] = "v>A",
            },
            ['A'] = new()
            {
                ['^'] = "<A",
                ['A'] = "A",
                ['<'] = "v<<A",
                ['v'] = "<vA",
                ['>'] = "vA",
            },
            ['<'] = new()
            {
                ['^'] = ">^A",
                ['A'] = ">>^A",
                ['<'] = "A",
                ['v'] = ">A",
                ['>'] = ">>A",
            },
            ['v'] = new()
            {
                ['^'] = "^A",
                ['A'] = "^>A",
                ['<'] = "<A",
                ['v'] = "A",
                ['>'] = ">A",
            },
            ['>'] = new()
            {
                ['^'] = "<^A",
                ['A'] = "^A",
                ['<'] = "<<A",
                ['v'] = "<A",
                ['>'] = "A",
            }
        };

        Dictionary<char, Dictionary<char, string>> distNumeric = new()
        {
            ['7'] = new()
            {
                ['7'] = "A",
                ['8'] = ">A",
                ['9'] = ">>A",

                ['4'] = "vA",
                ['5'] = "v>A",
                ['6'] = "v>>A",

                ['1'] = "vvA",
                ['2'] = "vv>A",
                ['3'] = "vv>>A",

                ['0'] = ">vvvA",
                ['A'] = ">>vvvA"
            },
            ['8'] = new()
            {
                ['7'] = "<A",
                ['8'] = "A",
                ['9'] = ">A",

                ['4'] = "<vA",
                ['5'] = "vA",
                ['6'] = "v>A",

                ['1'] = "<vvA",
                ['2'] = "vvA",
                ['3'] = "vv>A",

                ['0'] = "vvvA",
                ['A'] = "vvv>A"
            },
            ['9'] = new()
            {
                ['7'] = "<<A",
                ['8'] = "<A",
                ['9'] = "A",

                ['4'] = "<<vA",
                ['5'] = "<vA",
                ['6'] = "vA",

                ['1'] = "<<vvA",
                ['2'] = "<vvA",
                ['3'] = "vvA",

                ['0'] = "<vvvA",
                ['A'] = "vvvA"
            },

            ['4'] = new()
            {
                ['7'] = "^A",
                ['8'] = "^>A",
                ['9'] = "^>>A",

                ['4'] = "A",
                ['5'] = ">A",
                ['6'] = ">>A",

                ['1'] = "vA",
                ['2'] = "v>A",
                ['3'] = "v>>A",

                ['0'] = ">vvA",
                ['A'] = ">>vvA"
            },
            ['5'] = new()
            {
                ['7'] = "<^A",
                ['8'] = "^A",
                ['9'] = "^>A",

                ['4'] = "<A",
                ['5'] = "A",
                ['6'] = ">A",

                ['1'] = "<vA",
                ['2'] = "vA",
                ['3'] = "v>A",

                ['0'] = "vvA",
                ['A'] = "vv>A"
            },
            ['6'] = new()
            {
                ['7'] = "<<^A",
                ['8'] = "<^A",
                ['9'] = "^A",

                ['4'] = "<<A",
                ['5'] = "<A",
                ['6'] = "A",

                ['1'] = "<<vA",
                ['2'] = "<vA",
                ['3'] = "vA",

                ['0'] = "<vvA",
                ['A'] = "vvA"
            },

            ['1'] = new()
            {
                ['7'] = "^^A",
                ['8'] = "^^>A",
                ['9'] = "^^>>A",

                ['4'] = "^A",
                ['5'] = "^>A",
                ['6'] = "^>>A",

                ['1'] = "A",
                ['2'] = ">A",
                ['3'] = ">>A",

                ['0'] = ">vA",
                ['A'] = ">>vA"
            },
            ['2'] = new()
            {
                ['7'] = "<^^A",
                ['8'] = "^^A",
                ['9'] = "^^>A",

                ['4'] = "<^A",
                ['5'] = "^A",
                ['6'] = "^>A",

                ['1'] = "<A",
                ['2'] = "A",
                ['3'] = ">A",

                ['0'] = "vA",
                ['A'] = "v>A"
            },
            ['3'] = new()
            {
                ['7'] = "<<^^A",
                ['8'] = "<^^A",
                ['9'] = "^^A",

                ['4'] = "<<^A",
                ['5'] = "<^A",
                ['6'] = "^A",

                ['1'] = "<<A",
                ['2'] = "<A",
                ['3'] = "A",

                ['0'] = "<vA",
                ['A'] = "vA"
            },

            ['0'] = new()
            {
                ['7'] = "^^^<A",
                ['8'] = "^^^A",
                ['9'] = "^^^>A",

                ['4'] = "^^<A",
                ['5'] = "^^A",
                ['6'] = "^^>A",

                ['1'] = "^<A",
                ['2'] = "^A",
                ['3'] = "^>A",

                ['0'] = "A",
                ['A'] = ">A"
            },
            ['A'] = new()
            {
                ['7'] = "^^^<<A",
                ['8'] = "<^^^A",
                ['9'] = "^^^A",

                ['4'] = "^^<<A",
                ['5'] = "<^^A",
                ['6'] = "^^A",

                ['1'] = "^<<A",
                ['2'] = "<^A",
                ['3'] = "^A",

                ['0'] = "<A",
                ['A'] = "A"
            }
        };
        #endregion

        public Day21() : base(21, 2024, "Keypad Conundrum")
        {
            // DebugInput = @"029A
            // 980A
            // 179A
            // 456A
            // 379A";
        }

        Dictionary<string, ulong> cache = [];

        ulong GetBotPatternLength(int bot, char destination, ref char[] bots, bool part2 = false)
        {
            // Save the current position, update the bot
            var currentPos = bots[bot];
            bots[bot] = destination;

            if ((!part2 && bot == 2) || (part2 && bot == 25))
                return (ulong)distDirectional[currentPos][destination].Length;

            // Save our cache just in case
            // From, To, Bot Id / Depth
            var key = $"{currentPos}-{destination}-{bot}";
            if (bot > 0 && cache.TryGetValue(key, out var value))
                return value;

            var patternLength = (ulong)0;

            foreach (var c in bot == 0 ? distNumeric[currentPos][destination] : distDirectional[currentPos][destination])
                patternLength += GetBotPatternLength(bot + 1, c, ref bots, part2);

            cache[key] = patternLength;

            return patternLength;
        }

        ulong GetDigitPatternLength(string code, bool part2 = false)
        {
            char[] bots = Enumerable.Range(0, part2 ? 26 : 3).Select(i => 'A').ToArray();
            var patternLength = (ulong)0;

            // bot[0] needs to push the values of the code
            // bot[1] directs bot[0]
            // bot[2] directs bot[1]
            // user directs bot[3]
            // We need bots[0] to move from bots[0] value to each char in code
            // The directions are: distNumeric[bots[0]][c]
            foreach (var c in code)
            {
                patternLength += GetBotPatternLength(0, c, ref bots, part2);
            }

            return patternLength;
        }

        protected override string? SolvePartOne()
        {
            ulong total = 0;

            foreach (var code in Input.SplitByNewline(shouldTrim: true))
            {
                var numericCode = ulong.Parse(code.Split("A")[0]);
                var patternLength = GetDigitPatternLength(code);

                // Console.WriteLine($"{code}: {pattern}");

                total += numericCode * patternLength;
            }

            // Time: 00:00:00.0034515
            // Time with Part 2 Cache: 00:00:00.0050127
            return total.ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Reset the cache
            cache = [];

            ulong total = 0;

            foreach (var code in Input.SplitByNewline(shouldTrim: true))
            {
                var numericCode = ulong.Parse(code.Split("A")[0]);
                var patternLength = GetDigitPatternLength(code, true);

                // Console.WriteLine($"{code}: {pattern}");

                total += numericCode * patternLength;
            }

            // Time: 00:00:00.0006504
            return total.ToString();
        }
    }
}

