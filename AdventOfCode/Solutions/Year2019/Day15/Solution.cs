using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

using AdventOfCode.Solutions.Year2019.RepairDroid;

namespace AdventOfCode.Solutions.Year2019
{

    class Day15 : ASolution
    {
        List<RepairDroid.Tile> tiles = new List<RepairDroid.Tile>();

        public Day15() : base(15, 2019, "")
        {

        }

        protected override string SolvePartOne()
        {
            Intcode intcode = new Intcode(Input, 2);

            // We start at 0,0 and map it out
            int x = 0;
            int y = 0;
            RepairDroid.Direction direction = RepairDroid.Direction.North;

            // Add the first tile to kickstart us
            tiles.Add(new RepairDroid.Tile() { x = 0, y = 0, type = RepairDroid.TileType.Hallway });

            while(tiles.Count < 270400) {
                RepairDroid.Tile tile = tiles.First(a => a.x == x && a.y == y);

                // We know where we are, let's move
                direction = GetNextDirection(tile, direction);
                (int x, int y) pos = GetXY(tile, direction);

                // Set our direction and run
                intcode.SetInput((int) direction);
                intcode.Run();

                // Check our output
                RepairDroid.TileType newTile = (RepairDroid.TileType) Convert.ToInt32(intcode.output_register);

                // If this is a wall, don't change x,y
                if (newTile != RepairDroid.TileType.Wall) {
                    x = pos.x;
                    y = pos.y;
                }

                // Add the tile if not already known
                if (tiles.Count(a => a.x == pos.x && a.y == pos.y) == 0) {
                    tiles.Add(new RepairDroid.Tile() { x = pos.x, y = pos.y, type = newTile });
                }
            }

            return tiles.Count.ToString();
        }

        protected override string SolvePartTwo()
        {
            return string.Empty;
        }

        protected RepairDroid.Direction GetNextDirection(RepairDroid.Tile tile, RepairDroid.Direction currentDirection) {
            // Go in the order of the ENUM
            // Then if all are filled, go by the next available direction
            for(int i=(int) currentDirection+1; i<=(int) currentDirection+4; i++) {
                RepairDroid.Direction testDirection = (RepairDroid.Direction) (i%4);

                (int x, int y) pos = GetXY(tile, testDirection);

                // If we don't have a tile, we haven't explored. Let's do it!
                if (tiles.Count(a => a.x == pos.x && a.y == pos.y) == 0) return testDirection;
            }

            // We don't have a direction, all are explored
            // Find the next available direction that works
            for(int i=(int) currentDirection+1; i<=(int) currentDirection+4; i++) {
                RepairDroid.Direction testDirection = (RepairDroid.Direction) (i%4);

                (int x, int y) pos = GetXY(tile, testDirection);

                // If we don't have a tile, we haven't explored. Let's do it!
                if (tiles.First(a => a.x == pos.x && a.y == pos.y).type == RepairDroid.TileType.Hallway) return testDirection;
            }

            // Backup, this is an infinite loop so hopefully it doesn't happen
            return currentDirection;
        }

        protected (int x, int y) GetXY(RepairDroid.Tile tile, RepairDroid.Direction direction) =>
            direction switch {
                RepairDroid.Direction.North => (tile.x, tile.y-1),
                RepairDroid.Direction.South => (tile.x, tile.y+1),
                RepairDroid.Direction.West => (tile.x-1, tile.y),
                RepairDroid.Direction.East => (tile.x+1, tile.y),
                _ => throw new Exception("Invalid direction specified")
            };
    }
}

namespace AdventOfCode.Solutions.Year2019.RepairDroid {
    enum TileType {
        Wall,
        Hallway,
        Port
    }

    enum Direction {
        North,
        South,
        West,
        East
    }

    class Tile {
        public int x {get;set;}
        public int y {get;set;}
        public TileType type {get;set;}
    }
}