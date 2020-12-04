using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

using System.Text.RegularExpressions;

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
        
        public bool IsValidPart2() {
            /*
            byr (Birth Year) - four digits; at least 1920 and at most 2002.
            iyr (Issue Year) - four digits; at least 2010 and at most 2020.
            eyr (Expiration Year) - four digits; at least 2020 and at most 2030.
            hgt (Height) - a number followed by either cm or in:
                If cm, the number must be at least 150 and at most 193.
                If in, the number must be at least 59 and at most 76.
            hcl (Hair Color) - a # followed by exactly six characters 0-9 or a-f.
            ecl (Eye Color) - exactly one of: amb blu brn gry grn hzl oth.
            pid (Passport ID) - a nine-digit number, including leading zeroes.
            cid (Country ID) - ignored, missing or not.
            */

            if (byr == null || byr.Length != 4)
                return false;
            try {
                int year = Int32.Parse(byr);
                if (year < 1920 || 2002 < year)
                    return false;
            } catch { return false; }

            if (iyr == null || iyr.Length != 4)
                return false;
            try {
                int year = Int32.Parse(iyr);
                if (year < 2010 || 2020 < year)
                    return false;
            } catch { return false; }

            if (eyr == null || eyr.Length != 4)
                return false;
            try {
                int year = Int32.Parse(eyr);
                if (year < 2020 || 2030 < year)
                    return false;
            } catch { return false; }

            if (hgt == null || (hgt.Substring(hgt.Length-2) != "cm" && hgt.Substring(hgt.Length-2) != "in"))
                return false;
            try {
                int height = Int32.Parse(hgt.Substring(0, hgt.Length-2));
                switch(hgt.Substring(hgt.Length-2)) {
                    case "cm":
                        if (height < 150 || 193 < height)
                            return false;
                        break;
                        
                    case "in":
                        if (height < 59 || 76 < height)
                            return false;
                        break;
                }
            } catch { return false; }

            if (hcl == null || !(new Regex("^#[a-fA-F0-9]{6}$")).IsMatch(hcl))
                return false;

            if (ecl == null || !(new string[] {"amb", "blu", "brn", "gry", "grn", "hzl", "oth"}).Contains(ecl))
                return false;

            if (pid == null || !(new Regex("^[0-9]{9}$")).IsMatch(pid))
                return false;

            return true;
        }
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
            return passports.Count(a => a.IsValidPart2()).ToString();
        }
    }
}
