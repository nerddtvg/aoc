using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2017
{

    class Day23 : ASolution
    {

        public Day23() : base(23, 2017, "Coprocessor Conflagration")
        {

        }

        protected override string? SolvePartOne()
        {
            var p = new Day18.SoundProgram(Input);

            var ret = true;
            while(ret)
            {
                ret = p.Run();
            }

            return p.mulCount.ToString();
        }

        protected override string? SolvePartTwo()
        {
            var p = new Day18.SoundProgram(Input);

            /* Refactored into psuedo batch:
                b = 93
                c = b
                if (a != 0) goto A
                goto B
                A: mul b 100
                b += 100000
                c = b
                c += 17000
                B: f = 1
                d = 2
                E: e = 2
                D: g = d
                g *= e
                g -= b
                if (g != 0) goto C
                f = 0
                C: e += 1
                g = e
                g -= b
                if (g != 0) goto D
                d += 1
                g = d
                g -= b
                if (g != 0) goto E
                if (f != 0) goto F
                h += 1
                F: g = b
                g -= c
                if (g != 0) goto G
                goto H (EXIT)
                G: b += 17
                goto B
                H: (EXIT)
            */

            /* Reduced into psuedo-C
            b = 93
            c = b
            if (a != 0) {
                b = 109300
                c = 126300
            }

            while(true) {
                f = 1
                d = 2
                do {
                    e = 2
                    do {
                        if (d * e == b) f = 0
                        e += 1
                    } while(e != b)
                    d += 1
                } while (d != b)
                // For every number d = 2 to b (109300):
                //   For every number e = 2 to b (109300):
                //     Check if e*d is a factor of b, if so, f is zero
                
                if (f == 0) h += 1
                // Count if we found a factor in any of the list
                
                if (b-c == 0) break
                
                b += 17
                // Since c is 17,000 larger than b, this loop runs 1001 times
                // NOT 1000 because the check is on the bottom
            }

            H:
            (EXIT)
            */

            // Solution: Counting the number of NOT prime numbers between 109300 and 126317 (1001 * 17) numbers.

            var h = Enumerable.Range(0, 1001).Count(index =>
            {
                // We want to know what is or isn't a divisor here
                var num = 109300 + (17 * index);

                return num.GetDivisors().Count() > 2;
            });

            return h.ToString();
        }
    }
}

#nullable restore
