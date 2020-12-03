using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2018
{
    class GuardShift {
        public int guard {get;set;}
        public Dictionary<int, int> minutes{get;set;}

        // How many minutes where they asleep over all shifts?
        public int minutesAsleep {
            get{
                return this.minutes.Values.Sum();
            }
        }

        public DateTime date {get;set;}
    }

    class Day04 : ASolution
    {
        public List<GuardShift> shifts = new List<GuardShift>();

        public Day04() : base(04, 2018, "")
        {
            // Go through each line and parse it
            // First sort so it is in order
            GuardShift shift = new GuardShift();
            shift.minutes = new Dictionary<int, int>();

            DateTime lastSleep = DateTime.Now;

            foreach(string line in Input.SplitByNewline().OrderBy(a => a)) {
                // Samples:
                /*
                 * [1518-11-01 00:00] Guard #10 begins shift
                 * [1518-11-01 00:05] falls asleep
                 * [1518-11-01 00:25] wakes up
                 * [1518-11-01 00:30] falls asleep
                 * [1518-11-01 00:55] wakes up
                 * [1518-11-01 23:58] Guard #99 begins shift
                 * [1518-11-02 00:40] falls asleep
                 * [1518-11-02 00:50] wakes up
                 * [1518-11-03 00:05] Guard #10 begins shift
                 * [1518-11-03 00:24] falls asleep
                 * [1518-11-03 00:29] wakes up
                 * [1518-11-04 00:02] Guard #99 begins shift
                 * [1518-11-04 00:36] falls asleep
                 * [1518-11-04 00:46] wakes up
                 * [1518-11-05 00:03] Guard #99 begins shift
                 * [1518-11-05 00:45] falls asleep
                 * [1518-11-05 00:55] wakes up
                 */
                
                string[] parts = line.Replace("[", "").Split("]").Select(a => a.Trim()).ToArray();

                // Parse the date/time
                // Reformat into "u": YYYY-MM-dd HH:mm:ssZ
                parts[0] += ":00Z";
                DateTime shiftTime = DateTime.ParseExact(parts[0], "u", null);

                if (shiftTime.Hour > 0) {
                    shiftTime.AddDays(1);
                }

                if (parts[1].Contains("begins")) {
                    // This is a shift start
                    // Do we need to save the previous one?
                    if (!string.IsNullOrWhiteSpace(shift.guard.ToString())) {
                        shifts.Add(shift);

                        // Reset!
                        shift = new GuardShift();
                        shift.minutes = new Dictionary<int, int>();
                    }

                    shift.date = shiftTime;
                    shift.guard = Int32.Parse(parts[1].Replace("#", "").Split(" ")[1]);
                } else {
                    // We have a sleep or wake time
                    if (parts[1].Contains("sleep")) {
                        lastSleep = shiftTime;
                    } else {
                        // The guard has woken!
                        // Go through the minutes they were asleep and increment it
                        for(int min=lastSleep.Minute; min<shiftTime.Minute; min++)
                            shift.minutes.Add(min, 1);
                    }
                }
            }

            // Debug
            Console.WriteLine($"Shifts: {shifts.Count}");
        }

        protected override string SolvePartOne()
        {
            // Find the guard with the most minutes asleep
            int[] guard = shifts.GroupBy(a => a.guard).Select(a => new int[] {a.Key, shifts.Where(b => b.guard == a.Key).Sum(b => b.minutesAsleep)}).OrderByDescending(a => a[1]).First();

            // Get all the guard shifts (for easy references)
            List<GuardShift> guardShifts = shifts.Where(a => a.guard == guard[0]).ToList();

            // Now find the minute with the most times asleep
            int minute = Enumerable.Range(0, 60).ToList().Select(min =>
                // Count all the times this guard was asleep in this minute
                new Tuple<int, int>(min, guardShifts.Sum(shift => shift.minutes.ContainsKey(min) ? shift.minutes[min] : 0))
            ).OrderByDescending(a => a.Item2).First().Item1;

            // Let's draw this guard's shifts
            if (false) {
                int x;

                Console.WriteLine($"Guard: {guard[0]}");
                Console.WriteLine($"Minute: {minute}");
                
                Console.Write("      ");
                for(x=0; x<60; x++)
                    if (x == minute) Console.Write("*");
                    else Console.Write(" ");
                Console.WriteLine();
                
                Console.Write("      ");
                for(x=0; x<60; x++)
                    Console.Write($"{x/10}");
                Console.WriteLine();

                Console.Write("      ");
                for(x=0; x<60; x++)
                    Console.Write($"{x%10}");
                Console.WriteLine();

                foreach(GuardShift shift in guardShifts) {
                    Console.Write($"{shift.date.ToString("MM")}-{shift.date.ToString("dd")} ");

                    for(x=0; x<60; x++)
                        if (shift.minutes.ContainsKey(x))
                            Console.Write(shift.minutes[x] == 0 ? "." : "#");
                        else
                            Console.Write(".");
                    
                    Console.WriteLine();
                }
            }

            return (guard[0] * minute).ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
