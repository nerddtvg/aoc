using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2019
{
    enum DoorKeyType {
        Wall,
        Passage,
        Door,
        Key,
        Entrance
    }

    class DoorLockPos {
        public int x {get;set;}
        public int y {get;set;}
        public DoorKeyType type {get;set;}
        public string value {get;set;}
        public bool locked {get;set;}
    }

    class Day18 : ASolution
    {
        List<DoorLockPos> map = new List<DoorLockPos>();

        public Day18() : base(18, 2019, "")
        {
            int x = 0;
            int y = 0;

            foreach(string line in Input.SplitByNewline()) {
                foreach(char loc in line.ToCharArray()) {
                    map.Add(new DoorLockPos() {
                        x = x,
                        y = y,
                        value = loc.ToString(),
                        type = loc switch {
                            '#' => DoorKeyType.Wall,
                            '.' => DoorKeyType.Passage,
                            '@' => DoorKeyType.Entrance,
                            _ => (65 <= (int) loc && (int) loc <= 90) ? DoorKeyType.Door : DoorKeyType.Key
                        },
                        locked = (65 <= (int) loc && (int) loc <= 90) ? true : false
                    });
                    x++;
                }

                y++;
            }
        }

        protected override string SolvePartOne()
        {
            return null;
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
