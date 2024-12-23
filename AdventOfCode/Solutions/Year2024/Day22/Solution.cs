using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Numerics;
using System.Diagnostics;


namespace AdventOfCode.Solutions.Year2024
{

    class Day22 : ASolution
    {
        Dictionary<BigInteger, BigInteger> cache = [];
        List<List<BigInteger>> monkeys = [];

        public Day22() : base(22, 2024, "Monkey Market")
        {
            System.Diagnostics.Debug.Assert(Mix(15, 42) == new BigInteger(37), "Invalid Mix Assert");
            System.Diagnostics.Debug.Assert(Prune(100000000) == new BigInteger(16113920), "Invalid Prune Assert");

            var secret = BigInteger.Parse("123");
            HashSet<BigInteger> secrets = [15887950, 16495136, 527345, 704524, 1553684, 12683156, 11100544, 12249484, 7753432, 5908254];

            for (int i = 0; i < 10; i++)
            {
                var secret2 = Calculate(secret);

                if (!secrets.Contains(secret2))
                {
                    Console.WriteLine($"Invalid Value: {secret} => {secret2}");
                }

                secret = secret2;
            }

            // DebugInput = @"1
            // 2
            // 3
            // 2024";
        }

        static BigInteger Mix(BigInteger val, BigInteger secret) => val ^ secret;

        static BigInteger Prune(BigInteger val) => val % 16777216;

        BigInteger Calculate(BigInteger secret) {
            if (cache.TryGetValue(secret, out BigInteger retSecret))
                return retSecret;

            var s1 = Prune(Mix(secret * 64, secret));
            var s2 = Prune(Mix(s1 / 32, s1));
            var s3 = Prune(Mix(s2 * 2048, s2));

            cache[secret] = s3;
            return s3;
        }

        BigInteger GetNth(BigInteger secret, int iteration) {
            var monkey = monkeys.Count;
            monkeys.Add([secret]);

            for (int i = 0; i < iteration; i++)
            {
                monkeys[monkey].Add(Calculate(monkeys[monkey][^1]));
            }

            return monkeys[monkey][^1];
        }

        protected override string? SolvePartOne()
        {
            var sum = BigInteger.Zero;

            foreach (var line in Input.SplitByNewline(shouldTrim: true))
            {
                sum += GetNth(BigInteger.Parse(line), 2000);
            }

            // Time: 00:00:02.8972771
            return sum.ToString();
        }

        protected override string? SolvePartTwo()
        {
            // Load all of the prices
            var monkeyPrices = monkeys.Select(monkey => monkey.Select(secret => (int)(secret % 10)).ToArray()).ToArray();

            // Load all of the deltas (Skip 1 to ignore the first value, also offsets monkeyPrice[i - 1] becomes monkeyPrice[i])
            var monkeyChanges = monkeyPrices.Select((monkeyPrice, monkey_i) => monkeyPrice.Skip(1).Select((price, i) => price - monkeyPrice[i]).ToArray()).ToArray();

            // We need to find a 4-sequence price changes and the value of its first instance
            var sequences = monkeyChanges.Select((change, change_i) =>
            {
                var ret = new Dictionary<string, int>();
                for (int i = 3; i < change.Length; i++)
                {
                    var sequenceKey = $"{change[i - 3]},{change[i - 2]},{change[i - 1]},{change[i]}";

                    // If this exists already, skip it
                    if (ret.ContainsKey(sequenceKey)) continue;

                    // Get the price, offset by 1 above
                    ret[sequenceKey] = monkeyPrices[change_i][i + 1];
                }
                return ret;
            }).ToArray();

            // Then determine which is the *best* option
            var sequenceKeys = sequences.SelectMany(s => s.Keys).ToHashSet();

            int maxBananas = 0;
            string maxSequence = string.Empty;

            sequenceKeys.ForEach(key =>
            {
                var sum = sequences.Sum(sequence => sequence.TryGetValue(key, out int val) ? val : 0);
                if (sum > maxBananas)
                {
                    maxBananas = sum;
                    maxSequence = key;
                }
            });

            Debug.WriteLine($"Sequence: ${maxSequence}");

            // Time: 00:00:20.6890109
            return maxBananas.ToString();
        }
    }
}

