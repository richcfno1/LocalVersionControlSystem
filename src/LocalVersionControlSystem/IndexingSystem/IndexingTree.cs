using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using LocalVersionControlSystem.ObjectSystem;
using LocalVersionControlSystem.Helper;
using System.Linq;
using System.Globalization;

namespace LocalVersionControlSystem.IndexingSystem
{
    class IndexingTree
    {
        public string ID { get; }
        public string IndexFilePath { get; } //Place to save indexing.
        public DateTime UpdateTime { get; set; }

        private readonly Project _project;   // user's project.
        private IndexingNode? _root;    //Root node of the tree.
        

        //Return a list of lines that are only in indexingA.
        public static IEnumerable<string> CompareTwoIndexing(string indexingAPath, string indexingBPath)
        {
            var indexingA = File.ReadAllLines(indexingAPath);
            var indexingB = File.ReadAllLines(indexingBPath);
            indexingA[0] = indexingB[0];  //Ignore the line about time
            return indexingA.Where(a => !indexingB.Contains(a));
        }

        public IndexingTree(Project project)
        {
            // TODO: generate id from author, timestamp and/or file hash
            ID = HashHelper.HashString(Guid.NewGuid().ToString()).Substring(0,12);
            _project = project;
            IndexFilePath = Path.Combine(_project.IndexingFolderPath, ID + ".idxdata");
            DirectoryInfo d = new DirectoryInfo(IndexFilePath);
            if (d.Exists)
                ImportTreeFromIndexing();
            else
                UpdateTime = DateTime.Now;
        }

        //Initialize the path are needed
        public IndexingTree(Project project, string id)
        {
            ID = id;
            _project = project;
            IndexFilePath = Path.Combine(_project.IndexingFolderPath, ID + ".idxdata");
            DirectoryInfo d = new DirectoryInfo(IndexFilePath);
            if (d.Exists)
                ImportTreeFromIndexing();
            else
                UpdateTime = DateTime.Now;
        }

        //The function which can implement ImportTreeFromDirectory.
        private void CreateTreeFromDirectory(string path, IndexingNode parent)
        {
            var directoryInfo = new DirectoryInfo(path);
            foreach (var f in directoryInfo.GetFiles())
            {
                var fileNameHash = HashHelper.HashString(f.Name);
                var contentHash = HashHelper.HashFile(Path.Combine(path, f.Name));
                parent.AddChild(new IndexingNode(fileNameHash, contentHash, parent));
                _project.CreateObject(f, fileNameHash, contentHash);
            }

            foreach (var d in directoryInfo.GetDirectories())
            {
                if(d.Name == Project.PrivateFolderName)
                {
                    // do not index directory of ourself
                    // TODO: handle nested LocalVersionControlSystem
                    continue;
                }

                var directoryNameHash = HashHelper.HashString(d.Name);
                var subDirectoryNode = new IndexingNode(directoryNameHash, parent);
                parent.AddChild(subDirectoryNode);
                _project.CreateObject(d, directoryNameHash);
                CreateTreeFromDirectory(Path.Combine(path, d.Name), subDirectoryNode);
            }
        }

        //Build a tree based on files in directory, and create object for each node.
        public void ImportTreeFromDirectory()
        {
            var rootInfo = new DirectoryInfo(_project.Path);
            _project.CreateObject(rootInfo, HashHelper.HashString(rootInfo.Name));
            _root = new IndexingNode(HashHelper.HashString(new DirectoryInfo(_project.Path).Name), null);
            CreateTreeFromDirectory(_project.Path, _root);
        }

        //The function which can implement ImportTreeFromIndexing.
        private void CreateTreeFromIndexing(string[] indexing, int curLine, IndexingNode parent)
        {
            var isFinishedFile = false;
            while(curLine + 1 < indexing.Length)
            {
                //Update to the current line.
                curLine++;
                var line = indexing[curLine];

                //Get the node information.
                var layer = line.Length / 131;
                line = line.Substring((layer - 1) * 131, 131);

                var nameHash = line.Substring(1, 64);
                var contentHash = line.Substring(67, 64);

                if (contentHash == ObjectHelper.EmptyZeroes)
                {
                    isFinishedFile = true;
                    var tempNode = new IndexingNode(nameHash, parent);
                    parent.AddChild(tempNode);
                    CreateTreeFromIndexing(indexing, curLine, tempNode);
                }
                else if (!isFinishedFile)
                {
                    var tempNode = new IndexingNode(nameHash, contentHash, parent);
                    parent.AddChild(tempNode);
                }
            }
        }

        //Build a tree based on indexing, this action won't make any change on objects.
        public void ImportTreeFromIndexing()
        {
            var indexing = File.ReadAllLines(IndexFilePath);
            UpdateTime = new DateTime(int.Parse(indexing[0].Substring(0,4), NumberFormatInfo.InvariantInfo), //Year
                int.Parse(indexing[0].Substring(5, 2), NumberFormatInfo.InvariantInfo), //Month
                int.Parse(indexing[0].Substring(8, 2), NumberFormatInfo.InvariantInfo), //Day
                int.Parse(indexing[0].Substring(11, 2), NumberFormatInfo.InvariantInfo), //Hour
                int.Parse(indexing[0].Substring(14, 2), NumberFormatInfo.InvariantInfo), //Minute
                int.Parse(indexing[0].Substring(17, 2), NumberFormatInfo.InvariantInfo)); //Second
            _root = new IndexingNode(indexing[1].Substring(1, 64), null);
            CreateTreeFromIndexing(indexing, 1, _root);
        }

        //The function which can implement ExportTreeToDirectory.
        private void CreateDirectoryFromTree(IndexingNode curNode, string curPath)
        {
            var objectPath = _project.FindObjectPath(curNode.NameSHA256, curNode.ContentSHA256);
            var nextLayerPath = "";
            if (objectPath != null && curNode.ContentSHA256 != ObjectHelper.EmptyZeroes)
            {
                ObjectHelper.CreateFile(curPath, objectPath);
            }

            if (objectPath != null && curNode.ContentSHA256 == ObjectHelper.EmptyZeroes)
            {
                nextLayerPath = ObjectHelper.CreateDirectory(curPath, objectPath);
            }

            foreach(var child in curNode.Children)
            {
                CreateDirectoryFromTree(child, nextLayerPath);
            }
        }
        
        //Use current tree and objects to build directory.
        public void ExportTreeToDirectory()
        {
            if(_root == null)
            {
                throw new InvalidOperationException();
            }
            CreateDirectoryFromTree(_root, _project.Path);
        }

        //The function which can implement ExportTreeToIndexing.
        private string CreateIndexingFromTree(IndexingNode curNode, string pathHash)
        {
            var result = Path.Combine(pathHash, curNode.ToString());
            pathHash = result;
            return curNode.Children.Aggregate(result,
                (content, children) => $"{content}\n{CreateIndexingFromTree(children, pathHash)}");
        }

        //Save current tree to indexing.
        public void ExportTreeToIndexing()
        {
            if (_root == null)
            {
                throw new InvalidOperationException();
            }
            File.WriteAllText(IndexFilePath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", DateTimeFormatInfo.InvariantInfo) + "\n" +
                CreateIndexingFromTree(_root, string.Empty));
        }
    }
}
