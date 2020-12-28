using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2018
{
    enum WristOpCode {
        addr,
        addi,
        mulr,
        muli,
        banr,
        bani,
        borr,
        bori,
        setr,
        seti,
        gtir,
        gtri,
        gtrr,
        eqir,
        eqri,
        eqrr
    }

    enum WristInstruction {
        op,
        A,
        B,
        C
    }

    class Day16 : ASolution
    {

        public Day16() : base(16, 2018, "")
        {
            /*
            DebugInput = @"
            Before: [3, 2, 1, 1]
            9 2 1 2
            After:  [3, 2, 2, 1]";
            */
        }

        private int[] getState(string input)
            // Samples:
            // Before: [3, 2, 1, 1]
            // After:  [3, 2, 2, 1]
            => input.Split("[")[1].Replace("]", "").Replace(" ", "").ToIntArray(",");

        private List<WristOpCode> identifyOpCode(string[] sample) {
            // We should receive something like this:
            /*
                Before: [3, 2, 1, 1]
                9 2 1 2
                After:  [3, 2, 2, 1]
            */

            int[] before = this.getState(sample[0]);
            int[] ops = sample[1].ToIntArray(" ");
            int[] after = this.getState(sample[2]);

            // Now we know what the registers were before and after along with the instructions
            List<WristOpCode> ret = new List<WristOpCode>();

            // Need to go through each possible value of WristOpCode and check it
            int min = (int) Enum.GetValues(typeof(WristOpCode)).Cast<WristOpCode>().Min();
            int max = (int) Enum.GetValues(typeof(WristOpCode)).Cast<WristOpCode>().Max();
            for(int i=min; i<=max; i++) {
                var code = (WristOpCode) i;
                bool possible = false;

                switch(code) {
                    case WristOpCode.addr:
                        possible = after[ops[(int) WristInstruction.C]] == (before[ops[(int) WristInstruction.A]] + before[ops[(int) WristInstruction.B]]);
                        break;
                    
                    case WristOpCode.addi:
                        possible = after[ops[(int) WristInstruction.C]] == (before[ops[(int) WristInstruction.A]] + ops[(int) WristInstruction.B]);
                        break;
                    
                    case WristOpCode.mulr:
                        possible = after[ops[(int) WristInstruction.C]] == (before[ops[(int) WristInstruction.A]] * before[ops[(int) WristInstruction.B]]);
                        break;
                    
                    case WristOpCode.muli:
                        possible = after[ops[(int) WristInstruction.C]] == (before[ops[(int) WristInstruction.A]] * ops[(int) WristInstruction.B]);
                        break;
                    
                    case WristOpCode.banr:
                        possible = after[ops[(int) WristInstruction.C]] == (before[ops[(int) WristInstruction.A]] & before[ops[(int) WristInstruction.B]]);
                        break;
                    
                    case WristOpCode.bani:
                        possible = after[ops[(int) WristInstruction.C]] == (before[ops[(int) WristInstruction.A]] & ops[(int) WristInstruction.B]);
                        break;
                    
                    case WristOpCode.borr:
                        possible = after[ops[(int) WristInstruction.C]] == (before[ops[(int) WristInstruction.A]] ^ before[ops[(int) WristInstruction.B]]);
                        break;
                    
                    case WristOpCode.bori:
                        possible = after[ops[(int) WristInstruction.C]] == (before[ops[(int) WristInstruction.A]] ^ ops[(int) WristInstruction.B]);
                        break;
                    
                    case WristOpCode.setr:
                        possible = after[ops[(int) WristInstruction.C]] == before[ops[(int) WristInstruction.A]] && after[(int) WristInstruction.B] == after[(int) WristInstruction.B];
                        break;
                    
                    case WristOpCode.seti:
                        possible = after[ops[(int) WristInstruction.C]] == ops[(int) WristInstruction.A] && after[(int) WristInstruction.B] == after[(int) WristInstruction.B];
                        break;
                    
                    case WristOpCode.gtir:
                        possible = (
                            (ops[(int) WristInstruction.A] > before[ops[(int) WristInstruction.B]] && after[ops[(int) WristInstruction.C]] == 1)
                            ||
                            (ops[(int) WristInstruction.A] <= before[ops[(int) WristInstruction.B]] && after[ops[(int) WristInstruction.C]] == 0)
                        );
                        break;
                    
                    case WristOpCode.gtri:
                        possible = (
                            (before[ops[(int) WristInstruction.A]] > ops[(int) WristInstruction.B] && after[ops[(int) WristInstruction.C]] == 1)
                            ||
                            (before[ops[(int) WristInstruction.A]] <= ops[(int) WristInstruction.B] && after[ops[(int) WristInstruction.C]] == 0)
                        );
                        break;
                    
                    case WristOpCode.gtrr:
                        possible = (
                            (before[ops[(int) WristInstruction.A]] > before[ops[(int) WristInstruction.B]] && after[ops[(int) WristInstruction.C]] == 1)
                            ||
                            (before[ops[(int) WristInstruction.A]] <= before[ops[(int) WristInstruction.B]] && after[ops[(int) WristInstruction.C]] == 0)
                        );
                        break;
                    
                    case WristOpCode.eqir:
                        possible = (
                            (ops[(int) WristInstruction.A] == before[ops[(int) WristInstruction.B]] && after[ops[(int) WristInstruction.C]] == 1)
                            ||
                            (ops[(int) WristInstruction.A] != before[ops[(int) WristInstruction.B]] && after[ops[(int) WristInstruction.C]] == 0)
                        );
                        break;
                    
                    case WristOpCode.eqri:
                        possible = (
                            (before[ops[(int) WristInstruction.A]] == ops[(int) WristInstruction.B] && after[ops[(int) WristInstruction.C]] == 1)
                            ||
                            (before[ops[(int) WristInstruction.A]] != ops[(int) WristInstruction.B] && after[ops[(int) WristInstruction.C]] == 0)
                        );
                        break;
                    
                    case WristOpCode.eqrr:
                        possible = (
                            (before[ops[(int) WristInstruction.A]] == before[ops[(int) WristInstruction.B]] && after[ops[(int) WristInstruction.C]] == 1)
                            ||
                            (before[ops[(int) WristInstruction.A]] != before[ops[(int) WristInstruction.B]] && after[ops[(int) WristInstruction.C]] == 0)
                        );
                        break;
                    
                }

                if (possible) ret.Add(code);
            }

            return ret;
        }

        protected override string SolvePartOne()
        {
            // In this input, example sets are split from example code with 4 \n's
            var examples = Input.Split("\n\n\n\n")[0].Trim();

            // For each sample, count if they match 3 or more possibilities
            List<List<string>> samples = new List<List<string>>();
            List<List<WristOpCode>> sampleMatches = new List<List<WristOpCode>>();

            foreach(var sample in examples.SplitByBlankLine(true)) {
                samples.Add(sample.ToList());
                sampleMatches.Add(this.identifyOpCode(sample));
            }

            sampleMatches.ForEach(a => Console.WriteLine(string.Join(", ", a)));

            Console.WriteLine($"Samples Count: {samples.Count}");

            return sampleMatches.Count(a => a.Count >= 3).ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
