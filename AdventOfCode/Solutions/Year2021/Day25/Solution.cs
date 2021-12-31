using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

#nullable enable

namespace AdventOfCode.Solutions.Year2021
{

    class Day25 : ASolution
    {
        private Dictionary<(int x, int y), char> cucumbers = new Dictionary<(int x, int y), char>();

        private int width = 0;
        private int height = 0;

        public Day25() : base(25, 2021, "Sea Cucumber")
        {
//             DebugInput = @"v...>>.vv>
// .vv>>.vv..
// >>.>v>...v
// >>v>>.>.v.
// v>v.vv.v..
// >.>>..v...
// .vv..>.>v.
// v.v..>>v.v
// ....v..v.>";

            cucumbers.Clear();

            int y = 0;
            int x = 0;
            foreach(var line in Input.SplitByNewline())
            {
                x = 0;
                foreach(var c in line.ToCharArray())
                {
                    this.cucumbers[(x, y)] = c;
                    x++;
                }

                y++;
            }

            this.width = x;
            this.height = y;
        }

        private int RunRound()
        {
            int moved = 0;

            // First L to R moves
            // Find all that are possible to move
            var moves = this.cucumbers
                .Where(kvp => kvp.Value == '>' && this.cucumbers[((kvp.Key.x + 1) % this.width, kvp.Key.y)] == '.')
                .Select(kvp => (oldPos: kvp.Key, newPos: (x: (kvp.Key.x + 1) % this.width, kvp.Key.y)))
                .OrderBy(movement => movement.oldPos.y)
                .ThenBy(movement => movement.oldPos.x)
                .ToList();

            // Count these
            moved += moves.Count;

            // Change the values
            moves.ForEach(movement =>
            {
                this.cucumbers[movement.newPos] = this.cucumbers[movement.oldPos];
                this.cucumbers[movement.oldPos] = '.';
            });

            // Next U to D moves
            // Find all that are possible to move
            moves = this.cucumbers
                .Where(kvp => kvp.Value == 'v' && this.cucumbers[(kvp.Key.x, (kvp.Key.y + 1) % this.height)] == '.')
                .Select(kvp => (oldPos: kvp.Key, newPos: (kvp.Key.x, y: (kvp.Key.y + 1) % this.height)))
                .OrderBy(movement => movement.oldPos.y)
                .ThenBy(movement => movement.oldPos.x)
                .ToList();

            // Count these
            moved += moves.Count;

            // Change the values
            moves.ForEach(movement =>
            {
                this.cucumbers[movement.newPos] = this.cucumbers[movement.oldPos];
                this.cucumbers[movement.oldPos] = '.';
            });

            return moved;
        }

        protected override string? SolvePartOne()
        {
            int c = 1;
            while(RunRound() > 0)
            {
                c++;
            }

            return c.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
