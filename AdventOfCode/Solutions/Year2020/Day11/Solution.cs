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

        }

        private void loadMap() {
            int y = 0;
            int x;

            map = new Dictionary<(int x, int y), WaitingSpotType>();

            foreach(string line in Input.SplitByNewline()) {
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

        // If it is empty and no seats adjacent are occupied, then yes
        private WaitingSpotType ToBeOccupied(int x, int y) => GetAdjacentSpots(x, y).Count(a => a == WaitingSpotType.Occupied) == 0 ? WaitingSpotType.Occupied : WaitingSpotType.Empty;

        // If 4 or more adajcent seats are occupied, empty this one
        private WaitingSpotType ToBeEmptied(int x, int y) => GetAdjacentSpots(x, y).Count(a => a == WaitingSpotType.Occupied) >= 4 ? WaitingSpotType.Empty : WaitingSpotType.Occupied;

        // If it is empty and no seats adjacent are occupied, then yes
        private WaitingSpotType ToBeOccupied2(int x, int y) => GetVisibleSpots(x, y).Count(a => a == WaitingSpotType.Occupied) == 0 ? WaitingSpotType.Occupied : WaitingSpotType.Empty;

        // If 4 or more adajcent seats are occupied, empty this one
        private WaitingSpotType ToBeEmptied2(int x, int y) => GetVisibleSpots(x, y).Count(a => a == WaitingSpotType.Occupied) >= 5 ? WaitingSpotType.Empty : WaitingSpotType.Occupied;

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
            for(int tx=x+dx; tx>=0 && tx<=maxX; tx+=dx) {
                for(int ty=y+dy; ty>=0 && ty<=maxY; ty+=dy) {
                    var spot = GetSpotType(tx, ty);

                    // Is this occupied?
                    if (spot == WaitingSpotType.Empty || spot == WaitingSpotType.Occupied) return spot;

                    // Prevent an infinite loop
                    if (dy == 0) break;
                }

                // Prevent an infinite loop
                if (dx == 0) break;
            }

            // We didn't hit anything in this direction
            return WaitingSpotType.Empty;
        }

        private WaitingSpotType GetSpotType(int x, int y) => map.ContainsKey((x, y)) ? map[(x, y)] : WaitingSpotType.None;

        private int runMap(int part=1) {
            var newMap = new Dictionary<(int x, int y), WaitingSpotType>();
            int changes = 0;
            
            foreach(var kvp in map) {
                switch (kvp.Value) {
                    case WaitingSpotType.Floor:
                        newMap[kvp.Key] = kvp.Value;
                        break;
                    
                    case WaitingSpotType.Empty:
                        newMap[kvp.Key] = (part == 1 ? ToBeOccupied(kvp.Key.x, kvp.Key.y) : ToBeOccupied2(kvp.Key.x, kvp.Key.y));
                        changes += (newMap[kvp.Key] == kvp.Value ? 0 : 1);
                        break;
                    
                    case WaitingSpotType.Occupied:
                        newMap[kvp.Key] = (part == 1 ? ToBeEmptied(kvp.Key.x, kvp.Key.y) : ToBeEmptied2(kvp.Key.x, kvp.Key.y));
                        changes += (newMap[kvp.Key] == kvp.Value ? 0 : 1);
                        break;
                }
            }

            // Set the map
            this.map = newMap;
            return changes;
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
            while(runMap(2) > 1) {}

            return this.map.Count(a => a.Value == WaitingSpotType.Occupied).ToString();
        }
    }
}
