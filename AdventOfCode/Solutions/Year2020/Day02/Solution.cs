using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2020
{
    class Password {
        public int min {get;set;}
        public int max {get;set;}
        public char c {get;set;}
        public string password {get;set;}

        public Password(string line) {
            string[] a = line.Split(" ");

            this.min = Convert.ToInt32(a[0].Split("-")[0]);
            this.max = Convert.ToInt32(a[0].Split("-")[1]);

            this.c = a[1].ToCharArray()[0];

            this.password = a[2];
        }

        public bool IsValid() {
            int reqCharCount = password.ToCharArray().Where(a => a == this.c).Count();

            return (this.min <= reqCharCount && reqCharCount <= this.max);
        }
    }

    class Day02 : ASolution
    {
        List<Password> passwords = new List<Password>();

        public Day02() : base(02, 2020, "")
        {
            Input.SplitByNewline().ToList().ForEach(a => passwords.Add(new Password(a)));
        }

        protected override string SolvePartOne()
        {
            return passwords.Count(a => a.IsValid()).ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
