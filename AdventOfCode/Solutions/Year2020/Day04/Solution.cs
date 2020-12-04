using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2020
{
    class Passport {
        public string? byr {get;set;} // Birth Year
        public string? iyr {get;set;} // Issue Year
        public string? eyr {get;set;} // Expiration Year
        public string? hgt {get;set;} // Height
        public string? hcl {get;set;} // Hair Color
        public string? ecl {get;set;} // Eye Color
        public string? pid {get;set;} // Passport ID
        public string? cid {get;set;} // Country ID

        public bool IsValidSet() =>
            // We don't care if ONLY cid is missing
            byr != null && iyr != null && eyr != null && hgt != null && hcl != null && ecl != null && pid != null;
    }

    class Day04 : ASolution
    {
        List<Passport> passports = new List<Passport>();

        public Day04() : base(04, 2020, "")
        {
            Passport p = new Passport();
            bool set = false;

            // For each line, if it is blank, we start a new passport
            foreach(string line in Input.SplitByNewline(true, false)) {
                if (string.IsNullOrWhiteSpace(line) && set) {
                    // We have a passport to save
                    passports.Add(p);

                    // Reset the variables
                    set = false;
                    p = new Passport();
                } else {
                    // Parse this line
                    string[] parts = line.Split(" ");
                    foreach(string part in parts) {
                        string[] keyval = part.Split(":");

                        // keyval[0] = key
                        // keyval[1] = val

                        // Dynamically setting the property value
                        p.GetType().GetProperty(keyval[0])?.SetValue(p, keyval[1]);

                        // We've set a value
                        set = true;
                    }
                }
            }

            // Do we have a final passport to save?
            if (set) passports.Add(p);

            Console.WriteLine($"Passport Count: {passports.Count.ToString()}");
        }

        protected override string SolvePartOne()
        {
            return passports.Count(a => a.IsValidSet()).ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
