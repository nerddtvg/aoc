using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2018
{

    class Day02 : ASolution
    {

        public Day02() : base(02, 2018, "")
        {

        }

        protected override string SolvePartOne()
        {
            int a2 = 0;
            Input.SplitByNewline().ToList().ForEach(line => {
                if (line.GroupBy(a => a).Select(a => a.Count()).Count(a => a == 2) > 0) a2++;
            });

            int a3 = 0;
            Input.SplitByNewline().ToList().ForEach(line => {
                if (line.GroupBy(a => a).Select(a => a.Count()).Count(a => a == 3) > 0) a3++;
            });

            return (a2 * a3).ToString();
        }

        protected override string SolvePartTwo()
        {
            // Find the two IDs that have only one letter difference between them in the same position
            int k = 0;
            List<string> input = Input.SplitByNewline().ToList();

            foreach(string line in input) {
                // We only need to check the strings AFTER this key
                for(int i=k+1; i<input.Count; i++) {
                    int diffCount = 0;

                    char[] s1 = line.ToCharArray();
                    char[] s2 = input[i].ToCharArray();

                    string common = "";

                    for(int q=0; q<s1.Length; q++) {
                        if (s1[q] != s2[q]) {
                            diffCount++;
                        } else {
                            common += s1[q];
                        }

                        if (diffCount > 1) break;
                    }

                    if (diffCount == 1) {
                        // We found the answer!
                        return common;
                    }
                }

                // Next iteration
                k++;
            }

            return string.Empty;
        }
    }
}
