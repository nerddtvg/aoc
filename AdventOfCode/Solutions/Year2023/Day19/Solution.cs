using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2023
{
    using Test = (string property, bool lessThan, int value, bool isDefault, string destination);
    using Item = (int x, int m, int a, int s);
    using ItemRange = (int x1, int x2, int m1, int m2, int a1, int a2, int s1, int s2);

    class Day19 : ASolution
    {
        private Dictionary<string, Test[]> workflows = new();
        private List<Item> items;

        public Day19() : base(19, 2023, "Aplenty")
        {
            // DebugInput = @"px{a<2006:qkq,m>2090:A,rfg}
            //                pv{a>1716:R,A}
            //                lnx{m>1548:A,A}
            //                rfg{s<537:gd,x>2440:R,A}
            //                qs{s>3448:A,lnx}
            //                qkq{x<1416:A,crn}
            //                crn{x>2662:A,R}
            //                in{s<1351:px,qqz}
            //                qqz{s>2770:qs,m<1801:hdj,R}
            //                gd{a>3333:R,R}
            //                hdj{m>838:A,pv}

            //                {x=787,m=2655,a=1222,s=2876}
            //                {x=1679,m=44,a=2067,s=496}
            //                {x=2036,m=264,a=79,s=2244}
            //                {x=2461,m=1339,a=466,s=291}
            //                {x=2127,m=1623,a=2188,s=1013}";

            var regex = new Regex(@"^(?<name>[a-z]+)\{(?<condition>(?<property>[xmas])(?<lt>[<>])(?<value>[0-9]+):(?<dest>[a-zAR]+),)*(?<final>[a-zAR]+)\}");

            var groups = Input.SplitByBlankLine(shouldTrim: true);

            foreach(var line in groups[0])
            {
                var matches = regex.Match(line);

                var Tests = new List<Test>();
                for(int i=0; i<matches.Groups["condition"].Captures.Count; i++)
                    Tests.Add((matches.Groups["property"].Captures[i].Value, matches.Groups["lt"].Captures[i].Value == "<", int.Parse(matches.Groups["value"].Captures[i].Value), false, matches.Groups["dest"].Captures[i].Value));

                Tests.Add((string.Empty, true, 0, true, matches.Groups["final"].Value));

                workflows.Add(matches.Groups["name"].Value, Tests.ToArray());
            }

            regex = new Regex(@"[0-9]+");
            items = groups[1].Select(line =>
                {
                    var matches = regex.Matches(line);

                    return (x: int.Parse(matches[0].Value), m: int.Parse(matches[1].Value), a: int.Parse(matches[2].Value), s: int.Parse(matches[3].Value));
                })
                .ToList();
        }

        private bool ProcessItem(Item item)
        {
            // Every item starts with "in"
            var workflowName = "in";

            // Then we proceed until we are Accepted (A) or Rejected (R)
            do
            {
                foreach(var rule in workflows[workflowName])
                {
                    if (MatchesTest(item, rule))
                    {
                        workflowName = rule.destination;
                        break;
                    }
                }
            } while (workflowName != "A" && workflowName != "R");

            return workflowName == "A";
        }

        private bool MatchesTest(Item item, Test test)
        {
            if (test.isDefault)
                return true;

            var val = GetValue(item, test.property);

            if (test.lessThan)
                return val < test.value;

            return val > test.value;
        }

        private int GetValue(Item item, string property)
        {
            switch(property)
            {
                case "x":
                    return item.x;

                case "m":
                    return item.m;

                case "a":
                    return item.a;

                case "s":
                    return item.s;
            }

            throw new Exception();
        }

        private int GetSums(Item item)
        {
            return item.x + item.m + item.a + item.s;
        }

        protected override string? SolvePartOne()
        {
            return items.Where(ProcessItem).Sum(GetSums).ToString();
        }

        private bool IsValidRange(ItemRange range) =>
            range.x1 != 0 && range.x2 != 0
            &&
            range.m1 != 0 && range.m2 != 0
            &&
            range.a1 != 0 && range.a2 != 0
            &&
            range.s1 != 0 && range.s2 != 0
            && range.x1 <= range.x2 && range.m1 <= range.m2 && range.a1 <= range.a2 && range.s1 <= range.s2;

        private IEnumerable<ItemRange> GetAcceptedRanges(ItemRange range, Test[] rules)
        {
            foreach(var rule in rules)
            {
                if (IsValidRange(range))
                {
                    if (rule.isDefault)
                    {
                        if (rule.destination == "A")
                        {
                            yield return range;
                        }
                        else if (rule.destination != "R")
                        {
                            foreach (var t in GetAcceptedRanges(range, workflows[rule.destination]))
                                yield return t;
                        }
                    }
                    else
                    {
                        // Generating a split here
                        ItemRange split = (
                            x1: rule.property == "x" ? (rule.lessThan ? (rule.value < range.x1 ? 0 : range.x1) : (range.x1 < rule.value ? rule.value + 1 : range.x1)) : range.x1,
                            x2: rule.property == "x" ? (rule.lessThan ? (rule.value < range.x2 ? rule.value - 1 : range.x2) : (range.x2 < rule.value ? 0 : range.x2)) : range.x2,

                            m1: rule.property == "m" ? (rule.lessThan ? (rule.value < range.m1 ? 0 : range.m1) : (range.m1 < rule.value ? rule.value + 1 : range.m1)) : range.m1,
                            m2: rule.property == "m" ? (rule.lessThan ? (rule.value < range.m2 ? rule.value - 1 : range.m2) : (range.m2 < rule.value ? 0 : range.m2)) : range.m2,

                            a1: rule.property == "a" ? (rule.lessThan ? (rule.value < range.a1 ? 0 : range.a1) : (range.a1 < rule.value ? rule.value + 1 : range.a1)) : range.a1,
                            a2: rule.property == "a" ? (rule.lessThan ? (rule.value < range.a2 ? rule.value - 1 : range.a2) : (range.a2 < rule.value ? 0 : range.a2)) : range.a2,

                            s1: rule.property == "s" ? (rule.lessThan ? (rule.value < range.s1 ? 0 : range.s1) : (range.s1 < rule.value ? rule.value + 1 : range.s1)) : range.s1,
                            s2: rule.property == "s" ? (rule.lessThan ? (rule.value < range.s2 ? rule.value - 1 : range.s2) : (range.s2 < rule.value ? 0 : range.s2)) : range.s2
                        );

                        // Process the range if valid
                        if (IsValidRange(split))
                        {
                            if (rule.destination == "A")
                            {
                                yield return split;
                            }
                            else if (rule.destination != "R")
                            {
                                foreach (var t in GetAcceptedRanges(split, workflows[rule.destination]))
                                    yield return t;
                            }
                        }

                        // Reduce our working range for the parent loop
                        range = (
                            x1: rule.property == "x" ? (rule.lessThan ? (rule.value < range.x1 ? range.x1 : rule.value) : (range.x1 < rule.value ? range.x1 : 0)) : range.x1,
                            x2: rule.property == "x" ? (rule.lessThan ? (rule.value < range.x2 ? range.x2 : 0) : (range.x2 < rule.value ? range.x2 : rule.value)) : range.x2,

                            m1: rule.property == "m" ? (rule.lessThan ? (rule.value < range.m1 ? range.m1 : rule.value) : (range.m1 < rule.value ? range.m1 : 0)) : range.m1,
                            m2: rule.property == "m" ? (rule.lessThan ? (rule.value < range.m2 ? range.m2 : 0) : (range.m2 < rule.value ? range.m2 : rule.value)) : range.m2,

                            a1: rule.property == "a" ? (rule.lessThan ? (rule.value < range.a1 ? range.a1 : rule.value) : (range.a1 < rule.value ? range.a1 : 0)) : range.a1,
                            a2: rule.property == "a" ? (rule.lessThan ? (rule.value < range.a2 ? range.a2 : 0) : (range.a2 < rule.value ? range.a2 : rule.value)) : range.a2,

                            s1: rule.property == "s" ? (rule.lessThan ? (rule.value < range.s1 ? range.s1 : rule.value) : (range.s1 < rule.value ? range.s1 : 0)) : range.s1,
                            s2: rule.property == "s" ? (rule.lessThan ? (rule.value < range.s2 ? range.s2 : 0) : (range.s2 < rule.value ? range.s2 : rule.value)) : range.s2
                        );
                    }
                }
            }
        }

        protected override string? SolvePartTwo()
        {
            /*************************************************************************************
            // The following code was removed because, while effective and did its job properly,
            // the speed of the math alone negated the need for this optimization
            // With reduction:    00:00:00.0282129 (takes longer to re-process rules)
            // Without reduction: 00:00:00.0030995

            // Let's start by reducing our inputs
            // There are many cases where:
            // abc{?>1234:R,R} => everything is rejected (or similar for accepted)
            int changed = 1;
            while(changed > 0)
            {
                var remove = workflows.Where(kvp => kvp.Value.All(rule => rule.destination == "A") || kvp.Value.All(rule => rule.destination == "R")).ToList();
                changed = remove.Count;

                remove.ForEach(kvp => workflows.Remove(kvp.Key));

                // Now we go through and replace the references
                foreach (var toRemove in remove)
                {
                    workflows = workflows.Select(kvp =>
                    {
                        return new KeyValuePair<string, Test[]>(kvp.Key, kvp
                            .Value
                            .Select(itm => (itm.property, itm.lessThan, itm.value, itm.isDefault, destination: itm.destination == toRemove.Key ? toRemove.Value[^1].destination : itm.destination))
                            .ToArray()
                        );
                    }).ToDictionary();
                }
            }
            *************************************************************************************/

            // Start with a single range of 1-4000 for all values
            ItemRange range = (1, 4000, 1, 4000, 1, 4000, 1, 4000);
            
            // We're going to go through every possibility to find our ranges
            // Each time we hit a condition, we will split and start over
            return GetAcceptedRanges(range, workflows["in"])
                // The ranges (ex. x2-x1) are not inclusive, add one to account for the start
                .Sum(range => (ulong)(range.x2 - range.x1 + 1) * (ulong)(range.m2 - range.m1 + 1) * (ulong)(range.a2 - range.a1 + 1) * (ulong)(range.s2 - range.s1 + 1))
                .ToString();
        }
    }
}

