using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2018
{

    class Day20 : ASolution
    {
        // A list of where we came from
        public Dictionary<(int x, int y), (int x, int y)> cameFrom = new Dictionary<(int x, int y), (int x, int y)>();

        // A list of shortest distance here
        public Dictionary<(int x, int y), int> distance = new Dictionary<(int x, int y), int>() { { (0, 0), 0 } };

        public Day20() : base(20, 2018, "A Regular Map")
        {
            // DebugInput = @"^WNE$";
            // DebugInput = @"^ENWWW(NEEE|SSE(EE|N))$";
            // DebugInput = @"^ENNWSWW(NEWS|)SSSEEN(WNSE|)EE(SWEN|)NNN$";
            // DebugInput = @"^ESSWWN(E|NNENN(EESS(WNSE|)SSS|WWWSSSSE(SW|NNNE)))$";
            // DebugInput = @"^WSSEESWWWNW(S|NENNEEEENN(ESSSSW(NWSW|SSEN)|WSWWN(E|WWS(E|SS))))$";
        }

        private void CheckPosDistance((int x, int y) pt, int distance, (int x, int y) cameFrom)
        {
            // Log this information only if it is the shortest path
            if (!this.cameFrom.ContainsKey(pt) || distance < this.distance[pt])
            {
                this.cameFrom[pt] = cameFrom;
                this.distance[pt] = distance;
            }
        }

        private void Parse(string input, int x, int y, int distance)
        {
            int startX = x;
            int startY = y;
            int startDistance = distance;

            var groupX = Int32.MaxValue;
            var groupY = Int32.MaxValue;
            var groupDistance = Int32.MaxValue;

            string subGroupStr = string.Empty;
            var subGroupX = Int32.MaxValue;
            var subGroupY = Int32.MaxValue;
            var subGroupDistance = Int32.MaxValue;
            int openParen = 0;

            // Distance is the value to this x,y
            foreach(var kvp in input.Select((ch, idx) => (idx, ch)))
            {
                var cameFrom = (x, y);

                // If we're in a group, just log these letters
                if (openParen > 1)
                {
                    if (kvp.ch == ')')
                        openParen--;
                    else if (kvp.ch == '(')
                        openParen++;

                    subGroupStr += kvp.ch;

                    // If we are at the end of this paren
                    // Then we process into this group string
                    if (openParen == 1)
                    {
                        Parse(subGroupStr, subGroupX, subGroupY, subGroupDistance);
                        subGroupStr = string.Empty;
                    }

                    continue;
                }

                // Determine what to do
                switch(kvp.ch)
                {
                    // If this is a direction, process it
                    case 'S':
                        y--;
                        distance++;
                        CheckPosDistance((x, y), distance, cameFrom);
                        break;
                    
                    case 'N':
                        y++;
                        distance++;
                        CheckPosDistance((x, y), distance, cameFrom);
                        break;

                    case 'W':
                        x--;
                        distance++;
                        CheckPosDistance((x, y), distance, cameFrom);
                        break;

                    case 'E':
                        x++;
                        distance++;
                        CheckPosDistance((x, y), distance, cameFrom);
                        break;

                    case '(':
                        openParen++;
                        // We're in a group now, if this is our first one, we will work the letters
                        if (openParen > 1)
                        {
                            subGroupStr += kvp.ch;

                            // Start this counter
                            subGroupX = x;
                            subGroupY = y;
                            subGroupDistance = distance;
                        }
                        else
                        {
                            groupX = x;
                            groupY = y;
                            groupDistance = distance;
                        }
                        break;

                    case '|':
                    case ')':
                        if (kvp.ch == ')')
                            openParen--;
                        // Out of a group
                        // We should only be here for the outermost group or a |
                        x = groupX;
                        y = groupY;
                        distance = groupDistance;
                        break;
                }
            }
        }

        protected override string? SolvePartOne()
        {
            Parse(Input, 0, 0, 0);

            return this.distance.Max(kvp => kvp.Value).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return this.distance.Count(kvp => kvp.Value >= 1000).ToString();
        }
    }
}

#nullable restore
