using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{

    class Day22 : ASolution
    {
        Queue<int> player1 = new Queue<int>();
        Queue<int> player2 = new Queue<int>();

        public Day22() : base(22, 2020, "")
        {
            /*
            DebugInput = @"
            Player 1:
            9
            2
            6
            3
            1

            Player 2:
            5
            8
            4
            7
            10";
            */

            LoadPlayers();
        }

        public void LoadPlayers() {
            int playerC = 1;

            player1.Clear();
            player2.Clear();

            foreach(var player in Input.SplitByBlankLine(true)) {
                    foreach(var line in player) {
                        try {
                            if (playerC == 1)
                                player1.Enqueue(Int32.Parse(line));
                            else {
                                player2.Enqueue(Int32.Parse(line));
                            }
                        } catch {
                            // Skips the invalid player line
                        }
                    }

                playerC++;
            }
        }

        public void PlayGame() {
            int p1 = player1.Dequeue();
            int p2 = player2.Dequeue();

            if (p1 > p2) {
                player1.Enqueue(p1);
                player1.Enqueue(p2);
            } else {
                player2.Enqueue(p2);
                player2.Enqueue(p1);
            }
        }

        protected override string SolvePartOne()
        {
            while(player1.Count > 0 && player2.Count > 0) {PlayGame();}

            // Winner?
            var player = player1.Count > 0 ? player1 : player2;

            // Total
            long total = 0;
            long i = player.Count;

            while(player.Count > 0) {
                total += i-- * player.Dequeue();
            }

            return total.ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
