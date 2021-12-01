using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2017
{

    class Day03 : ASolution
    {

        public Day03() : base(03, 2017, "Spiral Memory")
        {
            // Each ring adds 8 more than the last ring
            // Ring 2: 1  =>  1 + 8                   =   9
            // Ring 3: 9  =>  9 + (8 + 8)             =  25
            // Ring 4: 25 => 25 + (8 + 8 + 8)         =  49
            // Ring 5: 49 => 49 + (8 + 8 + 8 + 8)     =  81
            // Ring 6: 81 => 81 + (8 + 8 + 8 + 8 + 8) = 121
            /*
                65  64  63  62  61  60  59  58  57
                66  37  36  35  34  33  32  31  56
                67  38  17  16  15  14  13  30  55
                68  39  18   5   4   3  12  29  54
                69  40  19   6 [ 1]  2  11  28  53
                70  41  20   7   8 [ 9] 10  27  52
                71  42  21  22  23  24 [25] 26  51
                72  43  44  45  46  47  48 [49] 50
                73  74  75  76  77  78  79  80 [81]
            */
            // A common formula would be:
            // Ring X => 1 + (Sum(0..X-1) * 8)
            // Or:
            // Ring X => 1 + (8 * ((X-1) * (X/2)))

            // We also know the side length is 1 + (2 * (X-1))
            // Ring 1: 1 + (2 * 0) =  1
            // Ring 2: 1 + (2 * 1) =  3
            // Ring 3: 1 + (2 * 2) =  5
            // Ring 4: 1 + (2 * 3) =  7
            // Ring 5: 1 + (2 * 4) =  9
            // Ring 6: 1 + (2 * 5) = 11

            // And ring length is: ((1 + (2 * (X-1))) * 4) - 4
            // Ring 1: ((1 + (2 * 0)) * 4) - 4 = 1
            // Ring 2: ((1 + (2 * 1)) * 4) - 4 = 8
            // Ring 3: ((1 + (2 * 2)) * 4) - 4 = 16
            // Ring 4: ((1 + (2 * 3)) * 4) - 4 = 24
            // Ring 5: ((1 + (2 * 4)) * 4) - 4 = 32
            // Ring 6: ((1 + (2 * 5)) * 4) - 4 = 40
        }

        private int CalcRingEnd(int index) =>
            (int) (1 + (8 * (index - 1) * ((double)index / 2)));

        /// <summary>
        /// Determine the ring that this value exists inside
        /// </summary>
        private int FindRing(int val)
        {
            for (int ring = 2; ring <= 10000; ring++)
            {
                // This ring:
                var thisRing = CalcRingEnd(ring);

                if (thisRing < val)
                    continue;

                // Found our ring
                return ring;
            }

            return 0;
        }

        protected override string? SolvePartOne()
        {
            return FindRing(Int32.Parse(Input)).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
