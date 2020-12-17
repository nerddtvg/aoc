using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{

    class Day17 : ASolution
    {
        Dictionary<(int x, int y, int z), bool> dim = new Dictionary<(int x, int y, int z), bool>();

        public Day17() : base(17, 2020, "")
        {

        }

        private bool GetCubeState((int x, int y, int z) a) =>
            dim.ContainsKey(a) ? dim[a] : false;

        private bool GetCubeState(int x, int y, int z) =>
            GetCubeState((x, y, z));
        
        private IEnumerable<bool> GetNeighbors((int x, int y, int z) a) {
            for(int dx=-1; dx<=1; dx++) {
                for(int dy=-1; dy<=1; dy++) {
                    for(int dz=-1; dz<=1; dz++) {
                        // Only neighbors
                        if (dx == 0 && dy == 0 && dz == 0) continue;

                        yield return GetCubeState((a.x+dx, a.y+dy, a.z+dz));
                    }
                }
            }
        }

        protected override string SolvePartOne()
        {
            return null;
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
