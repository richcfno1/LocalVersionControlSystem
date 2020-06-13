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
            Project project = new Project("C:\\Users\\14261\\Desktop\\TestFiles");
            IndexingTree it1 = new IndexingTree(project, "85A528AC24E8", "000000000000");
            it1.ImportTreeFromDirectory();
            Console.WriteLine(it1.GetAllNodes().Count);
            it1.ExportTreeToIndexing();
            Console.WriteLine(it1.GetAllNodes().Count);
            it1.ImportTreeFromIndexing();
            Console.WriteLine(it1.GetAllNodes().Count);
        }
    }
}
