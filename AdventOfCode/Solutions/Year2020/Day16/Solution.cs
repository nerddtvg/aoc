using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{
    class TicketField
    {
        public string name { get; set; } = string.Empty;
        public List<TicketFieldRange> ranges { get; set; } = new();

        public bool IsValid(int input)
        {
            foreach (var range in ranges)
                if (range.min <= input && input <= range.max) return true;

            return false;
        }
    }

    class TicketFieldRange
    {
        public int min { get; set; }
        public int max { get; set; }

        public TicketFieldRange(int a, int b)
        {
            if (a > b)
            {
                this.min = b;
                this.max = a;
            }
            else
            {
                this.min = a;
                this.max = b;
            }
        }
    }

    class Ticket
    {
        public List<int> values { get; set; }

        public Ticket(string input)
        {
            this.values = input.ToIntArray(",").ToList();
        }
    }

    class Day16 : ASolution
    {
        Dictionary<string, TicketField> fields = new Dictionary<string, TicketField>();
        List<Ticket> tickets = new List<Ticket>();
        Ticket? mine = default;

        public Day16() : base(16, 2020, "")
        {
            var groups = Input.SplitByBlankLine(true);

            // Rules: groups[0]
            // Your ticket: groups[1]
            // Other tickets: groups[2]

            foreach (var line in groups[0])
            {
                // Need to parse
                // Name can have spaces in it
                // Example: class: 1-3 or 5-7
                var split = line.Split(":", StringSplitOptions.TrimEntries);
                var split2 = split[1].Split(" ", StringSplitOptions.TrimEntries);

                var range1 = split2[0].Split("-", StringSplitOptions.TrimEntries);
                var range2 = split2[2].Split("-", StringSplitOptions.TrimEntries);

                fields.Add(
                    split[0],
                    new TicketField()
                    {
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
            foreach (var line in groups[2])
            {
                // There is a header
                if (!line.Contains(",")) continue;

                tickets.Add(new Ticket(line));
            }
        }

        private bool IsValidTicket(Ticket t)
        {
            foreach (var v in t.values)
            {
                bool valid = false;

                // Check this field against all rules given
                foreach (var f in this.fields)
                {
                    if (f.Value.IsValid(v))
                    {
                        // Found one rule that matched, return here
                        valid = true;
                        break;
                    }
                }

                if (!valid) return false;
            }

            // All values match something
            return true;
        }

        private int GetInvalidSum(Ticket t)
        {
            // Go through each value and identify if it is invalid
            // If so, add to the sum
            int sum = 0;

            foreach (int v in t.values)
            {
                // Take this value through each possible field
                // If there is at least one that is valid, it is a valid field
                bool valid = this.fields.Select(a => a.Value.IsValid(v)).Count(a => a == true) > 0;

                if (!valid) sum += v;
            }

            return sum;
        }

        protected override string SolvePartOne()
        {
            return this.tickets.Sum(a => GetInvalidSum(a)).ToString();
        }

        protected override string SolvePartTwo()
        {
            // Remove any tickets with invalid values
            Console.WriteLine($"Before Count {this.tickets.Count}");
            List<Ticket> remove = new List<Ticket>();

            foreach (var t in this.tickets)
                if (!IsValidTicket(t)) remove.Add(t);

            // Remove these from the list (after we've iterated through it)
            remove.ForEach(a => this.tickets.Remove(a));
            Console.WriteLine($"After Count {this.tickets.Count}");

            // So now we need to work through all of the values in all tickets
            // Identify which field types they have in common
            Dictionary<int, List<string>> possibles = new Dictionary<int, List<string>>();

            // Go through each field and figure out what matches, could be multiple
            for (int i = 0; i < this.fields.Count; i++)
            {
                // Getting a list of values in this position across all tickets
                List<int> values = this.tickets.Select(a => a.values[i]).ToList();

                // Go through every field:
                // Is every value given valid for this field? Yes, it should match the list of given values (count)
                // Add it to the list
                List<string> poss = this.fields.Where(field => values.Count(v => field.Value.IsValid(v)) == values.Count).Select(a => a.Key).ToList();

                possibles.Add(i, poss);
            }

            // MANUAL: We stop here to identify if we have duplicates
            /*
            foreach (var kvp in possibles)
                Console.WriteLine($"{kvp.Key}: {string.Join(", ", kvp.Value)}");
            */

            // We identified we have many fields with duplicate possible values
            // So we need to reduce this down to figure out what is truly the only possible value for each
            // Start by finding all single possible fields and remove that value from any other
            bool removed = false;
            do
            {
                // Reset
                removed = false;

                possibles.Where(a => a.Value.Count == 1).ToList().ForEach(v =>
                {
                    for (int i = 0; i < this.fields.Count; i++)
                    {
                        int before = possibles[i].Count;

                        if (before > 1)
                        {
                            // Remove this value from the list
                            possibles[i].Remove(v.Value.First());

                            // We removed something so loop again
                            if (possibles[i].Count != before) removed = true;
                        }
                    }
                });
            } while (removed);

            // MANUAL: We stop here to identify if we have duplicates
            foreach (var kvp in possibles)
                Console.WriteLine($"{kvp.Key}: {string.Join(", ", kvp.Value)}");

            // We found the above reduction simplified everything down to a single possible key for each value
            // Get the keys for every possible value that starts with 'departure'
            List<int> keys = possibles.Where(kvp => kvp.Value.First().StartsWith("departure")).Select(kvp => kvp.Key).ToList();

            // Ensure we use a ulong for this value
            return keys.Select(a => (ulong)(this.mine?.values[a] ?? 0)).Aggregate((a, b) => a * b).ToString();
        }
    }
}
