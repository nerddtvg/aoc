using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Text.Json.Nodes;
using System.Text.Json;
using QuikGraph.Algorithms.Search;


namespace AdventOfCode.Solutions.Year2023
{
    using Signal = (string source, string desintation, SignalType signal);

    public enum SignalType
    {
        Low,
        High
    }

    class Day20 : ASolution
    {

        public enum NodeType
        {
            Broadcast,
            FlipFlop,
            Conjunction
        }

        public class Node
        {
            public required string name;
            public required NodeType type;
            public required string[] outputs;
            public Dictionary<string, SignalType> inputs = new();
            public bool onOff = false;

            /// <summary>
            /// Default to low
            /// </summary>
            public void AddInput(string input)
            {
                inputs[input] = SignalType.Low;
            }

            /// <summary>
            /// Process an incoming signal, return an array of signals to send out if any
            /// </summary>
            public Signal[] ProcessInput(string source, SignalType signal)
            {
                if (type == NodeType.Broadcast)
                {
                    // Send the same signal to all
                    return outputs.Select(destination => (Signal)(name, destination, signal)).ToArray();
                }
                else if (type == NodeType.Conjunction)
                {
                    inputs[source] = signal;

                    // Only if all input memories are "High" do we send a "Low" signal
                    if (inputs.All(kvp => kvp.Value == SignalType.High))
                        return outputs.Select(destination => (Signal)(name, destination, SignalType.Low)).ToArray();


                    return outputs.Select(destination => (Signal)(name, destination, SignalType.High)).ToArray();
                }
                else if (type == NodeType.FlipFlop)
                {
                    // Do nothing on a high signal
                    if (signal == SignalType.High)
                        return Array.Empty<Signal>();

                    onOff = !onOff;

                    // Was off, now on, send a High signal
                    if (onOff)
                        return outputs.Select(destination => (Signal)(name, destination, SignalType.High)).ToArray();

                    // Was on, now off, send a Low signal
                    return outputs.Select(destination => (Signal)(name, destination, SignalType.Low)).ToArray();
                }

                return Array.Empty<Signal>();
            }
        }

        public Queue<Signal> signals = new();
        public Dictionary<string, Node> nodes = new();

        public int LowSignals = 0;
        public int HighSignals = 0;
        public Dictionary<string, ulong> cycles = new();
        public string finalNode = string.Empty;

        public Day20() : base(20, 2023, "Pulse Propagation")
        {
            // DebugInput = @"broadcaster -> a
            //                %a -> inv, con
            //                &inv -> b
            //                %b -> con
            //                &con -> output";
        }

        public void ResetInput()
        {
            nodes.Clear();
            signals.Clear();
            LowSignals = 0;
            HighSignals = 0;

            var regex = new Regex(@"^(?<type>[%&])*(?<name>[a-z]+) \-> ((?<destination>[a-z]+)(, )*)+$");

            Input.SplitByNewline(shouldTrim: true)
                .ForEach(line =>
                {
                    var matches = regex.Match(line);

                    nodes[matches.Groups["name"].Value] = new()
                    {
                        name = matches.Groups["name"].Value,
                        outputs = matches.Groups["destination"].Captures.Select(c => c.Value).ToArray(),
                        type = matches.Groups["type"].Value == "%" ? NodeType.FlipFlop : (matches.Groups["type"].Value == "&" ? NodeType.Conjunction : NodeType.Broadcast)
                    };
                });

            // We now have all of the nodes loaded, link the inputs
            nodes.ForEach(kvp =>
            {
                kvp.Value.outputs
                    .ForEach(output =>
                    {
                        // If the destination doesn't exist, it may be a test 'output'
                        if (nodes.TryGetValue(output, out Node? node) && node != default)
                        {
                            node.AddInput(kvp.Key);
                        }
                    });
            });

            finalNode = nodes.Single(node => node.Value.outputs.Contains("rx")).Key;
            cycles = nodes[finalNode].inputs.ToDictionary(kvp => kvp.Key, kvp => (ulong)0);
        }

        public void RunQueue(uint step = 0)
        {
            // Put the initial signal in the queue
            signals.Enqueue(("button", "broadcaster", SignalType.Low));

            while (signals.TryDequeue(out Signal signal))
            {
                // Add this signal to our counts
                if (signal.signal == SignalType.Low)
                    LowSignals++;
                else
                    HighSignals++;

                if (signal.desintation == "zg" && signal.signal == SignalType.High && cycles[signal.source] == 0)
                    cycles[signal.source] = step;

                // If the destination doesn't exist, it may be a test 'output'
                if (nodes.TryGetValue(signal.desintation, out Node? node) && node != default)
                {
                    // Run the signal process
                    var outSignals = node.ProcessInput(signal.source, signal.signal);

                    // Add anything output to the queue
                    outSignals.ForEach(signals.Enqueue);
                }
            }
        }

        protected override string? SolvePartOne()
        {
            ResetInput();
            Utilities.Repeat(() => RunQueue(), 1000);

            return (HighSignals * LowSignals).ToString();
        }

        protected override string? SolvePartTwo()
        {
            // I knew this was likely to be some ASM reduction problem
            // but I was hoping to figure out an efficient way to do it in code
            // Unfortunately that didn't happen
            // This is a great visualization: https://old.reddit.com/r/adventofcode/comments/18mypla/2023_day_20_input_data_plot/
            // Also: https://old.reddit.com/r/adventofcode/comments/18msq8g/2023_day_20_part_2python_terminal_visualization/

            // Starting from Part 1's end means we're on step 1001

            // The cycles should be 2^12 = 4095 or less
            // There are 12 bits in the integer values
            for(uint i=1001; i<5000 && cycles.Values.Any(c => c == 0); i++)
                RunQueue(i);

            if (cycles.Values.Any(c => c == 0))
                return string.Empty;

            return Utilities.FindLCM(cycles.Values.Select(c => (double)c).ToArray()).ToString();
        }
    }
}

