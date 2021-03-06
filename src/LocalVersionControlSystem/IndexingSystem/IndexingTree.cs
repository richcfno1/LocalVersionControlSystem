﻿using System;
using System.Collections.Generic;
using System.IO;
using LocalVersionControlSystem.ObjectSystem;
using LocalVersionControlSystem.Helper;
using System.Linq;
using Newtonsoft.Json;

namespace LocalVersionControlSystem.IndexingSystem
{
    class IndexingTree
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Describe { get; set; }
        public DateTime SubmitTime { get; set; }

        public Project Project { get; set; }   // user's project.
        public string IndexFilePath { get; } //Place to save indexing.
        public string LastIndexingID { get; set; }  //Last Indexing Tree ID

        private IndexingNode? _root;    //Root node of the tree.
        private List<IndexingNode> _allNodes;  //List of all nodes in the tree

        private bool _tempMode = false;

        //Return a list of lines that are only in indexing1.
        public static IEnumerable<string> CompareTwoIndexing(string indexingPath1, string indexingPath2)
        {
            var indexing1 = JsonConvert.DeserializeObject<Indexing>(File.ReadAllText(indexingPath1)).ProjectContents;
            var indexing2 = JsonConvert.DeserializeObject<Indexing>(File.ReadAllText(indexingPath2)).ProjectContents;
            indexing1[0] = indexing2[0];  //Ignore the line about time
            return indexing1.Where(a => !indexing2.Contains(a));
        }

        public static IEnumerable<string> CompareTwoIndexing(IndexingTree indexingTree1, IndexingTree indexingTree2)
        {
            if (indexingTree1 == null)
            {
                return new List<string>();
            }
            if (indexingTree2 == null)
            {
                List<string> result = JsonConvert.DeserializeObject<Indexing>(indexingTree1.ToJSON()).ProjectContents.ToList();
                result.RemoveAt(0);
                return result;
            }
            var indexing1 = JsonConvert.DeserializeObject<Indexing>(indexingTree1.ToJSON()).ProjectContents;
            var indexing2 = JsonConvert.DeserializeObject<Indexing>(indexingTree2.ToJSON()).ProjectContents;
            indexing1[0] = indexing2[0];  //Ignore the line about time
            return indexing1.Where(a => !indexing2.Contains(a));
        }

        //Constructor with a random id.
        //If TempMode = true, all files will be stored in Temp folder.
        public IndexingTree(Project project, string lastTreeID, bool tempMode = false)
        {
            ID = HashHelper.HashString(Guid.NewGuid().ToString()).Substring(0, 12);
            Name = String.Empty;
            Describe = String.Empty;
            SubmitTime = DateTime.Now;

            Project = project;
            IndexFilePath = Path.Combine(Project.IndexingFolderPath, ID + ".idxdata");
            _tempMode = tempMode;
            if (_tempMode)
                IndexFilePath = Path.Combine(Project.TemporaryFolderPath, Project.IndexingName, ID + ".idxdata");
            _allNodes = new List<IndexingNode>();
            LastIndexingID = lastTreeID;
        }

        //Constructor with a specific id
        //If TempMode = true, all files will be stored in Temp folder.
        public IndexingTree(Project project, string id, string lastTreeID, bool tempMode = false)
        {
            ID = id;
            Name = String.Empty;
            Describe = String.Empty;
            SubmitTime = DateTime.Now;

            Project = project;
            IndexFilePath = Path.Combine(Project.IndexingFolderPath, ID + ".idxdata");
            _tempMode = tempMode;
            if (_tempMode)
                IndexFilePath = Path.Combine(Project.TemporaryFolderPath, Project.IndexingName, ID + ".idxdata");

            _allNodes = new List<IndexingNode>();
            LastIndexingID = lastTreeID;
        }

        //Operate root
        public IndexingNode GetRoot()
        {
            if (_root == null)
                throw new Exception("Uninitialized tree");
            return _root;
        }

        public void SetRoot(IndexingNode root)
        {
            _root = root;
        }

        //Operate nodes
        public List<IndexingNode> GetAllNodes()
        {
            return _allNodes;
        }

        public void SetAllNodes(List<IndexingNode> allNodes)
        {
            _allNodes = allNodes;
        }

