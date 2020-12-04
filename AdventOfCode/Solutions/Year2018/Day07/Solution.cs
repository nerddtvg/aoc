using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2018
{

    class SleighStep {
        public string id {get;set;}
        public List<string> prereq {get;set;}
        public bool completed {get;set;}
        public bool start {get;set;}
    }

    class Day07 : ASolution
    {
        List<SleighStep> steps = new List<SleighStep>();

        public Day07() : base(07, 2018, "")
        {
            ParseInput();
        }

        private void ParseInput() {
            // Parse each line, samples:
            /*
             * Step C must be finished before step A can begin.
             * Step C must be finished before step F can begin.
             * Step A must be finished before step B can begin.
             * Step A must be finished before step D can begin.
             * Step B must be finished before step E can begin.
             * Step D must be finished before step E can begin.
             * Step F must be finished before step E can begin.
             */
            
            steps = new List<SleighStep>();

            foreach(string line in Input.SplitByNewline()) {
                string[] parts = line.Split(" ");
                if (steps.Count(a => a.id == parts[7]) == 0) {
                    // New step!
                    steps.Add(new SleighStep() {
                        id = parts[7],
                        completed = false,
                        start = false,
                        prereq = new List<string>() { parts[1] }
                    });
                } else {
                    steps.Where(a => a.id == parts[7]).First().prereq.Add(parts[1]);
                    steps.Where(a => a.id == parts[7]).First().prereq = steps.Where(a => a.id == parts[7]).First().prereq.Distinct().ToList();
                }
            }

            // So there will be something in the prereq lists that does not match anything, that's our starting step
            List<string> ids = steps.Select(a => a.id).ToList();
            List<string> prereqs = steps.SelectMany(a => a.prereq).Distinct().ToList();

            Console.WriteLine($"Ids: {ids.Count().ToString()}");
            Console.WriteLine($"Prereqs: {prereqs.Count().ToString()}");

            foreach(string missing in prereqs.Where(a => !ids.Contains(a))) {
                // These are "missing" which means they have no prereqs
                steps.Add(new SleighStep() {
                    id = missing,
                    completed = false,
                    start = true,
                    prereq = new List<string>()
                });
            }
        }

        protected override string SolvePartOne()
        {
            string order = "";

            // Go through and "process" the order
            string process = steps.Where(a => a.start == true).OrderBy(a => a.id).First().id;

            while(!string.IsNullOrEmpty(process)) {
                // Find all of the steps this "completes"
                steps.Where(a => a.id == process).ToList().ForEach(a => a.completed = true);
                
                // Add to the order
                order += process;

                // Go through and update the prereqs
                steps.ForEach(a => {
                    if (a.prereq.Contains(process))
                        a.prereq.Remove(process);
                });

                // Clear this in case we don't have a result below
                process = "";

                // Find the next step to process depending on who is not completed, who has no prereqs left, and alphabetical order
                process = steps.Where(a => a.completed == false && a.prereq.Count == 0).OrderBy(a => a.id).FirstOrDefault()?.id;
            }

            return order;
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
