using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;
using System.Diagnostics;

namespace AdventOfCode.Solutions.Year2021
{

    class Day18 : ASolution
    {
        public const bool printDebug = false;

        public Day18() : base(18, 2021, "Snailfish")
        {
            var ParsingSample = @"
[1,2]
[[1,2],3]
[9,[8,7]]
[[1,9],[8,5]]
[[[[1,2],[3,4]],[[5,6],[7,8]]],9]
[[[9,[3,8]],[[0,9],6]],[[[3,7],[4,9]],3]]
[[[[1,3],[5,3]],[[1,3],[8,7]]],[[[4,9],[6,9]],[[8,2],[7,3]]]]";

            // Make sure our parsing is going correctly
            foreach(var line in ParsingSample.SplitByNewline())
            {
                Debug.Assert(Debug.Equals(SnailfishNode.Parse(line).ToString(), line));
            }

            // Explode
            var explodeExamples = new Dictionary<string, string>()
            {
                { "[[[[[9,8],1],2],3],4]", "[[[[0,9],2],3],4]" },
                { "[7,[6,[5,[4,[3,2]]]]]", "[7,[6,[5,[7,0]]]]" },
                { "[[6,[5,[4,[3,2]]]],1]", "[[6,[5,[7,0]]],3]" },
                { "[[3,[2,[1,[7,3]]]],[6,[5,[4,[3,2]]]]]", "[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]" },
                { "[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]", "[[3,[2,[8,0]]],[9,[5,[7,0]]]]" }
            };

            foreach(var explode in explodeExamples)
            {
                var node = SnailfishNode.Parse(explode.Key);
                Debug.Assert(node.Explode(), $"Explode failed: {explode.Key}");
                Debug.Assert(Debug.Equals(node.ToString(), explode.Value), $"Expected: {explode.Value}\nActual: {node.ToString()}");
            }

            // Split
            Debug.Assert(Debug.Equals("[5,5]", SnailfishNode.SplitNode(10).ToString()));
            Debug.Assert(Debug.Equals("[5,6]", SnailfishNode.SplitNode(11).ToString()));
            Debug.Assert(Debug.Equals("[6,6]", SnailfishNode.SplitNode(12).ToString()));

            // Addition
            var additionExamples = new Dictionary<string, string>()
            {
                {
                    @"[[[[4,3],4],4],[7,[[8,4],9]]]
                    [1,1]",
                    "[[[[0,7],4],[[7,8],[6,0]]],[8,1]]"
                },
                {
                    @"[1,1]
                    [2,2]
                    [3,3]
                    [4,4]",
                    "[[[[1,1],[2,2]],[3,3]],[4,4]]"
                },
                {
                    @"[1,1]
                    [2,2]
                    [3,3]
                    [4,4]
                    [5,5]",
                    "[[[[3,0],[5,3]],[4,4]],[5,5]]"
                },
                {
                    @"[1,1]
                    [2,2]
                    [3,3]
                    [4,4]
                    [5,5]
                    [6,6]",
                    "[[[[5,0],[7,4]],[5,5]],[6,6]]"
                },
                {
                    @"[[[0,[4,5]],[0,0]],[[[4,5],[2,6]],[9,5]]]
                    [7,[[[3,7],[4,3]],[[6,3],[8,8]]]]",
                    "[[[[4,0],[5,4]],[[7,7],[6,0]]],[[8,[7,7]],[[7,9],[5,0]]]]"
                }
                ,{
                    @"[[[0,[4,5]],[0,0]],[[[4,5],[2,6]],[9,5]]]
                    [7,[[[3,7],[4,3]],[[6,3],[8,8]]]]
                    [[2,[[0,8],[3,4]]],[[[6,7],1],[7,[1,6]]]]
                    [[[[2,4],7],[6,[0,5]]],[[[6,8],[2,8]],[[2,1],[4,5]]]]
                    [7,[5,[[3,8],[1,4]]]]
                    [[2,[2,2]],[8,[8,1]]]
                    [2,9]
                    [1,[[[9,3],9],[[9,0],[0,7]]]]
                    [[[5,[7,4]],7],1]
                    [[[[4,2],2],6],[8,7]]",
                    "[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]"
                }
            };

            foreach(var addition in additionExamples)
            {
                var add = AddLines(addition.Key.SplitByNewline(true));
                Debug.Assert(
                    Debug.Equals(
                        add.ToString(),
                        addition.Value
                    ),
                    $"Expected: {addition.Value}\nActual: {add.ToString()}"
                );
            }
        }

        protected override string? SolvePartOne()
        {
            return null;
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }

