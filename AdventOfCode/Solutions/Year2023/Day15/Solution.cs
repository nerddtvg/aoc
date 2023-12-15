using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;


namespace AdventOfCode.Solutions.Year2023
{

    class Day15 : ASolution
    {

        public Day15() : base(15, 2023, "Lens Library")
        {
            // DebugInput = @"rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7";
        }

        private int GetHash(string str)
        {
            int hash = 0;

            str.ForEach(c =>
            {
                hash += c;
                hash *= 17;
                hash %= 256;
            });

            return hash;
        }

        protected override string? SolvePartOne()
        {
            return Input.Split(",").Sum(split => GetHash(split)).ToString();
        }

        protected override string? SolvePartTwo()
        {
            // We need a list of 256 empty queues to act as boxes
            var boxes = Enumerable.Range(0, 256)
                .Select(idx => new Queue<(string label, int focus)>())
                .ToList();

            var regex = new Regex(@"(?<label>[a-z]+)(?<action>[\-=])(?<focus>[0-9]*)");

            const char delete = '-';
            const char add = '=';

            regex.Matches(Input)
                .ForEach(match =>
                {
                    var label = match.Groups["label"].Value;
                    var action = match.Groups["action"].Value[0];
                    int.TryParse(match.Groups["focus"].Value, out int focus);

                    var box = GetHash(match.Groups["label"].Value);

                    if (boxes[box].Any(lens => lens.label == label))
                    {
                        // Box exists
                        if (action == delete)
                        {
                            // Remake the queue in order missing this box
                            boxes[box] = new Queue<(string label, int focus)>(boxes[box].Where(b => b.label != label));
                        }
                        else
                        {
                            // Box exists already, we need to replace it
                            boxes[box] = new Queue<(string label, int focus)>(boxes[box].Select(lens => lens.label == label ? (label, focus) : lens));
                        }
                    }
                    else if (action == add)
                    {
                        boxes[box].Enqueue((label, focus));
                    }
                });

            return boxes
                .Select((box, idx) =>
                {
                    return box.Select((lens, lensIdx) => (idx + 1) * (lensIdx + 1) * lens.focus).Sum();
                })
                .Sum()
                .ToString();
        }
    }
}

