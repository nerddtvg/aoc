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
        private string password2 { get; set; }

        public Day05() : base(05, 2016, "")
        {
            this.password = string.Empty;
            this.password2 = "________";

            // Super brute force attack here
            for (ulong i = 0; i < ulong.MaxValue && (password.Length < 8 || password2.Contains("_")); i++)
            {
                var md5 = CreateMD5($"{Input}{i}");

                if (md5.StartsWith("00000"))
                {
                    // Part 1
                    if (this.password.Length < 8)
                        this.password += md5.Substring(5, 1);

                    // Part 2 work
                    var posStr = md5.Substring(5, 1);
                    int pos = 0;

                    if (Int32.TryParse(posStr, out pos))
                    {
                        // Must be valid and unused
                        if (pos <= 7 && this.password2.Substring(pos, 1) == "_")
                        {
                            // Combine the rest of the password
                            this.password2 =
                                // Before the given position
                                (pos > 0 ? this.password2.Substring(0, pos) : "")
                                +
                                // The new character
                                md5.Substring(6, 1)
                                +
                                // After the given position
                                (pos < 7 ? this.password2.Substring(pos + 1) : "");
                        }
                    }
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
            return this.password2.ToLower();
        }
    }
}
