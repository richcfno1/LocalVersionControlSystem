using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using LocalVersionControlSystem.ObjectSystem;

namespace LocalVersionControlSystem.IndexingSystem
{
    class IndexingNode
    {
        public string NameSHA256 { get; }
        public string ContentSHA256 { get; }
        public List<IndexingNode> Children => _children;

        private readonly IndexingNode? _parent;
        private readonly List<IndexingNode> _children;

        public IndexingNode(string nameSHA256, IndexingNode? parent)
        {
            NameSHA256 = nameSHA256;
            ContentSHA256 = ObjectHelper.EmptyZeroes;
            _parent = parent;
            _children = new List<IndexingNode>();
        }

        public IndexingNode(string nameSHA256, string contentSHA256, IndexingNode? parent)
        {
            NameSHA256 = nameSHA256;
            ContentSHA256 = contentSHA256;
            _parent = parent;
            _children = new List<IndexingNode>();
        }

        public IndexingNode? GetParent()
        {
            return _parent;
        }

        public void AddChild(IndexingNode newChild)
        {
            _children.Add(newChild);
        }

        //Return its information.
        public override string ToString()
        {
            return NameSHA256 + "##" + ContentSHA256;
        }

        public bool Equals(IndexingNode node)
        {
            if (node == null)
                return false;
            //If root
            IndexingNode? parent = node.GetParent();
            if (_parent == null)
            {
                return parent == null;
            }
            else if (parent == null)
            {
                return _parent == null;
            }
            else if (_parent.ToString().Equals(parent.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return ToString().Equals(node.ToString(), StringComparison.InvariantCultureIgnoreCase);
            }
            return false;
        }

    }
}
