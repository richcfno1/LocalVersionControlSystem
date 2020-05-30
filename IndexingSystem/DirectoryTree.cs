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
        private string savePath;      //Place to save objects.
        private Node root;            //Root node of the tree.

        //Initialize the path are needed
        public DirectoryTree(String newDirectoryPath, string newIndexPath, string newSavePath)
        {
            directoryPath = newDirectoryPath;
            indexPath = newIndexPath;
            savePath = newSavePath;
        }

        //The function which can implement ImportTreeFromDirectory.
        public void CreateTreeFromDirectory(string path, Node parent)
        {
            DirectoryInfo temp = new DirectoryInfo(path);
            foreach (FileInfo f in temp.GetFiles())
            {
                string tempNameSHA256 = SHA256Helper.GetStringSHA256(f.Name);
                string tempContentSHA256 = SHA256Helper.GetFileSHA256(path + "/" + f.Name);
                parent.AddChild(new Node(tempNameSHA256, tempContentSHA256, parent));
                ObjectCreator.CreateObject(f, tempNameSHA256, tempContentSHA256, savePath);
            }

            foreach (DirectoryInfo d in temp.GetDirectories())
            {
                string tempNameSHA256 = SHA256Helper.GetStringSHA256(path);
                Node tempNode = new Node(tempNameSHA256, parent);
                parent.AddChild(tempNode);
                ObjectCreator.CreateObject(d, tempNameSHA256, savePath);
                CreateTreeFromDirectory(path + "/" + d.Name, tempNode);
            }
        }

        //The function which can implement ImportTreeFromIndexing.
        public void CreateTreeFromIndexing(string[] indexing, int curLine, Node parent)
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

        //Build a tree based on files in directory, and create object for each node.
        public void ImportTreeFromDirectory()
        {
            root = new Node(SHA256Helper.GetStringSHA256(directoryPath), null);
            CreateTreeFromDirectory(directoryPath, root);
        }

        //Build a tree based on indexing, this action won't make any change on objects.
        public void ImportTreeFromIndexing()
        {
            string[] indexing = File.ReadAllLines(indexPath);
            root = new Node(indexing[0].Substring(1, 64), null);
            CreateTreeFromIndexing(indexing, 0, root);
        }

        //Using indexing and objects to build the directory.
        public void ExportTreeToDirectory()
        {

        }

        //Save current tree to indexing.
        public void ExportTreeToIndexing()
        {
            StreamWriter sw = new StreamWriter(new FileStream("Test/testresult.txt", FileMode.Create));
            sw.WriteLine(root.ToString(""));
            sw.Close();
        }
    }
}
