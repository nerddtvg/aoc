using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using QuikGraph;


namespace AdventOfCode.Solutions.Year2024
{

    class Day16 : ASolution
    {
        public struct Path
        {
            public Direction direction;
            public int score;
            public Point<int> pos;
        }

        public required char[][] grid;
        public required Point<int> startPt;
        public required Point<int> endPt;
        public required Direction startDir;

        public const char start = 'S';
        public const char end = 'E';
        public const char open = '.';
        public const char wall = '#';

        public Day16() : base(16, 2024, "Reindeer Maze")
        {
            // DebugInput = @"###############
            // #.......#....E#
            // #.#.###.#.###.#
            // #.....#.#...#.#
            // #.###.#####.#.#
            // #.#.#.......#.#
            // #.#.#####.###.#
            // #...........#.#
            // ###.#.#####.#.#
            // #...#.....#.#.#
            // #.#.#.###.#.#.#
            // #.....#...#.#.#
            // #.###.#.#.#.#.#
            // #S..#.....#...#
            // ###############";

            // DebugInput = @"#################
            // #...#...#...#..E#
            // #.#.#.#.#.#.#.#.#
            // #.#.#.#...#...#.#
            // #.#.#.#.###.#.#.#
            // #...#.#.#.....#.#
            // #.#.#.#.#.#####.#
            // #.#...#.#.#.....#
            // #.#.#####.#.###.#
            // #.#.#.......#...#
            // #.#.###.#####.###
            // #.#.#...#.....#.#
            // #.#.#.#####.###.#
            // #.#.#.........#.#
            // #.#.#.#########.#
            // #S#.............#
            // #################";

            ResetGrid();
        }

        void ResetGrid() {
            grid = Input.ToCharGrid();

            grid.ForEach((line, y) => line.ForEach((c, x) =>
            {
                if (c == end)
                {
                    grid[y][x] = open;
                    endPt = new(x, y);
                }

                if (c == start)
                {
                    grid[y][x] = open;
                    startPt = new(x, y);
                }
            }));

            startDir = Direction.East;
        }

        string GetVisitedKey(Point<int> pos, Direction move) => $"{pos}-{move}";

        IEnumerable<Path> FindMoves(Path path)
        {
            // Get every possible step out of this position including the score of turning (if applicable) and stepping
            // We don't bother to turn around
            foreach(var turnDelta in new int[] { 0, -1, 1 })
            {
                var turn = (Direction)(((int)path.direction + 4 + turnDelta) % 4);

                var newPos = path.pos + Directions.directionPoint[turn];

                // Cannot move
                if (grid[newPos.y][newPos.x] == wall)
                    continue;

                // Otherwise, calculate the cost
                // Add the cost of stepping one
                var moveCost = Math.Abs(turnDelta) * 1000 + 1 + path.score;

                yield return new() { direction = turn, pos = newPos, score = moveCost };
            }
        }

        int LowestScore()
        {
            // To find the lowest score, we scour the maze for visited paths to see what will happen
            var queue = new PriorityQueue<Path, int>();
            queue.Enqueue(new() {
                direction = startDir,
                score = 0,
                pos = startPt
            }, 1);

            var visited = new Dictionary<string, int>();

            int minScore = int.MaxValue;

            // We implement BFS here with the priority queue so the first result will be our shortest path
            while (queue.TryDequeue(out Path currentState, out int priority))
            {
                // Shortcut if we have a path that is longer than a found path
                if (minScore <= currentState.score)
                    continue;

                // If we visited this already with a better score (include direction because turning matters), skip it
                var key = GetVisitedKey(currentState.pos, currentState.direction);
                if (visited.TryGetValue(key, out int visitScore))
                {
                    if (visitScore <= currentState.score)
                        continue;
                }

                // Otherwise let's queue up the next step
                visited[key] = currentState.score;

                // Get the list of possible moves
                foreach (var move in FindMoves(currentState))
                {
                    if (move.pos == endPt)
                    {
                        return move.score;
                        minScore = Math.Min(minScore, move.score);
                        continue;
                    }

                    // Otherwise let's queue up the next step
                    queue.Enqueue(move, move.score);
                }
            }

            return minScore;
        }

        protected override string? SolvePartOne()
        {
            // Time: 00:00:00.0707808
            return LowestScore().ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

