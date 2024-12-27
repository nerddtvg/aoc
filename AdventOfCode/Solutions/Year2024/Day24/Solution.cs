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
            // DebugInput = @"x00: 1
            // x01: 0
            // x02: 1
            // x03: 1
            // x04: 0
            // y00: 1
            // y01: 1
            // y02: 1
            // y03: 1
            // y04: 1

            // ntg XOR fgs -> mjb
            // y02 OR x01 -> tnw
            // kwq OR kpj -> z05
            // x00 OR x03 -> fst
            // tgd XOR rvg -> z01
            // vdt OR tnw -> bfw
            // bfw AND frj -> z10
            // ffh OR nrd -> bqk
            // y00 AND y03 -> djm
            // y03 OR y00 -> psh
            // bqk OR frj -> z08
            // tnw OR fst -> frj
            // gnj AND tgd -> z11
            // bfw XOR mjb -> z00
            // x03 OR x00 -> vdt
            // gnj AND wpb -> z02
            // x04 AND y00 -> kjc
            // djm OR pbm -> qhw
            // nrd AND vdt -> hwm
            // kjc AND fst -> rvg
            // y04 OR y02 -> fgs
            // y01 AND x02 -> pbm
            // ntg OR kjc -> kwq
            // psh XOR fgs -> tgd
            // qhw XOR tgd -> z09
            // pbm OR djm -> kpj
            // x03 XOR y03 -> ffh
            // x00 XOR y04 -> ntg
            // bfw OR bqk -> z06
            // nrd XOR fgs -> wpb
            // frj XOR qhw -> z04
            // bqk OR frj -> z07
            // y03 OR x01 -> nrd
            // hwm AND bqk -> z03
            // tgd XOR rvg -> z12
            // tnw OR pbm -> gnj";

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

        string PerformOperation((char op, string val1, string val2, string dest) inOperation)
        {
            var (operation, val1, val2, dest) = inOperation;

            switch (operation)
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

            return dest;
        }

        void LoopOperation((char op, string val1, string val2, string dest) inOperation)
        {
            // Go down any impacted operations and reprocess
            var queue = new Queue<(char op, string val1, string val2, string dest)>();
            queue.Enqueue(inOperation);

            while (queue.TryDequeue(out var operation))
            {
                PerformOperation(operation);

                // For any operation that has this destination as an input
                // we queue for processing
                operations.Where(op => op.val1 == operation.dest || op.val2 == operation.dest).ForEach(op => queue.Enqueue(op));
            }
        }

        protected override string? SolvePartOne()
        {
            // This can go two ways:
            // We work one by one updating gates top to bottom - Tested and this was wrong
            // Or we must run down every change in the chain
            foreach(var operation in operations)
            {
                LoopOperation(operation);
            }

            // Time: 00:00:00.2240480
            return Convert.ToInt64(values.Keys.Where(k => k[0] == 'z').OrderByDescending(k => k).Select(k => values[k] ? '1' : '0').JoinAsString(), 2).ToString();
        }

        protected override string? SolvePartTwo()
        {
            // As an adder, XOR must feed into output (z00) or AND & XOR
            // OR must feed into output (z00) or AND & XOR
            // AND must feed into OR
            // Let's search if any don't match
            HashSet<string> swapped = [];

            // Binary adder: https://www.electronics-lab.com/article/binary-adder/

            foreach ((char op, string val1, string val2, string dest) in operations)
            {
                // Graphviz helper output
                // Console.WriteLine($"{op}_{dest} [label=\"{(op == 'A' ? "AND" : op == 'X' ? "XOR" : "OR")}\"]");
                // Console.WriteLine($"{val1} -> {op}_{dest}");
                // Console.WriteLine($"{val2} -> {op}_{dest}");
                // Console.WriteLine($"{op}_{dest} -> {dest}");

                switch (op)
                {
                    case 'A':
                        // Feeds an output, invalid
                        if (dest[0] == 'z')
                            swapped.Add(dest);
                        // Feeds AND with x00/y00 as inputs, valid
                        else if (operations.Where(top => top.val1 == dest || top.val2 == dest).All(top => top.op == 'X' || (top.op == 'A' && (val1 == "x00" || val1 == "y00"))))
                            break;
                        // Feeds non-OR, invalid
                        else if (operations.Any(top => top.op != 'O' && (top.val1 == dest || top.val2 == dest)))
                            swapped.Add(dest);
                        break;

                    case 'X':
                        {
                            // XOR: x/y => XOR => var => XOR (or x00/y00 => z00)
                            // XOR: a/b => XOR => output
                            if (dest == "z00" && ((val1 == "x00" || val2 == "y00") ^ (val1 == "y00" || val2 == "x00")))
                                break;

                            // If XOR feeds into OR at all, it's invalid
                            if (operations.Any(top => top.op == 'O' && (top.val1 == dest || top.val2 == dest)))
                            {
                                swapped.Add(dest);
                                break;
                            }

                            var sameFeeds = operations.Where(top => (top.val1 == val1 || top.val2 == val2) ^ (top.val1 == val2 || top.val2 == val1)).ToArray();

                            if ((val1[0] == 'x' && val2[0] == 'y') ^ (val1[0] == 'y' && val2[0] == 'x'))
                            {
                                // sameFeeds should be 2, one is AND
                                if (sameFeeds.Count(feed => feed.op == 'X') != 1 || sameFeeds.Count(feed => feed.op == 'A') != 1)
                                    swapped.Add(dest);

                                // If it is going to output, it's invalid
                                if (dest[0] == 'z')
                                    swapped.Add(dest);
                            }
                            else
                            {
                                // This is an output XOR
                                if (dest[0] != 'z')
                                    swapped.Add(dest);
                            }

                            // If a/b are not x/y inputs, then expect an output

                            break;
                        }
                    case 'O':
                        {
                            // O Feeds an output, invalid
                            if (dest[0] == 'z')
                            {
                                // Only invalid if not the last one
                                if (op == 'O' && dest != "z45")
                                    swapped.Add(dest);

                                break;
                            }

                            // XOR should feed an output if the same two inputs are used in AND operations
                            var feeds = operations.Where(top => top.val1 == dest || top.val2 == dest).ToArray();

                            // Feeds OR, invalid
                            if (feeds.Any(feed => feed.op == 'O'))
                            {
                                swapped.Add(dest);
                                break;
                            }

                            // Must feed a single pair of XOR and AND
                            if (feeds.Count(feed => feed.op == 'X') != 1 || feeds.Count(feed => feed.op == 'A') != 1)
                                swapped.Add(dest);

                            break;
                        }
                }
            }

            // Time: 00:00:00.1228360
            return string.Join(",", swapped.OrderBy(k => k));
        }
    }
}

