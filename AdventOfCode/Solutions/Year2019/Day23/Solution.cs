using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2019
{

    class Day23 : ASolution
    {
        Dictionary<int, Intcode> computers = new Dictionary<int, Intcode>();

        public Day23() : base(23, 2019, "")
        {
            // Initialize 50 computers
            for(int i=0; i<50; i++) {
                // Add the computer
                // We don't add the stop for output here because we want it to keep going
                // The only wait state will be input
                computers.Add(i, new Intcode(Input.ToLongArray(",")));

                // Initialize it with its network address
                computers[i].input.Enqueue(i);

                //computers[i].debug_level = 2;
                
                // Run it
                computers[i].Run();
            }
        }

        protected override string SolvePartOne()
        {
            // For each computer, check if it is waiting, if so send -1
            // Check if there is output, if so, parse and handle it
            // Check if the output is going to address 255
            while(true) {
                for(int i=0; i<50; i++) {
                    // Check the output
                    while (computers[i].output_register.Count >= 3) {
                        int address = Convert.ToInt32(computers[i].output_register.Dequeue());
                        long x = computers[i].output_register.Dequeue();
                        long y = computers[i].output_register.Dequeue();

                        if (address == 255) {
                            // End game
                            return y.ToString();
                        }

                        computers[address].input.Enqueue(x);
                        computers[address].input.Enqueue(y);
                    }

                    if (computers[i].State == State.Waiting) {
                        // Need input, nothing to give, give it -1
                        if (computers[i].input.Count == 0)
                            computers[i].input.Enqueue(-1);

                        computers[i].Run();
                    }
                }
            }
        }

        protected override string SolvePartTwo()
        {
            return string.Empty;
        }
    }
}
