using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace AdventOfCode.Solutions.Year2018
{
    class LicenseTreeNode {
        public List<LicenseTreeNode> childNodes {get;set;}
        public List<int> metadata {get;set;}

        public int metadataSum {
            get {
                return this.metadata.Sum() + childNodes.Sum(a => a.metadataSum);
            }
        }

        public int nodeValue {
            get {
                // No child nodes, metadata is summarized
                if (this.childNodes.Count == 0) return metadataSum;

                // Child nodes becomes a nightmare
                return this.metadata.Sum(a => {
                    // if it is 0, no value
                    if (a == 0) return 0;

                    // If this node doesn't exist, no value
                    if (this.childNodes.Count < a) return 0;

                    return this.childNodes[a-1].nodeValue;
                });
            }
        }
    }

    class Day08 : ASolution
    {
        LicenseTreeNode root = new LicenseTreeNode();

        public Day08() : base(08, 2018, "")
        {
            // Read through the input and parse it
            root = ParseInput(Input.ToIntArray(" ").ToList()).node;
        }

        private (List<int> remaining, LicenseTreeNode node) ParseInput(List<int> parts) {
            // Check we have valid info
            if (parts == null || parts.Count == 0) return (new List<int>(), null);

            // Our return object
            var node = new LicenseTreeNode();

            // Format:
            /*
            2 3 (0 3 [10 11 12]) (1 1 (0 1 [99]) 2) [1 1 2]
            A----------------------------------------------
                 B-------------   C---------------
                                       D-------
            */

            // Required info:
            // * [0] Quantity of Child Nodes
            // * [1] Quantity of Metadata Entries
            // * [Len-M] Metadata Entries

            int childCount = parts[0];
            int metaCount = parts[1];

            // Remove these two parts
            parts.RemoveRange(0, 2);

            // Tracking our child work
            (List<int> remaining, LicenseTreeNode node) childOut = (new List<int>(), null);

            // Start a list
            node.childNodes = new List<LicenseTreeNode>();
            node.metadata = new List<int>();

            // Do we have children?
            if (childCount > 0) {
                for(int c=0; c<childCount; c++) {
                    // Go through and parse each of the children and handle the remainder
                    childOut = ParseInput(parts);

                    node.childNodes.Add(childOut.node);

                    // Remove what was used by the child
                    parts = childOut.remaining;
                }
            }

            if (metaCount > 0) {
                node.metadata = parts.GetRange(0, metaCount);
            
                // Remove these now
                parts.RemoveRange(0, metaCount);
            }

            return (parts, node);
        }

        protected override string SolvePartOne()
        {
            return root.metadataSum.ToString();
        }

        protected override string SolvePartTwo()
        {
            return root.nodeValue.ToString();
        }
    }
}
