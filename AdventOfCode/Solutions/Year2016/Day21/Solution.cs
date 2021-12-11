using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{

    class Day21 : ASolution
    {
        public string scrambled = string.Empty;

        public Day21() : base(21, 2016, "Scrambled Letters and Hash")
        {
//             DebugInput = @"swap position 4 with position 0
// swap letter d with letter b
// reverse positions 0 through 4
// rotate left 1 step
// move position 1 to position 4
// move position 3 to position 0
// rotate based on position of letter b
// rotate based on position of letter d";
        }

        private void Run(string line, int part = 1)
        {
            var matches = Regex.Match(line, "(?<action>swap position|swap letter|rotate left|rotate right|rotate based|reverse|move) ((?<swapA>[0-9]+) with position (?<swapB>[0-9]+)|(?<swapLetterA>[a-z]+) with letter (?<swapLetterB>[a-z]+)|(?<rotate>[0-9]+) step|on position of letter (?<rotateLetter>[a-z]+)|positions (?<reverseA>[0-9]+) through (?<reverseB>[0-9]+)|position (?<moveA>[0-9]+) to position (?<moveB>[0-9]+))");

            if (!matches.Success || string.IsNullOrEmpty(matches.Groups["action"].Value))
                throw new Exception("Unable to parse the line");

            var action          = matches.Groups["action"].Value;

            var swapA           = !string.IsNullOrEmpty(matches.Groups["swapA"].Value) ? Int32.Parse(matches.Groups["swapA"].Value) : 0;
            var swapB           = !string.IsNullOrEmpty(matches.Groups["swapB"].Value) ? Int32.Parse(matches.Groups["swapB"].Value) : 0;
            var swapLetterA     = matches.Groups["swapLetterA"].Value;
            var swapLetterB     = matches.Groups["swapLetterB"].Value;
            var rotate          = !string.IsNullOrEmpty(matches.Groups["rotate"].Value) ? Int32.Parse(matches.Groups["rotate"].Value) : 0;
            var rotateLetter    = matches.Groups["rotateLetter"].Value;
            var reverseA        = !string.IsNullOrEmpty(matches.Groups["reverseA"].Value) ? Int32.Parse(matches.Groups["reverseA"].Value) : 0;
            var reverseB        = !string.IsNullOrEmpty(matches.Groups["reverseB"].Value) ? Int32.Parse(matches.Groups["reverseB"].Value) : 0;
            var moveA           = !string.IsNullOrEmpty(matches.Groups["moveA"].Value) ? Int32.Parse(matches.Groups["moveA"].Value) : 0;
            var moveB           = !string.IsNullOrEmpty(matches.Groups["moveB"].Value) ? Int32.Parse(matches.Groups["moveB"].Value) : 0;

            if (action == "swap letter")
            {
                action = "swap position";
                swapA = this.scrambled.IndexOf(swapLetterA);
                swapB = this.scrambled.IndexOf(swapLetterB);
            }
            else if (action == "rotate based")
            {
                action = "rotate right";
                var index = this.scrambled.IndexOf(rotateLetter);
                rotate = 1 + index + (index >= 4 ? 1 : 0);
            }

            // Changes for Part 2
            if (part == 2)
            {
                if (action == "rotate right")
                    action = "rotate left";
                else if (action == "rotate left")
                    action = "rotate right";
                else if (action == "move")
                {
                    var t = moveA;
                    moveA = moveB;
                    moveB = moveA;
                }
            }

            switch(action)
            {
                case "swap position":
                    {
                        var t1 = Math.Min(swapA, swapB);
                        var t2 = Math.Max(swapA, swapB);

                        swapA = t1;
                        swapB = t2;

                        var a = this.scrambled[swapA];
                        var b = this.scrambled[swapB];
                        this.scrambled = (swapA == 0 ? string.Empty : this.scrambled.Substring(0, swapA)) + b + (a == b ? string.Empty : this.scrambled.Substring(swapA + 1, swapB - swapA - 1)) + a + (swapB == this.scrambled.Length - 1 ? string.Empty : this.scrambled.Substring(swapB + 1));
                        break;
                    }

                case "rotate left":
                    {
                        if (rotate == 0)
                            break;

                        rotate = rotate % this.scrambled.Length;
                            
                        this.scrambled = this.scrambled.Substring(rotate) + this.scrambled.Substring(0, rotate);
                        break;
                    }

                case "rotate right":
                    {
                        if (rotate == 0)
                            break;

                        rotate = rotate % this.scrambled.Length;

                        var endOfStr = this.scrambled.Substring(this.scrambled.Length - rotate);

                        this.scrambled = endOfStr + this.scrambled.Substring(0, this.scrambled.Length - rotate);
                        break;
                    }

                case "reverse":
                    {
                        this.scrambled = (reverseA == 0 ? string.Empty : this.scrambled.Substring(0, reverseA)) + this.scrambled.Substring(reverseA, reverseB - reverseA + 1).Reverse() + (reverseB == this.scrambled.Length - 1 ? string.Empty : this.scrambled.Substring(reverseB + 1));
                        break;
                    }

                case "move":
                    {
                        var t = this.scrambled[moveA];
                        var tempStr = this.scrambled.Remove(moveA, 1);
                        this.scrambled = tempStr.Insert(moveB, t.ToString());
                        break;
                    }

                default:
                    Console.WriteLine(action);
                    break;
            }
        }

        protected override string SolvePartOne()
        {
            this.scrambled = "abcdefgh";

            if (!string.IsNullOrEmpty(DebugInput))
                this.scrambled = "abcde";

            foreach (var line in Input.SplitByNewline())
            {
                Run(line);
            }

            return this.scrambled;
        }

        protected override string SolvePartTwo()
        {
            this.scrambled = "fbgdceah";

            if (!string.IsNullOrEmpty(DebugInput))
                this.scrambled = "abcde";

            foreach (var line in Input.SplitByNewline().Reverse())
            {
                Run(line, part: 2);
            }

            return this.scrambled;
        }
    }
}
