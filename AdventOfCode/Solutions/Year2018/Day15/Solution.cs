using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Diagnostics;

namespace AdventOfCode.Solutions.Year2018
{

    class Day15 : ASolution
    {
        Dictionary<(int x, int y), TileType> grid = new();
        List<Unit> units = new();

        public int minDistance { get; set; } = int.MaxValue;

        public Day15() : base(15, 2018, "Beverage Bandits")
        {
            var examples = new Dictionary<string, int>()
            {
                {
                    @"#######
                    #.G...#
                    #...EG#
                    #.#.#G#
                    #..G#E#
                    #.....#
                    #######",
                    27730
                }
            };

            foreach(var example in examples)
            {
                DebugInput = example.Key;
                ResetGrid();

                var outcome = PlayGame();

                Debug.Assert(Debug.Equals(outcome, example.Value), $"Expected: {example.Value}\nActual: {outcome}");
            }

            // Reset at the end
            DebugInput = string.Empty;
            ResetGrid();
        }

        private void ResetGrid()
        {
            int y = 0;
            int x = 0;

            grid = new();
            units = new();

            foreach(var line in Input.SplitByNewline(true))
            {
                x = 0;

                foreach(var c in line)
                {
                    if (c == '#')
                    {
                        grid[(x, y)] = TileType.Wall;
                    }
                    else
                    {
                        grid[(x, y)] = TileType.Open;

                        if (c == 'E' || c == 'G')
                        {
                            // New Elf or Goblin
                            units.Add(new Unit()
                            {
                                Type = c == 'E' ? UnitType.Elf : UnitType.Goblin,
                                x = x,
                                y = y
                            });
                        }
                    }

                    x++;
                }

                y++;
            }
        }

        protected override string? SolvePartOne()
        {
            var outcome = PlayGame();
            return outcome.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }

        private int PlayGame()
        {
            // Determine the units to play and the order
            var playableUnits = GetUnits();

            // Protection against endless loops
            int i = 0;
            int maxLoops = 1000;

            PrintGrid();

            // Play until there is only one unit type left
            while (!GameOver && i < maxLoops)
            {
                for (var k = 0; k < playableUnits.Count; k++)
                {
                    PlayUnit(playableUnits[k]);

                    // If we only complete a partial round, exit early for the math to work
                    if (GameOver && k < playableUnits.Count-1)
                        break;

                    i++;
                }

                PrintGrid();
            }

            // Return the Outcome = round count * HP remaining
            return i * units.Where(unit => unit.isAlive).Sum(unit => unit.HP);
        }

        private bool GameOver => this.units.Where(unit => unit.isAlive).Select(unit => unit.Type).Distinct().Count() == 1;

        private void PlayUnit(Unit unit)
        {
            // First double check if we're dead, in case we were killed mid-round
            if (!unit.isAlive) return;

            // Get units we can play
            var enemies = GetUnits(default, unit.Type == UnitType.Elf ? UnitType.Goblin : UnitType.Elf);

            // Anyone in range?
            var inRange = InRange(unit, enemies);

            if (inRange.Count == 0)
            {
                // Need to move
                // First find all spaces next to known enemies
                // (starting with the closest first for faster searching)
                // Distinct the list in case there is a spot next to two+ enemies
                // Then find the distance to each of those points
                // Find the minimum distance path, if ties, use tie breaker methods
                var endpoints = enemies
                    .SelectMany(enemy => GetNeighbors(enemy))
                    .OrderBy(pos => pos.ManhattanDistance((unit.x, unit.y)))
                    .Distinct();

                var positionScores = new Dictionary<(int x, int y), (int score, (int x, int y) move)>();

                // Calculate distances
                foreach (var endpoint in endpoints)
                {
                    // Reset our counter
                    minDistance = int.MaxValue;

                    var scores = new Dictionary<(int x, int y), (int score, (int x, int y) move)>()
                    {
                        { (unit.x, unit.y), (0, (unit.x, unit.y)) }
                    };
                    var queue = new PriorityQueue<(int x, int y), int>();

                    queue.Enqueue((unit.x, unit.y), 0);

                    while(queue.Count > 0)
                    {
                        var searchPos = queue.Dequeue();
                        var searchScore = scores[searchPos].score + 1;
                        var searchMove = scores[searchPos].move;

                        // Get all possible moves in this spot
                        foreach(var move in GetNeighbors(searchPos))
                        {
                            // Shortcut...
                            if (minDistance <= searchScore || (scores.ContainsKey(move) && scores[move].score <= searchScore))
                                continue;

                            // Update the move for state tracking
                            if (searchScore == 1)
                            {
                                searchMove = move;
                            }

                            // Save our spot
                            scores[move] = (searchScore, searchMove);

                            // Second shortcut
                            if (endpoint == searchPos)
                            {
                                // We found a possible solution
                                if (positionScores.ContainsKey(searchPos) && positionScores[searchPos].score < searchScore)
                                    continue;

                                // A good solution!
                                positionScores[searchPos] = (searchScore, searchMove);
                                continue;
                            }

                            // Didn't find a solution, so let's add it to the queue
                            queue.Enqueue(move, searchScore);
                        }

                        if (positionScores.Count > 0)
                            minDistance = positionScores.Min(score => score.Value.score);
                    }
                }

                // Hopefully we have moves now
                // Find the lowest positionScore
                // Tiebreaker: Reading order
                if (positionScores.Count > 0)
                {
                    var lowestScore = positionScores.Min(score => score.Value.score);
                    var move = positionScores.First(score => score.Value.score == lowestScore).Value.move;

                    if (positionScores.Count(score => score.Value.score == lowestScore) > 1)
                    {
                        // Find the first in reading order
                        move = positionScores.Where(score => score.Value.score == lowestScore)
                            .OrderBy(score => score.Value.move.x)
                            .ThenBy(score => score.Value.move.y)
                            .First()
                            .Value
                            .move;
                    }

                    // Make the move
                    // Double check this distance is one (if not, we have done math poorly)
                    if (move.ManhattanDistance((unit.x, unit.y)) != 1)
                        throw new Exception();

                    unit.x = move.x;
                    unit.y = move.y;
                }

                // Now re-find all units in range before attach
                inRange = InRange(unit, enemies);
            }

            if (inRange.Count > 0)
            {
                // Need to fire
                // the adjacent target with the fewest hit points is selected; in a tie,
                // the adjacent target with the fewest hit points which is first in reading order is selected.
                var lowestHP = inRange.Min(unit => unit.HP);
                var target = GetUnits(inRange.Where(unit => unit.HP == lowestHP)).FirstOrDefault();

                // If we have a target, we attack!
                if (target != default)
                {
                    target.HP -= unit.AttackPower;
                    target.isAlive = (target.HP > 0);
                }
            }
        }

