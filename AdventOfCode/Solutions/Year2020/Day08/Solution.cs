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

        public GameComputerOps ParseInstructionName(string name) {
            return (GameComputerOps) Enum.Parse(typeof(GameComputerOps), name, true);
        }

        public int DoOperation() {
            // Lookup the next step, check if we have done it before and fail out (0)
            if (this.visited.Contains(this.position)) return 0;

            // We may be done, if so, return 2
            if (this.position >= this.instructions.Count) return 2;

            // If not, do it!
            var ins = instructions[this.position];

            // Add it to the list
            this.visited.Add(this.position);

            switch(ParseInstructionName(ins.instruction)) {
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

            return 1;
        }
    }

    class Day08 : ASolution
    {
        GameComputer computer;
        Dictionary<int, GameComputerOps> instructions;

        public Day08() : base(08, 2020, "")
        {
            // Load the original computer
            computer = new GameComputer(Input);

            // Let's note all of the positions of jmp and nop
            instructions = new Dictionary<int, GameComputerOps>();
            for(int i=0; i<computer.instructions.Count; i++) {
                var name = computer.ParseInstructionName(computer.instructions[i].instruction);

                // Skip acc instructions for Part 2
                if (name == GameComputerOps.acc) continue;

                // Note the position and instruction we have (to possibly replace)
                instructions.Add(i, name);
            }
        }

        protected override string SolvePartOne()
        {
            // Go through each step
            while(computer.DoOperation() == 1) {};

            return computer.accumulator.ToString();
        }

        protected override string SolvePartTwo()
        {
            // For each instruction to replace, we will re-run this computer again and again
            foreach(var kvp in instructions) {
                // Reinitialize from the beginning
                computer = new GameComputer(Input);

                // Change this one instruction (must be done indirectly since I'm not using a class)
                var current = computer.instructions[kvp.Key];
                current.instruction = (kvp.Value == GameComputerOps.jmp ? "nop" : "jmp");
                computer.instructions[kvp.Key] = current;

                // Go through each step
                int ret = computer.DoOperation();
                while(ret == 1) {
                    ret = computer.DoOperation();
                }

                // Check if we exited normally
                if (ret == 2) break;

                // Otherwise, loop again and try over
            }

            return computer.accumulator.ToString();
        }
    }
}
