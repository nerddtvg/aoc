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

    class GameComputer {
        public List<(string instruction, int value)> instructions = new List<(string instruction, int value)>();

        private int position = 0;
        public int accumulator = 0;

        private List<int> visited = new List<int>();

        public GameComputer(string input) {
            foreach(string line in input.SplitByNewline()) {
                string[] parts = line.Trim().Split(" ");
                instructions.Add((parts[0], Int32.Parse(parts[1])));
            }

            // Start at the beginning
            position = 0;
            accumulator = 0;
        }

        public bool DoOperation() {
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
    }

    class Day08 : ASolution
    {
        GameComputer computer;

        public Day08() : base(08, 2020, "")
        {
            computer = new GameComputer(Input);
        }

        protected override string SolvePartOne()
        {
            // Go through each step
            while(computer.DoOperation()) {};

            return computer.accumulator.ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
