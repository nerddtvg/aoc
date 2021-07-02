using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2015
{

    class Day03 : ASolution
    {
        private Dictionary<(int x, int y), int> grid { get; set; }
        private int x = 0;
        private int y = 0;
        
        private Dictionary<(int x, int y), int> grid2 { get; set; }
        private int xSanta = 0;
        private int ySanta = 0;
        private int xRobo = 0;
        private int yRobo = 0;

        public Day03() : base(03, 2015, "")
        {
            // Start with the first square being delivered
            this.grid = new Dictionary<(int x, int y), int>() { { (this.x, this.y), 1 } };
            
            // Part 2
            this.grid2 = new Dictionary<(int x, int y), int>() { { (this.x, this.y), 2 } };
        }

        protected override string SolvePartOne()
        {
            foreach(var dir in Input.ToCharArray())
            {
                switch(dir)
                {
                    case '^':
                        this.y--;
                        incCount();
                        break;
                        
                    case '>':
                        this.x++;
                        incCount();
                        break;
                        
                    case 'v':
                        this.y++;
                        incCount();
                        break;
                        
                    case '<':
                        this.x--;
                        incCount();
                        break;
                }
            }

            return this.grid.Count(a => a.Value >= 1).ToString();
        }

        private void incCount()
        {
            (int x, int y) key = (this.x, this.y);

            if (this.grid.ContainsKey(key))
            {
                this.grid[key]++;
            }
            else
            {
                this.grid[key] = 1;
            }
        }

        private void incCountCharacter(int character)
        {
            (int x, int y) key;
            if (character == 0)
            {
                key = (this.xSanta, this.ySanta);
            }
            else
            {
                key = (this.xRobo, this.yRobo);
            }
            
            if (this.grid2.ContainsKey(key))
            {
                this.grid2[key]++;
            }
            else
            {
                this.grid2[key] = 1;
            }
        }

        protected override string SolvePartTwo()
        {
            int character = 0;

            foreach(var dir in Input.ToCharArray())
            {
                switch(dir)
                {
                    case '^':
                        if (character == 0)
                        {
                            this.ySanta--;
                        }
                        else
                        {
                            this.yRobo--;
                        }

                        incCountCharacter(character);
                        break;
                        
                    case '>':
                        if (character == 0)
                        {
                            this.xSanta++;
                        }
                        else
                        {
                            this.xRobo++;
                        }

                        incCountCharacter(character);
                        break;
                        
                    case 'v':
                        if (character == 0)
                        {
                            this.ySanta++;
                        }
                        else
                        {
                            this.yRobo++;
                        }

                        incCountCharacter(character);
                        break;
                        
                    case '<':
                        if (character == 0)
                        {
                            this.xSanta--;
                        }
                        else
                        {
                            this.xRobo--;
                        }

                        incCountCharacter(character);
                        break;
                }

                character = (character + 1) % 2;
            }

            return this.grid2.Count(a => a.Value > 0).ToString();
        }
    }
}
