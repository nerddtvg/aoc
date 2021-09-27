using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{

    class Day09 : ASolution
    {
        public Day09() : base(09, 2016, "")
        {

        }

        private ulong GetLength(string input, int part = 1)
        {
            // Shortcut: if there are no parenthesis, we don't do anything
            if (!input.Contains('('))
                return (ulong)input.Length;

            // This is mostly math
            ulong ret = 0;
            char[] arr = input.ToCharArray();

            for (int i = 0; i < input.Length; i++)
            {
                // We need to figure out what to do
                if (arr[i] == '(')
                {
                    // Start of a marker!
                    // Get the length of this and return the length of GetLength(entire_marker)
                    var nextX = input.IndexOf('x', i);
                    var nextParen = input.IndexOf(')', nextX);

                    int length = Int32.Parse(input.Substring(i + 1, nextX - (i + 1)));
                    ulong repeat = ulong.Parse(input.Substring(nextX + 1, nextParen - (nextX + 1)));

                    // If this is part 1, just repeat this string the desired times
                    if (part == 1)
                    {
                        ret += repeat * (ulong) length;
                    }
                    else
                    {
                        // Part 2: Now we gather the string up and process it
                        ret += repeat * GetLength(input.Substring(nextParen + 1, length), 2);
                    }

                    // Don't add one here because the loop iterator will do that
                    i = nextParen + length;
                }
                else
                {
                    // Add one for this character
                    ret++;
                }
            }

            return ret;
        }

        protected override string SolvePartOne()
        {
            return GetLength(Input).ToString();
        }

        protected override string SolvePartTwo()
        {
            return GetLength(Input, 2).ToString();
        }
    }
}
