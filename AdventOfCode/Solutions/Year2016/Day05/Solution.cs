using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{

    class Day05 : ASolution
    {
        private string password { get; set; }

        public Day05() : base(05, 2016, "")
        {
            this.password = string.Empty;

            // Super brute force attack here
            for (ulong i = 0; i < ulong.MaxValue && password.Length < 8; i++)
            {
                var md5 = CreateMD5($"{Input}{i}");

                if (md5.StartsWith("00000"))
                {
                    this.password += md5.Substring(5, 1);
                }
            }
        }

        /// <summary>
        /// From: https://stackoverflow.com/questions/11454004/calculate-a-md5-hash-from-a-string?rq=1
        /// </summary>
        private static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        protected override string SolvePartOne()
        {
            return this.password.ToLower();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
