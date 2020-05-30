using LocalVersionControlSystem.IndexingSystem;
using LocalVersionControlSystem.ObjectSystem;
using System;
using System.IO;
using System.Linq;

namespace LocalVersionControlSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter path of your project");
            string directoryPath = Console.ReadLine();
            string indexPath = new DirectoryInfo(directoryPath).Parent.FullName + "/indexing.txt";
            string objectsPath = new DirectoryInfo(directoryPath).Parent.FullName + "/Objects";

            if (!new DirectoryInfo(indexPath).Exists)
                new DirectoryInfo(objectsPath).Create();
            if (!new DirectoryInfo(indexPath).Exists)
                new DirectoryInfo(objectsPath).Create();

            DirectoryTree dt = new DirectoryTree(directoryPath, indexPath, objectsPath);
            dt.ImportTreeFromDirectory();
            dt.ExportTreeToIndexing();
        }
    }
}
