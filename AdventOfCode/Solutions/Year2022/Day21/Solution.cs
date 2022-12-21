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
            return string.Empty;
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

        public struct Monkey
        {
            public int? value;

            public char? op;

            public string? left;
            public string? right;
        }
    }
}

