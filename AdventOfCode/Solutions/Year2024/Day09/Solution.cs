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
            var DiskLayout = Input.ToIntArray();

            var newLayout = string.Empty;
            var debug = false;

            // Disk layout is a pair of single digit integers
            // First digit = File length
            // Second digit = Free space length
            // The position of the first digit determines the file ID value
            // To migrate files from the end, block by block, we will emulate it with loops
            var fileId = (ulong)0;

            // Determine the lastFileIdx and the lastFileId
            var lastFileIdx = DiskLayout.Length - (DiskLayout.Length % 2 == 0 ? 2 : 1);
            var lastFileId = (ulong)lastFileIdx / 2;

            var position = (ulong)0;

            // Track the results
            var checksum = (ulong)0;

            for (int layoutIdx = 0; layoutIdx < DiskLayout.Length; layoutIdx += 2, fileId++)
            {
                // For the current file in this position, append the checksum
                for (var idx = 0; idx < DiskLayout[layoutIdx]; idx++, position++)
                {
                    if (debug)
                    {
                        Console.WriteLine($"{position} * {fileId}");
                        newLayout += fileId;
                    }

                    checksum += fileId * position;
                }

                // Now for free space
                if (layoutIdx < DiskLayout.Length - 1)
                {
                    // How much free space do we have next?
                    var freeSpaceIdx = layoutIdx + 1;
                    while (DiskLayout[freeSpaceIdx] > 0 && fileId < lastFileId)
                    {
                        // Move one block from lastFileIdx to freeSpaceIdx, update the DiskLayout
                        for (var idx = 0; idx < DiskLayout[lastFileIdx] && DiskLayout[freeSpaceIdx] > 0; position++, idx++, DiskLayout[freeSpaceIdx]--, DiskLayout[lastFileIdx]--)
                        {
                            if (debug)
                            {
                                newLayout += lastFileId;
                                Console.WriteLine($"{position} * {lastFileId}");
                            }

                            checksum += position * lastFileId;
                        }

                        // Change the disk length in case we come back to it
                        // Prevents main loop from counting them as well
                        if (DiskLayout[lastFileIdx] == 0)
                        {
                            // We ran out of that file, move back
                            lastFileId--;
                            lastFileIdx -= 2;
                        }
                    }
                }
            }

            if (debug)
                Console.WriteLine(newLayout);

            // Time: 00:00:00.0046433
            return checksum.ToString();
        }

        protected override string? SolvePartTwo()
        {
            var DiskLayout = Input.ToIntArray();

            var newLayout = string.Empty;
            var debug = false;

            // Disk layout is a pair of single digit integers
            // First digit = File length
            // Second digit = Free space length
            // The position of the first digit determines the file ID value
            // To migrate files from the end, block by block, we will emulate it with loops

            // Determine the lastFileIdx and the lastFileId
            var lastFileIdx = DiskLayout.Length - (DiskLayout.Length % 2 == 0 ? 2 : 1);
            var fileId = (ulong)lastFileIdx / 2;

            // Track padding
            var padding = new List<int>();
            DiskLayout.ForEach(i => padding.Add(0));

            // Track the results
            var checksum = (ulong)0;

            // Can't move the first file
            for (int layoutIdx = lastFileIdx; layoutIdx >= 0; layoutIdx -= 2, fileId--)
            {
                // See if this file can move left anywhere
                for (int freeIdx = 1; freeIdx < layoutIdx; freeIdx += 2)
                {
                    if (DiskLayout[layoutIdx] <= DiskLayout[freeIdx])
                    {
                        // New position!
                        for (int i = 0; i < DiskLayout[layoutIdx]; i++)
                            checksum += (ulong)(DiskLayout[..freeIdx].Sum() + padding[..freeIdx].Sum() + i) * fileId;

                        // Pad the "file" position before freeIdx
                        padding[freeIdx - 1] += DiskLayout[layoutIdx];

                        // Remove the file's length from freeIdx
                        DiskLayout[freeIdx] -= DiskLayout[layoutIdx];

                        // Remove this file
                        DiskLayout[layoutIdx] = 0;

                        // Exit
                        break;
                    }
                }

                if (DiskLayout[layoutIdx] > 0)
                {
                    // Didn't move
                    for (int i = 0; i < DiskLayout[layoutIdx]; i++)
                        checksum += (ulong)(DiskLayout[..layoutIdx].Sum() + padding[..layoutIdx].Sum() + i) * fileId;
                }
            }

            if (debug)
                Console.WriteLine(newLayout);

            // Time: 00:00:00.4708549
            return checksum.ToString();
        }
    }
}

