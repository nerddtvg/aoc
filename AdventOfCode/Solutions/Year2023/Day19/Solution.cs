using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2023
{
    using Test = (string property, bool lessThan, int value, bool isDefault, string destination);
    using Item = (int x, int m, int a, int s);

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

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

