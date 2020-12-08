using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{
    enum GameComputerOps {
        nop,
        acc,
        jmp

    }

    class Day08 : ASolution
    {
        List<(string instruction, int value)> instructions = new List<(string instruction, int value)>();

        int position = 0;
        int accumulator = 0;

        List<int> visited = new List<int>();

        public Day08() : base(08, 2020, "")
        {
            foreach(string line in Input.SplitByNewline()) {
                string[] parts = line.Trim().Split(" ");
                instructions.Add((parts[0], Int32.Parse(parts[1])));
            }

            // Start at the beginning
            position = 0;
            accumulator = 0;
        }

        private bool DoOperation() {
            // Lookup the next step, check if we have done it before and fail out (false)
            if (this.visited.Contains(this.position)) return false;

            // If not, do it!
            var ins = instructions[this.position];

            // Add it to the list
            this.visited.Add(this.position);

            switch((GameComputerOps) Enum.Parse(typeof(GameComputerOps), ins.instruction, true)) {
                case GameComputerOps.acc:
                    this.position += 1;
                    this.accumulator += ins.value;
                    break;
                    
                case GameComputerOps.nop:
                    this.position += 1;
                    break;
                    
                case GameComputerOps.jmp:
                    this.position += ins.value;
                    break;
                
                default:
                    throw new Exception($"Invalid instruction: {ins.instruction}");
            }

            return true;
        }

        protected override string SolvePartOne()
        {
            // Go through each step
            while(DoOperation()) {};

            return this.accumulator.ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
