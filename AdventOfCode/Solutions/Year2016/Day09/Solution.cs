using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace AdventOfCode.Solutions.Year2016
{

    class Day09 : ASolution
    {
        private string decompressed = string.Empty;

        public Day09() : base(09, 2016, "")
        {

        }

        private void DecompressInput(string input)
        {
            this.decompressed = string.Empty;

            var insideData = false;
            var insideMarker = false;

            // When inside a marker, are we counting the data length or repeating section
            var countingRepeat = false;

            // How long the data section is
            var strDataLength = string.Empty;
            int dataLength = 0;

            // What character we're on inside a data section
            int count = 0;

            // How many times to repeat it
            var strRepeatCount = string.Empty;
            int repeatCount = 0;

            // The data section pulled out
            var tempData = string.Empty;

            foreach(var c in input)
            {
                // For each character, we need to determine what to do
                if (insideData)
                {
                    // This character gets added to our data length
                    tempData += c;

                    // Count our character
                    count++;

                    if (count == dataLength)
                    {
                        // We've finished this section, reset!
                        for (int i = 0; i < repeatCount; i++)
                        {
                            this.decompressed += tempData;
                        }

                        // Reset the values
                        tempData = string.Empty;
                        insideData = false;
                        insideMarker = false;
                        dataLength = 0;
                        repeatCount = 0;
                    }
                }
                else if (insideMarker)
                {
                    // We're inside the marker so we are determining what to do
                    if (c == ')')
                    {
                        // Make sure we're good
                        if (!countingRepeat)
                            throw new Exception("Invalid ')' inside a marker but not the repeating length");

                        // Marker is done, parse the integers
                        dataLength = Int32.Parse(strDataLength);
                        repeatCount = Int32.Parse(strRepeatCount);

                        insideData = true;
                        insideMarker = false;
                        
                        strDataLength = string.Empty;
                        strRepeatCount = string.Empty;

                        countingRepeat = false;
                        count = 0;
                    }
                    else if (c == 'x')
                    {
                        // Split between length and count
                        countingRepeat = true;
                    }
                    else
                    {
                        // Just append this character
                        if (countingRepeat)
                            strRepeatCount += c;
                        else
                            strDataLength += c;
                    }
                }
                else
                {
                    // We either have a '(' to denote a marker or just a random character
                    if (c == '(')
                    {
                        insideMarker = true;
                        
                        strDataLength = string.Empty;
                        strRepeatCount = string.Empty;

                        dataLength = 0;
                        repeatCount = 0;
                        count = 0;
                    }
                    else
                    {
                        // Add this non-important character to the string
                        this.decompressed += c;
                    }
                }
            }

            // At the very end, make sure we were not in the middle of something
            if (insideData)
            {
                // Found invalid data I guess?
                throw new Exception($"Invalid data at the end: {tempData}");
            }
        }

        protected override string SolvePartOne()
        {
            DecompressInput(Input);
            return this.decompressed.Length.ToString();
        }

        protected override string SolvePartTwo()
        {
            return null;
        }
    }
}