        private void PrintGrid()
        {
            var maxY = grid.Max(kvp => kvp.Key.y);
            var maxX = grid.Max(kvp => kvp.Key.x);

            Console.WriteLine();
            for (int y = 0; y <= maxY; y++)
            {
                for (int x = 0; x <= maxY; x++)
                {
                    var pos = (x, y);

                    var unit = units.FirstOrDefault(unit => unit.isAlive && unit.x == x && unit.y == y);
                    if (unit != default)
                    {
                        Console.Write(unit.Type == UnitType.Elf ? 'E' : 'G');
                    }
                    else
                    {
                        if (grid.ContainsKey(pos) && grid[pos] == TileType.Wall)
                        {
                            Console.Write('#');
                        }
                        else
                        {
                            Console.Write('.');
                        }
                    }
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Get possible neighbor positions
        /// </summary>
        private IEnumerable<(int x, int y)> GetNeighbors((int x, int y) pos)
        {
            foreach (var tPos in new (int x, int y)[] { (pos.x - 1, pos.y), (pos.x + 1, pos.y), (pos.x, pos.y - 1), (pos.x, pos.y + 1) })
                if (grid.ContainsKey(tPos) && grid[tPos] == TileType.Open && !units.Any(unitSearch => unitSearch.x == tPos.x && unitSearch.y == tPos.y))
                    yield return tPos;
        }

        /// <summary>
        /// Get any open spaces next to this unit
        /// </summary>
        private IEnumerable<(int x, int y)> GetNeighbors(Unit unit)
        {
            return GetNeighbors((unit.x, unit.y));
        }

        /// <summary>
        /// Get all units in range already
        /// </summary>
        private List<Unit> InRange(Unit thisUnit, IEnumerable<Unit> units)
        {
            var retList = new List<Unit>();
            
            if (units.Any(unit => unit.x == thisUnit.x && unit.y == thisUnit.y - 1))
                retList.Add(units.First(unit => unit.x == thisUnit.x && unit.y == thisUnit.y - 1));

            if (units.Any(unit => unit.x == thisUnit.x && unit.y == thisUnit.y + 1))
                retList.Add(units.First(unit => unit.x == thisUnit.x && unit.y == thisUnit.y + 1));

            if (units.Any(unit => unit.x == thisUnit.x - 1 && unit.y == thisUnit.y))
                retList.Add(units.First(unit => unit.x == thisUnit.x - 1 && unit.y == thisUnit.y));

            if (units.Any(unit => unit.x == thisUnit.x + 1 && unit.y == thisUnit.y))
                retList.Add(units.First(unit => unit.x == thisUnit.x + 1 && unit.y == thisUnit.y));

            return retList;
        }

        /// <summary>
        /// Get a list of units in order
        /// </summary>
        /// <param name="units">A given list of units to process. Default: <see cref="Day15.units" /></param>
        /// <param name="type">A given type of unit to return. Defualt: <c>null</c></param>
        private List<Unit> GetUnits(IEnumerable<Unit>? units = default, UnitType? type = null)
        {
            return (units ?? this.units)
                // Start with alive
                .Where(unit => unit.isAlive)
                // Specify a type
                .Where(unit => type.HasValue ? unit.Type == type.Value : true)
                // Proper reading order
                .OrderBy(unit => unit.y)
                .ThenBy(unit => unit.x)
                .ToList();
        }

        enum UnitType
        {
            Elf,
            Goblin
        }

        enum TileType
        {
            Open,
            Wall,
            Elf,
            Goblin
        }

        /// <summary>
        /// Playable unit
        /// </summary>
        class Unit
        {
            /// <summary>
            /// Elf or Goblin
            /// </summary>
            public UnitType Type { get; set; }

            /// <summary>
            /// Pos X
            /// </summary>
            public int x { get; set; }

            /// <summary>
            /// Pos Y
            /// </summary>
            public int y { get; set; }

            /// <summary>
            /// Is this unit alive?
            /// </summary>
            public bool isAlive { get; set; } = true;

            /// <summary>
            /// Attack Power: 3
            /// </summary>
            public int AttackPower { get; set; } = 3;

            /// <summary>
            /// Hit Points: Starts at 200
            /// </summary>
            public int HP { get; set; } = 200;

            public override string ToString() => string.Format("({0,2:D},{1,2:D}) [{2,2:D}] {3}", x, y, HP, Type.ToString());
        }
    }
}

