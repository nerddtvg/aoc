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
            /** /
            DebugInput = @"
            Before: [3, 2, 1, 1]
            9 2 1 2
            After:  [3, 2, 2, 1]";
            /**/
        }

        private int[] getState(string input)
            // Samples:
            // Before: [3, 2, 1, 1]
            // After:  [3, 2, 2, 1]
            => input.Split("[")[1].Replace("]", "").Replace(" ", "").ToIntArray(",");
        
        private List<int> performOperation(List<int> registers, List<int> ops, WristOpCode? code = null) {
            // The code override is used for testing in Part 1
            if (!code.HasValue)
                code = (WristOpCode) ops[(int) WristInstruction.op];

            // Easier referencing
            var a = ops[(int) WristInstruction.A];
            var b = ops[(int) WristInstruction.B];
            var c = ops[(int) WristInstruction.C];

            switch(code) {
                case WristOpCode.addr:
                    registers[c] = (registers[a] + registers[b]);
                    break;
                
                case WristOpCode.addi:
                    registers[c] = (registers[a] + b);
                    break;
                
                case WristOpCode.mulr:
                    registers[c] = (registers[a] * registers[b]);
                    break;
                
                case WristOpCode.muli:
                    registers[c] = (registers[a] * b);
                    break;
                
                case WristOpCode.banr:
                    registers[c] = (registers[a] & registers[b]);
                    break;
                
                case WristOpCode.bani:
                    registers[c] = (registers[a] & b);
                    break;
                
                case WristOpCode.borr:
                    registers[c] = (registers[a] | registers[b]);
                    break;
                
                case WristOpCode.bori:
                    registers[c] = (registers[a] | b);
                    break;
                
                case WristOpCode.setr:
                    registers[c] = registers[a];
                    break;
                
                case WristOpCode.seti:
                    registers[c] = a;
                    break;
                
                case WristOpCode.gtir:
                    registers[c] = (a > registers[b] ? 1 : 0);
                    break;
                
                case WristOpCode.gtri:
                    registers[c] = (registers[a] > b ? 1 : 0);
                    break;
                
                case WristOpCode.gtrr:
                    registers[c] = (registers[a] > registers[b] ? 1 : 0);
                    break;
                
                case WristOpCode.eqir:
                    registers[c] = (a == registers[b] ? 1 : 0);
                    break;
                
                case WristOpCode.eqri:
                    registers[c] = (registers[a] == b ? 1 : 0);
                    break;
                
                case WristOpCode.eqrr:
                    registers[c] = (registers[a] == registers[b] ? 1 : 0);
                    break;
            }

            return registers;
        }

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
                // What code are we testing?
                var code = (WristOpCode) i;

                // Get the results
                var afterList = this.performOperation(before.ToList(), ops.ToList(), code);

                // Check the results
                if (afterList.SequenceEqual(after)) ret.Add(code);
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
