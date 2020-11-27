using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2019
{

    class GameTile {
        public int x {get;set;}
        public int y {get;set;}
        public int tile {get;set;}
    }

    class Day13 : ASolution
    {

        enum TileType {
            Empty,
            Wall,
            Block,
            Paddle,
            Ball
        }

        public string[] output = new string[] {
            " ",
            "+",
            "#",
            "_",
            "o"
        };

        public List<GameTile> tiles = new List<GameTile>();

        public Intcode intcode = null;

        public Day13() : base(13, 2019, "")
        {
        }

        private void SetTiles() {
            while(intcode.State == State.Waiting) {
                // Each run is: x, y, tile
                GameTile tile = new GameTile();

                tile.x = Convert.ToInt32(intcode.output_register);
                intcode.Run();

                tile.y = Convert.ToInt32(intcode.output_register);
                intcode.Run();

                tile.tile = Convert.ToInt32(intcode.output_register);
                intcode.Run();

                tiles.Add(tile);
            }
        }

        protected override string SolvePartOne()
        {
            intcode = new Intcode(Input, 2);
            intcode.Run();

            SetTiles();

            return (tiles.Count(c => c.tile == (int) TileType.Block)).ToString();
        }

        protected override string SolvePartTwo()
        {
            intcode = new Intcode(Input, 2);
            intcode.memory[0] = 2;
            intcode.Run();

            int score = 0;
            bool stopNextScore = false;

            while(intcode.State != State.Stopped) {
                GameTile tile = new GameTile();

                tile.x = Convert.ToInt32(intcode.output_register);
                intcode.Run();

                tile.y = Convert.ToInt32(intcode.output_register);
                intcode.Run();

                tile.tile = Convert.ToInt32(intcode.output_register);

                // Score happens at the end of a screen draw
                if (tile.x == -1) {
                    score = tile.tile;

                    // Draw the screen
                    Console.WriteLine($"Score: {score}");
                    int maxX = tiles.Max(x => x.x);
                    int maxY = tiles.Max(y => y.y);

                    int y = 0;

                    while (y <= maxY) {
                        for(int x=0; x<=maxX; x++) {
                            GameTile t = tiles.Where(t => t.x == x && t.y == y).First();
                            
                            Console.Write(this.output[t.tile]);
                        }

                        Console.WriteLine();
                        y++;
                    }

                    Console.WriteLine();

                    if (stopNextScore) break;
                } else {
                    if (tiles.Count(t => t.x == tile.x && t.y == tile.y) > 0) {
                        GameTile t = tiles.Where(t => t.x == tile.x && t.y == tile.y).First();
                        tiles.Remove(t);
                    }

                    tiles.Add(tile);
                }

                
                stopNextScore = (tiles.Count > 0 && tiles.Count(c => c.tile == (int) TileType.Block) == 0);

                // Check where the ball is
                GameTile ball = tiles.Where(t => t.tile == (int) TileType.Ball).FirstOrDefault();
                GameTile paddle = tiles.Where(t => t.tile == (int) TileType.Paddle).FirstOrDefault();

                intcode.ClearInput();
                
                if (ball != null && paddle != null) {
                    if (ball.x < paddle.x) intcode.SetInput(-1);
                    else if (ball.x > paddle.x) intcode.SetInput(1);
                    else intcode.SetInput(0);
                }

                intcode.Run();
            }

            return score.ToString();
        }
    }
}
