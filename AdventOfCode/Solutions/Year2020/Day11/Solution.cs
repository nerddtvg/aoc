using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;
using System.Collections;

namespace AdventOfCode.Solutions.Year2020
{
    enum WaitingSpotType {
        Empty,
        Occupied,
        Floor,
        None
    }

    class Day11 : ASolution
    {
        Dictionary<(int x, int y), WaitingSpotType> map;

        int maxX;
        int maxY;

        public Day11() : base(11, 2020, "")
        {
            // We set shouldtrim to true so we can indent here
            DebugInput = @"
                L.LL.LL.LL
                LLLLLLL.LL
                L.L.L..L..
                LLLL.LL.LL
                L.LL.LL.LL
                L.LLLLL.LL
                ..L.L.....
                LLLLLLLLLL
                L.LLLLLL.L
                L.LLLLL.LL
                ";
        }

        private void loadMap() {
            int y = 0;
            int x;

            map = new Dictionary<(int x, int y), WaitingSpotType>();

            foreach(string line in Input.SplitByNewline(true)) {
                x = 0;

                foreach(char c in line) {
                    map[(x, y)] = (c == '.' ? WaitingSpotType.Floor : (c == 'L' ? WaitingSpotType.Empty : WaitingSpotType.Occupied));
                    x++;
                }

                y++;
            }

            this.maxX = this.map.Max(a => a.Key.x);
            this.maxY = this.map.Max(a => a.Key.y);
        }

        // Helper to get the adjacent seats
        private List<WaitingSpotType> GetAdjacentSpots(int x, int y) => 
            new List<WaitingSpotType>() {
                GetSpotType(x-1, y-1),
                GetSpotType(x  , y-1),
                GetSpotType(x+1, y-1),
                GetSpotType(x-1, y ),
                GetSpotType(x+1, y ),
                GetSpotType(x-1, y+1),
                GetSpotType(x  , y+1),
                GetSpotType(x+1, y+1),
            };

        // Helper to get the adjacent seats
        private List<WaitingSpotType> GetVisibleSpots(int x, int y) => 
            new List<WaitingSpotType>() {
                // Up => Left to right
                getVisibleSpot(x, y, -1, -1),
                getVisibleSpot(x, y,  0, -1),
                getVisibleSpot(x, y,  1, -1),

                // Left and right
                getVisibleSpot(x, y, -1,  0),
                getVisibleSpot(x, y,  1,  0),

                // Down => Left to right
                getVisibleSpot(x, y, -1,  1),
                getVisibleSpot(x, y,  0,  1),
                getVisibleSpot(x, y,  1,  1),
            };
        
        private WaitingSpotType getVisibleSpot(int x, int y, int dx, int dy) {
            // Loop through all of the spots in the direction provided and return the first one
            for(int i=1; ; i++) {
                int tx = x + (dx * i);
                int ty = y + (dy * i);
                
                // Shortcut some checks
                if (tx < 0 || tx > maxX) return WaitingSpotType.None;
                if (ty < 0 || ty > maxY) return WaitingSpotType.None;

                var spot = GetSpotType(tx, ty);

                // Is this occupied?
                if (spot == WaitingSpotType.Empty || spot == WaitingSpotType.Occupied) return spot;
            }
        }

        private WaitingSpotType GetSpotType(int x, int y) => map.ContainsKey((x, y)) ? map[(x, y)] : WaitingSpotType.None;

        private int runMap(int part=1) {
            var newMap = new Dictionary<(int x, int y), WaitingSpotType>();
            int changes = 0;
            
            foreach(var kvp in map) {
                // If this is floor, skip it
                if (kvp.Value == WaitingSpotType.Floor) {
                    newMap[kvp.Key] = kvp.Value;
                    continue;
                }

                // Get occupied
                int count = (part == 1 ? GetAdjacentSpots(kvp.Key.x, kvp.Key.y) : GetVisibleSpots(kvp.Key.x, kvp.Key.y)).Count(a => a == WaitingSpotType.Occupied);

                if (kvp.Value == WaitingSpotType.Empty && count == 0) {
                    // Parts 1 and 2 => Empty has no spots occupied, it will be occupied
                    newMap[kvp.Key] = WaitingSpotType.Occupied;
                    changes++;
                } else if (kvp.Value == WaitingSpotType.Occupied && count >= 4 && part == 1) {
                    // Part 1 => If 4 or more adjacent spots are occupied, this becomes empty
                    newMap[kvp.Key] = WaitingSpotType.Empty;
                    changes++;
                } else if (kvp.Value == WaitingSpotType.Occupied && count >= 5 && part == 2) {
                    // Part 2 => If 5 or more *visibly* adjacent spots are occupied, this becomes empty
                    newMap[kvp.Key] = WaitingSpotType.Empty;
                    changes++;
                } else {
                    // No change
                    newMap[kvp.Key] = kvp.Value;
                }
            }

            // Set the map
            this.map = newMap;
            return changes;
        }

        private void drawMap() {
            for(int y=0; y<=maxY; y++) {
                for(int x=0; x<=maxX; x++) {
                    switch(GetSpotType(x, y)) {
                        case WaitingSpotType.Empty:
                            Console.Write("L");
                            break;
                        
                        case WaitingSpotType.Occupied:
                            Console.Write("#");
                            break;
                        
                        case WaitingSpotType.Floor:
                            Console.Write(".");
                            break;
                        
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        protected override string SolvePartOne()
        {
            loadMap();
            while(runMap(1) > 1) {}

            return this.map.Count(a => a.Value == WaitingSpotType.Occupied).ToString();
        }

        protected override string SolvePartTwo()
        {
            loadMap();
            drawMap();
            while(runMap(2) > 1) {
                drawMap();
            }

            return this.map.Count(a => a.Value == WaitingSpotType.Occupied).ToString();
        }
    }
}
