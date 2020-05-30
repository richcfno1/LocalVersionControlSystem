using System.Collections;
using System.Collections.Generic;

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

        public void AddChild(Node newChild)
        {
            children.Add(newChild);
        }

        public string ToString(string pathSHA256)
        {
            string result = pathSHA256 + "/" + GetNameSHA256() + "##" + GetContentSHA256();
            pathSHA256 = result;
            int size = children.Count;
            for (int i = 0; i < size; i++)
            {
                result = result + "\n" + children[i].ToString(pathSHA256);
            }
            return result;
        }

    }
}
