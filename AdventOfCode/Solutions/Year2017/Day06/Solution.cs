using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2017
{

    class Day06 : ASolution
    {
        private SortedSet<string> history = new SortedSet<string>();
        private LinkedList<int> memoryBanks = new LinkedList<int>();

        public Day06() : base(06, 2017, "")
        {

        }

        private void Reset()
        {
            memoryBanks.Clear();
            history.Clear();

            foreach(var entry in Input.Split('\t', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                memoryBanks.AddLast(Int32.Parse(entry));
            }
        }

        private string GetHash() => string.Join('-', memoryBanks);

        /// <summary>
        /// Balances the memory and returns a string hash of this configuration for comparison
        /// </summary>
        private string BalanceMemory()
        {
            var max = memoryBanks.Max();
            var node = memoryBanks.FirstNode(v => v == max);

            // Clear this node
            node.Value = 0;

            while(max > 0)
            {
                node = node.Next != null ? node.Next : memoryBanks.First;

                // Avoid a warning
                if (node == null)
                    throw new InvalidOperationException();

                // Increase / decrease
                node.Value++;
                max--;
            }

            // We're done with this round, generate a hash
            return GetHash();
        }

        protected override string? SolvePartOne()
        {
            Reset();

            var count = 0;

            // Add initial state to history
            this.history.Add(GetHash());

            do
            {
                count++;

                var hash = BalanceMemory();

                if (this.history.Contains(hash))
                    return count.ToString();

                this.history.Add(hash);
            } while (true);
        }

        protected override string? SolvePartTwo()
        {
            Reset();

            var firstHash = string.Empty;
            var count = 0;

            // Add initial state to history
            this.history.Add(GetHash());

            do
            {
                if (!string.IsNullOrEmpty(firstHash))
                    count++;

                var hash = BalanceMemory();

                if (count == 0 && this.history.Contains(hash))
                    // We start counting here
                    firstHash = hash;

                // We check here
                if (count > 0)
                {
                    if (hash == firstHash)
                        return count.ToString();
                }
                else
                {
                    // Only care about hash history when we aren't counting
                    this.history.Add(hash);
                }
            } while (true);
        }
    }
}

#nullable restore
