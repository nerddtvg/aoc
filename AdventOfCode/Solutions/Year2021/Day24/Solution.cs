using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2021
{

    class Day24 : ASolution
    {

        public Day24() : base(24, 2021, "Arithmetic Logic Unit")
        {

        }

        protected override string? SolvePartOne()
        {
            // Start with the largest possible 14-digit number and find the first one that z is returned 0
            for (long i = 99999999999999; i >= 11111111111111; i--)
            {
                if (i.ToString().Contains('0')) continue;

                if (ALU(i) == 0)
                    return i.ToString();
            }

            return null;
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }

        private int ALU(long input)
        {
            // Split our input into digits in a queue to pop them off
            var inQueue = new Queue<int>(input.ToString().ToIntArray());

            int x = 0, y = 0, z = 0;
            int digit = 0;

            digit = inQueue.Dequeue();
            x *= 0;
            x += z;
            x %= 26;
            z /= 1;
            x += 14;
            if (x == digit) x = 1; else x = 0;
            if (x == 0) x = 1; else x = 0;
            y *= 0;
            y += 25;
            y *= x;
            y += 1;
            z *= y;
            y *= 0;
            y += digit;
            y += 12;
            y *= x;
            z += y;
            
            digit = inQueue.Dequeue();
            x *= 0;
            x += z;
            x %= 26;
            z /= 1;
            x += 10;
            if (x == digit) x = 1; else x = 0;
            if (x == 0) x = 1; else x = 0;
            y *= 0;
            y += 25;
            y *= x;
            y += 1;
            z *= y;
            y *= 0;
            y += digit;
            y += 9;
            y *= x;
            z += y;

            digit = inQueue.Dequeue();
            x *= 0;
            x += z;
            x %= 26;
            z /= 1;
            x += 13;
            if (x == digit) x = 1; else x = 0;
            if (x == 0) x = 1; else x = 0;
            y *= 0;
            y += 25;
            y *= x;
            y += 1;
            z *= y;
            y *= 0;
            y += digit;
            y += 8;
            y *= x;
            z += y;

            digit = inQueue.Dequeue();
            x *= 0;
            x += z;
            x %= 26;
            z /= 26;
            x += -8;
            if (x == digit) x = 1; else x = 0;
            if (x == 0) x = 1; else x = 0;
            y *= 0;
            y += 25;
            y *= x;
            y += 1;
            z *= y;
            y *= 0;
            y += digit;
            y += 3;
            y *= x;
            z += y;

            digit = inQueue.Dequeue();
            x *= 0;
            x += z;
            x %= 26;
            z /= 1;
            x += 11;
            if (x == digit) x = 1; else x = 0;
            if (x == 0) x = 1; else x = 0;
            y *= 0;
            y += 25;
            y *= x;
            y += 1;
            z *= y;
            y *= 0;
            y += digit;
            y += 0;
            y *= x;
            z += y;

            digit = inQueue.Dequeue();
            x *= 0;
            x += z;
            x %= 26;
            z /= 1;
            x += 11;
            if (x == digit) x = 1; else x = 0;
            if (x == 0) x = 1; else x = 0;
            y *= 0;
            y += 25;
            y *= x;
            y += 1;
            z *= y;
            y *= 0;
            y += digit;
            y += 11;
            y *= x;
            z += y;
            
            digit = inQueue.Dequeue();
            x *= 0;
            x += z;
            x %= 26;
            z /= 1;
            x += 14;
            if (x == digit) x = 1; else x = 0;
            if (x == 0) x = 1; else x = 0;
            y *= 0;
            y += 25;
            y *= x;
            y += 1;
            z *= y;
            y *= 0;
            y += digit;
            y += 10;
            y *= x;
            z += y;

            digit = inQueue.Dequeue();
            x *= 0;
            x += z;
            x %= 26;
            z /= 26;
            x += -11;
            if (x == digit) x = 1; else x = 0;
            if (x == 0) x = 1; else x = 0;
            y *= 0;
            y += 25;
            y *= x;
            y += 1;
            z *= y;
            y *= 0;
            y += digit;
            y += 13;
            y *= x;
            z += y;

            digit = inQueue.Dequeue();
            x *= 0;
            x += z;
            x %= 26;
            z /= 1;
            x += 14;
            if (x == digit) x = 1; else x = 0;
            if (x == 0) x = 1; else x = 0;
            y *= 0;
            y += 25;
            y *= x;
            y += 1;
            z *= y;
            y *= 0;
            y += digit;
            y += 3;
            y *= x;
            z += y;

            digit = inQueue.Dequeue();
            x *= 0;
            x += z;
            x %= 26;
            z /= 26;
            x += -1;
            if (x == digit) x = 1; else x = 0;
            if (x == 0) x = 1; else x = 0;
            y *= 0;
            y += 25;
            y *= x;
            y += 1;
            z *= y;
            y *= 0;
            y += digit;
            y += 10;
            y *= x;
            z += y;

            digit = inQueue.Dequeue();
            x *= 0;
            x += z;
            x %= 26;
            z /= 26;
            x += -8;
            if (x == digit) x = 1; else x = 0;
            if (x == 0) x = 1; else x = 0;
            y *= 0;
            y += 25;
            y *= x;
            y += 1;
            z *= y;
            y *= 0;
            y += digit;
            y += 10;
            y *= x;
            z += y;

            digit = inQueue.Dequeue();
            x *= 0;
            x += z;
            x %= 26;
            z /= 26;
            x += -5;
            if (x == digit) x = 1; else x = 0;
            if (x == 0) x = 1; else x = 0;
            y *= 0;
            y += 25;
            y *= x;
            y += 1;
            z *= y;
            y *= 0;
            y += digit;
            y += 14;
            y *= x;
            z += y;

            digit = inQueue.Dequeue();
            x *= 0;
            x += z;
            x %= 26;
            z /= 26;
            x += -16;
            if (x == digit) x = 1; else x = 0;
            if (x == 0) x = 1; else x = 0;
            y *= 0;
            y += 25;
            y *= x;
            y += 1;
            z *= y;
            y *= 0;
            y += digit;
            y += 6;
            y *= x;
            z += y;

            digit = inQueue.Dequeue();
            x *= 0;
            x += z;
            x %= 26;
            z /= 26;
            x += -6;
            if (x == digit) x = 1; else x = 0;
            if (x == 0) x = 1; else x = 0;
            y *= 0;
            y += 25;
            y *= x;
            y += 1;
            z *= y;
            y *= 0;
            y += digit;
            y += 5;
            y *= x;
            z += y;

            return z;
        }
    }
}

#nullable restore
