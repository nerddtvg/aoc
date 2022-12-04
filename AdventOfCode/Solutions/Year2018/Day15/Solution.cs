using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Diagnostics;

// Part 1 Answer: 193476 (69 rounds)
// Found with another solution to assist in debugging this one

namespace AdventOfCode.Solutions.Year2018
{

    class Day15 : ASolution
    {
        Dictionary<(int x, int y), TileType> grid = new();
        List<Unit> units = new();

        public int minDistance { get; set; } = int.MaxValue;

        public const bool printDebug = true;
        public const bool printFinal = true;

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
                },
                {
                    @"#######
                    #G..#E#
                    #E#E.E#
                    #G.##.#
                    #...#E#
                    #...E.#
                    #######",
                    36334
                },
                {
                    @"#######
                    #E..EG#
                    #.#G.E#
                    #E.##E#
                    #G..#.#
                    #..E#.#
                    #######",
                    39514
                },
                {
                    @"#######
                    #E.G#.#
                    #.#G..#
                    #G.#.G#
                    #G..#.#
                    #...E.#
                    #######",
                    27755
                },
                {
                    @"#######
                    #.E...#
                    #.#..G#
                    #.###.#
                    #E#G#G#
                    #...#G#
                    #######",
                    28944
                },
                {
                    @"#########
                    #G......#
                    #.E.#...#
                    #..##..G#
                    #...##..#
                    #...#...#
                    #.G...G.#
                    #.....G.#
                    #########",
                    18740
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
            // return "";
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }

        private int PlayGame()
        {
            // Protection against endless loops
            int i = 0;
            int maxLoops = 1000;

            if (printDebug) PrintGrid();

            // Play until there is only one unit type left
            while (!GameOver && i < maxLoops)
            {
                // Determine the units to play and the order
                var playableUnits = GetUnits();

                for (var k = 0; k < playableUnits.Count; k++)
                {       
                    // If we only complete a partial round, exit early for the math to work
                    if (GameOver)
                    {
                        i--;
                        break;
                    }

                    PlayUnit(playableUnits[k]);
                }

                i++;

                if (printDebug) PrintGrid();
            }

            if (printFinal && !printDebug)
                PrintGrid();

            if (printDebug || printFinal)
            {
                Console.WriteLine($"Complete Rounds: {i}");
                Console.WriteLine($"Remaining HP: {units.Where(unit => unit.isAlive).Sum(unit => unit.HP)}");
            }

            // Return the Outcome = round count * HP remaining
            return i * units.Where(unit => unit.isAlive).Sum(unit => unit.HP);
        }

        private bool GameOver => this.units.Where(unit => unit.isAlive).Select(unit => unit.Type).Distinct().Count() == 1;

        struct FoundAnswer
        {
            public (int x, int y) endpoint;
            public (int x, int y) target;
            public int score;
            public (int x, int y) startingMove;

            public FoundAnswer((int x, int y) endpoint, (int x, int y) target, int score, (int x, int y) startingMove)
            {
                this.endpoint = endpoint;
                this.target = target;
                this.score = score;
                this.startingMove = startingMove;
            }
        }

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
                    .SelectMany(enemy => GetOpenNeighbors(enemy).Select(endpoint => (enemy, endpoint)))
                    .OrderBy(target => target.endpoint.ManhattanDistance((unit.x, unit.y)))
                    .Distinct();

                var positionScores = new List<FoundAnswer>();

