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
            return null;
        }
    }
}

#nullable restore
