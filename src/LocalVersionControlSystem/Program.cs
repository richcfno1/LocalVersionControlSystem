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
            CommitsList cml = new CommitsList("Test"); //Create the list
            cml.LoadCommits(); //Load list from Test/Indexing
            Commit cm = new Commit("Test"); //Create a new commit
            cm.LoadFromDirectory(); //Use current diretroy and file to make a tree
            cm.BuildIndexing(); //Export the tree as indexing file
            cml.AddCommit(cm); //Add the commit to list
            cml.ShowDiff(); //Show difference between commits.
        }
    }
}
