using LocalVersionControlSystem.IndexingSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LocalVersionControlSystem.CommitSystem
{
    class CommitsList
    {
        private List<Commit> commits;

        private string projectPath;
        private string projectDirectoryPath;
        private string projectIndexingPath;
        private string projectObjectsPath;

        public CommitsList(string newProjectPath)
        {
            commits = new List<Commit>();
            projectPath = newProjectPath;
            projectDirectoryPath = projectPath + "/Project";
            projectIndexingPath = projectPath + "/Indexing";
            projectObjectsPath = projectPath + "/Objects";
        }

        public void LoadCommits()
        {
            foreach (FileInfo f in new DirectoryInfo(projectIndexingPath).GetFiles())
            {
                if (f.Name.Substring(0, 8).Equals("indexing") && f.Extension.Contains("idxdata"))
                {
                    AddCommit(new Commit(projectPath, f.Name.Substring(8, 8)));
                }
            }
        }

        public void AddCommit(Commit newCommit)
        {
            for (int i = 0; i < commits.Count; i++)
            {
                if (commits[i].GetSubmitTime().CompareTo(newCommit.GetSubmitTime()) > 0)
                {
                    commits.Insert(i, newCommit);
                    return;
                }
            }
            commits.Add(newCommit);
        }

        public void ShowDiff()
        {
            for (int i = 0; i < commits.Count - 1; i++)
            {
                string[] addedNodes = IndexingTree.CompareTwoIndexing(commits[i + 1].GetProjectIndexingPath(), commits[i].GetProjectIndexingPath());
                string[] deletedNodes = IndexingTree.CompareTwoIndexing(commits[i].GetProjectIndexingPath(), commits[i + 1].GetProjectIndexingPath());
                Console.WriteLine(commits[i + 1].GetSerialNumber() + "Compares To" + commits[i].GetSerialNumber());
                Console.WriteLine("Added:");
                foreach (string a in addedNodes)
                {
                    Console.WriteLine(a);
                }
                Console.WriteLine("Deleted:");
                foreach (string d in deletedNodes)
                {
                    Console.WriteLine(d);
                }
            }
        }
    }
}
