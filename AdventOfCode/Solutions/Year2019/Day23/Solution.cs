using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2019
{

    class Day23 : ASolution
    {
        List<Intcode> computers = new();

        public Day23() : base(23, 2019, "Category Six")
        {

        }

        private void ResetComputers()
        {
            computers = new();

            // Initialize 50 computers
            for (int i = 0; i < 50; i++)
            {
                // Add the computer
                // We don't add the stop for output here because we want it to keep going
                // The only wait state will be input
                computers.Add(new Intcode(Input));

                // Initialize it with its network address
                computers[i].SetInput(i);

                // Run it
                computers[i].Run();
            }
        }

        protected override string SolvePartOne()
        {
            ResetComputers();

            // For each computer, check if it is waiting, if so send -1
            // Check if there is output, if so, parse and handle it
            // Check if the output is going to address 255
            while(true) {
                for (int i = 0; i < 50; i++)
                {
                    // Check the output
                    while (computers[i].output_register.Count >= 3)
                    {
                        int address = Convert.ToInt32(computers[i].output_register.Dequeue());
                        long x = computers[i].output_register.Dequeue();
                        long y = computers[i].output_register.Dequeue();

                        if (address == 255)
                        {
                            // End game
                            return y.ToString();
                        }

                        computers[address].input.Enqueue(x);
                        computers[address].input.Enqueue(y);
                    }
                }

                // Now queue up any missing items
                // then run the computers
                for (int i = 0; i < 50; i++)
                {
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
            ResetComputers();

            var nat = new Queue<long>();
            var natDuplicates = new HashSet<long>();

            // For each computer, check if it is waiting, if so send -1
            // Check if there is output, if so, parse and handle it
            // Check if the output is going to address 255
            while (true)
            {
                for (int i = 0; i < 50; i++)
                {
                    // Check the output
                    while (computers[i].output_register.Count >= 3)
                    {
                        int address = Convert.ToInt32(computers[i].output_register.Dequeue());
                        long x = computers[i].output_register.Dequeue();
                        long y = computers[i].output_register.Dequeue();

                        if (address == 255)
                        {
                            // Clear NAT and append new values
                            nat.Clear();
                            nat.Enqueue(x);
                            nat.Enqueue(y);
                        }
                        else
                        {
                            computers[address].SetInput(x);
                            computers[address].SetInput(y);
                        }
                    }
                }

                // Now queue up any missing items
                // then run the computers
                for (int i = 0; i < 50; i++)
                {
                    if (computers[i].State == State.Waiting)
                    {
                        // Need input, nothing to give, give it -1
                        if (computers[i].input.Count == 0)
                            computers[i].input.Enqueue(-1);
                    }
                }

                // Determine if all computers are idle
                // Could be -1, but we should always have at least 2
                if (computers.All(c => c.State == State.Waiting && c.input.Count < 2) && nat.Count > 0)
                {
                    // Send the nat queue to address zero
                    computers[0].ClearInput();
                    
                    // Send x
                    computers[0].SetInput(nat.Dequeue());

                    // Send and save y
                    var y = nat.Dequeue();
                    computers[0].SetInput(y);

                    if (natDuplicates.Contains(y))
                    {
                        return y.ToString();
                    }

                    natDuplicates.Add(y);
                }

                // Now queue up any missing items
                // then run the computers
                for (int i = 0; i < 50; i++)
                {
                    if (computers[i].State == State.Waiting)
                    {
                        computers[i].Run();
                    }
                }
            }
        }
    }
}
