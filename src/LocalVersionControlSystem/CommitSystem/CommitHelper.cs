using LocalVersionControlSystem.IndexingSystem;
using System.IO;
using LocalVersionControlSystem.ObjectSystem;
using System.IO.Compression;
using System;

namespace LocalVersionControlSystem.CommitSystem
{
    class CommitHelper
    {
        //Commit is a function to zip an indexing file and related object file
        public static void ExportCommit(Project project, IndexingTree targetIndexing)
        {
            var path = Path.Combine(project.TemporaryFolderPath, targetIndexing.ID);
            //Step 1: Copy all of them to a temp directory
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            else
            {
                Directory.Delete(path, true);
                Directory.CreateDirectory(path);
            }  
            //Indexing
            FileInfo indexingFile = new FileInfo(targetIndexing.IndexFilePath);
            indexingFile.CopyTo(Path.Combine(project.TemporaryFolderPath, targetIndexing.ID, indexingFile.Name));
            //Objects
            foreach (IndexingNode n in targetIndexing.GetAllNodes())
            {
                FileInfo objectFile = new FileInfo(ObjectHelper.FindObjectPath(project, n.NameSHA256, n.ContentSHA256));
                objectFile.CopyTo(Path.Combine(project.TemporaryFolderPath, targetIndexing.ID, objectFile.Name));
            }

            //Step 2: Zip them
            ZipFile.CreateFromDirectory(path, path + ".zip");

            //Step 3: Delete temp file
            Directory.Delete(path, true);
        }

        public static void ImportCommit(Project project, string id)
        {
            if (Directory.Exists(Path.Combine(project.TemporaryFolderPath, id)))
                Directory.Delete(Path.Combine(project.TemporaryFolderPath, id), true);
            if (!File.Exists(Path.Combine(project.TemporaryFolderPath, id + ".zip")))
                throw new Exception("No such commit");
            ZipFile.ExtractToDirectory(Path.Combine(project.TemporaryFolderPath, id + ".zip"), Path.Combine(project.TemporaryFolderPath, id));
            foreach (FileInfo f in new DirectoryInfo(Path.Combine(project.TemporaryFolderPath, id)).GetFiles())
            {
                if (f.Extension == ".idxdata")
                    f.CopyTo(Path.Combine(project.IndexingFolderPath, f.Name), true);
                else
                    f.CopyTo(Path.Combine(project.ObjectsFolderPath, f.Name), true);
            }
            Directory.Delete(Path.Combine(project.TemporaryFolderPath, id), true);
        }
    }
}
