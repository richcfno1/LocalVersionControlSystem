using LocalVersionControlSystem.IndexingSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LocalVersionControlSystem.CommitSystem
{
    class Commit
    {
        private string serialNumber;

        private string projectPath;
        private string projectDirectoryPath;
        private string projectIndexingPath;
        private string projectObjectsPath;
        
        private IndexingTree directoryTree;

        public Commit(string newProjectPath)
        {
            serialNumber = SHA256Helper.GetStringSHA256(Guid.NewGuid().ToString()).Substring(0, 8);

            projectPath = newProjectPath;
            projectDirectoryPath = projectPath + "/Project";
            projectIndexingPath = projectPath + "/Indexing/indexing" + serialNumber + ".idxdata";
            projectObjectsPath = projectPath + "/Objects";

            directoryTree = new IndexingTree(projectDirectoryPath, projectIndexingPath, projectObjectsPath);
        }

        public Commit(string newProjectPath, string newSerialNumber)
        {
            serialNumber = newSerialNumber;

            projectPath = newProjectPath;
            projectDirectoryPath = projectPath + "/Project";
            projectIndexingPath = projectPath + "/Indexing/indexing" + serialNumber + ".idxdata";
            projectObjectsPath = projectPath + "/Objects";

            directoryTree = new IndexingTree(projectDirectoryPath, projectIndexingPath, projectObjectsPath);
        }

        public string GetSerialNumber()
        {
            return serialNumber;
        }

        public string GetProjectIndexingPath()
        {
            return projectIndexingPath;
        }

        public DateTime GetSubmitTime()
        {
            return new FileInfo(projectIndexingPath).CreationTime;
        }

        public void LoadFromDirectory()
        {
            directoryTree.ImportTreeFromDirectory();
        }

        public void LoadFromIndexing()
        {
            directoryTree.ImportTreeFromIndexing();
        }
        public void BuildDirectory()
        {
            directoryTree.ExportTreeToDirectory();
        }

        public void BuildIndexing()
        {
            directoryTree.ExportTreeToIndexing();
        }
    }
}
