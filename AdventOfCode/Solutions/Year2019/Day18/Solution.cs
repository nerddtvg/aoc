using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2019
{
    enum DoorKeyType
    {
        Wall,
        Passage,
        Door,
        Key
    }

    class DoorLockPos
    {
        public int x { get; set; }
        public int y { get; set; }
        public DoorKeyType type { get; set; }
        /// <summary>
        /// What character is this door or key
        /// </summary>
        /// <value></value>
        public char? value { get; set; }
        /// <summary>
        /// Notes if the door is unlocked and/or key collected
        /// </summary>
        /// <value></value>
        public bool collected { get; set; }
    }

    class Day18 : ASolution
    {
        Dictionary<(int x, int y), DoorLockPos> map = new();

        public (int x, int y) start = (0, 0);

        public Day18() : base(18, 2019, "Many-Worlds Interpretation")
        {
            ResetGrid();
        }

        private void ResetGrid()
        {
            int x = 0;
            int y = 0;

            map = new();

            foreach (string line in Input.SplitByNewline())
            {
                foreach (char loc in line.ToCharArray())
                {
                    if (loc == '@')
                        start = (x, y);

                    map.Add((x, y), new DoorLockPos()
                    {
                        value = loc.ToString().ToUpperInvariant()[0],
                        type = loc switch
                        {
                            '#' => DoorKeyType.Wall,
                            '.' => DoorKeyType.Passage,
                            '@' => DoorKeyType.Passage,
                            _ => (65 <= (int)loc && (int)loc <= 90) ? DoorKeyType.Door : DoorKeyType.Key
                        },
                        collected = false
                    });
                    x++;
                }

                y++;
            }
        }

        protected override string SolvePartOne()
        {
            return string.Empty;
        }

        protected override string SolvePartTwo()
        {
            return string.Empty;
        }
    }
}
