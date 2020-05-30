using LocalVersionControlSystem.IndexingSystem;
using System;
using System.IO;

namespace LocalVersionControlSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectoryTree dt = new DirectoryTree("Test/SampleProject", "Test/indexing.txt", "Test/Objects");
            dt.ImportTreeFromIndexing();
            dt.ExportTreeToIndexing();
        }
    }
}
