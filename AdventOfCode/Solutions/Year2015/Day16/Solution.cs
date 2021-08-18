using System;
using System.Collections.Generic;
using System.Text;

using System.Text.RegularExpressions;
using System.Linq;

namespace AdventOfCode.Solutions.Year2015
{

    class Day16 : ASolution
    {
        private Dictionary<int, Dictionary<string, int>> _sues = new Dictionary<int, Dictionary<string, int>>();

        public Day16() : base(16, 2015, "")
        {
            // Sample: Sue 1: children: 1, cars: 8, vizslas: 7
            var matches = Regex.Matches(Input, "Sue ([0-9]+): ([a-z]+): ([0-9]+), ([a-z]+): ([0-9]+), ([a-z]+): ([0-9]+)", RegexOptions.IgnoreCase);

            foreach(Match match in matches)
            {
                var sue_id = Int32.Parse(match.Groups[1].Value);
                this._sues[sue_id] = new Dictionary<string, int>();

                this._sues[sue_id][match.Groups[2].Value] = Int32.Parse(match.Groups[3].Value);
                this._sues[sue_id][match.Groups[4].Value] = Int32.Parse(match.Groups[5].Value);
                this._sues[sue_id][match.Groups[6].Value] = Int32.Parse(match.Groups[7].Value);
            }
        }

        protected override string SolvePartOne()
        {
            /* Analysis to match:
                children: 3
                cats: 7
                samoyeds: 2
                pomeranians: 3
                akitas: 0
                vizslas: 0
                goldfish: 5
                trees: 3
                cars: 2
                perfumes: 1
            */

            // Supposedly there will be only one
            return this._sues.Where(sue => {
                var vals = sue.Value;

                return
                    (!vals.ContainsKey("children") || vals["children"] == 3)
                    &&
                    (!vals.ContainsKey("cats") || vals["cats"] == 7)
                    &&
                    (!vals.ContainsKey("samoyeds") || vals["samoyeds"] == 2)
                    &&
                    (!vals.ContainsKey("pomeranians") || vals["pomeranians"] == 3)
                    &&
                    (!vals.ContainsKey("akitas") || vals["akitas"] == 0)
                    &&
                    (!vals.ContainsKey("vizslas") || vals["vizslas"] == 0)
                    &&
                    (!vals.ContainsKey("goldfish") || vals["goldfish"] == 5)
                    &&
                    (!vals.ContainsKey("trees") || vals["trees"] == 3)
                    &&
                    (!vals.ContainsKey("cars") || vals["cars"] == 2)
                    &&
                    (!vals.ContainsKey("perfumes") || vals["perfumes"] == 1);
            })
            .OrderBy(a => a.Key).FirstOrDefault().Key.ToString();
        }

        protected override string SolvePartTwo()
        {
            // Convert to ranges for cats and trees
            return this._sues.Where(sue => {
                var vals = sue.Value;

                return
                    (!vals.ContainsKey("children") || vals["children"] == 3)
                    &&
                    (!vals.ContainsKey("cats") || vals["cats"] > 7)
                    &&
                    (!vals.ContainsKey("samoyeds") || vals["samoyeds"] == 2)
                    &&
                    (!vals.ContainsKey("pomeranians") || vals["pomeranians"] < 3)
                    &&
                    (!vals.ContainsKey("akitas") || vals["akitas"] == 0)
                    &&
                    (!vals.ContainsKey("vizslas") || vals["vizslas"] == 0)
                    &&
                    (!vals.ContainsKey("goldfish") || vals["goldfish"] < 5)
                    &&
                    (!vals.ContainsKey("trees") || vals["trees"] > 3)
                    &&
                    (!vals.ContainsKey("cars") || vals["cars"] == 2)
                    &&
                    (!vals.ContainsKey("perfumes") || vals["perfumes"] == 1);
            })
            .OrderBy(a => a.Key).FirstOrDefault().Key.ToString();
        }
    }
}
