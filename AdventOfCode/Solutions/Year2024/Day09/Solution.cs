using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2024
{

    class Day09 : ASolution
    {

        public Day09() : base(09, 2024, "Disk Fragmenter")
        {
            // DebugInput = @"2333133121414131402";
        }

        protected override string? SolvePartOne()
        {
            // 6520497575151 - Too High
            var DiskLayout = Input.ToIntArray();

            var newLayout = string.Empty;
            var debug = false;

            // Disk layout is a pair of single digit integers
            // First digit = File length
            // Second digit = Free space length
            // The position of the first digit determines the file ID value
            // To migrate files from the end, block by block, we will emulate it with loops
            var fileId = (ulong)0;

            // Our lastFileId is also the lastFilePosition
            var lastFilePosition = DiskLayout.Length - (DiskLayout.Length % 2 == 0 ? 2 : 1);
            var lastFileId = (ulong)lastFilePosition / 2;

            var positionPrefix = (ulong)0;

            // Track the results
            var checksum = (ulong)0;

            for (int position = 0; position < DiskLayout.Length && position <= lastFilePosition; position += 2)
            {
                // For the current file in this position, append the checksum
                for (uint idx = 0; idx < DiskLayout[position] && (idx + position) < DiskLayout.Length; idx++, positionPrefix++)
                {
                    if (debug)
                    {
                        Console.WriteLine($"{positionPrefix} * {fileId}");
                        newLayout += fileId;
                    }

                    checksum += fileId * positionPrefix;
                }

                // Move ahead a fileId
                fileId++;

                // Now for free space
                if (position < DiskLayout.Length - 1 && position < lastFilePosition)
                {
                    // Take our last file and work on it
                    var freeSpace = DiskLayout[position + 1];

                    while (freeSpace > 0)
                    {
                        // Move one block, update the DiskLayout
                        var fileLength = (ulong)DiskLayout[lastFilePosition];
                        var idx = (ulong)0;

                        for (; idx < fileLength && freeSpace > 0; positionPrefix++, idx++, freeSpace--)
                        {
                            if (debug)
                            {
                                newLayout += lastFileId;
                                Console.WriteLine($"{positionPrefix} * {lastFileId}");
                            }

                            checksum += positionPrefix * lastFileId;
                        }

                        // If we loop again, we need to change positionPrefix
                        // positionPrefix += fileLength;

                        // Change the disk length in case we come back to it
                        DiskLayout[lastFilePosition] = (int)(fileLength - idx);

                        if (idx == fileLength)
                        {
                            // We ran out of that file, move back
                            lastFileId--;
                            lastFilePosition -= 2;
                        }
                    }
                }
            }

            if (debug)
                Console.WriteLine(newLayout);

            return checksum.ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }
    }
}

