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

    public class Intcode {
        // Holds the codes in memory
        public Dictionary<long, long> memory = new Dictionary<long, long>();

        // Holds the position in memory
        public long position = 0;

        // State of machine
        public State State = State.NotInitialized;

        // Count the inputs used
        public int print_output = 1;

        // Output register
        public long output_register = Int64.MinValue;

        // Relative base
        public long relative_base = 0;

        // Holder for input
        public long? input = null;

        public Intcode(long[] codes) {
            // Import the hashtable
            this.SetMemory(codes);
            this.print_output = 0;

            this.State = State.Waiting;
        }

        public Intcode(long[] codes, int print_output) {
            this.SetMemory(codes);
            this.print_output = print_output;

            this.State = State.Waiting;
        }

        public void SetMemory(long[] codes) {
            long k = 0;
            foreach(long c in codes) {
                this.memory.Add(k, c);
                k++;
            }
        }

        public void SetInput(long input) {
            this.input = input;
        }

        public void Run() {
            // We shouldn't be here
            if (this.State != State.Waiting && this.State != State.Running) {
                return;
            }

            // Reset to running
            this.State = State.Running;

            // Do we need to return?
            bool ret = false;

            // Count the inputs used
            long in_index = 0;

            // Opcodes are every 4 spaces (0, 4, 8, etc.)
            for(long i=this.position; (i < this.memory.Keys.Max(em => em) && ret == false); ) {
                Debug.WriteLine("Position: {0}", i);

                // Check this is valid memory
                if (!this.memory.Keys.Contains(i)) {
                    ret = true;
                    throw new System.Exception(string.Format("Invalid operation: No memory at {0}", i));
                }

                long code = (long) this.memory[i];
                Debug.WriteLine(string.Format("Code: {0}", code));

                // Saving the param_mode and opcode
                long opcode;
                string param_mode;
                
                // Save the current position
                this.position = i;

                // Get the opcode
                if (code >= 100) {
                    Match Matches = Regex.Match(code.ToString(), "^([0-9]{0,3})([0-9]{2})");
                    opcode = Int64.Parse(Matches.Groups[2].Value);
                    param_mode = Matches.Groups[1].Value;
                } else {
                    opcode = (long) code;
                    param_mode = "0";
                }
                
                Debug.WriteLine(string.Format("Op Code: {0}", opcode));
                Debug.WriteLine(string.Format("Param Mode: {0}", param_mode));

                long param_value1;
                long param_value2;
                int ret_mode;
                long ret_pos;

                switch(opcode) {
                    case 1:
                        // Add (destination is position mode)
                        //this.memory[this.memory[i+3]] = this.memory[this.memory[i+1]] + this.memory[this.memory[i+2]]
                        ret_mode = this.GetParameterMode(3, param_mode);
                        ret_pos = this.GetParameterPosition((i+3), ret_mode);

                        param_value1 = this.GetParameterValue(1, (i+1), param_mode);
                        param_value2 = this.GetParameterValue(2, (i+2), param_mode);
                        Debug.WriteLine(string.Format("Param 1: {0}", param_value1));
                        Debug.WriteLine(string.Format("Param 2: {0}", param_value2));
                        Debug.WriteLine(string.Format("Destination: {0}", ret_pos));

                        this.memory[ret_pos] = param_value1 + param_value2;
                        i += 4;
                        break;
                    
                    case 2:
                        // Multiply (destination is position mode)
                        //this.memory[this.memory[i+3]] = this.memory[this.memory[i+1]] * this.memory[this.memory[i+2]]
                        ret_mode = this.GetParameterMode(3, param_mode);
                        ret_pos = this.GetParameterPosition((i+3), ret_mode);

                        param_value1 = this.GetParameterValue(1, (i+1), param_mode);
                        param_value2 = this.GetParameterValue(2, (i+2), param_mode);
                        Debug.WriteLine(string.Format("Param 1: {0}", param_value1));
                        Debug.WriteLine(string.Format("Param 2: {0}", param_value2));
                        Debug.WriteLine(string.Format("Destination: {0}", ret_pos));

                        this.memory[ret_pos] = param_value1 * param_value2;
                        i += 4;
                        break;

                    case 3:
                        // Take an integer input and save it to somewhere (destination is position mode)
                        // See if we've been given inputs and if we have one for this
                        // If we only got one input, let's see if this is our first
                        if (in_index == 0 && this.input.HasValue) {
                            in_index++;

                            this.State = State.Running;
                            ret_mode = this.GetParameterMode(1, param_mode);
                            this.memory[this.GetParameterPosition((i+1), ret_mode)] = this.input.Value;
                            i += 2;
                        } else {
                            // Nope, let's read it
                            // long in = Read-Host -Prompt 'Provide an integer input: '
                            // We no longer read the input, we will return and wait
                            this.State = State.Waiting;
                            this.input = null;
                            ret = true;
                            
                            // Don't increment our position because we will start back here
                        }
                        break;

                    case 4:
                        // Output an integer
                        this.output_register = this.GetParameterValue(1, (i+1), param_mode);
                        if (this.print_output == 1) {
                            Debug.WriteLine(string.Format("Output: {0}", this.output_register));
                        }

                        // We should return and wait for the next run command
                        // Move forward for the next time we come back
                        i += 2;

                        // We may want to return a value and re-run
                        if (this.print_output > 1) {
                            this.position += 2;
                            this.State = State.Waiting;
                            ret = true;
                        }

                        // Returns the output and then we wait
                        break;

                    case 5:
                        // jump-if-true: if the first parameter is non-zero, it sets the instruction pointer to the value from the second parameter. Otherwise, it does nothing.
                        param_value1 = this.GetParameterValue(1, (i+1), param_mode);
                        param_value2 = this.GetParameterValue(2, (i+2), param_mode);
                        Debug.WriteLine(string.Format("Param 1: {0}", param_value1));
                        Debug.WriteLine(string.Format("Param 2: {0}", param_value2));

                        if (param_value1 != 0) {
                            i = param_value2;
                        } else {
                            i += 3;
                        }
                        break;

                    case 6:
                        // jump-if-false: if the first parameter is zero, it sets the instruction pointer to the value from the second parameter. Otherwise, it does nothing.
                        param_value1 = this.GetParameterValue(1, (i+1), param_mode);
                        param_value2 = this.GetParameterValue(2, (i+2), param_mode);
                        Debug.WriteLine(string.Format("Param 1: {0}", param_value1));
                        Debug.WriteLine(string.Format("Param 2: {0}", param_value2));

                        if (param_value1 == 0) {
                            i = param_value2;
                        } else {
                            i += 3;
                        }
                        break;

                    case 7:
                        // less-than: if the first parameter is less than the second parameter, it stores 1 in the position given by the third parameter. Otherwise, it stores 0.
                        param_value1 = this.GetParameterValue(1, (i+1), param_mode);
                        param_value2 = this.GetParameterValue(2, (i+2), param_mode);
                        Debug.WriteLine(string.Format("Param 1: {0}", param_value1));
                        Debug.WriteLine(string.Format("Param 2: {0}", param_value2));

                        ret_mode = this.GetParameterMode(3, param_mode);
                        if (param_value1 < param_value2) {
                            this.memory[this.GetParameterPosition((i+3), ret_mode)] = 1;
                            i += 4;
                        } else {
                            this.memory[this.GetParameterPosition((i+3), ret_mode)] = 0;
                            i += 4;
                        }
                        break;

                    case 8:
                        // equals: if the first parameter is equal to the second parameter, it stores 1 in the position given by the third parameter. Otherwise, it stores 0.
                        param_value1 = this.GetParameterValue(1, (i+1), param_mode);
                        param_value2 = this.GetParameterValue(2, (i+2), param_mode);
                        Debug.WriteLine(string.Format("Param 1: {0}", param_value1));
                        Debug.WriteLine(string.Format("Param 2: {0}", param_value2));

                        ret_mode = this.GetParameterMode(3, param_mode);
                        if (param_value1 == param_value2) {
                            this.memory[this.GetParameterPosition((i+3), ret_mode)] = 1;
                            i += 4;
                        } else {
                            this.memory[this.GetParameterPosition((i+3), ret_mode)] = 0;
                            i += 4;
                        }
                        break;

                    case 9:
                        // adjusts the relative base by the value of its only parameter. The relative base increases (or decreases, if the value is negative) by the value of the parameter.
                        param_value1 = this.GetParameterValue(1, (i+1), param_mode);
                        Debug.WriteLine(string.Format("Param 1: {0}", param_value1));

                        this.relative_base += param_value1;
                        i += 2;
                        break;
                    
                    case 99:
                        // Add
                        ret = true;
                        i += 4;
                        this.State = State.Stopped;
                        break;

                    default:
                        throw new System.Exception(string.Format("Halt! Bad opcode at pos ({0}): {1}", i, opcode));
                }

                Debug.WriteLine("");
            }
        }

        public int GetParameterMode(int param_num, string param_mode) {
            // Get the parameter mode
            // Pad the string to ensure we have enough characters
            param_mode = param_mode.PadLeft(param_num, '0');
            return Int32.Parse(param_mode.Substring(param_mode.Length - param_num, 1));
        }

        public long GetParameterPosition(long index, int param_mode) {
            switch (param_mode) {
                case 0:
                    // Position mode
                    // Index tells us where the parameter is in this.memory
                    // Then we return the value referenced by that position
                    return ((long) this.memory[index]);
        
                case 1:
                    // Immediate mode
                    // Index tells us where the parameter is in this.memory
                    // Then we return the value of that position
                    return index;

                case 2:
                    // Relative mode
                    // Take the relative_base and add the value of the param
                    return ((long) this.memory[index]) + this.relative_base;

                default:
                    throw new System.Exception("[GetParameterPosition] Invalid param_mode: param_mode");
            }
        }
        
        public long GetParameterValue(int param_num, long index, string param_mode) {
            /*
            // The memory position of the parameter
            index

            // Is this the first param, second param, etc. of the opcode
            int param_num

            // A number representing the param mode
            // Singles = first param mode
            // Tens = second param mode
            // Hundreds = second param mode
            // Thousands = second param mode
            string param_mode
            */
        
            int l_param_mode = this.GetParameterMode(param_num, param_mode);

            long p = this.GetParameterPosition(index, l_param_mode);
            
            return ((long) this.memory[p]);
        }

        public void DrawPuzzle(System.Collections.Generic.List<Tile> Tiles, int score) {
            Console.Clear();

            for(int y=0; y<=Tiles.Max(em => em.y); y++) {
                for(int x=0; x<=Tiles.Max(em => em.x); x++) {
                    Tile t = Tiles.Where(em => em.x == x && em.y == y).FirstOrDefault();

                    /*
                    0 is an empty tile. No game object appears in this tile.
                    1 is a wall tile. Walls are indestructible barriers.
                    2 is a block tile. Blocks can be broken by the ball.
                    3 is a horizontal paddle tile. The paddle is indestructible.
                    4 is a ball tile. The ball moves diagonally and bounces off objects.
                    */

                    switch(t.tile_id) {
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