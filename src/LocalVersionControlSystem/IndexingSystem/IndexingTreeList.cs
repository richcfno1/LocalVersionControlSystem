using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using LocalVersionControlSystem.CommitSystem;

namespace LocalVersionControlSystem.IndexingSystem
{
    class IndexingTreeList
    {
        private readonly SortedList<DateTime, IndexingTree> _indexingTrees;
        private readonly Project _project;

        public IndexingTreeList(Project project)
        {
            _indexingTrees = new SortedList<DateTime, IndexingTree>();
            _project = project;
        }

        public void LoadIndexingTrees()
        {
            foreach (var f in new DirectoryInfo(_project.IndexingFolderPath).GetFiles())
            {
                AddIndexingTree(new IndexingTree(_project, f.Name.Substring(0, 12)));
            }
        }

        public void AddIndexingTree(IndexingTree newIndexingTree)
        {
            _indexingTrees.Add(newIndexingTree.SubmitTime, newIndexingTree);
        }

        public IndexingTree? GetIndexingTree(string id)
        {
            for(int i = 0; i < _indexingTrees.Count; i++)
            {
                if (_indexingTrees.Values[i].ID == id)
                    return _indexingTrees.Values[i];
            }
            return null;
        }

        public void ShowDiff()
        {
            for (var i = 0; i < _indexingTrees.Count - 1; i++)
            {
                var oldIndexingTree = _indexingTrees.Values[i];
                var newIndexingTree = _indexingTrees.Values[i + 1];
                var addedNodes = IndexingTree.CompareTwoIndexing(newIndexingTree.IndexFilePath, oldIndexingTree.IndexFilePath);
                var deletedNodes = IndexingTree.CompareTwoIndexing(oldIndexingTree.IndexFilePath, newIndexingTree.IndexFilePath);
                Console.WriteLine(newIndexingTree.ID.Substring(0, 8) + " Compares To " + oldIndexingTree.ID.Substring(0, 8));
                Console.WriteLine("Added: ");
                foreach (var a in addedNodes)
                {
                    Console.WriteLine(a);
                }
                Console.WriteLine("Deleted: ");
                foreach (var d in deletedNodes)
                {
                    Console.WriteLine(d);
                }
            }
        }
    }
}
