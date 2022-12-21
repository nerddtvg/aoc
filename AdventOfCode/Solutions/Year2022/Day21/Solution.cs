using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2022
{

    class Day21 : ASolution
    {
        public Dictionary<string, Monkey> monkeys = new();

        public const string humanKey = "humn";

        public Day21() : base(21, 2022, "Monkey Math")
        {
            // DebugInput = @"root: pppw + sjmn
            //     dbpl: 5
            //     cczh: sllz + lgvd
            //     zczc: 2
            //     ptdq: humn - dvpt
            //     dvpt: 3
            //     lfqf: 4
            //     humn: 5
            //     ljgn: 2
            //     sjmn: drzm * dbpl
            //     sllz: 4
            //     pppw: cczh / lfqf
            //     lgvd: ljgn * ptdq
            //     drzm: hmdt - zczc
            //     hmdt: 32";

            ReadMonkeys();
        }

        private void ReadMonkeys()
        {
            monkeys.Clear();

            var regex = new Regex(@"(?<monkey>[a-z]+): ((?<number>[0-9]+)|(?<left>[a-z]+) (?<op>[\+\-\*\/]) (?<right>[a-z]+))$");

            foreach(var line in Input.SplitByNewline(true))
            {
                var match = regex.Match(line);

                if (!match.Success)
                    throw new Exception();

                var monkey = new Monkey();

                if (match.Groups.ContainsKey("number") && match.Groups["number"].Length > 0)
                {
                    monkey.value = Int32.Parse(match.Groups["number"].Value);
                }
                else
                {
                    monkey.op = match.Groups["op"].Value[0];
                    monkey.left = match.Groups["left"].Value;
                    monkey.right = match.Groups["right"].Value;
                }

                monkeys.Add(match.Groups["monkey"].Value, monkey);
            }
        }

        protected override string? SolvePartOne()
        {
            return SolveMonkey(monkeys["root"]).ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Part 2:
            // Ignore root and look at the values for left and right
            // humn is now us guessing numbers, so we will simply change its
            // value until we are satisfied
            var leftMonkey = monkeys[monkeys["root"].left ?? throw new Exception()];
            var rightMonkey = monkeys[monkeys["root"].right ?? throw new Exception()];
            var human = monkeys[humanKey];

            // Reduce what we can
            ReduceMonkeys();

            // Make it easier by printing the formula
            // Console.WriteLine(PrintMonkey(leftMonkey));
            // Console.WriteLine(PrintMonkey(rightMonkey));
            
            // Let's actually work to reduce the formula programmatically
            // In our input, leftMonkey has a formula, rightMonkey has a value
            // So we will shift leftMonkey formulas all the way down until we get to humn
            while(leftMonkey != human)
            {
                // Find which of the children are a number
                var valueOnSide = monkeys[leftMonkey.left ?? throw new Exception()].value.HasValue && leftMonkey.left != humanKey ? 'l' : 'r';
                var value = valueOnSide == 'l' ? (monkeys[leftMonkey.left ?? throw new Exception()].value ?? throw new Exception()) : (monkeys[leftMonkey.right ?? throw new Exception()].value ?? throw new Exception());

                switch(leftMonkey.op)
                {
                    case '+':
                        rightMonkey.value -= value;
                        break;

                    case '-':
                        if (valueOnSide == 'l')
                        {
                            // Subtract right's value from this value
                            rightMonkey.value = value - rightMonkey.value;
                        }
                        else
                        {
                            rightMonkey.value += value;
                        }
                        break;

                    case '*':
                        rightMonkey.value /= value;
                        break;

                    case '/':
                        if (valueOnSide == 'l')
                        {
                            rightMonkey.value /= value;
                        }
                        else
                        {
                            rightMonkey.value = value * rightMonkey.value;
                        }
                        break;
                }

                // Now move down
                if (valueOnSide == 'l')
                    leftMonkey = monkeys[leftMonkey.right ?? throw new Exception()];
                else
                    leftMonkey = monkeys[leftMonkey.left ?? throw new Exception()];
            }

            // Once we have reduced everything down, the answer is rightMonkey
            var newValue = rightMonkey.value;

            // Now we test it
            ReadMonkeys();
            monkeys[humanKey].value = newValue;
            System.Diagnostics.Debug.Assert(
                System.Diagnostics.Debug.Equals(
                    SolveMonkey(monkeys[monkeys["root"].left ?? throw new Exception()]),
                    SolveMonkey(monkeys[monkeys["root"].right ?? throw new Exception()])
                )
            );

            return newValue.ToString();
        }

        private string PrintMonkey(Monkey monkey)
        {
            if (monkey.value.HasValue)
                return monkey.value.Value.ToString();

            var left = PrintMonkey(monkeys[monkey.left ?? throw new Exception()]);
            var right = PrintMonkey(monkeys[monkey.right ?? throw new Exception()]);

            switch (monkey.op ?? throw new Exception())
            {
                case '+':
                    return $"({left} + {right})";

                case '-':
                    return $"({left} - {right})";

                case '*':
                    return $"({left} * {right})";

                case '/':
                    return $"({left} / {right})";
            }

            throw new Exception();
        }

        /// <summary>
        /// int overflowed
        /// </summary>
        private long SolveMonkey(Monkey monkey)
        {
            if (monkey.value.HasValue)
                return monkey.value.Value;

            var left = SolveMonkey(monkeys[monkey.left ?? throw new Exception()]);
            var right = SolveMonkey(monkeys[monkey.right ?? throw new Exception()]);

            switch(monkey.op ?? throw new Exception())
            {
                case '+':
                    return left + right;

                case '-':
                    return left - right;

                case '*':
                    return left * right;

                case '/':
                    return left / right;
            }

            throw new Exception();
        }

        private void ReduceMonkeys()
        {
            // Go through our monkey list, find any that have left and right defined
            // and if those left and right are numbers, reduce the formulas
            int reduced = 0;
            do
            {
                reduced = 0;

                var canReduce = monkeys
                    .Where(m =>
                        !m.Value.value.HasValue
                        &&
                        !string.IsNullOrEmpty(m.Value.left)
                        &&
                        !string.IsNullOrEmpty(m.Value.right)
                        &&
                        m.Value.left != humanKey
                        &&
                        m.Value.right != humanKey
                        &&
                        monkeys[m.Value.left].value.HasValue
                        &&
                        monkeys[m.Value.right].value.HasValue
                    )
                    .Select(m => m.Key)
                    .ToArray();

                foreach(var key in canReduce)
                {
                    // Solve this monkey
                    var monkey = monkeys[key];
                    monkey.value = SolveMonkey(monkeys[key]);
                    monkey.left = null;
                    monkey.right = null;
                }

                reduced = canReduce.Length;
            } while (reduced > 0);
        }

        public class Monkey
        {
            public long? value;

            public char? op;

            public string? left;
            public string? right;
        }
    }
}

