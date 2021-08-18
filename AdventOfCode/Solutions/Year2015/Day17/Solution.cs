using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2015
{

    class Day17 : ASolution
    {
        private List<int> _containers = new List<int>();
        private List<List<int>> _combos = new List<List<int>>();

        public Day17() : base(17, 2015, "")
        {
            // Input it is a list of container sizes
            foreach(var line in Input.SplitByNewline())
            {
                if (line.Trim().Length == 0) continue;

                int a;
                if (Int32.TryParse(line.Trim(), out a))
                {
                    _containers.Add(a);
                }
            }
            
            // Order our containers
            this._containers = this._containers.OrderBy(a => a).ToList();

            this._combos = this.GetOptions(150, this._containers.ToArray());
        }

        private List<List<int>> GetOptions(int required_size, int[] sizes)
        {
            // Is this the end of the list?
            if (sizes.Length == 1)
                if (sizes[0] != required_size)
                    return new List<List<int>>();
                else
                    return new List<List<int>> { new List<int> { required_size } };

            // Otherwise, take another step down
            var ret = new List<List<int>>();
            if (required_size >= sizes[0])
            {
                // We can work with and without this container
                var temp_size = required_size - sizes[0];
                if (temp_size > 0)
                {
                    var tRet = GetOptions(temp_size, sizes.Skip(1).ToArray());

                    // We have supposed combos that work, prepend our size
                    tRet.ForEach(a => a.Insert(0, temp_size));
                    ret.AddRange(tRet);
                }
                else
                {
                    ret.Add(new List<int>() { sizes[0] });
                }
            }

            // Now try it without the first one
            {
                var tRet = GetOptions(required_size, sizes.Skip(1).ToArray());
                ret.AddRange(tRet);
            }

            return ret;
        }

        protected override string SolvePartOne()
        {

            // For each container in the list, go through and figure out if we can make it work or not
            return this._combos.Count.ToString();
        }

        protected override string SolvePartTwo()
        {
            // Find the fewest container combos
            var minCount = this._combos.Min(a => a.Count);

            // How many combos have this number of containers?
            return this._combos.Count(a => a.Count == minCount).ToString();
        }
    }
}
