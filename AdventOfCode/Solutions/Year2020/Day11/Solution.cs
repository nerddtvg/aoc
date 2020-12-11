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

        public Day11() : base(11, 2020, "")
        {
            loadMap();
        }

        private void loadMap() {
            int y = 0;
            int x;

            map = new Dictionary<(int x, int y), WaitingSpotType>();

            foreach(string line in Input.SplitByNewline()) {
                x = 0;

                foreach(char c in line) {
                    map[(c, y)] = (c == '.' ? WaitingSpotType.Floor : (c == 'L' ? WaitingSpotType.Empty : WaitingSpotType.Occupied));
                }

                y++;
            }
        }

        // If it is empty and no seats adjacent are occupied, then yes
        private WaitingSpotType ToBeOccupied(int x, int y) => GetAdjacentSpots(x, y).Count(a => a == WaitingSpotType.Occupied) == 0 ? WaitingSpotType.Occupied : WaitingSpotType.Empty;
        private WaitingSpotType ToBeEmptied(int x, int y) => GetAdjacentSpots(x, y).Count(a => a == WaitingSpotType.Occupied) >= 4 ? WaitingSpotType.Empty : WaitingSpotType.Occupied;

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

        private WaitingSpotType GetSpotType(int x, int y) => map.ContainsKey((x, y)) ? map[(x, y)] : WaitingSpotType.None;

        private int runMap() {
            var newMap = new Dictionary<(int x, int y), WaitingSpotType>();
            int changes = 0;
            
            foreach(var kvp in map) {
                // If this spot is a floor tile, skip it
                if (kvp.Value == WaitingSpotType.Floor) {
                    newMap[kvp.Key] = kvp.Value;
                    continue;
                }

                switch (kvp.Value) {
                    case WaitingSpotType.Floor:
                        newMap[kvp.Key] = kvp.Value;
                        break;
                    
                    case WaitingSpotType.Empty:
                        newMap[kvp.Key] = ToBeOccupied(kvp.Key.x, kvp.Key.y);
                        changes += (newMap[kvp.Key] == kvp.Value ? 0 : 1);
                        break;
                    
                    case WaitingSpotType.Occupied:
                        newMap[kvp.Key] = ToBeEmptied(kvp.Key.x, kvp.Key.y);
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
            return null;
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
