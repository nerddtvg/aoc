using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Text.Json.Nodes;
using System.Text.Json;


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
            public required string name { get; set; }
            public required NodeType type { get; set; }
            public required string[] outputs { get; set; }
            public Dictionary<string, SignalType> inputs { get; set; } = new();
            public bool onOff { get; set; } = false;

            /// <summary>
            /// Default to low
            /// </summary>
            public void AddInput(string input)
            {
                inputs[input] = SignalType.Low;
            }

            public string GetState()
            {
                return JsonSerializer.Serialize(this);
            }

            public IEnumerable<Node> GetStates()
            {
                if (type == NodeType.Broadcast)
                {
                    yield return new Node()
                    {
                        name = name,
                        type = type,
                        outputs = outputs
                    };
                }
                else if (type == NodeType.FlipFlop)
                {
                    // Flip flops have two states with onOff
                    yield return new Node()
                    {
                        name = name,
                        type = type,
                        outputs = outputs,
                        onOff = false
                    };

                    yield return new Node()
                    {
                        name = name,
                        type = type,
                        outputs = outputs,
                        onOff = true
                    };
                }
                else if (type == NodeType.Conjunction)
                {
                    // Get every state of inputs
                    // which can be High/Low
                    // Start with alternating High/Low on the first input
                    var keys = inputs.Keys.OrderBy(k => k).ToArray();

                    var ret = Enumerable.Range(0, inputs.Count * 2)
                        .Select(idx => new List<(string input, SignalType signal)>() { (keys[0], idx % 2 == 0 ? SignalType.Low : SignalType.High) })
                        .ToList();

                    for(int i=1; i<keys.Length; i++)
                    {
                        Enumerable.Range(0, inputs.Count * 2)
                            .ForEach(idx => ret[idx].Add((keys[i], idx % 2 == 0 ? SignalType.Low : SignalType.High)));
                    }

                    // Now that we have a combination of High/Low for all inputs, make new nodes
                    foreach(var combo in ret)
                    {
                        yield return new Node()
                        {
                            name = name,
                            type = type,
                            outputs = outputs,
                            inputs = combo.ToDictionary(itm => itm.input, itm => itm.signal)
                        };
                    }
                }
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
        public Dictionary<string, string> maps = new();

        public int LowSignals = 0;
        public int HighSignals = 0;

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
                        if (nodes.TryGetValue(output, out Node node))
                        {
                            node.AddInput(kvp.Key);
                        }
                    });
            });
        }

        public bool RunQueue()
        {
            // Save our current state
            var state = GetState(nodes.Values);

            // If the state exists in our dictionary, then skip ahead
            string newState = string.Empty;
            if (maps.TryGetValue(state, out newState))
            {
                nodes = (JsonSerializer.Deserialize<Node[]>(newState) ?? throw new Exception())
                    .ToDictionary(node => node.name, node => node);

                return false;
            }

            // Put the initial signal in the queue
            signals.Enqueue(("button", "broadcaster", SignalType.Low));

            while (signals.TryDequeue(out Signal signal))
            {
                // Add this signal to our counts
                if (signal.signal == SignalType.Low)
                    LowSignals++;
                else
                    HighSignals++;

                // If the signal is going to rx and Low, end early
                if (signal.desintation == "rx" && signal.signal == SignalType.Low)
                    return true;

                // If the destination doesn't exist, it may be a test 'output'
                if (nodes.TryGetValue(signal.desintation, out Node node))
                {
                    // Run the signal process
                    var outSignals = node.ProcessInput(signal.source, signal.signal);

                    // Add anything output to the queue
                    outSignals.ForEach(signals.Enqueue);
                }
            }

            // We have a new state!
            newState = GetState(nodes.Values);
            maps[state] = newState;

            return false;
        }

        protected override string? SolvePartOne()
        {
            ResetInput();
            Utilities.Repeat(() => RunQueue(), 1000);

            return (HighSignals * LowSignals).ToString();
        }

        public string GetState(IEnumerable<Node> nodes)
        {
            return JsonSerializer.Serialize(nodes.OrderBy(node => node.name).ToArray());
        }

        protected override string? SolvePartTwo()
        {
            ResetInput();

            // Commit Note: Tried to precalculate the states but there are too many
            // Tried to hash the states and skip ahead, again too many

            // // Gather all possible states and pre-process the outcomes
            // var individualNodeStates = nodes.Keys.Select(k => nodes[k].GetStates().ToList()).ToList();

            // // Build a list of all states starting with the first
            // var totalStates = individualNodeStates.Aggregate((long)1, (x, y) => x * y.Count);
            // var allNodeStates = new List<List<Node>>();

            // for (long idx = 0; idx < totalStates; idx++)
            //     allNodeStates.Add(new List<Node>() { individualNodeStates[0][(int)(idx % individualNodeStates.Count)] });

            // // Append the rest of them
            // foreach(var newNode in individualNodeStates[1..])
            // {
            //     for(long idx=0; idx<totalStates; idx++)
            //         allNodeStates[idx].Add(newNode[idx % newNode.Count]));
            // }

            int buttonPresses = 0;

            // Sends to rx, used to determine end
            // Assumes 1
            var sendsToEnd = nodes.Single(node => node.Value.outputs.Contains("rx")).Key;

            while (true)
            {
                buttonPresses++;
                if (RunQueue())
                    break;
            }

            return buttonPresses.ToString();
        }
    }
}