        /// <summary>
        /// For an array of lines, add the first two lines together, then add the third, and so on
        /// </summary>
        private SnailfishNode AddLines(string[] lines)
        {
            if (lines.Length < 2)
                throw new Exception();

            var tLine = SnailfishNode.Parse(lines[0]);
            foreach(var line in lines.Skip(1))
            {
                var newLine = SnailfishNode.Parse(line);

                Debug.WriteLineIf(printDebug, $"  {tLine.ToString()}");
                Debug.WriteLineIf(printDebug, $"+ {newLine.ToString()}");
                tLine = SnailfishNode.AddNodes(tLine, newLine);
                Debug.WriteLineIf(printDebug, $"= {tLine.ToString()}");
                Debug.WriteLineIf(printDebug, "");
            }

            return tLine;
        }

        class SnailfishNode
        {
            public SnailfishNode? Left { get; set; }
            public SnailfishNode? Right { get; set; }

            public SnailfishNode? Parent { get; set; }

            public int? LeftRegular { get; set; }
            public int? RightRegular { get; set; }

            /// <summary>
            /// Receives a line of Snailfish numbers and parses it into a tree
            /// </summary>
            public static SnailfishNode Parse(string input)
            {
                if (input.Length <= 0)
                    throw new ArgumentException(nameof(input));

                if (input[0] != '[' || input[input.Length - 1] != ']')
                    throw new ArgumentException(nameof(input));

                var tree = new SnailfishNode();

                // Track if we are inside a subnode
                int subNodeCount = 0;
                var tString = string.Empty;

                foreach(var c in input[1..(input.Length - 1)])
                {
                    switch(c)
                    {
                        case '[':
                            subNodeCount++;
                            tString += c;
                            break;

                        case ',':
                            if (subNodeCount == 0)
                            {
                                // Found our center, process the string
                                if (tString[0] == '[')
                                {
                                    tree.Left = SnailfishNode.Parse(tString);
                                    tree.Left.Parent = tree;
                                }
                                else
                                {
                                    tree.LeftRegular = Int32.Parse(tString);
                                }

                                tString = string.Empty;
                            }
                            else
                            {
                                tString += c;
                            }
                            break;

                        case ']':
                            subNodeCount--;
                            tString += c;
                            break;

                        default:
                            tString += c;
                            break;
                    }
                }

                // Finish part 2 of the string
                if (tString[0] == '[')
                {
                    tree.Right = SnailfishNode.Parse(tString);
                    tree.Right.Parent = tree;
                }
                else
                {
                    tree.RightRegular = Int32.Parse(tString);
                }

                return tree;
            }

            public void Reduce()
            {
                // If this is not the top of a tree, error out
                if (this.Parent != default)
                    throw new Exception("Must reduce from the top.");

                // We track if changes have occurred and restart as needed
                bool changes = false;
                do
                {
                    Debug.WriteLineIf(Day18.printDebug, $"  * {ToString()}");

                    // Reset each loop
                    changes = false;

                    // First try to explode
                    if (Left?.Explode() ?? false)
                    {
                        changes = true;
                        continue;
                    }

                    // First try to explode
                    if (Right?.Explode() ?? false)
                    {
                        changes = true;
                        continue;
                    }

                    // Then split
                    if (Left?.Split() ?? false)
                    {
                        changes = true;
                        continue;
                    }

                    // Then split
                    if (Right?.Split() ?? false)
                    {
                        changes = true;
                        continue;
                    }
                } while (changes);
            }

            public bool Explode()
            {
                // First check children
                if (Left?.Explode() ?? false)
                    return true;

                if (Right?.Explode() ?? false)
                    return true;

                // Determine if we are 3 deep and have children
                int depth = 1;
                var p = this.Parent;

                while(p != default)
                {
                    depth++;
                    p = p.Parent;
                }

                // Need to explode if we have children
                if (depth == 4 && (Left != default || Right != default))
                {
                    if (Left != default)
                    {
                        // Left explodes
                        // This finds the next node on the Left that has a RightRegular value
                        var leftNode = FindLeftMostRegularNumberNode();

                        // Our RightRegular += Left.RightRegular
                        // Or if we have a Right node, LeftRegular
                        if (Right != default)
                            Right.LeftRegular += Left.RightRegular;
                        else
                            RightRegular += Left.RightRegular;

                        // Left becomes LeftRegular == 0
                        // Add Left.LeftRegular to leftNode.RightRegular
                        if (leftNode != default)
                        {
                            if (leftNode.RightRegular.HasValue)
                                leftNode.RightRegular += Left.LeftRegular;
                            else
                            {
                                // It's possible we came at it from the same level on the right
                                leftNode.LeftRegular += Left.LeftRegular;
                            }
                        }

                        Left = default;
                        LeftRegular = 0;
                    }
                    else if (Right != default)
                    {
                        // Right explodes
                        // This finds the next node on the Right that has a LeftRegular value
                        var rightNode = FindRightMostRegularNumberNode();

                        // Different: For Right explosions, there is no Left node
                        // because otherwise the Left would have been exploded first
                        if (Left != default)
                            throw new Exception();

                        // Our LeftRegular += Right.LeftRegular
                        LeftRegular += Right.LeftRegular;

                        // Right becomes RightRegular == 0
                        // Add Right.RightRegular to rightNode.LeftRegular
                        if (rightNode != default)
                        {
                            if (rightNode.LeftRegular.HasValue)
                                rightNode.LeftRegular += Right.RightRegular;
                            else
                            {
                                // It's possible we came at it from the same level on the left
                                rightNode.RightRegular += Right.RightRegular;
                            }
                        }

                        Right = default;
                        RightRegular = 0;
                    }

                    return true;
                }

                return false;
            }

