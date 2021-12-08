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
                
                if (f == 0) h += 1
                
                if (b-c == 0) break
                
                b += 17
            }

            H:
            (EXIT)
            */

            // Set debug
            p.SetRegister('a', 1);

            var ret = true;
            var c = 0;
            while(ret && c++ < 100)
            {
                ret = p.Run();

                // This goes into a pretty crazy loop comparing the values of b and c over and over
                // To try and short-circuit it, on step 2 we override those values
                if (c == 2)
                {
                    p.SetRegister('b', 4);
                    //p.SetRegister('c', 0);
                }

                for (char i = 'a'; i <= 'h'; i++)
                {
                    Console.WriteLine($"{i}: {p.GetRegister(i)}");
                }

                Console.WriteLine("");
            }

            return p.GetRegister('h').ToString();
        }
    }
}

#nullable restore
