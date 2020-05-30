using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using LocalVersionControlSystem.ObjectSystem;

namespace LocalVersionControlSystem.IndexingSystem
{
    class DirectoryTree
    {
        private string directoryPath; //Actual directory for user's project.
        private string indexPath;     //Place to save indexing.
        private string objectsPath;   //Place to save objects.
        private Node root;            //Root node of the tree.

        //Initialize the path are needed
        public DirectoryTree(String newDirectoryPath, string newIndexPath, string newobjectsPath)
        {
            directoryPath = newDirectoryPath;
            indexPath = newIndexPath;
            objectsPath = newobjectsPath;
        }

        //The function which can implement ImportTreeFromDirectory.
        private void CreateTreeFromDirectory(string path, Node parent)
        {
            DirectoryInfo temp = new DirectoryInfo(path);
            foreach (FileInfo f in temp.GetFiles())
            {
                string tempNameSHA256 = SHA256Helper.GetStringSHA256(f.Name);
                string tempContentSHA256 = SHA256Helper.GetFileSHA256(path + "/" + f.Name);
                parent.AddChild(new Node(tempNameSHA256, tempContentSHA256, parent));
                ObjectCreator.CreateObject(f, tempNameSHA256, tempContentSHA256, objectsPath);
            }

            foreach (DirectoryInfo d in temp.GetDirectories())
            {
                string tempNameSHA256 = SHA256Helper.GetStringSHA256(d.Name);
                Node tempNode = new Node(tempNameSHA256, parent);
                parent.AddChild(tempNode);
                ObjectCreator.CreateObject(d, tempNameSHA256, objectsPath);
                CreateTreeFromDirectory(path + "/" + d.Name, tempNode);
            }
        }

        //Build a tree based on files in directory, and create object for each node.
        public void ImportTreeFromDirectory()
        {
            DirectoryInfo rootInfo = new DirectoryInfo(directoryPath);
            ObjectCreator.CreateObject(rootInfo, SHA256Helper.GetStringSHA256(rootInfo.Name), objectsPath);
            root = new Node(SHA256Helper.GetStringSHA256((new DirectoryInfo(directoryPath)).Name), null);
            CreateTreeFromDirectory(directoryPath, root);
        }

        //The function which can implement ImportTreeFromIndexing.
        private void CreateTreeFromIndexing(string[] indexing, int curLine, Node parent)
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
                    Node tempNode = new Node(nameSHA256, parent);
                    parent.AddChild(tempNode);
                    CreateTreeFromIndexing(indexing, curLine, tempNode);
                }

                else if (!isFinishedFile)
                {
                    Node tempNode = new Node(nameSHA256, contentSHA256, parent);
                    parent.AddChild(tempNode);
                }

            }
        }

        //Build a tree based on indexing, this action won't make any change on objects.
        public void ImportTreeFromIndexing()
        {
            string[] indexing = File.ReadAllLines(indexPath);
            root = new Node(indexing[0].Substring(1, 64), null);
            CreateTreeFromIndexing(indexing, 0, root);
        }

        //The function which can implement ExportTreeToDirectory.
        private void CreateDirectoryFromTree(Node curNode, string curPath)
        {
            string objectPath = ObjectCreator.FindObjectPath(curNode.GetNameSHA256(), curNode.GetContentSHA256(), objectsPath);
            string nextLayerPath = "";
            if (objectPath != "" && curNode.GetContentSHA256() != "0000000000000000000000000000000000000000000000000000000000000000")
                ObjectCreator.CreateFile(curPath, objectPath);
            if (objectPath != "" && curNode.GetContentSHA256() == "0000000000000000000000000000000000000000000000000000000000000000")
                nextLayerPath = ObjectCreator.CreateDirectory(curPath, objectPath);
            List<Node> children = curNode.GetChildren();
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
        private string CreateIndexingFromTree(Node curNode, string pathSHA256)
        {
            string result = pathSHA256 + "/" + curNode.ToString();
            pathSHA256 = result;
            List<Node> children = curNode.GetChildren();
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
