using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


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

        public int LowSignals = 0;
        public int HighSignals = 0;

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

        public Day20() : base(20, 2023, "Pulse Propagation")
        {
            // DebugInput = @"broadcaster -> a
            //                %a -> inv, con
            //                &inv -> b
            //                %b -> con
            //                &con -> output";

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
                        if (nodes.TryGetValue(output, out Node node))
                        {
                            node.AddInput(kvp.Key);
                        }
                    });
            });
        }

        public void RunQueue()
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

                // If the destination doesn't exist, it may be a test 'output'
                if (nodes.TryGetValue(signal.desintation, out Node node))
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
            Utilities.Repeat(RunQueue, 1000);

            return (HighSignals * LowSignals).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

