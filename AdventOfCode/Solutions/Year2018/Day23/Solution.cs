using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2018
{

    class Day23 : ASolution
    {
        public class NanoBot
        {
            public Int64 x;
            public Int64 y;
            public Int64 z;
            public UInt64 r;

            public bool IsInRange(NanoBot bot2) =>
                (x, y, z).ManhattanDistance((bot2.x, bot2.y, bot2.z)) <= r;

            public bool IsInRange((Int64 x, Int64 y, Int64 z) pt) =>
                (x, y, z).ManhattanDistance(pt) <= r;
        }

        public List<NanoBot> bots = new List<NanoBot>();

        public Day23() : base(23, 2018, "Experimental Emergency Teleportation")
        {
            //             DebugInput = @"pos=<0,0,0>, r=4
            // pos=<1,0,0>, r=1
            // pos=<4,0,0>, r=3
            // pos=<0,2,0>, r=1
            // pos=<0,5,0>, r=3
            // pos=<0,0,3>, r=1
            // pos=<1,1,1>, r=1
            // pos=<1,1,2>, r=1
            // pos=<1,3,1>, r=1";

            this.bots.Clear();

            var input = Input.Replace("pos=", "").Replace(" r=", "").Replace("<", "").Replace(">", "").Trim();

            foreach (var line in input.SplitByNewline())
            {
                var s = line.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                this.bots.Add(new NanoBot()
                {
                    x = Int64.Parse(s[0]),
                    y = Int64.Parse(s[1]),
                    z = Int64.Parse(s[2]),
                    r = UInt64.Parse(s[3])
                });
            }
        }

        protected override string? SolvePartOne()
        {
            var largestRadius = this.bots.Max(b => b.r);
            var bot = this.bots.First(b => b.r == largestRadius);

            // Mistake: Don't ignore the source bot itself
            return this.bots.Count(b => bot.IsInRange(b)).ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Simply brute forcing this is not going to work
            // We need some algoritm that gets us a small range to check
            // This is not an algoritm or method I know, so I looked at the thread again
            // https://old.reddit.com/r/adventofcode/comments/a8s17l/2018_day_23_solutions/ecfmpy0/

            Func<(Int64 x1, Int64 y1, Int64 z1, Int64 x2, Int64 y2, Int64 z2), NanoBot, bool> doesIntersect = (box, bot) =>
            {
                // Does the bot intersect?
                Int64 d = 0;
                Int64 boxhigh, boxlow;

                boxlow = box.x1;
                boxhigh = box.x2 - 1;
                d += Math.Abs(bot.x - boxlow) + Math.Abs(bot.x - boxhigh);
                d -= boxhigh - boxlow;

                boxlow = box.y1;
                boxhigh = box.y2 - 1;
                d += Math.Abs(bot.y - boxlow) + Math.Abs(bot.y - boxhigh);
                d -= boxhigh - boxlow;

                boxlow = box.z1;
                boxhigh = box.z2 - 1;
                d += Math.Abs(bot.z - boxlow) + Math.Abs(bot.z - boxhigh);
                d -= boxhigh - boxlow;

                d /= 2;
                return ((ulong)d) <= bot.r;
            };

            Func<(Int64 x1, Int64 y1, Int64 z1, Int64 x2, Int64 y2, Int64 z2), uint> sumIntersect = (box) =>
            {
                return (uint)this.bots.Count(bot => doesIntersect(box, bot));
            };

            var maxX = this.bots.Select(b => (long)Math.Abs(b.x) + (long)b.r).Max();
            var maxY = this.bots.Select(b => (long)Math.Abs(b.y) + (long)b.r).Max();
            var maxZ = this.bots.Select(b => (long)Math.Abs(b.z) + (long)b.r).Max();
            var maxAbsCord = Math.Max(maxX, Math.Max(maxY, maxZ));

            var boxSize = 1;
            while (boxSize <= maxAbsCord)
            {
                boxSize *= 2;
            }

            var initalBox = (-boxSize, -boxSize, -boxSize, boxSize, boxSize, boxSize);

            var comparer = new PqComparer();

            var pq = new PriorityQueue<(Int64 x1, Int64 y1, Int64 z1, Int64 x2, Int64 y2, Int64 z2), (Int64 negReach, Int64 negSz, Int64 distToOrig)>(comparer);
            pq.Enqueue(initalBox, (-1 * this.bots.LongCount(), -2 * boxSize, 3 * boxSize));

            do
            {
                if (!pq.TryDequeue(out var box, out var priority))
                {
                    throw new Exception();
                }

                if (priority.negSz == -1)
                {
                    return (-1 * priority.negReach).ToString();
                }

                var newSz = priority.negSz / 2;

                // Move one box in each direction: x, y, z
                var newBox = (
                    x1: box.x1,
                    y1: box.y1,
                    z1: box.z1,
                    x2: box.x1 + newSz,
                    y2: box.y1 + newSz,
                    z2: box.z1 + newSz
                );

                var newReach = sumIntersect(newBox);
                pq.Enqueue(newBox, (-1 * newReach, -1 * newSz, (Int64)Math.Min((newBox.x1, newBox.y1, newBox.z1).ManhattanDistance((0, 0, 0)), (newBox.x2, newBox.y2, newBox.z2).ManhattanDistance((0, 0, 0)))));

                newBox = (
                    x1: box.x1,
                    y1: box.y1,
                    z1: box.z1 + newSz,
                    x2: box.x1 + newSz,
                    y2: box.y1 + newSz,
                    z2: box.z1 + (2 * newSz)
                );

                newReach = sumIntersect(newBox);
                pq.Enqueue(newBox, (-1 * newReach, -1 * newSz, (Int64)Math.Min((newBox.x1, newBox.y1, newBox.z1).ManhattanDistance((0, 0, 0)), (newBox.x2, newBox.y2, newBox.z2).ManhattanDistance((0, 0, 0)))));

                newBox = (
                    x1: box.x1,
                    y1: box.y1 + newSz,
                    z1: box.z1,
                    x2: box.x1 + newSz,
                    y2: box.y1 + (2 * newSz),
                    z2: box.z1 + newSz
                );

                newReach = sumIntersect(newBox);
                pq.Enqueue(newBox, (-1 * newReach, -1 * newSz, (Int64)Math.Min((newBox.x1, newBox.y1, newBox.z1).ManhattanDistance((0, 0, 0)), (newBox.x2, newBox.y2, newBox.z2).ManhattanDistance((0, 0, 0)))));

                newBox = (
                    x1: box.x1,
                    y1: box.y1 + newSz,
                    z1: box.z1 + newSz,
                    x2: box.x1 + newSz,
                    y2: box.y1 + (2 * newSz),
                    z2: box.z1 + (2 * newSz)
                );

                newReach = sumIntersect(newBox);
                pq.Enqueue(newBox, (-1 * newReach, -1 * newSz, (Int64)Math.Min((newBox.x1, newBox.y1, newBox.z1).ManhattanDistance((0, 0, 0)), (newBox.x2, newBox.y2, newBox.z2).ManhattanDistance((0, 0, 0)))));

                newBox = (
                    x1: box.x1 + newSz,
                    y1: box.y1,
                    z1: box.z1,
                    x2: box.x1 + (2 * newSz),
                    y2: box.y1 + newSz,
                    z2: box.z1 + newSz
                );

                newReach = sumIntersect(newBox);
                pq.Enqueue(newBox, (-1 * newReach, -1 * newSz, (Int64)Math.Min((newBox.x1, newBox.y1, newBox.z1).ManhattanDistance((0, 0, 0)), (newBox.x2, newBox.y2, newBox.z2).ManhattanDistance((0, 0, 0)))));

                newBox = (
                    x1: box.x1 + newSz,
                    y1: box.y1,
                    z1: box.z1 + newSz,
                    x2: box.x1 + (2 * newSz),
                    y2: box.y1 + newSz,
                    z2: box.z1 + (2 * newSz)
                );

                newReach = sumIntersect(newBox);
                pq.Enqueue(newBox, (-1 * newReach, -1 * newSz, (Int64)Math.Min((newBox.x1, newBox.y1, newBox.z1).ManhattanDistance((0, 0, 0)), (newBox.x2, newBox.y2, newBox.z2).ManhattanDistance((0, 0, 0)))));

                newBox = (
                    x1: box.x1 + newSz,
                    y1: box.y1 + newSz,
                    z1: box.z1 + newSz,
                    x2: box.x1 + (2 * newSz),
                    y2: box.y1 + (2 * newSz),
                    z2: box.z1 + (2 * newSz)
                );

                newReach = sumIntersect(newBox);
                pq.Enqueue(newBox, (-1 * newReach, -1 * newSz, (Int64)Math.Min((newBox.x1, newBox.y1, newBox.z1).ManhattanDistance((0, 0, 0)), (newBox.x2, newBox.y2, newBox.z2).ManhattanDistance((0, 0, 0)))));
            } while (pq.Count > 0);

            return null;

            // https://old.reddit.com/r/adventofcode/comments/a8s17l/comment/ecddus1/
            /*
            var xs = this.bots.Select(b => b.x).Append(0).ToList();
            var ys = this.bots.Select(b => b.y).Append(0).ToList();
            var zs = this.bots.Select(b => b.z).Append(0).ToList();

            var dist = 1;
            while (dist < (xs.Max() - xs.Min()) || dist < (ys.Max() - ys.Min()) || dist < (zs.Max() - zs.Min()))
            {
                dist *= 2;
            }

            // Something about wrapping issues?
            var ox = -1 * xs.Min();
            var oy = -1 * ys.Min();
            var oz = -1 * zs.Min();

            var span = 1;
            while (span < this.bots.Count)
            {
                span *= 2;
            }

            var forced_check = 1;
            var tried = new Dictionary<int, (ulong val, int count)>();

            ulong best_val = 0;
            int best_count = 0;

            while(true)
            {
                if (!tried.ContainsKey(forced_check))
                {
                    tried[forced_check] = Find(xs.ToArray(), ys.ToArray(), zs.ToArray(), dist, ox, oy, oz, forced_check);
                }

                (var val, var count) = tried[forced_check];

                if (val == 0)
                {
                    // Go up a level
                    if (span > 1)
                    {
                        span /= 2;
                    }

                    forced_check = Math.Max(1, forced_check - span);
                }
                else
                {
                    if (best_count == 0 || count > best_count)
                    {
                        best_count = count;
                        best_val = val;
                    }
                    
                    if (span == 1)
                    {
                        // We're done
                        break;
                    }

                    forced_check += span;
                }
            }

            return best_val.ToString();
            */

            // This gave 33000593 which was not correct
            // https://old.reddit.com/r/adventofcode/comments/a8s17l/comment/ecdqzdg/
            // This is a neat way of solving it using "simple" lines
            /* var queue = new Queue<(ulong distance, int counter)>();

            (Int64, Int64, Int64) origin = (0, 0, 0);

            foreach(var bot in this.bots)
            {
                var d = origin.ManhattanDistance((bot.x, bot.y, bot.z));
                queue.Enqueue((Math.Max(0, d - bot.r), 1));
                queue.Enqueue((d + bot.r + 1, -1));
            }

            var count = 0;
            var maxCount = 0;
            ulong result = 0;

            while(queue.Count > 0)
            {
                (var distance, var counter) = queue.Dequeue();
                count += counter;

                if (count > maxCount)
                {
                    result = distance;
                    maxCount = count;
                }
            }

            return result.ToString(); */
        }

        /*
        private (UInt64 val, int count) Find(Int64[] xs, Int64[] ys, Int64[] zs, int dist, Int64 ox, Int64 oy, Int64 oz, int forced_count)
        {
            var at_target = new List<(Int64 x, Int64 y, Int64 z, int count, UInt64 dist)>();

            for (Int64 x = xs.Min(); x <= xs.Max() + 1; x += dist)
            {
                for (Int64 y = ys.Min(); y <= ys.Max() + 1; y += dist)
                {
                    for (Int64 z = zs.Min(); z <= zs.Max() + 1; z += dist)
                    {
                        var count = 0;
                        foreach(var bot in this.bots)
                        {
                            if (dist == 1)
                            {
                                if (bot.IsInRange((x, y, z)))
                                {
                                    count++;
                                }
                            }
                            else
                            {
                                var calc = Math.Abs((ox + x) - (ox + bot.x));
                                calc += Math.Abs((oy + y) - (oy + bot.y));
                                calc += Math.Abs((oz + z) - (oz + bot.z));

                                if ((calc / dist - 3) <= (Int64) bot.r)
                                {
                                    count++;
                                }
                            }
                        }

                        if (count >= forced_count)
                        {
                            at_target.Add((x, y, z, count, (ulong) (Math.Abs(x) + Math.Abs(y) + Math.Abs(z))));
                        }
                    }
                }
            }

            while(at_target.Count > 0)
            {
                (Int64 x, Int64 y, Int64 z, int count, UInt64 dist)? best = null;
                var best_i = -1;

                // Find the best candidate (for now)
                for (int i = 0; i < at_target.Count; i++)
                {
                    if (best_i < 0 || (best.HasValue && at_target[i].dist < best.Value.dist))
                    {
                        best = at_target[i];
                        best_i = i;
                    }

                    if (!best.HasValue)
                        break;

                    if (dist == 1)
                    {
                        return (best.Value.dist, best.Value.count);
                    }
                    else
                    {
                        xs = new Int64[] { best.Value.x, best.Value.x + (dist / 2) };
                        ys = new Int64[] { best.Value.y, best.Value.y + (dist / 2) };
                        zs = new Int64[] { best.Value.z, best.Value.z + (dist / 2) };
                        (var a, var b) = Find(xs, ys, zs, dist / 2, ox, oy, oz, forced_count);

                        if (a == 0)
                        {
                            at_target.RemoveAt(best_i);
                        }
                        else
                        {
                            return (a, b);
                        }
                    }
                }
            }

            return (0, 0);
        }
        */
    }

    public class PqComparer : IComparer<(long, long, long)>
    {
        public int Compare((long, long, long) a, (long, long, long) b)
        {
            var comparer = Comparer<long>.Default;

            var ca = comparer.Compare(a.Item1, b.Item1);

            if (ca != 0)
                return ca;

            ca = comparer.Compare(a.Item2, b.Item2);

            if (ca != 0)
                return ca;

            return comparer.Compare(a.Item3, b.Item3);
        }
    }
}

