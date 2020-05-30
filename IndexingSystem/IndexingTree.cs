using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using LocalVersionControlSystem.ObjectSystem;
using System.Collections;

namespace LocalVersionControlSystem.IndexingSystem
{
    class IndexingTree
    {
        private string directoryPath; //Actual directory for user's project.
        private string indexPath;     //Place to save indexing.
        private string objectsPath;   //Place to save objects.
        private IndexingNode root;    //Root node of the tree.

        //Return a list of lines that are only in indexingA.
        public static string[] CompareTwoIndexing(string indexingAPath, string indexingBPath)
        {
            string[] indexingA = File.ReadAllLines(indexingAPath);
            string[] indexingB = File.ReadAllLines(indexingBPath);
            List<string> result = new List<string>();

            foreach (string i in indexingA)
            {
                bool isFind = false;
                foreach (string j in indexingB)
                {
                    if (i.Equals(j))
                    {
                        isFind = true;
                        break;
                    }
                }
                if (!isFind)
                {
                    result.Add(i);
                }
            }

            return result.ToArray();
        }

        //Initialize the path are needed
        public IndexingTree(String newDirectoryPath, string newIndexPath, string newobjectsPath)
        {
            directoryPath = newDirectoryPath;
            indexPath = newIndexPath;
            objectsPath = newobjectsPath;
        }

        //The function which can implement ImportTreeFromDirectory.
        private void CreateTreeFromDirectory(string path, IndexingNode parent)
        {
            DirectoryInfo temp = new DirectoryInfo(path);
            foreach (FileInfo f in temp.GetFiles())
            {
                string tempNameSHA256 = SHA256Helper.GetStringSHA256(f.Name);
                string tempContentSHA256 = SHA256Helper.GetFileSHA256(path + "/" + f.Name);
                parent.AddChild(new IndexingNode(tempNameSHA256, tempContentSHA256, parent));
                ObjectManager.CreateObject(f, tempNameSHA256, tempContentSHA256, objectsPath);
            }

            foreach (DirectoryInfo d in temp.GetDirectories())
            {
                string tempNameSHA256 = SHA256Helper.GetStringSHA256(d.Name);
                IndexingNode tempNode = new IndexingNode(tempNameSHA256, parent);
                parent.AddChild(tempNode);
                ObjectManager.CreateObject(d, tempNameSHA256, objectsPath);
                CreateTreeFromDirectory(path + "/" + d.Name, tempNode);
            }
        }

        //Build a tree based on files in directory, and create object for each node.
        public void ImportTreeFromDirectory()
        {
            DirectoryInfo rootInfo = new DirectoryInfo(directoryPath);
            ObjectManager.CreateObject(rootInfo, SHA256Helper.GetStringSHA256(rootInfo.Name), objectsPath);
            root = new IndexingNode(SHA256Helper.GetStringSHA256((new DirectoryInfo(directoryPath)).Name), null);
            CreateTreeFromDirectory(directoryPath, root);
        }

        //The function which can implement ImportTreeFromIndexing.
        private void CreateTreeFromIndexing(string[] indexing, int curLine, IndexingNode parent)
        {
            bool isFinishedFile = false;
            while(curLine + 1 < indexing.Length)
            {
                //Update to the current line.
                curLine++;
                string line = indexing[curLine];

                //Get the node information.
                int layer = line.Length / 131;
                line = line.Substring((layer - 1) * 131, 131);

                string nameSHA256 = line.Substring(1, 64);
                string contentSHA256 = line.Substring(67, 64);

                if (contentSHA256.Equals("0000000000000000000000000000000000000000000000000000000000000000"))
                {
                    isFinishedFile = true;
                    IndexingNode tempNode = new IndexingNode(nameSHA256, parent);
                    parent.AddChild(tempNode);
                    CreateTreeFromIndexing(indexing, curLine, tempNode);
                }

                else if (!isFinishedFile)
                {
                    IndexingNode tempNode = new IndexingNode(nameSHA256, contentSHA256, parent);
                    parent.AddChild(tempNode);
                }

            }
        }

        //Build a tree based on indexing, this action won't make any change on objects.
        public void ImportTreeFromIndexing()
        {
            string[] indexing = File.ReadAllLines(indexPath);
            root = new IndexingNode(indexing[0].Substring(1, 64), null);
            CreateTreeFromIndexing(indexing, 0, root);
        }

        //The function which can implement ExportTreeToDirectory.
        private void CreateDirectoryFromTree(IndexingNode curNode, string curPath)
        {
            string objectPath = ObjectManager.FindObjectPath(curNode.GetNameSHA256(), curNode.GetContentSHA256(), objectsPath);
            string nextLayerPath = "";
            if (objectPath != "" && curNode.GetContentSHA256() != "0000000000000000000000000000000000000000000000000000000000000000")
                ObjectManager.CreateFile(curPath, objectPath);
            if (objectPath != "" && curNode.GetContentSHA256() == "0000000000000000000000000000000000000000000000000000000000000000")
                nextLayerPath = ObjectManager.CreateDirectory(curPath, objectPath);
            List<IndexingNode> children = curNode.GetChildren();
            int size = children.Count;
            for (int i = 0; i < size; i++)
            {
                CreateDirectoryFromTree(children[i], nextLayerPath);
            }
        }
        
        //Use current tree and objects to build directory.
        public void ExportTreeToDirectory()
        {
            CreateDirectoryFromTree(root, (new DirectoryInfo(directoryPath)).Parent.FullName);
        }

        //The function which can implement ExportTreeToIndexing.
        private string CreateIndexingFromTree(IndexingNode curNode, string pathSHA256)
        {
            string result = pathSHA256 + "/" + curNode.ToString();
            pathSHA256 = result;
            List<IndexingNode> children = curNode.GetChildren();
            int size = children.Count;
            for (int i = 0; i < size; i++)
            {
                result = result + "\n" + CreateIndexingFromTree(children[i], pathSHA256);
            }
            return result;
        }

        //Save current tree to indexing.
        public void ExportTreeToIndexing()
        {
            StreamWriter sw = new StreamWriter(new FileStream(indexPath, FileMode.Create));
            sw.WriteLine(CreateIndexingFromTree(root, ""));
            sw.Close();
        }
    }
}
