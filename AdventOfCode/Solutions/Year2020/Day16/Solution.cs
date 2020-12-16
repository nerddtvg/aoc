using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

using System.Collections;

namespace AdventOfCode.Solutions.Year2020
{
    class TicketField {
        public string name {get;set;}
        public List<TicketFieldRange> ranges {get;set;}

        public bool IsValid(int input) {
            foreach(var range in ranges)
                if (range.min <= input && input <= range.max) return true;

            return false;
        }
    }

    class TicketFieldRange {
        public int min {get;set;}
        public int max {get;set;}

        public TicketFieldRange(int a, int b) {
            if (a > b) {
                this.min = b;
                this.max = a;
            } else {
                this.min = a;
                this.max = b;
            }
        }
    }

    class Ticket {
        public List<int> values {get;set;}

        public Ticket(string input) {
            this.values = input.ToIntArray(",").ToList();
        }
    }

    class Day16 : ASolution
    {
        Dictionary<string, TicketField> fields = new Dictionary<string, TicketField>();
        List<Ticket> tickets = new List<Ticket>();
        Ticket mine = null;

        public Day16() : base(16, 2020, "")
        {
            var groups = Input.SplitByBlankLine(true);

            // Rules: groups[0]
            // Your ticket: groups[1]
            // Other tickets: groups[2]

            foreach(var line in groups[0]) {
                // Need to parse
                // Name can have spaces in it
                // Example: class: 1-3 or 5-7
                var split = line.Split(":", StringSplitOptions.TrimEntries);
                var split2 = split[1].Split(" ", StringSplitOptions.TrimEntries);

                var range1 = split2[0].Split("-", StringSplitOptions.TrimEntries);
                var range2 = split2[2].Split("-", StringSplitOptions.TrimEntries);

                fields.Add(
                    split[0],
                    new TicketField() {
                        name = split[0],
                        ranges = new List<TicketFieldRange>() {
                            new TicketFieldRange(Int32.Parse(range1[0]), Int32.Parse(range1[1])),
                            new TicketFieldRange(Int32.Parse(range2[0]), Int32.Parse(range2[1]))
                        }
                    }
                );
            }

            // Our ticket, skip the header
            mine = new Ticket(groups[1][1]);

            // Other tickets
            foreach(var line in groups[2]) {
                // There is a header
                if (!line.Contains(",")) continue;

                tickets.Add(new Ticket(line));
            }
        }

        protected override string SolvePartOne()
        {
            return null;
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
