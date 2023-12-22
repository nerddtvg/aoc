using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Diagnostics;
using MathNet.Numerics.Distributions;
using QuikGraph;


namespace AdventOfCode.Solutions.Year2023
{
    using Brick = ((int x, int y, int z) a, (int x, int y, int z) b);

    class Day22 : ASolution
    {
        public List<Brick> bricks;

        /// <summary>
        /// Contains a list of the bricks that hold up the Key brick
        /// </summary>
        public Dictionary<Brick, List<Brick>> intersects = new();

        public Day22() : base(22, 2023, "Sand Slabs")
        {
            // DebugInput = @"1,0,1~1,2,1
            //                0,0,2~2,0,2
            //                0,2,3~2,2,3
            //                0,0,4~0,2,4
            //                2,0,5~2,2,5
            //                0,1,6~2,1,6
            //                1,1,8~1,1,9";

            var regex = new Regex(@"^([0-9]+),([0-9]+),([0-9]+)~([0-9]+),([0-9]+),([0-9]+)$");

            bricks = Input.SplitByNewline(shouldTrim: true)
                .Select(line =>
                {
                    var match = regex.Match(line);

                    return (Brick)((int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value)), (int.Parse(match.Groups[4].Value), int.Parse(match.Groups[5].Value), int.Parse(match.Groups[6].Value)));
                })
                .OrderBy(brick => brick.a.z)
                .ThenBy(brick => brick.b.z)
                .ThenBy(brick => brick.a.x)
                .ThenBy(brick => brick.b.x)
                .ThenBy(brick => brick.a.y)
                .ThenBy(brick => brick.b.y)
                .ToList();
        }

        private Brick MoveDown(Brick brick) => (Brick)(brick.a.Add((0, 0, -1)), brick.b.Add((0, 0, -1)));

        private bool RangeIntersect(int a1, int a2, int b1, int b2)
        {
            if (a1 <= b1)
            {
                if (a2 < b1)
                    return false;
            }
            else
            {
                if (b2 < a1)
                    return false;
            }

            return true;
        }

        private bool BricksIntersect(Brick a, Brick b)
        {
            return
                RangeIntersect(a.a.x, a.b.x, b.a.x, b.b.x)
                &&
                RangeIntersect(a.a.y, a.b.y, b.a.y, b.b.y)
                &&
                RangeIntersect(a.a.z, a.b.z, b.a.z, b.b.z);
        }

        private void FallBricks()
        {
            var settled = new List<Brick>();

            // Go through the bricks and find where they land and their new z positions
            // When they land, we move them to settled
            // Start with z = 1
            // bricks.Where(b => b.a.z == 1).ToList().ForEach(b => { settled.Add(b); bricks.Remove(b); });

            while(bricks.Count > 0)
            {
                // Work on a temporary list so we can modify the old ones
                var tmpBricks = bricks.ToList();
                bricks.Clear();
                
                foreach(var brick in tmpBricks)
                {
                    var falling = MoveDown(brick);

                    // If we have hit the ground, stop
                    if (falling.a.z == 0)
                    {
                        settled.Add(brick);
                        intersects[brick] = [];
                    }
                    else
                    {
                        // Track who we intersect with
                        var intersect = settled.Where(b => BricksIntersect(b, falling)).ToList();

                        if (intersect.Count > 0)
                        {
                            settled.Add(brick);
                            intersects[brick] = intersect;
                        }
                        else
                        {
                            bricks.Add(falling);
                        }
                    }
                }
            }

            bricks = settled;
        }

        protected override string? SolvePartOne()
        {
            FallBricks();

            // Our intersects dict tells us what bricks
            // hold up the Key brick
            // but we need to convert that to be a ditionary
            // of what bricks the Key brick is holding up
            var holdsUp = bricks.ToDictionary(brick => brick, brick => intersects.Where(kvp => kvp.Value.Contains(brick)).Select(kvp => kvp.Key).ToList());

            // Now we can work both dictionaries
            // Going through holdsUp, we can ensure that any brick being held has another supporting brick (2+ in intersects dictionary)
            return holdsUp.Count(holding =>
            {
                if (holding.Value.Count == 0) return true;

                // For any brick that we are holding up, ensure it is supported by another
                return holding.Value.All(held => intersects[held].Count > 1);
            }).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

