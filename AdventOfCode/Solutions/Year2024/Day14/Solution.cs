using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    class Day14 : ASolution
    {
        public struct Robot
        {
            public int x;
            public int y;
            public int vx;
            public int vy;
        }

        public List<Robot> robots;
        public int height = 0;
        public int width = 0;

        public Day14() : base(14, 2024, "Restroom Redoubt")
        {
            // DebugInput = @"p=0,4 v=3,-3
            // p=6,3 v=-1,-3
            // p=10,3 v=-1,2
            // p=2,0 v=2,-1
            // p=0,0 v=1,3
            // p=3,0 v=-2,-2
            // p=7,6 v=-1,-3
            // p=3,0 v=-1,-2
            // p=9,3 v=2,3
            // p=7,3 v=-1,2
            // p=2,4 v=2,-3
            // p=9,5 v=-3,-3";

            // width = 11;
            // height = 7;

            // Real numbers
            width = 101;
            height = 103;

            var robotRegex = new Regex(@"p=(?<x>[0-9]+),(?<y>[0-9]+) v=(?<vx>[\-0-9]+),(?<vy>[\-0-9]+)");

            robots = Input.SplitByNewline(true)
            .Select(line =>
            {
                var matches = robotRegex.Match(line);

                return new Robot
                {
                    x = int.Parse(matches.Groups["x"].Value),
                    y = int.Parse(matches.Groups["y"].Value),
                    vx = int.Parse(matches.Groups["vx"].Value),
                    vy = int.Parse(matches.Groups["vy"].Value)
                };
            })
            .ToList();
        }

        public int MoveValue(int i, int di, int seconds, int max)
        {
            // Add di*seconds to i
            i += di * seconds;

            // Now get the remainder
            i %= max;

            // Because this is remainder and not modulo, check for negatives
            return i < 0 ? i + max : i;
        }

        public Robot CalculateRobot(Robot robot, int seconds)
        {
            // We don't need to loop the calculations, modulus will work fine for everything we need
            return new Robot
            {
                x = MoveValue(robot.x, robot.vx, seconds, width),
                y = MoveValue(robot.y, robot.vy, seconds, height),
                vx = robot.vx,
                vy = robot.vy
            };
        }

        public (int q1, int q2, int q3, int q4) CountQuadrants(List<Robot> robots)
        {
            // For each of the quadrants, calculate them
            // Half the width/height
            var hx = width / 2;
            var hy = height / 2;

            int q1 = 0;
            int q2 = 0;
            int q3 = 0;
            int q4 = 0;

            robots.ForEach(robot =>
            {
                // We use < because we don't count the line at indexes hx and hy
                if (robot.x < hx)
                {
                    if (robot.y < hy)
                        q1++;
                    else if (robot.y >= hy + 1)
                        q2++;
                }
                else if (robot.x >= hx + 1)
                {
                    if (robot.y < hy)
                        q3++;
                    else if (robot.y >= hy + 1)
                        q4++;
                }
            });

            return (q1, q2, q3, q4);
        }

        protected override string? SolvePartOne()
        {
            var newRobots = robots.Select(robot => CalculateRobot(robot, 100)).ToList();
            var (q1, q2, q3, q4) = CountQuadrants(newRobots);

            return (q1 * q2 * q3 * q4).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

