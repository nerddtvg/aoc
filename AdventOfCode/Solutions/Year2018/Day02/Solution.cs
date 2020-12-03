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
            return null;
        }
    }
}