        //The function which can implement ImportTreeFromDirectory.
        private void CreateTreeFromDirectory(string path, IndexingNode parent)
        {
            var directoryInfo = new DirectoryInfo(path);
            foreach (var f in directoryInfo.GetFiles())
            {
                var fileNameHash = HashHelper.HashString(f.Name);
                var contentHash = HashHelper.HashFile(Path.Combine(path, f.Name));
                var subFileNode = new IndexingNode(fileNameHash, contentHash, parent);
                _allNodes.Add(subFileNode);
                parent.AddChild(subFileNode);
                Project.CreateObject(f, fileNameHash, contentHash, _tempMode);
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
                _allNodes.Add(subDirectoryNode);
                parent.AddChild(subDirectoryNode);
                Project.CreateObject(d, directoryNameHash, _tempMode);
                CreateTreeFromDirectory(Path.Combine(path, d.Name), subDirectoryNode);
            }
        }

        //Build a tree based on files in directory, and create object for each node.
        public void ImportTreeFromDirectory()
        {
            _allNodes.Clear();
            var rootInfo = new DirectoryInfo(Project.Path);
            Project.CreateObject(rootInfo, HashHelper.HashString(rootInfo.Name), _tempMode);
            _root = new IndexingNode(HashHelper.HashString(new DirectoryInfo(Project.Path).Name), null);
            _allNodes.Add(_root);
            CreateTreeFromDirectory(Project.Path, _root);
        }

        //The function which can implement ImportTreeFromIndexing.
        private void CreateTreeFromIndexing(string[] indexing, int curLine, IndexingNode parent, int curLayer)
        {
            while(curLine + 1 < indexing.Length)
            {
                //Update to the current line.
                curLine++;
                var line = indexing[curLine];

                //Get the node information.
                var layer = line.Length / 131;
                if (layer != 1)
                    if (line.Substring((layer - 2) * 131, 131).Substring(1, 64) != parent.NameSHA256)
                        continue;
                if (layer != curLayer + 1)
                    continue;
                line = line.Substring((layer - 1) * 131, 131);

                var nameHash = line.Substring(1, 64);
                var contentHash = line.Substring(67, 64);

                if (contentHash == ObjectHelper.EmptyZeroes)
                {
                    var tempNode = new IndexingNode(nameHash, parent);
                    _allNodes.Add(tempNode);
                    parent.AddChild(tempNode);
                    CreateTreeFromIndexing(indexing, curLine, tempNode, layer);
                }
                else
                {
                    var tempNode = new IndexingNode(nameHash, contentHash, parent);
                    _allNodes.Add(tempNode);
                    parent.AddChild(tempNode);
                }
            }
        }

        //Build a tree based on indexing, this action won't make any change on objects.
        public void ImportTreeFromIndexing()
        {
            _allNodes.Clear();
            Indexing temp = JsonConvert.DeserializeObject<Indexing>(File.ReadAllText(IndexFilePath));

            Name = temp.IndexingName;
            Describe = temp.IndexingDescribe;
            SubmitTime = temp.SubmitTime;

            LastIndexingID = temp.LastIndexingID;

            var contents = temp.ProjectContents;
            _root = new IndexingNode(contents[0].Substring(1, 64), null);
            _allNodes.Add(_root);
            CreateTreeFromIndexing(contents, 0, _root, 1);
        }

        //The function which can implement ExportTreeToDirectory.
        private void CreateDirectoryFromTree(IndexingNode curNode, string curPath)
        {
            if (_root != null && curNode.Equals(_root))
            {
                foreach (var child in curNode.Children)
                {
                    CreateDirectoryFromTree(child, curPath);
                }
                return;
            }
            var objectPath = Project.FindObjectPath(curNode.NameSHA256, curNode.ContentSHA256, _tempMode);
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
            CreateDirectoryFromTree(_root, Project.Path);
        }

        //The function which can implement ExportTreeToIndexing.
        private void CreateIndexingFromTree(List<String> result, IndexingNode curNode, string pathHash)
        {
            result.Add(pathHash + "\\" + curNode.ToString());
            foreach (IndexingNode n in curNode.Children)
                CreateIndexingFromTree(result, n, pathHash + "\\" + curNode.ToString());

        }

        //Save current tree to indexing.
        public void ExportTreeToIndexing()
        {
            if (_root == null)
            {
                throw new InvalidOperationException();
            }
            List<string> contents = new List<string>();

            CreateIndexingFromTree(contents, _root, "");

            File.WriteAllText(IndexFilePath, JsonConvert.SerializeObject(new Indexing(ID, Name, Describe,
                SubmitTime, LastIndexingID, contents.ToArray())));
        }

        public string ToJSON()
        {
            if (_root == null)
            {
                throw new InvalidOperationException();
            }
            List<string> contents = new List<string>();

            CreateIndexingFromTree(contents, _root, "");

            return JsonConvert.SerializeObject(new Indexing(ID, Name, Describe, SubmitTime, LastIndexingID, contents.ToArray()));
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
