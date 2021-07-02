using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

using System.Security.Cryptography;

namespace AdventOfCode.Solutions.Year2015
{

    class Day04 : ASolution
    {

        public Day04() : base(04, 2015, "")
        {

        }

        protected override string SolvePartOne()
        {
            uint i;

            for (i = 1; i < uint.MaxValue; i++)
            {
                var str = $"{Input}{i}";
                var md5 = string.Join("", MD5.HashData(Encoding.ASCII.GetBytes(str)).SelectMany(a => a.ToString("X2")));

                // Find the MD5 hash that stats with five zeros
                if (md5.StartsWith("00000"))
                    break;
            }

            return i.ToString();
        }

        protected override string SolvePartTwo()
        {
            uint i;

            for (i = 1; i < uint.MaxValue; i++)
            {
                var str = $"{Input}{i}";
                var md5 = string.Join("", MD5.HashData(Encoding.ASCII.GetBytes(str)).SelectMany(a => a.ToString("X2")));

                // Find the MD5 hash that stats with six zeros
                if (md5.StartsWith("000000"))
                    break;
            }

            return i.ToString();
        }
    }
}
