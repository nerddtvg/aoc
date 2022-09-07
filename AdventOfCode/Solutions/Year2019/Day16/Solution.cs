using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2019
{

    class Day16 : ASolution
    {

        public Day16() : base(16, 2019, "")
        {

        }

        protected override string SolvePartOne()
        {
            List<int> pattern = new List<int>() { 0, 1, 0, -1 };

            int[] inputArr = Input.ToIntArray();

            // Phase count
            int phase_count = 100;

            for(int c=1; c<phase_count; c++) {
                int[] t_arr = inputArr;

                // For each character of `input`, we need to calculate the new value
                for(int i=1; i <= t_arr.Length; i++) {
                    int sum = 0;

                    for(int k=i; k <= t_arr.Length; k++) {
                        int pos = Convert.ToInt32(Math.Round(((decimal) c / k), System.MidpointRounding.ToZero) % 4);
                        sum += t_arr[c-1] * pattern[pos];
                    }

                    t_arr[i-1] = (Math.Abs(sum) % 10);
                }

                inputArr = t_arr;
            }

            return string.Join("", inputArr).Substring(0, 8);
        }

        protected override string SolvePartTwo()
        {
            return string.Empty;
        }
    }
}
