using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Encodings.Web;

using System.Linq;

namespace AdventOfCode.Solutions.Year2015
{

    class Day08 : ASolution
    {

        public Day08() : base(08, 2015, "")
        {
            /** /
            DebugInput = @"
""""
""abc""
""aaa\""aaa""
""\x27""
";
/ **/
        }

        private string ConvertToStringLiteral(string input)
        {
            // Start left to right and parse it individually, the safest method
            var ret = "";
            int i = 0;
            int skip = 0;

            foreach(var c in input.ToCharArray())
            {
                if (skip > 0)
                {
                    i++;
                    skip--;
                    continue;
                }

                switch(c)
                {
                    case '\\':
                        {
                            // Read ahead if possible
                            if (i+1 >= input.Length)
                            {
                                ret += c;
                                break;
                            }

                            var c2 = input.Substring(i + 1, 1);

                            if (c2 == @"\")
                            {
                                ret += @"\";
                                skip = 1;
                                break;
                            }
                            else if (c2 == "\"")
                            {
                                ret += '"';
                                skip = 1;
                                break;
                            }
                            else if (c2 == "x")
                            {
                                // Read ahead another two, if possible
                                if (i+3 >= input.Length)
                                {
                                    ret += c;
                                    break;
                                }

                                var c3 = input.Substring(i + 2, 2);
                                if (Regex.IsMatch(c3, "[a-f0-9]{2}"))
                                {
                                    ret += Convert.ToChar(Convert.ToUInt32($"0x{c3}", 16)).ToString();
                                    skip = 3;
                                }
                                else
                                {
                                    // Not hex
                                    ret += c;
                                    break;
                                }
                            }
                        }
                        break;

                    default:
                        ret += c;
                        break;
                }

                // Increment
                i++;
            }

            // Remove surrounding quotes
            if (ret.StartsWith('"') && ret.EndsWith('"'))
                ret = ret.Substring(1, ret.Length - 2);

            return ret;
        }

        // A cheap way to get the encoding we want but the default replaces " with \u0022 instead of \"
        private string NewEncoding(string str) => JsonSerializer.Serialize<string>(str).Replace(@"u0022", "\"");

        protected override string SolvePartOne()
        {
            int memLength = 0;
            int strLength = 0;

            foreach(var line in Input.SplitByNewline())
            {
                memLength += line.Trim().Length;
                strLength += ConvertToStringLiteral(line.Trim()).Length;
            }

            return (memLength - strLength).ToString();
        }

        protected override string SolvePartTwo()
        {
            int newLength = 0;
            int strLength = 0;

            foreach(var line in Input.SplitByNewline())
            {
                newLength += NewEncoding(line.Trim()).Length;
                strLength += line.Trim().Length;
            }

            return (newLength - strLength).ToString();
        }
    }
}
