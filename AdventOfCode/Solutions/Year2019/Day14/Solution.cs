using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2019
{

    class Element {
        public string name { get; set; }
        public long qty { get; set; }
        public long used { get; set; }

        public Element(string formula) {
            formula = formula.Trim();

            // Take a format:
            // 8 CNZTR
            // And save it

            Regex rx = new Regex(@"^([0-9]+) ([A-Z]+)$");
            Match match = rx.Match(formula);

            if (match.Groups.Count != 3) {
                throw new Exception($"Invalid Formula: {formula}");
            }

            this.name = match.Groups[2].Value;
            this.qty = Int64.Parse(match.Groups[1].Value);
            this.used = 0;
        }

        public Element(string name, long qty) {
            this.name = name;
            this.qty = qty;
            this.used = 0;
        }

        public override string ToString()
        {
            return $"{this.name} [{this.qty}]";
        }
    }

    class Formula {
        public List<Element> prereq;
        public Element result;
        public long depth;
        public long made;
        public long used;
        public long req;

        public Formula(string formula) {
            // Parse the string
            // Examples:
            // 171 ORE => 8 CNZTR
            // 7 ZLQW, 3 BMBT, 9 XCVML, 26 XMNCP, 1 WPTQ, 2 MZWV, 1 RJRHP => 4 PLWSL
            // 114 ORE => 4 BHXH

            this.depth = 0;
            this.made = 0;
            this.used = 0;

            string pre = (formula.Split("=>"))[0].Trim();
            string res = (formula.Split("=>"))[1].Trim();

            this.result = new Element(res);
            this.prereq = new List<Element>();

            foreach(string pre_s in pre.Split(',')) {
                this.prereq.Add(new Element(pre_s.Trim()));
            }
        }

        public void MakeQty(long required_qty, ref List<Formula> formulas, ref long ORE) {
            // Add to our used count
            this.used += required_qty;

            // We've made more and still have enough
            if (this.made > this.used) {
                return;
            }

            // We must use this formula again, increment everything
            long add_count = Convert.ToInt64(Math.Round((this.used - this.made * 1.0) / this.result.qty, System.MidpointRounding.ToPositiveInfinity));
            this.made += this.result.qty * add_count;

            // Now go through and increment our prereqs
            foreach(Element element in this.prereq) {
                // Find the formula where this applies and increment the amount
                IEnumerable<Formula> formula = formulas.Where(a => a.result.name == element.name);

                if (formula.Count() > 0) {
                    formula.First().MakeQty((element.qty * add_count), ref formulas, ref ORE);
                } else {
                    // We have hit ORE, add the required amount multiplied by what it takes
                    ORE += element.qty * add_count;
                }
            }
        }

        public void SetDepth(List<Formula> formulas) {
            // Find how many units are needed in all formulas
            this.req = formulas.Where(a => a.prereq.Select(b => b.name).Contains(this.result.name)).SelectMany(a => a.prereq.Where(b => b.name == this.result.name).Select(b => b.qty)).Sum();

            this.used = Convert.ToInt64(Math.Round((this.req / this.result.qty * 1.0), System.MidpointRounding.ToPositiveInfinity));
        }

        public void SetDepth(long depth, ref List<Formula> formulas) {
            this.depth += depth;

            List<string> prereq_names = this.prereq.Select(a => a.name).ToList();

            foreach(Formula formula in formulas.Where(a => prereq_names.Contains(a.result.name)).ToList()) {
                formula.SetDepth(depth + 1, ref formulas);
            }
        }
    }

    class Day14 : ASolution
    {
        public long ORE = 0;
        public List<Formula> formulas = new List<Formula>();

        public Day14() : base(14, 2019, "")
        {
            resetList(ref this.formulas);
        }

        protected override string SolvePartOne()
        {
            // Now that we have the formulas, we need to determine required quantities
            // First, find how many times each element is found in prereqs
            formulas.Where(a => a.result.name == "FUEL").First().SetDepth(0, ref formulas);
            
            // Now go through the order of depth and set the use case
            formulas.Where(a => a.result.name == "FUEL").First().MakeQty(1, ref formulas, ref ORE);

            return ORE.ToString();
        }

        protected void resetList(ref List<Formula> formulas) {
            foreach(string input in Input.SplitByNewline()) {
                formulas.Add(new Formula(input));
            }
        }

        protected override string SolvePartTwo()
        {
            // Let's guess this by divisions of two

            // Started with 1 and 2000000
            // Eventually widdled it down by brute force on each error thrown
            long f_min = 1;
            long f_max = 2000000;
            long o_min = 0;
            long o_max = 0;

            long desired_ore = 1000000000000;

            List<Formula> fo = new List<Formula>();
            List<Formula> f1 = new List<Formula>();
            List<Formula> f2 = new List<Formula>();

            resetList(ref fo);
            fo.Where(a => a.result.name == "FUEL").First().SetDepth(0, ref fo);

            // Each step, we divide by two until we're closer
            while(true) {
                // Copy the original
                f1 = fo.ConvertAll(a => a);
                f2 = fo.ConvertAll(a => a);

                o_min = 0;
                o_max = 0;

                formulas.Where(a => a.result.name == "FUEL").First().MakeQty(f_min, ref f1, ref o_min);
                formulas.Where(a => a.result.name == "FUEL").First().MakeQty(f_max, ref f2, ref o_max);

                // Compare our values
                if (desired_ore < o_min || desired_ore > o_max) {
                    throw new Exception($"Desired ore ({desired_ore}) outside range (min: {o_min}, max: {o_max})");
                }

                // Check if we have an answer
                if (o_max == desired_ore) {
                    return f_max.ToString();
                } else if (o_min == desired_ore) {
                    return f_min.ToString();
                }

                // Half way point
                if (desired_ore <= (o_max - o_min) / 2) {
                    f_max = f_max - Convert.ToInt64(Math.Round((f_max - f_min) / 2.0));
                } else {
                    f_min = f_min + Convert.ToInt64(Math.Round((f_max - f_min) / 2.0));
                }
                
                Console.WriteLine($"New Min: {f_min}");
                Console.WriteLine($"New Max: {f_max}");
                Console.WriteLine();
            }

            return null;
        }
    }
}
