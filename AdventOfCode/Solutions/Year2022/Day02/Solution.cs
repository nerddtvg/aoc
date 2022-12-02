using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2022
{

    class Day02 : ASolution
    {

        public Day02() : base(02, 2022, "Rock Paper Scissors")
        {

        }

        protected override string? SolvePartOne()
        {
            int score = 0;
            foreach(var line in Input.SplitByNewline())
            {
                var moves = line.Split(' ');
                var opp = GetType(moves[0][0]);
                var player = GetType(moves[1][0]);

                score += Win(opp, player) + Score(player);
            }
            return score.ToString();
        }

        protected override string? SolvePartTwo()
        {
            int score = 0;
            foreach (var line in Input.SplitByNewline())
            {
                var moves = line.Split(' ');
                var opp = GetType(moves[0][0]);

                var player = Choose(opp, (moves[1][0] == 'X' ? 0 : (moves[1][0] == 'Y' ? 3 : 6)));

                score += Win(opp, player) + Score(player);
            }
            return score.ToString();
        }

        int Win(RockPaperScissors opp, RockPaperScissors player)
        {
            if (opp == RockPaperScissors.Rock)
            {
                if (player == RockPaperScissors.Rock)
                    return 3;
                if (player == RockPaperScissors.Paper)
                    return 6;
                if (player == RockPaperScissors.Scissors)
                    return 0;
            }

            if (opp == RockPaperScissors.Paper)
            {
                if (player == RockPaperScissors.Rock)
                    return 0;
                if (player == RockPaperScissors.Paper)
                    return 3;
                if (player == RockPaperScissors.Scissors)
                    return 6;
            }

            if (opp == RockPaperScissors.Scissors)
            {
                if (player == RockPaperScissors.Rock)
                    return 6;
                if (player == RockPaperScissors.Paper)
                    return 0;
                if (player == RockPaperScissors.Scissors)
                    return 3;
            }

            throw new Exception();
        }

        RockPaperScissors Choose(RockPaperScissors opp, int loseDrawWin)
        {
            if (loseDrawWin == 0)
            {
                if (opp == RockPaperScissors.Rock)
                    return RockPaperScissors.Scissors;

                if (opp == RockPaperScissors.Paper)
                    return RockPaperScissors.Rock;

                if (opp == RockPaperScissors.Scissors)
                    return RockPaperScissors.Paper;
            }

            if (loseDrawWin == 3)
            {
                if (opp == RockPaperScissors.Rock)
                    return RockPaperScissors.Rock;

                if (opp == RockPaperScissors.Paper)
                    return RockPaperScissors.Paper;

                if (opp == RockPaperScissors.Scissors)
                    return RockPaperScissors.Scissors;
            }

            if (loseDrawWin == 6)
            {
                if (opp == RockPaperScissors.Rock)
                    return RockPaperScissors.Paper;

                if (opp == RockPaperScissors.Paper)
                    return RockPaperScissors.Scissors;

                if (opp == RockPaperScissors.Scissors)
                    return RockPaperScissors.Rock;
            }
            throw new Exception();
        }

        RockPaperScissors GetType(char x)
        {
            if (x == 'A' || x == 'X')
                return RockPaperScissors.Rock;

            if (x == 'B' || x == 'Y')
                return RockPaperScissors.Paper;

            if (x == 'C' || x == 'Z')
                return RockPaperScissors.Scissors;

            throw new Exception();
        }

        int Score(RockPaperScissors move)
        {
            if (move == RockPaperScissors.Rock)
                return 1;

            if (move == RockPaperScissors.Paper)
                return 2;

            if (move == RockPaperScissors.Scissors)
                return 3;

            throw new Exception();
        }

        enum RockPaperScissors
        {
            Rock,
            Paper,
            Scissors
        }
    }
}

