using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Numerics;

namespace AdventOfCode.Solutions.Year2022
{

    class Day11 : ASolution
    {
        public Monkey[] monkeys = new Monkey[10];

        public List<int[]> states = new();

        public Day11() : base(11, 2022, "Monkey in the Middle")
        {

        }

        public void ReadMonkeys(string input)
        {
            states = new();

            monkeys = input.SplitByBlankLine()
                .Select(grp => ReadMonkey(grp))
                .ToArray();
        }

        public void ProcessMonkey(Monkey monkey, int part = 1)
        {
            // Add our item count to inspected
            monkey.inspected += monkey.items.Count;

            // A monkey inspects the item (perform operation)
            // Our relief drops (worry is divided by 3 and rounded down)
            // Test worry level
            // Toss item to another monkey
            while(monkey.items.Count > 0)
            {
                var item = monkey.items.Dequeue();

                // Inspect the item
                item = monkey.operation(item);

                // Drop relief
                if (part > 2)
                    item /= 3;

                var result = (item % monkey.testDivisor) == 0;

                if (result)
                    monkeys[monkey.trueMonkey].items.Enqueue(item);
                else
                    monkeys[monkey.falseMonkey].items.Enqueue(item);
            }
        }

        protected override string? SolvePartOne()
        {
            ReadMonkeys(Input);
            for (int i = 0; i < 20; i++)
            {
                for (int m = 0; m < monkeys.Length; m++)
                {
                    ProcessMonkey(monkeys[m]);
                }
            }

            return monkeys
                // Get the two highest inspected items
                .OrderByDescending(m => m.inspected)
                .Take(2)
                .Select(m => m.inspected)
                // Multiply them together
                .Aggregate(BigInteger.One, (x, y) => x * y)
                .ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Let's run this another 480 times (total 500)
            // and see if we can find a pattern
            ReadMonkeys(Input);
            for (int i = 0; i < 10000; i++)
            {
                for (int m = 0; m < monkeys.Length; m++)
                {
                    ProcessMonkey(monkeys[m], 2);
                }
            }

            monkeys.ToList()
                .ForEach(m => Console.WriteLine($"{m.inspected}"));

            return monkeys
                // Get the two highest inspected items
                .OrderByDescending(m => m.inspected)
                .Take(2)
                .Select(m => m.inspected)
                // Multiply them together
                .Aggregate(BigInteger.One, (x, y) => x * y)
                .ToString();
        }

        private Monkey ReadMonkey(string[] lines)
        {
            /*
            Monkey 0:
              Starting items: 79, 98
              Operation: new = old * 19
              Test: divisible by 23
                If true: throw to monkey 2
                If false: throw to monkey 3
            */
            var monkey = new Monkey();

            // Starting items
            lines[1].Split(':', 2, StringSplitOptions.TrimEntries)
                .Skip(1)
                .SelectMany(c => c.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                .Select(c => BigInteger.Parse(c))
                .ToList()
                .ForEach(c => monkey.items.Enqueue(c));

            var operation = lines[2].Split(' ', StringSplitOptions.TrimEntries);
            var operationInt = operation[^1] == "old" ? 0 : Int32.Parse(operation[^1]);

            // We only handle addition and multiplication
            if (operation[^2] == "*")
            {
                if (operationInt > 0)
                    monkey.operation = old => old * operationInt;
                else
                    monkey.operation = old => old * old;
            }
            else
            {
                if (operationInt > 0)
                    monkey.operation = old => old + operationInt;
                else
                    monkey.operation = old => old + old;
            }

            monkey.testDivisor = Int32.Parse(lines[3].Split(' ', StringSplitOptions.TrimEntries).Last());
            monkey.trueMonkey = Int32.Parse(lines[4].Split(' ', StringSplitOptions.TrimEntries).Last());
            monkey.falseMonkey = Int32.Parse(lines[5].Split(' ', StringSplitOptions.TrimEntries).Last());

            return monkey;
        }

        public class Monkey
        {
            public Queue<BigInteger> items { get; set; } = new();
            public Func<BigInteger, BigInteger> operation { get; set; } = old => 0;
            public int testDivisor { get; set; } = 1;
            public int trueMonkey { get; set; } = 0;
            public int falseMonkey { get; set; } = 0;
            public BigInteger inspected { get; set; } = 0;
        }
    }
}

