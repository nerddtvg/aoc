using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2021
{

    class Day12 : ASolution
    {
        public struct Path
        {
            public string start = string.Empty;
            public string end = string.Empty;

            public Path() { }
        }

        public Path[] paths;

        public Day12() : base(12, 2021, "Passage Pathing")
        {
//             DebugInput = @"start-A
// start-b
// A-c
// A-b
// b-d
// A-end
// b-end";

//             DebugInput = @"dc-end
// HN-start
// start-kj
// dc-start
// dc-HN
// LN-dc
// HN-end
// kj-sa
// kj-HN
// kj-dc";

//             DebugInput = @"fs-end
// he-DX
// fs-he
// start-DX
// pj-DX
// end-zg
// zg-sl
// zg-pj
// pj-he
// RW-he
// fs-DX
// pj-RW
// zg-RW
// start-pj
// he-WI
// zg-he
// pj-fs
// start-RW";

            this.paths = Input.SplitByNewline().Select(line =>
            {
                var s = line.Split('-');
                return new Path
                {
                    start = s[0],
                    end = s[1]
                };
            }).ToArray();
        }

        public IEnumerable<IEnumerable<Path>> FindPaths(string start, IEnumerable<Path>? currentPath, int part = 1)
        {
            // Special case, if currentPath is null, we start it out
            if (currentPath == null)
            {
                currentPath = new List<Path>();
            }

            var currentList = currentPath.ToList();

            // Hit the end
            if (currentList.Count > 0 && (currentList.Last().start == "end" || currentList.Last().end == "end"))
            {
                yield return currentList;
                yield break;
            }

            // Find all lowercase caves already visited
            var smallCaves = currentPath
                .SelectMany(node => new string[] { node.start, node.end })
                .Where(cave => cave != "end" && !cave.ToCharArray().Any(ch => ch >= 'A' && ch <= 'Z'))
                .ToArray();

            // Part 2: We can visit a single small cave twice
            // This would appear as 4 in our count
            // A -> b, b -> C, C -> b, b -> D
            if (part == 2)
            {
                // Get all of the smallCaves we've visited from above and see if any have been visited 3 times
                // If not, clear out the small caves by resetting it to "start"
                if (!smallCaves.GroupBy(cave => cave).Any(grp => grp.Count() >= 3))
                {
                    smallCaves = new string[] { "start" };
                }
            }

            // Find all possible options
            var next = this.paths.Where(p => (p.start == start && !smallCaves.Contains(p.end)) || (p.end == start && !smallCaves.Contains(p.start))).ToList();

            // Return each of these
            foreach(var node in next)
            {
                var other = node.start == start ? node.end : node.start;

                foreach(var result in FindPaths(other, currentList.Append(node), part))
                {
                    yield return result;
                }
            }

            yield break;
        }

        protected override string? SolvePartOne()
        {
            return FindPaths("start", null).Count().ToString();
        }

        protected override string? SolvePartTwo()
        {
            return FindPaths("start", null, 2).Count().ToString();
        }
    }
}

#nullable restore
