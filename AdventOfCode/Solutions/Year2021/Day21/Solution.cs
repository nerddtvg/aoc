using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2021
{

    class Day21 : ASolution
    {
        // Positions
        int player1Pos = 0;
        int player2Pos = 0;

        // Scores
        int player1 = 0;
        int player2 = 0;

        // Die counter
        int die = 0;
        int dieCount = 0;

        public Day21() : base(21, 2021, "Dirac Dice")
        {
            // DebugInput = "Player 1 starting position: 4\nPlayer 2 starting position: 8";

            Reset();
        }

        private void Reset()
        {
            player1 = 0;
            player2 = 0;

            die = 0;
            dieCount = 0;

            var s = Input.SplitByNewline(true, true);
            player1Pos = Int32.Parse(s[0][s[0].Length - 1].ToString());
            player2Pos = Int32.Parse(s[1][s[1].Length - 1].ToString());
        }

        private void RunPlayer(ref int playerScore, ref int playerPos)
        {
            int rolls = GetDie() + GetDie() + GetDie();

            // Ten positions, 1 through 10 not 0 through 9
            playerPos = (playerPos + rolls) % 10;

            if (playerPos == 0)
                playerPos = 10;

            playerScore += playerPos;
        }

        private int GetDie()
        {
            die = (die % 100) + 1;
            dieCount++;
            return die;
        }

        protected override string? SolvePartOne()
        {
            while (player1 < 1000 && player2 < 1000)
            {
                RunPlayer(ref player1, ref player1Pos);
                
                if (player1 >= 1000) break;

                RunPlayer(ref player2, ref player2Pos);
            }

            return (Math.Min(player1, player2) * dieCount).ToString();
        }

        protected override string? SolvePartTwo()
        {
            // For Part 2, I needed a hint and went to the megathread
            // There's no reason to track all of the games. We just need to track
            // every possible state (pos of player1, pos of player2, player1 score, player 2 score)
            // And with that, just count how many games are in each state
            // Each round, we go through and "play" the games, and whoever wins, add that
            // to their total counts.
            // Source hint: https://old.reddit.com/r/adventofcode/comments/rl6p8y/2021_day_21_solutions/hpfagnp/

            // I also relearned that multi-dimensional arrays and jagged arrays aren't the same thing
            Reset();

            // Track the number of games in each state (using 11 so we can go 1..10 for positions, ignoring zero)
            ulong[,,,] states = new ulong[11, 11, 21, 21];

            // Set the initial game
            states[this.player1Pos, this.player2Pos, 0, 0] = 1;

            // Helpful hint from that code, we can simplify our roll calculations
            // Since the die can produce any combination of (1, 2, 3), we know that
            // the only possible values are: 3, 4, 5, 6, 7, 8, 9
            // And the quantities are: 1, 3, 6, 7, 6, 3, 1
            var rolls = new List<(int role, int count)>()
            {
                (3, 1),
                (4, 3),
                (5, 6),
                (6, 7),
                (7, 6),
                (8, 3),
                (9, 1)
            };

            // Counters to keep us going
            var gameCount = 0;
            ulong player1Wins = 0;
            ulong player2Wins = 0;

            do
            {
                // We need to check every state possible
                // See how many games are in that state
            } while (gameCount > 0);

            return Math.Max(player1Wins, player2Wins).ToString();
        }
    }
}

#nullable restore