            /// <summary>
            /// Find the node that owns the Left-Most Regular Number
            /// </summary>
            public SnailfishNode? FindLeftMostRegularNumberNode()
            {
                // Go up until we find the first viable Left node (one that doesn't point back to this),
                // and then proceed down the tree Right and look for the first RightRegular
                var tParent = Parent;
                var tNode = this;
                while (tParent != default)
                {
                    // If "we" are the Right node, move up another level
                    if (tParent.Left == tNode)
                    {
                        tNode = tParent;
                        tParent = tParent.Parent;
                    }
                    else
                    {
                        // We have moved up and to the Left, now look down to the Right for our closest neighbor
                        // This cannot go into an infinite loop because we're guaranteed to have a Regular number
                        if (tParent.LeftRegular.HasValue)
                            return tParent;

                        // First move down to the Left
                        tParent = tParent.Left;

                        // Otherwise, go down the tree now
                        while(tParent != default)
                        {
                            if (tParent.RightRegular.HasValue)
                                return tParent;

                            tParent = tParent.Right;
                        }
                    }
                }

                return default;
            }

            /// <summary>
            /// Find the node that owns the Right-Most Regular Number
            /// </summary>
            public SnailfishNode? FindRightMostRegularNumberNode()
            {
                // Go up until we find the first viable Right node (one that doesn't point back to this),
                // and then proceed down the tree Left and look for the first LeftRegular
                var tParent = Parent;
                var tNode = this;
                while (tParent != default)
                {
                    // If "we" are the Right node, move up another level
                    if (tParent.Right == tNode)
                    {
                        tNode = tParent;
                        tParent = tParent.Parent;
                    }
                    else
                    {
                        // We have moved up and to the Right, now look down to the Left for our closest neighbor
                        // This cannot go into an infinite loop because we're guaranteed to have a Regular number
                        if (tParent.RightRegular.HasValue)
                            return tParent;

                        // First move down to the Right
                        tParent = tParent.Right;

                        // Otherwise, go down the tree now
                        while (tParent != default)
                        {
                            if (tParent.LeftRegular.HasValue)
                                return tParent;

                            tParent = tParent.Left;
                        }
                    }
                }

                return default;
            }

            public bool Split()
            {
                // First check children
                if (Left?.Split() ?? false)
                    return true;

                if (LeftRegular.HasValue && LeftRegular.Value >= 10)
                {
                    // Need to split
                    Left = SplitNode(LeftRegular.Value);
                    Left.Parent = this;
                    LeftRegular = default;

                    return true;
                }

                if (Right?.Split() ?? false)
                    return true;

                if (RightRegular.HasValue && RightRegular.Value >= 10)
                {
                    // Need to split
                    Right = SplitNode(RightRegular.Value);
                    Right.Parent = this;
                    RightRegular = default;

                    return true;
                }

                return false;
            }

            /// <summary>
            /// Adding two nodes is simply combining and then reducing
            /// </summary>
            public static SnailfishNode AddNodes(SnailfishNode a, SnailfishNode b)
            {
                var newParent = new SnailfishNode()
                {
                    Left = a,
                    Right = b
                };

                a.Parent = newParent;
                b.Parent = newParent;

                newParent.Reduce();

                return newParent;
            }

            public static SnailfishNode SplitNode(int value)
            {
                // the left element of the pair should be the regular number divided by two and rounded down,
                // while the right element of the pair should be the regular number divided by two and rounded up

                var node = new SnailfishNode()
                {
                    LeftRegular = (int)Math.Floor(value / 2.0),
                    RightRegular = (int)Math.Ceiling(value / 2.0)
                };

                return node;
            }

            public override string ToString()
            {
                // For assertions and debug
                return $"[{LeftRegular?.ToString() ?? Left?.ToString() ?? "XXX"},{RightRegular?.ToString() ?? Right?.ToString() ?? "XXX"}]";
            }
        }
    }
}

