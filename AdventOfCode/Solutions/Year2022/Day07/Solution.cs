using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2022
{

    class Day07 : ASolution
    {
        public List<DirectoryNode> directories = new();

        public Day07() : base(07, 2022, "No Space Left On Device")
        {

        }

        private DirectoryNode ReadTree(List<string> lines, string name = "/")
        {
            if (name == "/")
                directories = new();

            var node = new DirectoryNode()
            {
                Name = name
            };

            directories.Add(node);

            var inLs = false;
            for (int i = 0; lines.Count > 0; i++)
            {
                var line = lines.First();
                lines.RemoveAt(0);

                if (line[0] == '$')
                    inLs = false;

                // If we get a cd, add it to the list or return
                if (line[2..4] == "cd")
                {
                    // Moved back up, return
                    if (line.Length >= 7 && line[5..7] == "..")
                    {
                        return node;
                    }

                    // Add a new subdirectory
                    node.Directories.Add(ReadTree(lines, line[5..]));
                }
                else if (line[2..4] == "ls")
                {
                    // List of information
                    inLs = true;
                } else if (inLs)
                {
                    // Have a list of something
                    if (line[0..3] == "dir")
                    {
                        node.DirectoryNames.Add(line[4..]);
                    }
                    else
                    {
                        node.Files.Add(new()
                        {
                            Name = line.Split(" ", 2, StringSplitOptions.TrimEntries)[1],
                            Size = Int32.Parse(line.Split(" ")[0])
                        });
                    }
                }
            }

            return node;
        }

        protected override string? SolvePartOne()
        {
            var tree = ReadTree(Input.SplitByNewline().Skip(1).ToList());
            return directories.Where(d => d.Size <= 100000).Sum(d => d.Size).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return string.Empty;
        }

        public struct FileNode
        {
            public string Name { get; set; }
            public int Size { get; set; }
        }

        public class DirectoryNode
        {
            public DirectoryNode? Parent { get; set; }
            public string Name { get; set; } = string.Empty;
            public List<FileNode> Files { get; set; } = new();
            public List<DirectoryNode> Directories { get; set; } = new();
            public List<string> DirectoryNames { get; set; } = new();

            public int Size => Files.Sum(f => f.Size) + Directories.Sum(d => d.Size);
        }
    }
}

