using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2019 {
    public enum State {
        NotInitialized = 0,
        Running = 1,
        Waiting = 2,
        Stopped = 3
    }

    public class Tile {
        public long x;
        public long y;
        public int tile_id;

        public Tile(long x, long y, int tile_id) {
            this.x = x;
            this.y = y;
            this.tile_id = tile_id;
        }
    }

    public enum Opcode {
        Add = 1,
        Multiply,
        Input,
        Output,
        JumpIfTrue,
        JumpIfFalse,
        LessThan,
        Equals,
        RelativeBase,
        Stop = 99
    }

    enum Mode { Position, Immediate, Relative }

    public class Intcode {
        // Holds the codes in memory
        public Dictionary<long, long> memory = new Dictionary<long, long>();

        // Holds the position in memory
        public long position = 0;

        // State of machine
        public State State = State.NotInitialized;

        // Will we stop when we have output? (>1 is yes)
        public int stopOnOutput = 1;

        // Output register
        public Queue<long> output_register;

        // Relative base
        public long relative_base = 0;

        // Holder for input
        public Queue<long> input;

        // Debug level
        public int debug_level = 0;

        public Intcode(long[] codes) {
            // Import the hashtable
            this.SetMemory(codes);
            this.stopOnOutput = 0;

            this.State = State.Waiting;

            this.input = new Queue<long>();
            this.output_register = new Queue<long>();
        }

        public Intcode(long[] codes, int stopOnOutput, int debugLevel = 0) {
            this.SetMemory(codes);
            this.stopOnOutput = stopOnOutput;

            this.State = State.Waiting;
            this.debug_level = debugLevel;

            this.input = new Queue<long>();
            this.output_register = new Queue<long>();
        }

        public Intcode(string codes) : this(codes.Split(',').Select(a => Int64.Parse(a)).ToArray()) {
            
        }

        public Intcode(string codes, int stopOnOutput, int debugLevel = 0) : this(codes.Split(',').Select(a => Int64.Parse(a)).ToArray(), stopOnOutput, debugLevel) {
            
        }

        public void SetMemory(long[] codes) {
            long k = 0;
            foreach(long c in codes) {
                this.memory.Add(k, c);
                k++;
            }
        }

        public void ClearInput() {
            this.input.Clear();
        }

        public void SetInput(long input) {
            this.input.Enqueue(input);
        }

        private long GetMemory(long pos) {
            if (!this.memory.Keys.Contains(pos)) {
                this.memory[pos] = 0;
            }

            return this.memory[pos];
        }

        private bool DoOperation((Opcode opcode, Mode[] modes) instruction) {
            Debug.WriteLineIf(debug_level > 0, string.Format("Position: {0}", this.position));

            // Check this is valid memory
            if (!this.memory.Keys.Contains(this.position)) {
                throw new System.Exception(string.Format("Invalid operation: No memory at {0}", this.position));
            }

            long code = (long) GetMemory(this.position);
            Debug.WriteLineIf(debug_level > 0, string.Format("Code: {0}", code));
            
            Debug.WriteLineIf(debug_level > 0, string.Format("Op Code: {0}", instruction.opcode));
            //Debug.WriteLineIf(debug_level > 0, string.Format("Param Mode: {0}", param_mode));

            long param_value1;
            long param_value2;

            long[] pointers;

            switch(instruction.opcode) {
                case Opcode.Add:
                    // Add (destination is position mode)
                    //this.memory[this.memory[i+3]] = this.memory[this.memory[i+1]] + this.memory[this.memory[i+2]]
                    pointers = this.ParseParams(instruction.modes, 3);

                    param_value1 = GetMemory(pointers[0]);
                    param_value2 = GetMemory(pointers[1]);
                    Debug.WriteLineIf(debug_level > 0, string.Format("Pointer 1: {0}", pointers[0]));
                    Debug.WriteLineIf(debug_level > 0, string.Format("Pointer 2: {0}", pointers[1]));
                    Debug.WriteLineIf(debug_level > 0, string.Format("Param 1: {0}", param_value1));
                    Debug.WriteLineIf(debug_level > 0, string.Format("Param 2: {0}", param_value2));
                    Debug.WriteLineIf(debug_level > 0, string.Format("Destination: {0}", pointers[2]));

                    this.memory[pointers[2]] = param_value1 + param_value2;
                    break;
                
                case Opcode.Multiply:
                    // Multiply (destination is position mode)
                    //this.memory[this.memory[i+3]] = this.memory[this.memory[i+1]] * this.memory[this.memory[i+2]]
                    pointers = this.ParseParams(instruction.modes, 3);

                    param_value1 = GetMemory(pointers[0]);
                    param_value2 = GetMemory(pointers[1]);
                    Debug.WriteLineIf(debug_level > 0, string.Format("Pointer 1: {0}", pointers[0]));
                    Debug.WriteLineIf(debug_level > 0, string.Format("Pointer 2: {0}", pointers[1]));
                    Debug.WriteLineIf(debug_level > 0, string.Format("Param 1: {0}", param_value1));
                    Debug.WriteLineIf(debug_level > 0, string.Format("Param 2: {0}", param_value2));
                    Debug.WriteLineIf(debug_level > 0, string.Format("Destination: {0}", pointers[2]));

                    this.memory[pointers[2]] = param_value1 * param_value2;
                    break;

                case Opcode.Input:
                    // Take an integer input and save it to somewhere (destination is position mode)
                    // See if we've been given inputs and if we have one for this
                    // If we only got one input, let's see if this is our first
                    if (this.input.Count > 0) {
                        this.State = State.Running;
                        this.memory[this.ParseParams(instruction.modes, 1)[0]] = this.input.Dequeue();
                    } else {
                        // Nope, let's read it
                        // long in = Read-Host -Prompt 'Provide an integer input: '
                        // We no longer read the input, we will return and wait
                        this.State = State.Waiting;
                        return false;
                        
                        // Don't increment our position because we will start back here
                    }
                    break;

                case Opcode.Output:
                    // Output an integer
                    this.output_register.Enqueue(GetMemory(ParseParams(instruction.modes, 1)[0]));

                    // We should return and wait for the next run command
                    // Move forward for the next time we come back
                    //this.position += 2;

                    // We may want to return a value and re-run
                    if (this.stopOnOutput > 1) {
                        this.State = State.Waiting;
                        return false;
                    }

                    // Returns the output and then we wait
                    break;

                case Opcode.JumpIfTrue:
                    // jump-if-true: if the first parameter is non-zero, it sets the instruction pointer to the value from the second parameter. Otherwise, it does nothing.
                    pointers = this.ParseParams(instruction.modes, 2);

                    param_value1 = GetMemory(pointers[0]);
                    param_value2 = GetMemory(pointers[1]);
                    Debug.WriteLineIf(debug_level > 0, string.Format("Pointer 1: {0}", pointers[0]));
                    Debug.WriteLineIf(debug_level > 0, string.Format("Pointer 2: {0}", pointers[1]));
                    Debug.WriteLineIf(debug_level > 0, string.Format("Param 1: {0}", param_value1));
                    Debug.WriteLineIf(debug_level > 0, string.Format("Param 2: {0}", param_value2));

                    if (param_value1 != 0) this.position = param_value2;
                    break;

                case Opcode.JumpIfFalse:
                    // jump-if-false: if the first parameter is zero, it sets the instruction pointer to the value from the second parameter. Otherwise, it does nothing.
                    pointers = this.ParseParams(instruction.modes, 2);

                    param_value1 = GetMemory(pointers[0]);
                    param_value2 = GetMemory(pointers[1]);
                    Debug.WriteLineIf(debug_level > 0, string.Format("Pointer 1: {0}", pointers[0]));
                    Debug.WriteLineIf(debug_level > 0, string.Format("Pointer 2: {0}", pointers[1]));
                    Debug.WriteLineIf(debug_level > 0, string.Format("Param 1: {0}", param_value1));
                    Debug.WriteLineIf(debug_level > 0, string.Format("Param 2: {0}", param_value2));

                    if (param_value1 == 0) this.position = param_value2;
                    break;

                case Opcode.LessThan:
                    // less-than: if the first parameter is less than the second parameter, it stores 1 in the position given by the third parameter. Otherwise, it stores 0.
                    pointers = this.ParseParams(instruction.modes, 3);

                    param_value1 = GetMemory(pointers[0]);
                    param_value2 = GetMemory(pointers[1]);
                    Debug.WriteLineIf(debug_level > 0, string.Format("Pointer 1: {0}", pointers[0]));
                    Debug.WriteLineIf(debug_level > 0, string.Format("Pointer 2: {0}", pointers[1]));
                    Debug.WriteLineIf(debug_level > 0, string.Format("Param 1: {0}", param_value1));
                    Debug.WriteLineIf(debug_level > 0, string.Format("Param 2: {0}", param_value2));

                    this.memory[pointers[2]] = (param_value1 < param_value2) ? 1 : 0;
                    break;

                case Opcode.Equals:
                    // equals: if the first parameter is equal to the second parameter, it stores 1 in the position given by the third parameter. Otherwise, it stores 0.
                    pointers = this.ParseParams(instruction.modes, 3);

                    param_value1 = GetMemory(pointers[0]);
                    param_value2 = GetMemory(pointers[1]);
                    Debug.WriteLineIf(debug_level > 0, string.Format("Pointer 1: {0}", pointers[0]));
                    Debug.WriteLineIf(debug_level > 0, string.Format("Pointer 2: {0}", pointers[1]));
                    Debug.WriteLineIf(debug_level > 0, string.Format("Param 1: {0}", param_value1));
                    Debug.WriteLineIf(debug_level > 0, string.Format("Param 2: {0}", param_value2));

                    this.memory[pointers[2]] = (param_value1 == param_value2) ? 1 : 0;
                    
                    break;

                case Opcode.RelativeBase:
                    // adjusts the relative base by the value of its only parameter. The relative base increases (or decreases, if the value is negative) by the value of the parameter.
                    pointers = this.ParseParams(instruction.modes, 1);
                    Debug.WriteLineIf(debug_level > 0, string.Format("Pointer 1: {0}", pointers[0]));
                    Debug.WriteLineIf(debug_level > 0, string.Format("Param 1: {0}", GetMemory(pointers[0])));

                    this.relative_base += GetMemory(pointers[0]);
                    break;
                
                case Opcode.Stop:
                    // Stop
                    this.State = State.Stopped;
                    return false;

                default:
                    throw new System.Exception(string.Format("Halt! Bad opcode at pos ({0}): {1}", this.position, instruction.opcode));
            }

            Debug.WriteLineIf(debug_level > 0, "");

            return true;
        }

        public void Run() {
            // We shouldn't be here
            if (this.State != State.Waiting && this.State != State.Running) {
                return;
            }

            // Reset to running
            this.State = State.Running;

            while(DoOperation(ParseInstruction(GetMemory(this.position))));
        }

        (Opcode opcode, Mode[] modes) ParseInstruction(long instruction) =>
        (
            (Opcode)(instruction % 100),
            instruction.ToString("D5").Remove(3).Select<char, Mode>(c => Enum.Parse<Mode>(c.ToString())).ToArray().Reverse().ToArray()
        );

        long[] ParseParams(Mode[] modes, int amount)
        {
            var result = new long[amount];
            for(int i = 0; i < amount; i++)
            {
                result[i] = modes[i] switch
                {
                    Mode.Position => GetMemory(++this.position),
                    Mode.Immediate => ++this.position,
                    Mode.Relative => GetMemory(++this.position) + this.relative_base,
                    _ => throw new Exception("Something went wrong")
                };
            }

            // Account for the last parameter
            this.position++;
            
            return result;
        }

        public void DrawPuzzle(System.Collections.Generic.List<Tile> Tiles, int score) {
            Console.Clear();

            for(int y=0; y<=Tiles.Max(em => em.y); y++) {
                for(int x=0; x<=Tiles.Max(em => em.x); x++) {
                    Tile? t = Tiles.Where(em => em.x == x && em.y == y).FirstOrDefault();

                    /*
                    0 is an empty tile. No game object appears in this tile.
                    1 is a wall tile. Walls are indestructible barriers.
                    2 is a block tile. Blocks can be broken by the ball.
                    3 is a horizontal paddle tile. The paddle is indestructible.
                    4 is a ball tile. The ball moves diagonally and bounces off objects.
                    */

                    switch(t?.tile_id ?? 5) {
                        case 0:
                            System.Console.Write(" ");
                            break;

                        case 1:
                            System.Console.Write("0");
                            break;

                        case 2:
                            System.Console.Write("#");
                            break;

                        case 3:
                            System.Console.Write("_");
                            break;

                        case 4:
                            System.Console.Write("o");
                            break;
                    }
                }
                
                System.Console.WriteLine(" ");
            }

            System.Console.WriteLine("Score: {0}", score);
        }
    }
}
