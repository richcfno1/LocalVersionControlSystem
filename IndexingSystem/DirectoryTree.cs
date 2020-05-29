using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace LocalVersionControlSystem.IndexingSystem
{
    class DirectoryTree
    {
        private string directoryPath;
        private string indexPath;
        private Node root;

        public DirectoryTree(String newDirectoryPath, string newindexPath)
        {
            directoryPath = newDirectoryPath;
            indexPath = newindexPath;
            root = new Node(SHA256Helper.GetStringSHA256(directoryPath), null);
        }

        public void CreateTree(string path, Node parent)
        {
            DirectoryInfo temp = new DirectoryInfo(path);
            foreach (FileInfo f in temp.GetFiles())
            {
                parent.AddChild(new Node(SHA256Helper.GetStringSHA256(f.Name), SHA256Helper.GetFileSHA256(path + "/" + f.Name), parent));
            }

            foreach (DirectoryInfo d in temp.GetDirectories())
            {
                Node tempNode = new Node(SHA256Helper.GetStringSHA256(path), parent);
                parent.AddChild(tempNode);
                CreateTree(path + "/" + d.Name, tempNode);
            }
        }

        public void update()
        {
            CreateTree(directoryPath, root);
        }

        public void export()
        {
            StreamWriter sw = new StreamWriter(new FileStream(indexPath, FileMode.Create));
            sw.WriteLine(root.ToString(""));
            sw.Close();
        }
    }
}
