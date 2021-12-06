using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2017
{

    class Day09 : ASolution
    {
        private int GarbageTotal = 0;

        public Day09() : base(09, 2017, "Stream Processing")
        {
            // DebugInput = @"{}";
            // DebugInput = @"{{{}}}";
            // DebugInput = @"{{},{}}";
            // DebugInput = @"{{{},{},{{}}}}";
            // DebugInput = @"{<a>,<a>,<a>,<a>}";
            // DebugInput = @"{{<ab>},{<ab>},{<ab>},{<ab>}}";
            // DebugInput = @"{{<!!>},{<!!>},{<!!>},{<!!>}}";
            // DebugInput = @"{{<a!>},{<a!>},{<a!>},{<ab>}}";

            // DebugInput = "<>";
            // DebugInput = "<random characters>";
            // DebugInput = "<<<<>";
            // DebugInput = "<{!>}>";
            // DebugInput = "<!!>";
            // DebugInput = "<!!!>>";
            // DebugInput = "<{o\"i!a,<{i<a>";
        }

        private int ScoreGroup(int baseScore, string group)
        {
            // A group text will NOT start or end with {} here, we trim those out
            var inGroup = false;
            var inGarbage = false;
            var startGroup = -1;
            var countBraces = 0;

            var totalScore = 0;

            for (int i = 0; i < group.Length; i++)
            {
                if (group[i] == '!')
                {
                    if (!inGarbage)
                        throw new Exception("Invalid '!' outside garbage");

                    // skip another character
                    i += 1;
                    continue;
                }
                
                // Count this character if we're in garbage but not inside a group
                if (baseScore == 0 && inGarbage)
                    this.GarbageTotal++;

                if (group[i] == '>')
                {
                    if (!inGarbage)
                        throw new Exception("Invalid '>' outside garbage");

                    // Remove one from the count
                    if (baseScore == 0)
                        this.GarbageTotal--;

                    // Go on
                    inGarbage = false;
                    continue;
                }

                if (group[i] == '<')
                {
                    inGarbage = true;
                    continue;
                }

                if (group[i] == '{')
                {
                    if (inGarbage)
                        continue;

                    countBraces++;

                    if (inGroup)
                    {
                        continue;
                    }

                    inGroup = true;
                    startGroup = i;
                    continue;
                }

                if (group[i] == '}')
                {
                    if (inGarbage)
                        continue;

                    countBraces--;

                    if (countBraces == 0)
                    {
                        inGroup = false;

                        if ((i - startGroup - 1) > 0)
                            totalScore += baseScore + 1 + ScoreGroup(baseScore + 1, group.Substring(startGroup + 1, (i - startGroup - 1)));
                        else
                            // Score an empty group
                            totalScore += baseScore + 1;
                    }
                }
            }

            return totalScore;
        }

        protected override string? SolvePartOne()
        {
            return ScoreGroup(0, Input).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return this.GarbageTotal.ToString();
        }
    }
}

#nullable restore
