using LocalVersionControlSystem.IndexingSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LocalVersionControlSystem.CommitSystem
{
    class Commit
    {
        public string ID { get; }
        public string ProjectIndexFilePath => _directoryTree.IndexFilePath;
        public DateTime SubmitTime =>
            // TODO: this should be saved inside a file
            // instead of relying on filesystem time
            new FileInfo(ProjectIndexFilePath).CreationTime;

        private readonly IndexingTree _directoryTree;

        public Commit(Project project)
        {
            // TODO: generate id from author, timestamp and/or file hash
            ID = HashHelper.HashString(Guid.NewGuid().ToString());

            _directoryTree = new IndexingTree(project, ID);
        }

        public Commit(Project project, string id)
        {
            ID = id;
            _directoryTree = new IndexingTree(project, ID);
        }

        public void LoadFromDirectory()
        {
            _directoryTree.ImportTreeFromDirectory();
        }

        public void LoadFromIndexing()
        {
            _directoryTree.ImportTreeFromIndexing();
        }
        public void BuildDirectory()
        {
            _directoryTree.ExportTreeToDirectory();
        }

        public void BuildIndexing()
        {
            _directoryTree.ExportTreeToIndexing();
        }
    }
}
