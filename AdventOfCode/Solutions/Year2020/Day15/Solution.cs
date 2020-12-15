using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;
using System.Collections;

namespace AdventOfCode.Solutions.Year2020
{

    class Day15 : ASolution
    {
        // <spoken, positions spoken>
        Dictionary<int, List<int>> numbers = new Dictionary<int, List<int>>();
        List<int> spoken = new List<int>();
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

                numbers[arr[i]] = new List<int>{ turn };
                spoken.Add(arr[i]);
            }
        }

        private void SpeakNumber() {
            // If the previous number had never been spoken before (only one item in list)
            // then the next number is 0
            int last_spoken = spoken.Last();
            int next = 0;

            var position = numbers[last_spoken];

            if (position.Count == 1) {
                // 0!
            } else {
                // Need to get the difference between the last two numbers
                next = position[position.Count-1] - position[position.Count-2];
            }

            // Add to the list of spoke
            spoken.Add(next);

            // Increment the turn
            turn++;

            if (numbers.ContainsKey(next)) {
                numbers[next].Add(turn);
            } else {
                numbers[next] = new List<int>() { turn };
            }
        }

        protected override string SolvePartOne()
        {
            while(spoken.Count < 2020)
                SpeakNumber();

            return spoken.Last().ToString();
        }

        protected override string SolvePartTwo()
        {
            while(spoken.Count < 30000000)
                SpeakNumber();

            return spoken.Last().ToString();
        }
    }
}
