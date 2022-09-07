using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2018
{

    class SleighStep
    {
        public string id { get; set; } = string.Empty;
        public List<string> prereq { get; set; } = new();
        public bool completed { get; set; }
        public bool start { get; set; }

        public int timeRequried
        {
            get
            {
                // Add 60 + 1 to account for the shift of 'A'
                return 61 + ((int)(((int)id.ToCharArray()[0]) - ((int)'A')));
            }
        }
    }

    class Worker
    {
        public string id { get; set; } = string.Empty;
        public int tick { get; set; }
        public int timeRequired { get; set; }

        public bool IsIdle() => string.IsNullOrEmpty(this.id);

        public override string ToString()
        {
            return $"{id}: {tick} / {timeRequired}";
        }
    }

    class Day07 : ASolution
    {
        List<SleighStep> steps = new List<SleighStep>();

        public Day07() : base(07, 2018, "")
        {

        }

        private void ParseInput()
        {
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

            foreach (string line in Input.SplitByNewline())
            {
                string[] parts = line.Split(" ");
                if (steps.Count(a => a.id == parts[7]) == 0)
                {
                    // New step!
                    steps.Add(new SleighStep()
                    {
                        id = parts[7],
                        completed = false,
                        start = false,
                        prereq = new List<string>() { parts[1] }
                    });
                }
                else
                {
                    steps.Where(a => a.id == parts[7]).First().prereq.Add(parts[1]);
                    steps.Where(a => a.id == parts[7]).First().prereq = steps.Where(a => a.id == parts[7]).First().prereq.Distinct().ToList();
                }
            }

            // So there will be something in the prereq lists that does not match anything, that's our starting step
            List<string> ids = steps.Select(a => a.id).ToList();
            List<string> prereqs = steps.SelectMany(a => a.prereq).Distinct().ToList();

            Console.WriteLine($"Ids: {ids.Count().ToString()}");
            Console.WriteLine($"Prereqs: {prereqs.Count().ToString()}");

            foreach (string missing in prereqs.Where(a => !ids.Contains(a)))
            {
                // These are "missing" which means they have no prereqs
                steps.Add(new SleighStep()
                {
                    id = missing,
                    completed = false,
                    start = true,
                    prereq = new List<string>()
                });
            }
        }

        protected override string SolvePartOne()
        {
            ParseInput();
            string order = "";

            // Go through and "process" the order
            string process = steps.Where(a => a.start == true).OrderBy(a => a.id).First().id;

            while (!string.IsNullOrEmpty(process))
            {
                // Find all of the steps this "completes"
                steps.Where(a => a.id == process).ToList().ForEach(a => a.completed = true);

                // Add to the order
                order += process;

                // Go through and update the prereqs
                steps.ForEach(a =>
                {
                    if (a.prereq.Contains(process))
                        a.prereq.Remove(process);
                });

                // Clear this in case we don't have a result below
                process = string.Empty;

                // Find the next step to process depending on who is not completed, who has no prereqs left, and alphabetical order
                process = steps.Where(a => a.completed == false && a.prereq.Count == 0).OrderBy(a => a.id).FirstOrDefault()?.id ?? string.Empty;
            }

            return order;
        }

        private List<string> GetNextAvailable() =>
            steps.Where(a => a.completed == false && a.prereq.Count == 0).OrderBy(a => a.id).Select(a => a.id).ToList();

        protected override string SolvePartTwo()
        {
            ParseInput();

            // Need Queues for the workers
            // id of step it is on
            // tick is the number of seconds they've been working on it
            // Setup with 5 workers
            var workers = new List<Worker>() {
                new Worker() { id = string.Empty, tick = 0 },
                new Worker() { id = string.Empty, tick = 0 },
                new Worker() { id = string.Empty, tick = 0 },
                new Worker() { id = string.Empty, tick = 0 },
                new Worker() { id = string.Empty, tick = 0 }
            };

            bool start = true;

            // Offset because our first step is to increment this
            int seconds = -1;

            while (start || workers.Count(a => !a.IsIdle()) > 0)
            {
                // Tick, tick, tick!
                start = false;
                List<string> completed = new List<string>();
                seconds++;

                // Increment the worker times
                workers.ForEach(worker =>
                {
                    if (!worker.IsIdle())
                    {
                        worker.tick++;

                        // Are they done?!
                        if (worker.tick == steps.Where(a => a.id == worker.id).First().timeRequried)
                        {
                            completed.Add(worker.id);

                            // Reset this worker
                            worker.id = string.Empty;
                            worker.tick = 0;
                        }
                    }
                });

                // Go through what was completed and get things ready
                completed.ForEach(complete =>
                {
                    // Find all of the steps this "completes"
                    steps.Where(a => a.id == complete).ToList().ForEach(a => a.completed = true);

                    // Go through and update the prereqs
                    steps.ForEach(a =>
                    {
                        if (a.prereq.Contains(complete))
                            a.prereq.Remove(complete);
                    });
                });

                // Get a list of what is available to work on (or being worked on)
                List<string> available = GetNextAvailable();

                // Find any idle workers and MAKE THEM WORK
                // Need to do this through a manual loop because Linq ForEach didn't update variables
                workers.Where(worker => worker.IsIdle()).ToList().ForEach(worker =>
                {
                    // Get a current list of what's in progress (refresh each loop to account for anything new)
                    List<string> inProgress = workers.Where(worker => !string.IsNullOrEmpty(worker.id)).Select(a => a.id).ToList();

                    // Find the first one in the list that is not being worked on
                    worker.id = available.FirstOrDefault(a => !inProgress.Contains(a)) ?? string.Empty;

                    // This is only set to help debugging
                    if (!string.IsNullOrEmpty(worker.id))
                        worker.timeRequired = steps.Where(a => a.id == worker.id).First().timeRequried;
                });

                // Check if we have done everything
                if (available.Count == 0 && workers.Count(a => a.IsIdle()) == workers.Count) break;
            }

            return seconds.ToString();
        }
    }
}
