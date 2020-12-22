using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{
    class ImageRule {
        public string id {get;set;}
        public string original {get;set;}
        public List<List<string>> rules {get;set;}
        public List<string> parsedRules {get;set;}
        public List<string> dependencies {get;set;}

        public ImageRule(string input) {
            this.rules = new List<List<string>>();
            this.parsedRules = new List<string>();
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
                this.parsedRules.Add(this.original.Replace("\"", "").Trim());
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

        public List<string> getParsedRules(List<ImageRule> rules) {
            // It's possible we have the rules already
            if (this.parsedRules.Count > 0) return this.parsedRules;

            // If not, we need to figure out what to return
            foreach(var rule in this.rules) {
                // New string to return
                List<List<string>> ret = new List<List<string>>();

                foreach(var item in rule)
                    ret.Add(rules.Where(a => a.id == item).First().getParsedRules(rules));

                // Now that we have all possible outputs of each rule
                // We need all combinations of each
                List<string> temp = ret[0];

                for(int i=1; i<ret.Count; i++) {
                    List<string> tTemp = new List<string>();

                    // For each result here...
                    foreach(var r in ret[i])
                        // Append it to a copy of the previous results, making ths list longer and longer
                        for(int q=0; q<temp.Count; q++)
                            tTemp.Add(temp[q] + r);
                    
                    // Replace
                    temp = tTemp;
                }

                this.parsedRules.AddRange(temp);
            }

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

            // Get all possible image combinations
            this.possibleValues = this.rules.Where(a => a.id == "0").First().getParsedRules(this.rules);
            this.possibleValues.Sort();
        }

        protected override string SolvePartOne()
        {
            return Input.SplitByBlankLine()[1].Count(line => this.possibleValues.Contains(line, StringComparer.InvariantCulture)).ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
