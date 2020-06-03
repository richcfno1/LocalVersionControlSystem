using LocalVersionControlSystem.IndexingSystem;
using LocalVersionControlSystem.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;
using LocalVersionControlSystem.ObjectSystem;

namespace LocalVersionControlSystem.CommitSystem
{
    class Commit
    {
        public static void Export(Project project, string indexingTreeID, string commitID)
        {
            string commitPath = commitID + ".cmtdata";
            foreach (FileInfo f in new DirectoryInfo(project.IndexingFolderPath).GetFiles())
            {
                if (f.Name.Substring(0, 12) == indexingTreeID)
                {
                    File.WriteAllText(commitPath, commitID + "\n"); //Commit ID
                    File.AppendAllText(commitPath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss",DateTimeFormatInfo.InvariantInfo) + "\n"); //Commit time

                    //Indexing update time + indexing content
                    string[] indexing;
                    indexing = File.ReadAllLines(f.FullName);
                    File.AppendAllText(commitPath, indexingTreeID + "#" + indexing.Length + "#" +"\n"); //IndexingTree ID + number of lines
                    File.AppendAllLines(commitPath, indexing);

                    //Objects data area
                    FileStream resultFS = new FileStream(commitPath, FileMode.Append, FileAccess.Write, FileShare.Write);

                    //Objects data
                    for (int i = 1; i < indexing.Length - 1; i++)
                    {
                        string objNameHash = indexing[i].Substring(indexing[i].Length - 130, 64);
                        string objContentHash = indexing[i].Substring(indexing[i].Length - 64, 64);
                        string? path = ObjectHelper.FindObjectPath(project, objNameHash, objContentHash);

                        if(path != null)
                        {
                            byte[] result = File.ReadAllBytes(path);
                            Console.WriteLine(result.Length);
                            resultFS.Write(System.Text.Encoding.UTF8.GetBytes("\nOBJDATA\n"));
                            resultFS.Write(result);
                        }
                    }
                    resultFS.Close();
                }
            }
        }

        public static DateTime Import(Project project, string commitID)
        {
            int curIndex = 0;
            string commitPath = commitID + ".cmtdata";
            DateTime commitTime = new DateTime();
            byte[] content = File.ReadAllBytes(commitPath);

            //Commit ID
            if (commitID != System.Text.Encoding.UTF8.GetString(content, 0, commitID.Length))
                throw new Exception("File Content Error: Commit ID does not match!");
            curIndex += commitID.Length + 1;

            //Commit time
            string tempCT = System.Text.Encoding.UTF8.GetString(content, curIndex, 20);
            commitTime = new DateTime(int.Parse(tempCT.Substring(0, 4), NumberFormatInfo.InvariantInfo), //Year
                int.Parse(tempCT.Substring(5, 2), NumberFormatInfo.InvariantInfo), //Month
                int.Parse(tempCT.Substring(8, 2), NumberFormatInfo.InvariantInfo), //Day
                int.Parse(tempCT.Substring(11, 2), NumberFormatInfo.InvariantInfo), //Hour
                int.Parse(tempCT.Substring(14, 2), NumberFormatInfo.InvariantInfo), //Minute
                int.Parse(tempCT.Substring(17, 2), NumberFormatInfo.InvariantInfo)); //Second
            curIndex += 20;

            //IndexingTreeID
            string indexingTreeID = System.Text.Encoding.UTF8.GetString(content, curIndex, 12);
            curIndex += 13;

            return commitTime;
        }
    }
}
