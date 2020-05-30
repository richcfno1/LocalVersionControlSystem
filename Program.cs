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
            DirectoryTree dt = new DirectoryTree("Test/SampleProject", "Test/indexing.txt", "Test/Objects");
            dt.ImportTreeFromDirectory();
            dt.ExportTreeToIndexing();

            //ObjectCreator.CreateFile("Test/", "Test/Objects/56AD8DAD6AA9.objdata");
        }
    }
}
