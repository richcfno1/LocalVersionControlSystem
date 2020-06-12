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
            IndexingTree it = new IndexingTree(project, "BAD94AD56AF0");
            it.ImportTreeFromIndexing();
            it.ExportTreeToDirectory();
        }
    }
}
