using System.Collections;
using System.Collections.Generic;
using System.Data.Common;

namespace LocalVersionControlSystem.IndexingSystem
{
    class Node
    {
        private string nameSHA256;
        private string contentSHA256;
        private Node parent;
        private List<Node> children;

        public Node(string newNameSHA256, Node newParent)
        {
            nameSHA256 = newNameSHA256;
            contentSHA256 = "0000000000000000000000000000000000000000000000000000000000000000";
            parent = newParent;
            children = new List<Node>();
        }

        public Node(string newNameSHA256, string newContentSHA256, Node newParent)
        {
            nameSHA256 = newNameSHA256;
            contentSHA256 = newContentSHA256;
            parent = newParent;
            children = new List<Node>();
        }

        public string GetNameSHA256()
        {
            return nameSHA256;
        }
        public string GetContentSHA256()
        {
            return contentSHA256;
        }

        public List<Node> GetChildren()
        {
            return children;
        }

        public void AddChild(Node newChild)
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
