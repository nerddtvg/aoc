using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2017
{

    class Day25 : ASolution
    {
        public class TuringMachine
        {
            public enum State
            {
                A,
                B,
                C,
                D,
                E,
                F
            }

            // Starts in State A
            public State state = State.A;

            // Linked List may get too big, so let's use a dictionary and only set what we need
            public Dictionary<int, int> tape = new Dictionary<int, int>() { { 0, 0 } };

            // Cursor starts at zero
            public int cursor = 0;

            public void Run(bool debug = false)
            {
                if (debug == true)
                {
                    if (state == State.A)
                    {
                        if (GetValue() == 0)
                        {
                            this.tape[this.cursor] = 1;
                            this.cursor++;
                            this.state = State.B;
                        }
                        else
                        {
                            this.tape[this.cursor] = 0;
                            this.cursor--;
                            this.state = State.B;
                        }
                    }
                    else if (state == State.B)
                    {
                        if (GetValue() == 0)
                        {
                            this.tape[this.cursor] = 1;
                            this.cursor--;
                            this.state = State.A;
                        }
                        else
                        {
                            this.tape[this.cursor] = 1;
                            this.cursor++;
                            this.state = State.A;
                        }
                    }
                    return;
                }

                // Instructions from the input
                if (state == State.A)
                {
                    if (GetValue() == 0)
                    {
                        this.tape[this.cursor] = 1;
                        this.cursor++;
                        this.state = State.B;
                    }
                    else
                    {
                        this.tape[this.cursor] = 0;
                        this.cursor--;
                        this.state = State.C;
                    }
                }
                else if (state == State.B)
                {
                    if (GetValue() == 0)
                    {
                        this.tape[this.cursor] = 1;
                        this.cursor--;
                        this.state = State.A;
                    }
                    else
                    {
                        this.tape[this.cursor] = 1;
                        this.cursor++;
                        this.state = State.D;
                    }
                }
                else if (state == State.C)
                {
                    if (GetValue() == 0)
                    {
                        this.tape[this.cursor] = 0;
                        this.cursor--;
                        this.state = State.B;
                    }
                    else
                    {
                        this.tape[this.cursor] = 0;
                        this.cursor--;
                        this.state = State.E;
                    }
                }
                else if (state == State.D)
                {
                    if (GetValue() == 0)
                    {
                        this.tape[this.cursor] = 1;
                        this.cursor++;
                        this.state = State.A;
                    }
                    else
                    {
                        this.tape[this.cursor] = 0;
                        this.cursor++;
                        this.state = State.B;
                    }
                }
                else if (state == State.E)
                {
                    if (GetValue() == 0)
                    {
                        this.tape[this.cursor] = 1;
                        this.cursor--;
                        this.state = State.F;
                    }
                    else
                    {
                        this.tape[this.cursor] = 1;
                        this.cursor--;
                        this.state = State.C;
                    }
                }
                else if (state == State.F)
                {
                    if (GetValue() == 0)
                    {
                        this.tape[this.cursor] = 1;
                        this.cursor++;
                        this.state = State.D;
                    }
                    else
                    {
                        this.tape[this.cursor] = 1;
                        this.cursor++;
                        this.state = State.A;
                    }
                }
            }

            public int GetChecksum() => this.tape.Count(kvp => kvp.Value == 1);

            public int GetValue()
            {
                if (tape.ContainsKey(this.cursor))
                    return tape[this.cursor];

                return 0;
            }
        }

        public Day25() : base(25, 2017, "The Halting Problem")
        {

        }

        protected override string? SolvePartOne()
        {
            var p = new TuringMachine();
            
            // Utilities.Repeat(() => p.Run(debug: true), 6);

            Utilities.Repeat(() => p.Run(), 12667664);

            return p.GetChecksum().ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

