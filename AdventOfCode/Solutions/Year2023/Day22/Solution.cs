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
        public Dictionary<Brick, List<Brick>> heldUpBy = new();

        public Dictionary<Brick, List<Brick>> holdsUp = new();

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
                        heldUpBy[brick] = [];
                    }
                    else
                    {
                        // Track who we are held up by
                        var intersect = settled.Where(b => BricksIntersect(b, falling)).ToList();

                        if (intersect.Count > 0)
                        {
                            settled.Add(brick);
                            heldUpBy[brick] = intersect;
                        }
                        else
                        {
                            bricks.Add(falling);
                        }
                    }
                }
            }

            bricks = settled;

            // Our heldUpBy dict tells us what bricks
            // hold up the Key brick
            // but we need to convert that to be a ditionary
            // of what bricks the Key brick is holding up
            holdsUp = bricks.ToDictionary(brick => brick, brick => heldUpBy.Where(kvp => kvp.Value.Contains(brick)).Select(kvp => kvp.Key).ToList());
        }

        protected override string? SolvePartOne()
        {
            FallBricks();

            // Now we can work both dictionaries
            // Going through holdsUp, we can ensure that any brick being held has another supporting brick (2+ in intersects dictionary)
            return holdsUp.Count(holding =>
            {
                if (holding.Value.Count == 0) return true;

                // For any brick that we are holding up, ensure it is supported by another
                return holding.Value.All(held => heldUpBy[held].Count > 1);
            }).ToString();
        }

        protected override string? SolvePartTwo()
        {
            // This is a directed graph (holdsUp and heldUpBy)
            // We can use this information to determine what gets moved
            // With a starting brick:
            // 1. Look at the bricks it holds up
            //    a. If that brick is held by no other bricks, add to the moved pile
            //    b. Otherwise, that brick does not count

            // This would be a lot faster if it was processed as a tree or graph
            // However 4 seconds is not too bad for being a brute force check

            return bricks.Sum(brick =>
                {
                    var moved = new HashSet<Brick>();
                    var queue = new Queue<Brick>();

                    holdsUp[brick].ForEach(b => queue.Enqueue(b));

                    while (queue.TryDequeue(out Brick heldUp))
                    {
                        if (heldUpBy[heldUp].All(b => b == brick || moved.Contains(b)))
                        {
                            // This brick will fall
                            moved.Add(heldUp);

                            // Add the bricks being held up by heldUp to the queue to analyze
                            holdsUp[heldUp].ForEach(b => queue.Enqueue(b));
                        }
                    }

                    return moved.Count;
                })
                .ToString();
        }
    }
}