                // Calculate distances
                foreach ((var enemy, var endpoint) in endpoints)
                {
                    // Reset our counter
                    minDistance = int.MaxValue;

                    var scores = new Dictionary<(int x, int y), (int score, (int x, int y) startingMove)>()
                    {
                        { (unit.x, unit.y), (0, (unit.x, unit.y)) }
                    };
                    var queue = new PriorityQueue<(int x, int y), int>();

                    queue.Enqueue((unit.x, unit.y), 0);

                    while(queue.Count > 0)
                    {
                        var searchPos = queue.Dequeue();
                        var searchScore = scores[searchPos].score + 1;
                        var startingMove = scores[searchPos].startingMove;

                        // Get all possible moves in this spot
                        foreach(var newPos in GetOpenNeighbors(searchPos))
                        {
                            // Shortcut...
                            // Don't skip if minDistance == searchScore so we can compare positions later
                            if (minDistance < searchScore)
                                continue;

                            // Update the move for state tracking
                            if (searchScore == 1)
                            {
                                startingMove = newPos;
                            }

                            if (scores.ContainsKey(newPos) && scores[newPos].score <= searchScore)
                            {
                                // Shortcut only if score < searchScore
                                if (scores[newPos].score < searchScore) continue;

                                // FOUND Bug: This was the issue I was having
                                // When a square was searched more than once with the same searchScore, the last
                                // startingMove overwrote the scores array. This checks against the tie breaker rules.
                                // If it equals the score, we check against the reading order of the starting points
                                if (new (int x, int y)[] { scores[newPos].startingMove, startingMove }.ReadingOrder().First() != startingMove)
                                    continue;
                            }

                            // Save our spot
                            scores[newPos] = (searchScore, startingMove);

                            // Second shortcut
                            if (endpoint == newPos)
                            {
                                // We found a possible solution
                                if (positionScores.Any(answer => answer.endpoint == endpoint))
                                {
                                    if (positionScores.Any(answer => answer.endpoint == endpoint && answer.score < searchScore))
                                        continue;
                                }

                                // A good solution!
                                positionScores.Add(new FoundAnswer(endpoint, (enemy.x, enemy.y), searchScore, startingMove));
                                continue;
                            }

                            // Make sure we're not requeueing the same thing
                            var newPriority = WeightedPriority(newPos, endpoint, searchScore);
                            if (queue.UnorderedItems.Any(kvp => kvp.Element == newPos && kvp.Priority <= newPriority))
                            {
                                continue;
                            }

                            // Didn't find a solution, so let's add it to the queue
                            queue.Enqueue(newPos, newPriority);
                        }

                        if (positionScores.Count > 0)
                            minDistance = positionScores.Min(answer => answer.score);
                    }
                }

                // Hopefully we have moves now
                // Find the lowest positionScore
                // Tiebreaker: Reading order of various things
                if (positionScores.Count > 0)
                {
                    // First find all of the minimum moves
                    // Recalc to be safe
                    minDistance = positionScores.Min(answer => answer.score);

                    // All endpoints that fit this minimum distance, then find the first in reading order
                    var endpoint = positionScores
                        .Where(answer => answer.score == minDistance)
                        .Select(answer => answer.endpoint)
                        .ReadingOrder()
                        .First();

                    // If we have multiple choices...
                    // Find the startingMove to endppint first in Reading Order
                    var move = positionScores
                        .Where(answer => answer.endpoint == endpoint && answer.score == minDistance)
                        .Select(answer => answer.startingMove)
                        .ReadingOrder()
                        .First();

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
                }
            }
        }

        /// <summary>
        /// Generate a weighted score for the queue
        /// </summary>
        private int WeightedPriority((int a, int b) start, (int a, int b) end, int score)
        {
            return score + (int)start.ManhattanDistance(end);
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

                // Print HPs
                var str = "   " + string.Join(", ",
                    units
                    .Where(unit => unit.isAlive && unit.y == y)
                    .OrderBy(unit => unit.x)
                    .Select(unit => string.Format("{0}({1,3:D})", (unit.Type == UnitType.Elf ? 'E' : 'G'), unit.HP))
                );
                Console.Write(str);

                Console.WriteLine();
            }
        }

        /// <summary>
        /// Get possible neighbor positions
        /// </summary>
        private IEnumerable<(int x, int y)> GetOpenNeighbors((int x, int y) pos)
        {
            foreach (var tPos in new (int x, int y)[] { (pos.x - 1, pos.y), (pos.x + 1, pos.y), (pos.x, pos.y - 1), (pos.x, pos.y + 1) })
                if (grid.ContainsKey(tPos) && grid[tPos] == TileType.Open && !units.Any(unitSearch => unitSearch.isAlive && unitSearch.x == tPos.x && unitSearch.y == tPos.y))
                    yield return tPos;
        }

        /// <summary>
        /// Get any open spaces next to this unit
        /// </summary>
        private IEnumerable<(int x, int y)> GetOpenNeighbors(Unit unit)
        {
            return GetOpenNeighbors((unit.x, unit.y));
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
            public bool isAlive { get => HP > 0; }

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

