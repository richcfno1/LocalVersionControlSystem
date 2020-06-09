using System;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Linq;
using LocalVersionControlSystem.CommitSystem;
using LocalVersionControlSystem.Helper;
using LocalVersionControlSystem.IndexingSystem;
using LocalVersionControlSystem.ObjectSystem;

namespace LocalVersionControlSystem
{
    class Program
    {
        [STAThread]
        static void Main(string[] _)
        {
            var project = new Project("TestFiles");
            //Console.WriteLine($"Will use current directory {project.Path} as project directory");
            //if (!Directory.Exists(project.IndexingFolderPath) || !Directory.Exists(project.ObjectsFolderPath))
            //{
            //    Console.WriteLine("Initializing repository by creating index and object folders...");
            //    Directory.CreateDirectory(project.IndexingFolderPath);
            //    Directory.CreateDirectory(project.ObjectsFolderPath);
            //}
            //var idl = new IndexingTreeList(project); //Create the list
            //idl.LoadIndexingTrees(); //Load list from Test/Indexing

            //Console.WriteLine("Current Status:");
            //idl.ShowDiff();

            //while (true)
            //{
            //    Console.WriteLine("Write anything to commit everything, or type ENTER to exit");
            //    if (Console.ReadLine().Length == 0)
            //    {
            //        break;
            //    }

            //    Console.WriteLine("Creating commit");
            //    var indexingTree = new IndexingTree(project); //Create a new commit
            //    indexingTree.ImportTreeFromDirectory(); //Use current diretroy and file to make a tree
            //    indexingTree.ExportTreeToIndexing(); //Export the tree as indexing file
            //    idl.AddIndexingTree(indexingTree); //Add the commit to list
            //    idl.ShowDiff(); //Show difference between commits.
            //}

            //var indexingTree1 = new IndexingTree(project, "BAD94AD56AF0", "000000000000");
            //indexingTree1.ImportTreeFromIndexing();
            //CommitHelper.ExportCommit(project, indexingTree1);

            CommitHelper.ImportCommit(project, "BAD94AD56AF0");
        }
    }
}
