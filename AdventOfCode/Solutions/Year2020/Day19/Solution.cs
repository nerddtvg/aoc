using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2020
{
    class ImageRule {
        public string id {get;set;}
        public string original {get;set;}
        public List<List<string>> rules {get;set;}
        public string parsedRules {get;set;}
        public List<string> dependencies {get;set;}

        public ImageRule(string input) {
            this.rules = new List<List<string>>();
            this.parsedRules = "";
            this.dependencies = new List<string>();

            // Parse the incoming rule set
            // Examples:
            /*
            0: 4 1 5
            1: 2 3 | 3 2
            2: 4 4 | 5 5
            3: 4 5 | 5 4
            4: "a"
            5: "b"
            */

            var parts = input.Split(":", StringSplitOptions.TrimEntries);
            this.id = parts[0];
            this.original = parts[1];

            // Parse the original ruleset
            if (this.original.Contains("\"")) {
                // Just a character!
                // Consider this parsed
                this.parsedRules = this.original.Replace("\"", "").Trim();
            } else {
                // One or more rules
                var tRules = this.original.Split("|", StringSplitOptions.TrimEntries);

                foreach(var rule in tRules) 
                    // Need to append the rulesets
                    this.rules.Add(rule.Split(" ", StringSplitOptions.TrimEntries).ToList());
                
                // Add up the dependencies
                this.dependencies = this.rules.SelectMany(a => a.Select(b => b)).Distinct().ToList();
            }
        }

        // Generates a regex for this
        public string getParsedRules(List<ImageRule> rules) {
            // It's possible we have the rules already
            if (this.parsedRules.Length > 0) return this.parsedRules;

            List<string> tRules = new List<string>();

            // If not, we need to figure out what to return
            foreach(var rule in this.rules) {
                // We need to get every part of our possible rules and add them
                string tRule = "";

                foreach(var part in rule)
                    tRule += rules.Where(a => a.id == part).First().getParsedRules(rules);
                
                tRules.Add(tRule);
            }

            // If we have more than one
            if (tRules.Count > 1)
                this.parsedRules = "(" + string.Join("|", tRules) + ")";
            else
                this.parsedRules = tRules[0];

            // Return the objects
            return this.parsedRules;
        }
    }

    class Day19 : ASolution
    {
        List<ImageRule> rules = new List<ImageRule>();
        List<string> possibleValues = new List<string>();

        public Day19() : base(19, 2020, "")
        {
            foreach(var line in Input.SplitByBlankLine()[0]) {
                this.rules.Add(new ImageRule(line));
            }
        }

        protected override string SolvePartOne()
        {
            var regex = new Regex("^" + this.rules.Where(a => a.id == "0").First().getParsedRules(this.rules) + "$", RegexOptions.IgnoreCase);

            return Input.SplitByBlankLine()[1].Count(a => regex.IsMatch(a)).ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
