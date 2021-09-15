using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2015
{
    class Day7Operation
    {
        public string operation { get; set; }
        public string inA { get; set; }
        public string inB { get; set; }
        public string outputRegister { get; set; }

        // Parsed values
        public UInt16 inAVal { get; set; } = UInt16.MaxValue;
        public UInt16 inBVal { get; set; } = UInt16.MaxValue;

        public UInt16 setValue { get; set; } = UInt16.MaxValue;

        // Ordering the operations
        public uint order { get; set; } = uint.MaxValue;
    }

    class Day07 : ASolution
    {
        private Dictionary<string, UInt16> registers = new Dictionary<string, UInt16>();
        private List<Day7Operation> circuit = new List<Day7Operation>();

        public Day07() : base(07, 2015, "")
        {
            
        }

        /// <summary>
        /// Parse an incoming line based on '->'
        /// </summary>
        private Day7Operation ParseLine(string line)
        {
            var parts = line.Split("->", StringSplitOptions.TrimEntries);

            if (parts.Length != 2)
                throw new Exception($"Invalid split: {line}");

            // Start our return object
            var ret = new Day7Operation() { outputRegister = parts[1] };

            // 2 params:
            // AND
            // OR
            // LSHIFT
            // RSHIFT

            // NOT = 1 param

            // SET (1234 -> abc) = No params-ish

            var matches = (new Regex("^([a-z0-9]+) (AND|OR|LSHIFT|RSHIFT) ([a-z0-9]+)$")).Match(parts[0]);
            if (matches.Success)
            {
                ret.inA = matches.Groups[1].Value;
                ret.operation = matches.Groups[2].Value;
                ret.inB = matches.Groups[3].Value;

                UInt16 o;
                if (UInt16.TryParse(ret.inA, out o))
                {
                    ret.inA = string.Empty;
                    ret.inAVal = o;
                }
                
                if (UInt16.TryParse(ret.inB, out o))
                {
                    ret.inB = string.Empty;
                    ret.inBVal = o;
                }

                return ret;
            }

            matches = (new Regex("^(NOT) ([a-z]+)$")).Match(parts[0]);
            if (matches.Success)
            {
                ret.inA = matches.Groups[2].Value;
                ret.operation = matches.Groups[1].Value;

                return ret;
            }

            matches = (new Regex("^([0-9a-z]+)$")).Match(parts[0]);
            if (matches.Success)
            {
                ret.inA = matches.Groups[1].Value;
                ret.operation = "SET";

                // If this is a hard-coded integer, this is a starting point
                UInt16 o;
                if (UInt16.TryParse(ret.inA, out o))
                {
                    ret.setValue = o;
                    ret.order = 0;
                }

                return ret;
            }

            // No results
            throw new Exception($"Unable to find a match for: {line}");
        }

        private void ParseCircuit()
        {
            // Go through each line and get the operation
            foreach(var line in Input.SplitByNewline())
            {
                circuit.Add(ParseLine(line));
            }

            // Now that we've loaded everything, we need to loop through and order it all
            do
            {
                // Find everything we haven't set yet
                foreach(var c in this.circuit.Where(a => a.order == uint.MaxValue).ToList())
                {
                    // If our pre-reqs are completed, find the highest number of those two and add one

                    // We should always have one of these, however it may not always be A because of hardcoded numbers
                    var inAExists = !string.IsNullOrEmpty(c.inA);
                    var preA = inAExists ? this.circuit.SingleOrDefault(a => a.outputRegister == c.inA && a.order < uint.MaxValue) : default;

                    var inBExists = !string.IsNullOrEmpty(c.inB);
                    var preB = inBExists ? this.circuit.SingleOrDefault(a => a.outputRegister == c.inB && a.order < uint.MaxValue) : default;

                    // If we only have A, look at A
                    if (inAExists && inBExists)
                    {
                        if (preA != default && preB != default)
                        {
                            c.order = Math.Max(preA.order, preB.order) + 1;
                        }
                    }
                    else if (inAExists)
                    {
                        if (preA != default)
                        {
                            c.order = preA.order + 1;
                        }
                    }
                    else if (inBExists)
                    {
                        if (preB != default)
                        {
                            c.order = preB.order + 1;
                        }
                    }
                    else
                    {
                        throw new Exception("Should not be here.");
                    }
                }
            } while (this.circuit.Count(a => a.order == uint.MaxValue) > 0);
        }

        private void ProcessOperation(Day7Operation operation)
        {
            UInt16 value = 0;

            switch(operation.operation)
            {
                case "SET":
                    {
                        if (operation.setValue < UInt16.MaxValue)
                        {
                            value = operation.setValue;
                        }
                        else
                        {
                            value = GetRegister(operation.inA);
                        }
                        break;
                    }

                case "AND":
                    {
                        UInt16 a = operation.inAVal < UInt16.MaxValue ? operation.inAVal : GetRegister(operation.inA);
                        UInt16 b = operation.inBVal < UInt16.MaxValue ? operation.inBVal : GetRegister(operation.inB);
                        value = (UInt16) (a & b);
                        break;
                    }

                case "OR":
                    {
                        UInt16 a = operation.inAVal < UInt16.MaxValue ? operation.inAVal : GetRegister(operation.inA);
                        UInt16 b = operation.inBVal < UInt16.MaxValue ? operation.inBVal : GetRegister(operation.inB);
                        value = (UInt16) (a | b);
                        break;
                    }

                case "NOT":
                    {
                        UInt16 a = operation.inAVal < UInt16.MaxValue ? operation.inAVal : GetRegister(operation.inA);
                        value = (UInt16) (~a);
                        break;
                    }

                case "LSHIFT":
                    {
                        UInt16 a = operation.inAVal < UInt16.MaxValue ? operation.inAVal : GetRegister(operation.inA);
                        UInt16 b = operation.inBVal < UInt16.MaxValue ? operation.inBVal : GetRegister(operation.inB);
                        value = (UInt16) (a << b);
                        break;
                    }

                case "RSHIFT":
                    {
                        UInt16 a = operation.inAVal < UInt16.MaxValue ? operation.inAVal : GetRegister(operation.inA);
                        UInt16 b = operation.inBVal < UInt16.MaxValue ? operation.inBVal : GetRegister(operation.inB);
                        value = (UInt16) (a >> b);
                        break;
                    }
            }

            SaveRegister(operation.outputRegister, value);
        }

        /// <summary>
        /// Gets the requested value
        /// </summary>
        private UInt16 GetRegister(string register)
        {
            return this.registers.ContainsKey(register) ? this.registers[register] : (UInt16) 0;
        }

        /// <summary>
        /// Save the given value
        /// </summary>
        private void SaveRegister(string register, UInt16 value)
        {
            this.registers[register] = value;
        }

        /// <summary>
        /// Loop through and run the circuit
        /// </summary>
        private void RunCircuit()
        {
            uint MaxOrder = this.circuit.Max(a => a.order);

            for (uint i = 0; i <= MaxOrder; i++)
            {
                // Get all of the steps in this order
                var steps = this.circuit.Where(a => a.order == i).ToArray();

                foreach(var s in steps)
                    ProcessOperation(s);
            }
        }

        private void PrintRegisters()
        {
            foreach(var k in this.registers.Keys.OrderBy(a => a))
            {
                System.Console.WriteLine($"Register {k}: {this.registers[k]}");
            }
        }

        protected override string SolvePartOne()
        {
            ParseCircuit();
            RunCircuit();

            if (!string.IsNullOrEmpty(DebugInput))
            {
                PrintRegisters();
                return null;
            }

            return this.registers["a"].ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
