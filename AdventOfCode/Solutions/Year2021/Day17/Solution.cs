using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2021
{

    class Day17 : ASolution
    {
        private int x1;
        private int y1;

        private int x2;
        private int y2;

        public Day17() : base(17, 2021, "Trick Shot")
        {
            // DebugInput = @"target area: x=20..30, y=-10..-5";

            var split = Input
                .Replace(",", "")
                .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .TakeLast(2)
                .SelectMany(s => s.Split("=")[1].Split("..").Select(c => Int32.Parse(c)))
                .ToArray();

            this.x1 = split[0];
            this.x2 = split[1];
            this.y1 = split[2];
            this.y2 = split[3];
        }

        protected override string? SolvePartOne()
        {
            int maxHeight = 0;
            (int vx, int vy) maxVel = (0, 0);

            for (int vy = 1; vy <= 1000; vy++)
            {
                for (int vx = 1; vx <= 1000; vx++)
                {
                    // Our initial velocities are (vx, vy)
                    // Now let's get some points...
                    var inArea = false;
                    int dx = vx;
                    int dy = vy;
                    (int x, int y) point = (0, 0);

                    // Track this velocity combo to see if we had a higher point
                    int thisHeight = 0;
                    (int vx, int vy) thisVel = (vx, vy);

                    // Originally I had this capped at 100 but I had to bump up past 1000 to get a different answer
                    // And that's a first for me:
                    /*
                    That's not the right answer; your answer is too low.
                    Curiously, it's the right answer for someone else;
                    you might be logged in to the wrong account or just
                    unlucky. In any case, you need to be using your
                    puzzle input. If you're stuck, make sure you're
                    using the full input data; there are also some
                    general tips on the about page, or you can ask for
                    hints on the subreddit. Please wait one minute
                    before trying again.
                    */
                    for (int t = 1; t < 10000; t++)
                    {
                        // Get the point coords at time t
                        // The point we have:
                        point = (point.x + dx, point.y + dy);

                        // If dx != 0, move towards zero
                        if (dx > 0) dx--;
                        else if (dx < 0) dx++;

                        // Always changes
                        dy--;

                        // Check for values!
                        if (point.y > thisHeight)
                        {
                            thisHeight = point.y;
                        }

                        // Check to see if we have gone into the target area
                        if (this.x1 <= point.x && point.x <= this.x2 && this.y1 <= point.y && point.y <= this.y2)
                        {
                            inArea = true;
                            break;
                        }

                        // If we are below (y) or past (x), we're done with this attempt
                        if (point.x > this.x2 || point.y < this.y2)
                            break;
                    }

                    if (inArea)
                    {
                        // We found something
                        if (thisHeight > maxHeight)
                        {
                            maxHeight = thisHeight;
                            maxVel = thisVel;
                        }
                    }
                }
            }

            Console.WriteLine($"Max Velo: ({maxVel.vx}, {maxVel.vy})");
            return maxHeight.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
