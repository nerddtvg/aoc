using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{

    class Day01 : ASolution
    {

        public List<int> input;

        public Day01() : base(01, 2020, "")
        {
            input = Input.ToIntArray("\n").ToList();
        }

        protected override string SolvePartOne()
        {
            // Specifically, they need you to find the two entries that sum to 2020 and then multiply those two numbers together.
            int n1, n2;

            n1 = 0;
            n2 = 0;

            foreach(int a in input) {
               int? temp = input.Where(b => b != a && b + a == 2020).FirstOrDefault();

               if (temp.HasValue && temp.Value > 0) {
                   n1 = a;
                   n2 = temp.Value;
               }
            }

            return (n1 * n2).ToString();
        }

        protected override string SolvePartTwo()
        {
            // They offer you a second one if you can find three numbers in your expense report that meet the same criteria.
            int n1, n2, n3;

            n1 = 0;
            n2 = 0;
            n3 = 0;

            foreach(int a in input) {
                foreach(int b in input) {
                    // Skip early
                    if (a == b) continue;

                    int? temp = input.Where(c => c != a && c != b && b + a + c == 2020).FirstOrDefault();

                    if (temp.HasValue && temp.Value > 0) {
                        n1 = a;
                        n2 = b;
                        n3 = temp.Value;
                    }
                }
            }

            return (n1 * n2 * n3).ToString();
        }
    }
}
