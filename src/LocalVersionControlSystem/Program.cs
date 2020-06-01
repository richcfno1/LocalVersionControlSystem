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
        static void Main(string[] _)
        {
            //var project = new Project(Environment.CurrentDirectory);
            //Console.WriteLine($"Will use current directory {project.Path} as project directory");
            //if (!Directory.Exists(project.IndexingFolderPath) || !Directory.Exists(project.ObjectsFolderPath))
            //{
            //    Console.WriteLine("Initializing repository by creating index and object folders...");
            //    Directory.CreateDirectory(project.IndexingFolderPath);
            //    Directory.CreateDirectory(project.ObjectsFolderPath);
            //}
            //var cml = new CommitsList(project); //Create the list
            //cml.LoadCommits(); //Load list from Test/Indexing

            //Console.WriteLine("Current Status:");
            //cml.ShowDiff();

            //while(true)
            //{
            //    Console.WriteLine("Write anything to commit everything, or type ENTER to exit");
            //    if(Console.ReadLine().Length == 0)
            //    {
            //        break;
            //    }

            //    Console.WriteLine("Creating commit");
            //    var cm = new Commit(project); //Create a new commit
            //    cm.LoadFromDirectory(); //Use current diretroy and file to make a tree
            //    cm.BuildIndexing(); //Export the tree as indexing file
            //    cml.AddCommit(cm); //Add the commit to list
            //    cml.ShowDiff(); //Show difference between commits.
            //}

            PSDHelper ip = new PSDHelper("rc-.psd");
            using FileStream fs = new FileStream("test.png", FileMode.Create);
            Bitmap bm = ip.PSDImage;
            bm.Save(fs, ImageFormat.Png);

        }
    }
}
