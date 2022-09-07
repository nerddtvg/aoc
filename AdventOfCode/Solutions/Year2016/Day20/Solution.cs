using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{

    class Day20 : ASolution
    {
        public class IPRange
        {
            public uint start = 0;
            public uint end = 0;

            public IPRange(uint s, uint e)
            {
                start = Math.Min(s, e);
                end = Math.Max(s, e);
            }
        }

        public List<IPRange> blocked = new List<IPRange>();

        public uint minimum = 0;

        public Day20() : base(20, 2016, "Firewall Rules")
        {
            
        }

        protected override string SolvePartOne()
        {
            foreach(var line in Input.SplitByNewline())
            {
                var split = line.Split('-');
                var a = uint.Parse(split[0]);
                var b = uint.Parse(split[1]);

                this.blocked.Add(new IPRange(a, b));
            }

            // Sort the list
            blocked = blocked.OrderBy(block => block.start).ThenBy(block => block.end).ToList();

            var removed = 0;
            do
            {
                removed = 0;
                
                // Go through each entry and remove it if it matched something further down the line
                for (int start = 0; start < this.blocked.Count - 1; start++)
                {
                    var rangeA = this.blocked[start];

                    for (int end = start + 1; end < this.blocked.Count; end++)
                    {
                        var rangeB = this.blocked[end];

                        // Check if we've gone out of range here
                        // Weird bug where if rangeA.end is MaxValue, we can't properly compare
                        if (rangeA.end != uint.MaxValue && rangeA.end < rangeB.start-1)
                            break;

                        // If the end range is longer than the rangeStart, modify rangeStar
                        if (rangeA.end != uint.MaxValue && rangeB.end >= rangeA.end)
                        {
                            rangeA.end = rangeB.end;
                        }

                        // We will now remove this
                        this.blocked.Remove(rangeB);
                        end--;  // Readjust for the new count
                        removed++;
                    }
                }
            } while (removed > 0);

            if (blocked.First().start > 0)
                minimum = 0;
            else
                minimum = blocked.First().end + 1;

            return minimum.ToString();
        }

        protected override string SolvePartTwo()
        {
            var c = ((ulong)uint.MaxValue + 1) - this.blocked.Sum(block => (block.end - block.start) + 1);

            return c.ToString() ?? string.Empty;
        }
    }
}
