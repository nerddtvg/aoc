using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2018
{

    class Day05 : ASolution
    {

        public Day05() : base(05, 2018, "")
        {

        }

        protected override string SolvePartOne()
        {
            return RunPolymer(Input).Length.ToString();
        }

        protected override string SolvePartTwo()
        {
            // Get a list of all of our different units (char)
            char[] units = Input.ToLower().ToCharArray().Distinct().ToArray();

            int lowest = Int32.MaxValue;

            // For each of these units, let's run a test
            foreach(char u in units) {
                // Remove the unit from the input
                string i2 = Input.Replace(u.ToString(), "").Replace(u.ToString().ToUpper(), "");
                int length = RunPolymer(i2).Length;

                // Is this the lowest?
                if (length < lowest)
                    lowest = length;
            }

            return lowest.ToString();
        }

        protected string RunPolymer(string input) {
            // Any adjoining positions of opposite polarity (upper versus lower-case) remove each other
            // Loop this until no more can be done
            List<char> inputArr = input.ToCharArray().ToList();

            while(true) {
                // Track if we have removed anything
                bool removed = false;
                int i;
                List<int> removePos = new List<int>();
                
                for(i=0; i<inputArr.Count-1; i++) {
                    // Check this letter and the next one
                    // Integers are 65-90 [A-Z] and 97-122 [a-z]
                    if (Math.Abs((int) inputArr[i] - (int) inputArr[i+1]) == 32) {
                        removed = true;
                        
                        // Add this to the list of those to remove
                        removePos.Add(i);
                        removePos.Add(i+1);

                        // Skip ahead
                        i = i + 1;
                    }
                }

                if (removed) {
                    // Remove i and i+1
                    removePos.Reverse();
                    removePos.ForEach(a => inputArr.RemoveAt(a));
                } else {
                    break;
                }
            }

            return string.Join("", inputArr);
        }
    }
}
