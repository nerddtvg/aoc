using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

using System.Diagnostics;

namespace AdventOfCode.Solutions.Year2020
{

    class Day15 : ASolution
    {
        // <spoken, positions spoken>
        Dictionary<int, Queue<int>> numbers = new Dictionary<int, Queue<int>>();
        int last_spoken = 0;
        int turn = 0;

        public Day15() : base(15, 2020, "")
        {
            /*
            DebugInput = "1,3,2";
            DebugInput = "2,1,3";
            DebugInput = "1,2,3";
            DebugInput = "2,3,1";
            DebugInput = "3,2,1";
            DebugInput = "3,1,2";
            */

            var arr = Input.ToIntArray(",");

            for(int i=0; i<arr.Length; i++) {
                turn++;

                numbers[arr[i]] = new Queue<int>();
                numbers[arr[i]].Enqueue(turn);
                last_spoken = arr[i];
            }
        }

        private void SpeakNumber() {
            // If the previous number had never been spoken before (only one item in list)
            // then the next number is 0
            int next = 0;

            var position = numbers[last_spoken];

            if (position.Count == 1) {
                // 0!
            } else {
                // Need to get the difference between the last two numbers
                next = position.Last() - position.Dequeue();
            }

            // Add to the list of spoke
            last_spoken = next;

            // Increment the turn
            turn++;

            if (numbers.ContainsKey(next)) {
                numbers[next].Enqueue(turn);
            } else {
                numbers[next] = new Queue<int>();
                numbers[next].Enqueue(turn);
            }
        }

        protected override string SolvePartOne()
        {
            var ts = new Stopwatch();
            ts.Start();

            while(turn < 2020)
                SpeakNumber();

            ts.Stop();
            Console.WriteLine($"Part 1 Time: {ts.ElapsedMilliseconds}");

            return last_spoken.ToString();
        }

        protected override string SolvePartTwo()
        {
            var ts = new Stopwatch();
            ts.Start();

            while(turn < 30000000)
                SpeakNumber();

            ts.Stop();
            Console.WriteLine($"Part 2 Time: {ts.ElapsedMilliseconds}");

            return last_spoken.ToString();
        }
    }
}
