using LocalVersionControlSystem.IndexingSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LocalVersionControlSystem.CommitSystem
{
    class CommitsList
    {
        private readonly SortedList<DateTime, Commit> _commits;
        private readonly Project _project;

        public CommitsList(Project project)
        {
            _commits = new SortedList<DateTime, Commit>();
            _project = project;
        }

        public void LoadCommits()
        {
            foreach (var f in new DirectoryInfo(_project.IndexingFolderPath).GetFiles())
            {
                AddCommit(new Commit(_project, f.Name));
            }
        }

        public void AddCommit(Commit newCommit)
        {
            _commits.Add(newCommit.SubmitTime, newCommit);
        }

        public void ShowDiff()
        {
            for (var i = 0; i < _commits.Count - 1; i++)
            {
                var oldCommit = _commits.Values[i];
                var newCommit = _commits.Values[i + 1];
                var addedNodes = IndexingTree.CompareTwoIndexing(newCommit.ProjectIndexFilePath, oldCommit.ProjectIndexFilePath);
                var deletedNodes = IndexingTree.CompareTwoIndexing(oldCommit.ProjectIndexFilePath, newCommit.ProjectIndexFilePath);
                Console.WriteLine(newCommit.ID.Substring(0, 8) + " Compares To " + oldCommit.ID.Substring(0, 8));
                Console.WriteLine("Added: ");
                foreach (var a in addedNodes)
                {
                    Console.WriteLine(a.Substring(0, 8));
                }
                Console.WriteLine("Deleted: ");
                foreach (var d in deletedNodes)
                {
                    Console.WriteLine(d.Substring(0, 8));
                }
            }
        }
    }
}
