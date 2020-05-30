using System.Collections;
using System.Collections.Generic;
using System.Data.Common;

namespace LocalVersionControlSystem.IndexingSystem
{
    class IndexingNode
    {
        private string nameSHA256;
        private string contentSHA256;
        private IndexingNode parent;
        private List<IndexingNode> children;

        public IndexingNode(string newNameSHA256, IndexingNode newParent)
        {
            nameSHA256 = newNameSHA256;
            contentSHA256 = "0000000000000000000000000000000000000000000000000000000000000000";
            parent = newParent;
            children = new List<IndexingNode>();
        }

        public IndexingNode(string newNameSHA256, string newContentSHA256, IndexingNode newParent)
        {
            nameSHA256 = newNameSHA256;
            contentSHA256 = newContentSHA256;
            parent = newParent;
            children = new List<IndexingNode>();
        }

        public string GetNameSHA256()
        {
            return nameSHA256;
        }
        public string GetContentSHA256()
        {
            return contentSHA256;
        }

        public List<IndexingNode> GetChildren()
        {
            return children;
        }

        public void AddChild(IndexingNode newChild)
        {
            children.Add(newChild);
        }

        //Return its information.
        public override string ToString()
        {
            return GetNameSHA256() + "##" + GetContentSHA256();
        }

    }
}
