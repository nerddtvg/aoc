using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{

    class Day17 : ASolution
    {
        Dictionary<(int x, int y, int z), bool> dim = new Dictionary<(int x, int y, int z), bool>();

        // Part 2
        Dictionary<(int x, int y, int z, int w), bool> dim2 = new Dictionary<(int x, int y, int z, int w), bool>();

        public Day17() : base(17, 2020, "")
        {
            DebugInput = @"
            .#.
            ..#
            ###
            ";

            // Load the initial active cubes
            // Initial state is x,y with z=0
            int y = 0;
            foreach(var line in Input.SplitByNewline(true, true)) {
                for(int x=0; x<line.Length; x++) {
                    if (line.Substring(x, 1) == "#")
                        dim[(x, y, 0)] = true;
                        
                        // Part 2
                        dim2[(x, y, 0, 0)] = true;
                }

                y++;
            }
        }

        private bool GetCubeState((int x, int y, int z) a) =>
            dim.ContainsKey(a) ? dim[a] : false;

        private bool GetCubeState2((int x, int y, int z, int w) a) =>
            dim2.ContainsKey(a) ? dim2[a] : false;
        
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
        
        private IEnumerable<bool> GetNeighbors2((int x, int y, int z, int w) a) {
            for(int dx=-1; dx<=1; dx++) {
                for(int dy=-1; dy<=1; dy++) {
                    for(int dz=-1; dz<=1; dz++) {
                        for(int dw=-1; dw<=1; dw++) {
                            // Only neighbors
                            if (dx == 0 && dy == 0 && dz == 0 && dw == 0) continue;

                            yield return GetCubeState2((a.x+dx, a.y+dy, a.z+dz, a.w+dw));
                        }
                    }
                }
            }
        }

        private bool CurrentlyInactiveCheck((int x, int y, int z) a) {
            // Cube is currently inactive
            // If a cube is inactive but exactly 3 of its neighbors are active, the cube becomes active
            var c = GetNeighbors(a).Count(a => a == true);

            if (c == 3) return true;

            return false;
        }

        private bool CurrentlyActiveCheck((int x, int y, int z) a) {
            // Cube is currently active
            // If exactly 2 or 3 neighbors are active, cube stays active
            var c = GetNeighbors(a).Count(a => a == true);

            if (c == 2 || c == 3) return true;

            return false;
        }

        private bool CurrentlyInactiveCheck2((int x, int y, int z, int w) a) {
            // Cube is currently inactive
            // If a cube is inactive but exactly 3 of its neighbors are active, the cube becomes active
            var c = GetNeighbors2(a).Count(a => a == true);

            if (c == 3) return true;

            return false;
        }

        private bool CurrentlyActiveCheck2((int x, int y, int z, int w) a) {
            // Cube is currently active
            // If exactly 2 or 3 neighbors are active, cube stays active
            var c = GetNeighbors2(a).Count(a => a == true);

            if (c == 2 || c == 3) return true;

            return false;
        }

        private void RunGeneration() {
            int minX = this.dim.Min(a => a.Key.x);
            int maxX = this.dim.Max(a => a.Key.x);
            int minY = this.dim.Min(a => a.Key.y);
            int maxY = this.dim.Max(a => a.Key.y);
            int minZ = this.dim.Min(a => a.Key.z);
            int maxZ = this.dim.Max(a => a.Key.z);

            // Make a new dimension
            var newDim = new Dictionary<(int x, int y, int z), bool>();

            for(int x=minX-1; x<=maxX+1; x++) {
                for(int y=minY-1; y<=maxY+1; y++) {
                    for(int z=minZ-1; z<=maxZ+1; z++) {
                        var addr = (x, y, z);
                        var cube = GetCubeState(addr);

                        if (cube)
                            newDim[addr] = CurrentlyActiveCheck(addr);
                        else
                            newDim[addr] = CurrentlyInactiveCheck(addr);
                    }
                }
            }

            // At the end, replace the old dimension
            this.dim = newDim;
        }

        private void RunGeneration2() {
            int minX = this.dim2.Min(a => a.Key.x);
            int maxX = this.dim2.Max(a => a.Key.x);
            int minY = this.dim2.Min(a => a.Key.y);
            int maxY = this.dim2.Max(a => a.Key.y);
            int minZ = this.dim2.Min(a => a.Key.z);
            int maxZ = this.dim2.Max(a => a.Key.z);
            int minW = this.dim2.Min(a => a.Key.w);
            int maxW = this.dim2.Max(a => a.Key.w);

            // Make a new dimension
            var newDim = new Dictionary<(int x, int y, int z, int w), bool>();

            for(int x=minX-1; x<=maxX+1; x++) {
                for(int y=minY-1; y<=maxY+1; y++) {
                    for(int z=minZ-1; z<=maxZ+1; z++) {
                        for(int w=minW-1; w<=maxW+1; w++) {
                            var addr = (x, y, z, w);
                            var cube = GetCubeState2(addr);

                            if (cube)
                                newDim[addr] = CurrentlyActiveCheck2(addr);
                            else
                                newDim[addr] = CurrentlyInactiveCheck2(addr);
                        }
                    }
                }
            }

            // At the end, replace the old dimension
            this.dim2 = newDim;
        }

        protected override string SolvePartOne()
        {
            for(int i=0; i<6; i++)
                RunGeneration();
            
            return this.dim.Count(a => a.Value == true).ToString();
        }

        protected override string SolvePartTwo()
        {
            for(int i=0; i<6; i++)
                RunGeneration2();
            
            return this.dim2.Count(a => a.Value == true).ToString();
        }
    }
}
