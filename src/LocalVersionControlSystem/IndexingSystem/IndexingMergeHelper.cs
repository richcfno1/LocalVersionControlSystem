using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;

namespace LocalVersionControlSystem.IndexingSystem
{
    class IndexingMergeHelper
    {
        public static List<IndexingNode> OnlyInIndexing1 = new List<IndexingNode>();
        public static List<IndexingNode> OnlyInIndexing2 = new List<IndexingNode>();
        public static List<IndexingNode> UpdateNode = new List<IndexingNode>();  //The name exist in both tree but different content.
        private static List<IndexingNode> ResultIndexingNodes = new List<IndexingNode>();
        private static IndexingTree? ResultTree;

        public static IndexingTree Merge(IndexingTree indexingTree1, IndexingTree indexingTree2)
        {
            OnlyInIndexing1 = new List<IndexingNode>();
            OnlyInIndexing2 = new List<IndexingNode>();
            UpdateNode = new List<IndexingNode>();
            ResultIndexingNodes = new List<IndexingNode>();
            ResultTree = new IndexingTree(indexingTree1.Project, "000000000000");

            if (!indexingTree1.GetRoot().Equals(indexingTree2.GetRoot()))
                throw new Exception("Not same root!");
            foreach (IndexingNode n in indexingTree1.GetAllNodes())
            {
                bool isFind = false;
                foreach (IndexingNode m in indexingTree2.GetAllNodes())
                {
                    if (n.Equals(m))
                    {
                        isFind = true;
                        break;
                    }
                }
                if (!isFind)
                    OnlyInIndexing1.Add(n);
            }

            foreach (IndexingNode n in indexingTree2.GetAllNodes())
            {
                bool isFind = false;
                foreach (IndexingNode m in indexingTree1.GetAllNodes())
                {
                    if (n.Equals(m))
                    {
                        isFind = true;
                        break;
                    }
                }
                if (!isFind)
                    OnlyInIndexing2.Add(n);
            }

            CopyFromTree1(null, indexingTree1.GetRoot());

            foreach (IndexingNode n in OnlyInIndexing2)
            {
                Insert(ResultTree.GetRoot(), n);
            }

            ResultTree.SetAllNodes(ResultIndexingNodes);
            return ResultTree;
        }

        //Step 1: Copy Tree 1 to result
        private static void CopyFromTree1(IndexingNode? resultTreeParent, IndexingNode originalTreeCurrent)
        {
            if (ResultTree == null)
                throw new Exception("Uninitialized tree");
            IndexingNode temp = new IndexingNode(originalTreeCurrent.NameSHA256, originalTreeCurrent.ContentSHA256, resultTreeParent);
            ResultIndexingNodes.Add(temp);

            if (resultTreeParent != null)
            {
                resultTreeParent.AddChild(temp);
            }
            else
            {
                ResultTree.SetRoot(temp);
            }
            foreach (IndexingNode node in originalTreeCurrent.Children)
            {
                CopyFromTree1(temp, node);
            }
        }

        //Step 2: Insert node only in Tree 2 without conflicts
        private static void Insert(IndexingNode curNode, IndexingNode insertNode)
        {
            IndexingNode? parent = insertNode.GetParent();
            if (parent == null)
                throw new Exception();
            if (curNode.NameSHA256.Equals(parent.NameSHA256, StringComparison.InvariantCulture)){
                IndexingNode temp = new IndexingNode(insertNode.NameSHA256, insertNode.ContentSHA256, curNode);
                foreach (IndexingNode n in curNode.Children)
                {
                    //Same name, different content
                    if (n.NameSHA256.Equals(insertNode.NameSHA256, StringComparison.InvariantCulture) &&
                        !n.ContentSHA256.Equals(insertNode.ContentSHA256, StringComparison.InvariantCulture))
                    {
                        UpdateNode.Add(temp);
                        return;
                    }
                }
                curNode.AddChild(temp);
                ResultIndexingNodes.Add(temp);
                return;
            }
            foreach (IndexingNode n in curNode.Children)
                Insert (n, insertNode);
        }
    }
}
