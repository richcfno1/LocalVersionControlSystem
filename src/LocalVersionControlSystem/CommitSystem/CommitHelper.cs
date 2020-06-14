using LocalVersionControlSystem.IndexingSystem;
using System.IO;
using LocalVersionControlSystem.ObjectSystem;
using System.IO.Compression;
using System;
using Newtonsoft.Json;
using LocalVersionControlSystem.Helper;

namespace LocalVersionControlSystem.CommitSystem
{
    class CommitHelper
    {
        //Commit is a function to zip an indexing file and related object file
        public static void ExportCommit(Project project, string targetPath, IndexingTree targetIndexing)
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
            ZipFile.CreateFromDirectory(path, targetPath);

            //Step 3: Delete temp file
            Directory.Delete(path, true);
        }

        //Extract .cmtdata file and return the id of the tree.
        public static string ImportCommit(Project project, string path, string cmtdataName)
        {
            string result = string.Empty;
            if (Directory.Exists(Path.Combine(project.TemporaryFolderPath, cmtdataName)))
                Directory.Delete(Path.Combine(project.TemporaryFolderPath, cmtdataName), true);
            if (!File.Exists(path))
                throw new Exception("No such commit");
            ZipFile.ExtractToDirectory(path, Path.Combine(project.TemporaryFolderPath, cmtdataName));
            foreach (FileInfo f in new DirectoryInfo(Path.Combine(project.TemporaryFolderPath, cmtdataName)).GetFiles())
            {
                if (f.Extension == ".idxdata")
                {
                    result = f.Name.Substring(0, f.Name.LastIndexOf(".idxdata"));
                    f.CopyTo(Path.Combine(project.IndexingFolderPath, f.Name), true);
                }
                else
                    f.CopyTo(Path.Combine(project.ObjectsFolderPath, f.Name), true);
            }
            Directory.Delete(Path.Combine(project.TemporaryFolderPath, cmtdataName), true);
            return result;
        }
    }
}
