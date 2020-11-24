using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

using AdventOfCode.Solutions.Year2019;

namespace AdventOfCode.Solutions.Year2019
{

    class Day07 : ASolution
    {

        public Day07() : base(07, 2019, "")
        {

        }

        protected override string SolvePartOne()
        {
            List<List<int>> phases = new List<List<int>>();
            List<int> maxPhase = new List<int>();
            long maxThrust = 0;

            // All possible phases
            for(int a = 0; a <= 4; a++)
                for(int b = 0; b <= 4; b++)
                    for(int c = 0; c <= 4; c++)
                        for(int d = 0; d <= 4; d++)
                            for(int e = 0; e <= 4; e++) {
                                var l = new List<int>() { a, b, c, d, e};
                                if (l.Distinct().Count() == 5)
                                    // Remove anything where the phase setting is used more than once
                                    phases.Add(l);
                            }

            foreach(var p in phases) {
                long out1, out2, out3, out4, out5;

                Intcode intcode1 = new Intcode(Input, 2);
                Intcode intcode2 = new Intcode(Input, 2);
                Intcode intcode3 = new Intcode(Input, 2);
                Intcode intcode4 = new Intcode(Input, 2);
                Intcode intcode5 = new Intcode(Input, 2);

                intcode1.SetInput(p[0]);
                intcode1.Run();

                // Set to initial 0
                intcode1.SetInput(0);
                intcode1.Run();

                out1 = intcode1.output_register;

                intcode2.SetInput(p[1]);
                intcode2.Run();

                // Set to Amp 1 output
                intcode2.SetInput(out1);
                intcode2.Run();

                out2 = intcode2.output_register;

                intcode3.SetInput(p[2]);
                intcode3.Run();

                // Set to Amp 2 output
                intcode3.SetInput(out2);
                intcode3.Run();

                out3 = intcode3.output_register;

                intcode4.SetInput(p[3]);
                intcode4.Run();

                // Set to Amp 3 output
                intcode4.SetInput(out3);
                intcode4.Run();

                out4 = intcode4.output_register;

                intcode5.SetInput(p[4]);
                intcode5.Run();

                // Set to Amp 4 output
                intcode5.SetInput(out4);
                intcode5.Run();

                out5 = intcode5.output_register;

                if (out5 > maxThrust) {
                    maxThrust = out5;
                    maxPhase = p;
                }
            }

            return maxThrust.ToString();
        }

        protected override string SolvePartTwo()
        {
            List<List<int>> phases = new List<List<int>>();
            List<int> maxPhase = new List<int>();
            long maxThrust = 0;

            // All possible phases
            for(int a = 5; a <= 9; a++)
                for(int b = 5; b <= 9; b++)
                    for(int c = 5; c <= 9; c++)
                        for(int d = 5; d <= 9; d++)
                            for(int e = 5; e <= 9; e++) {
                                var l = new List<int>() { a, b, c, d, e};
                                if (l.Distinct().Count() == 5)
                                    // Remove anything where the phase setting is used more than once
                                    phases.Add(l);
                            }

            foreach(var p in phases) {
                List<Intcode> computers = new List<Intcode>() {
                    new Intcode(Input, 2),
                    new Intcode(Input, 2),
                    new Intcode(Input, 2),
                    new Intcode(Input, 2),
                    new Intcode(Input, 2)
                };

                computers[0].SetInput(p[0]);
                computers[0].Run();

                computers[1].SetInput(p[1]);
                computers[1].Run();

                computers[2].SetInput(p[2]);
                computers[2].Run();

                computers[3].SetInput(p[3]);
                computers[3].Run();

                computers[4].SetInput(p[4]);
                computers[4].Run();

                List<long> lastOut = new List<long>() { 0, 0, 0, 0, 0};

                // Go through each computer and run it until we're done
                int i = 0;
                while(true) {
                    int outIndex = i == 0 ? 4 : i-1;

                    // Check if we aren't running
                    if (computers[i].State == State.Waiting) {
                        // Set the input
                        computers[i].SetInput(lastOut[outIndex]);
                        computers[i].Run();

                        // Get the output
                        lastOut[i] = computers[i].output_register;
                    }

                    // If this is the last computer, check for output
                    if (i == 4 && computers[i].State == State.Stopped) {
                        break;
                    }

                    // Increment
                    i = (i+1) % 5;
                }

                if (computers[4].output_register > maxThrust) {
                    maxThrust = computers[4].output_register;
                    maxPhase = p;
                }
            }

            return maxThrust.ToString();
        }
    }
}
