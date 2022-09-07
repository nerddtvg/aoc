using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2019
{

    class TreeNode
    {
        public int depth { get; set; }
        public string name { get; set; } = string.Empty;
        public string parent_name { get; set; } = string.Empty;

        public TreeNode()
        {

        }

        public TreeNode(string name, string parent_name)
        {
            this.name = name;
            this.parent_name = parent_name;
            this.depth = 0;
        }
    }

    class Day06 : ASolution
    {
        public List<TreeNode> nodes { get; set; }

        public Day06() : base(06, 2019, "")
        {
            nodes = new List<TreeNode>();
            Input.SplitByNewline().ToList().ForEach(r =>
            {
                string n1 = r.Split(')')[0];
                string n2 = r.Split(')')[1];
                nodes.Add(new TreeNode(n2, n1));
            });

            nodes.ForEach(r => CountDepth(r));
        }

        protected override string SolvePartOne()
        {
            return nodes.Sum(r => r.depth).ToString();
        }

        protected override string SolvePartTwo()
        {
            // Get the path from the desired node to COM
            List<TreeNode> YOU = FindPath(nodes, "YOU");
            List<TreeNode> SAN = FindPath(nodes, "SAN");

            // Search from SAN back towards COM to find the first overlap
            int c = 0;
            TreeNode common = default!;

            foreach (TreeNode n in SAN)
            {
                c++;

                if (YOU.Where(r => r.name == n.name).Count() > 0)
                {
                    common = YOU.Where(r => r.name == n.name).First();
                    break;
                }
            }

            // Now we count for YOU
            foreach (TreeNode n in YOU)
            {
                if (n.name == common.name)
                {
                    break;
                }

                c++;
            }

            // Remove YOU, SAN, and the common node once
            c -= 3;

            return c.ToString();
        }

        public List<TreeNode> FindPath(List<TreeNode> nodes, string name)
        {
            if (name == "COM")
            {
                return new List<TreeNode>();
            }

            List<TreeNode> n = new List<TreeNode>() { nodes.Where(a => a.name == name).First() };

            n.AddRange(FindPath(nodes, n[0].parent_name));

            return n;
        }

        public void CountDepth(TreeNode node)
        {
            TreeNode? actualNode = this.nodes.First(r => r.name == node.name);

            if (node.parent_name == "COM")
            {
                actualNode.depth = 1;
                return;
            }

            TreeNode? parent = this.nodes.First(r => r.name == node.parent_name);

            if (parent.depth == 0)
            {
                CountDepth(parent);
            }

            TreeNode? actualParent = this.nodes.First(r => r.name == node.parent_name);

            actualNode.depth = actualParent.depth + 1;
        }
    }
}
