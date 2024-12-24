using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    class Day24 : ASolution
    {
        Dictionary<string, bool> values = [];
        List<(char op, string val1, string val2, string dest)> operations = [];

        public Day24() : base(24, 2024, "Crossed Wires")
        {
            DebugInput = @"x00: 1
            x01: 0
            x02: 1
            x03: 1
            x04: 0
            y00: 1
            y01: 1
            y02: 1
            y03: 1
            y04: 1

            ntg XOR fgs -> mjb
            y02 OR x01 -> tnw
            kwq OR kpj -> z05
            x00 OR x03 -> fst
            tgd XOR rvg -> z01
            vdt OR tnw -> bfw
            bfw AND frj -> z10
            ffh OR nrd -> bqk
            y00 AND y03 -> djm
            y03 OR y00 -> psh
            bqk OR frj -> z08
            tnw OR fst -> frj
            gnj AND tgd -> z11
            bfw XOR mjb -> z00
            x03 OR x00 -> vdt
            gnj AND wpb -> z02
            x04 AND y00 -> kjc
            djm OR pbm -> qhw
            nrd AND vdt -> hwm
            kjc AND fst -> rvg
            y04 OR y02 -> fgs
            y01 AND x02 -> pbm
            ntg OR kjc -> kwq
            psh XOR fgs -> tgd
            qhw XOR tgd -> z09
            pbm OR djm -> kpj
            x03 XOR y03 -> ffh
            x00 XOR y04 -> ntg
            bfw OR bqk -> z06
            nrd XOR fgs -> wpb
            frj XOR qhw -> z04
            bqk OR frj -> z07
            y03 OR x01 -> nrd
            hwm AND bqk -> z03
            tgd XOR rvg -> z12
            tnw OR pbm -> gnj";

            var split = Input.SplitByBlankLine(shouldTrim: true);
            var regex = new Regex(@"(?<val1>[a-z0-9]+) (?<op>AND|OR|XOR) (?<val2>[a-z0-9]+) \-> (?<dest>[a-z0-9]+)");

            foreach(var line in split[1])
            {
                var match = regex.Match(line);
                if (!match.Success) continue;

                operations.Add((match.Groups["op"].Value[0], match.Groups["val1"].Value, match.Groups["val2"].Value, match.Groups["dest"].Value));
                values[match.Groups["val1"].Value] = false;
                values[match.Groups["val2"].Value] = false;
            }

            foreach(var line in split[0])
            {
                values[line[0..3]] = line[5] == '1';
            }
        }

        protected override string? SolvePartOne()
        {
            // This can go three ways:
            // We work one by one updating gates top to bottom - Tested and this was wrong
            // Or we track the changes and update all the gates once everything is calculated
            // Or we must run down every change in the chain
            foreach(var (operation, val1, val2, dest) in operations)
            {
                switch(operation)
                {
                    case 'A':
                        values[dest] = values[val1] && values[val2];
                        break;

                    case 'O':
                        values[dest] = values[val1] || values[val2];
                        break;

                    case 'X':
                        values[dest] = values[val1] ^ values[val2];
                        break;
                }            
            }

            foreach(var key in values.Keys.OrderBy(k => k))
            {
                Console.WriteLine($"{key}: {(values[key] ? '1' : '0')}");
            }

            return string.Empty;
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

