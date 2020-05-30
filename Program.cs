using LocalVersionControlSystem.CommitSystem;
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
            CommitsList cml = new CommitsList("Test");
            cml.LoadCommits();
            Commit cm = new Commit("Test");
            cm.LoadFromDirectory();
            cm.BuildIndexing();
            cml.AddCommit(cm);
            cml.ShowDiff();
        }
    }
}
