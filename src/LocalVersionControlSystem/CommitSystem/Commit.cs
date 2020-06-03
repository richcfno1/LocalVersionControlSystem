using LocalVersionControlSystem.IndexingSystem;
using LocalVersionControlSystem.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;
using LocalVersionControlSystem.ObjectSystem;
using System.Linq;

namespace LocalVersionControlSystem.CommitSystem
{
    class Commit
    {
        private static int IndexOf(int start, byte[] a, byte[] b)
        {
            for(int i = start; i < a.Length - b.Length; i++)
            {
                if (a[i] != b[0])
                    continue;
                bool find = true;
                for (int j = 0; j < b.Length; j++)
                {
                    if (b[j] != a[i + j])
                        find = false;
                }
                if (find)
                    return i;
            }
            return -1;
        }
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
                    for (int i = 1; i < indexing.Length; i++)
                    {
                        string objNameHash = indexing[i].Substring(indexing[i].Length - 130, 64);
                        string objContentHash = indexing[i].Substring(indexing[i].Length - 64, 64);
                        string? path = ObjectHelper.FindObjectPath(project, objNameHash, objContentHash);

                        if(path != null)
                        {
                            byte[] result = File.ReadAllBytes(path);
                            resultFS.Write(System.Text.Encoding.UTF8.GetBytes("\nOBJDATA\n"));
                            resultFS.Write(result);
                        }
                    }
                    resultFS.Write(System.Text.Encoding.UTF8.GetBytes("\nOBJDATA\n"));
                    resultFS.Close();
                }
            }
        }

        public static DateTime Import(Project project, string commitID)
        {
            int curIndex = 0;
            string commitPath = commitID + ".cmtdata";
            byte[] content = File.ReadAllBytes(commitPath);

            //Commit ID
            if (commitID != System.Text.Encoding.UTF8.GetString(content, 0, commitID.Length))
                throw new Exception("File Content Error: Commit ID does not match!");
            curIndex += commitID.Length + 1;

            //Commit time
            string tempCT = System.Text.Encoding.UTF8.GetString(content, curIndex, 20);
            DateTime commitTime = new DateTime(int.Parse(tempCT.Substring(0, 4), NumberFormatInfo.InvariantInfo), //Year
                int.Parse(tempCT.Substring(5, 2), NumberFormatInfo.InvariantInfo), //Month
                int.Parse(tempCT.Substring(8, 2), NumberFormatInfo.InvariantInfo), //Day
                int.Parse(tempCT.Substring(11, 2), NumberFormatInfo.InvariantInfo), //Hour
                int.Parse(tempCT.Substring(14, 2), NumberFormatInfo.InvariantInfo), //Minute
                int.Parse(tempCT.Substring(17, 2), NumberFormatInfo.InvariantInfo)); //Second
            curIndex += 20;

            //IndexingTreeID
            string indexingTreeID = System.Text.Encoding.UTF8.GetString(content, curIndex, 12);
            curIndex += 13;
            int nextSharp = IndexOf(curIndex, content, System.Text.Encoding.UTF8.GetBytes("#"));
            int numberOfLines = int.Parse(System.Text.Encoding.UTF8.GetString(content, curIndex, nextSharp - curIndex), NumberFormatInfo.InvariantInfo);
            curIndex = nextSharp + 2;

            List<string> objNameHash = new List<string>();
            List<string> objContentHash = new List<string>();

            //.idxdata
            curIndex += 21;
            for (int i = 1; i < numberOfLines; i++)
            {
                curIndex = IndexOf(curIndex, content, System.Text.Encoding.UTF8.GetBytes("\n"));
                objNameHash.Add(System.Text.Encoding.UTF8.GetString(content, curIndex - 131, 64));
                objContentHash.Add(System.Text.Encoding.UTF8.GetString(content, curIndex - 65, 64));
                curIndex += 130;
            }

            File.WriteAllBytes(Path.Combine(project.IndexingFolderPath, indexingTreeID + ".idxdata"), content.Skip<byte>(nextSharp + 2).
                Take(IndexOf(0, content, System.Text.Encoding.UTF8.GetBytes("\nOBJDATA")) - nextSharp - 4).ToArray());

            curIndex = IndexOf(0, content, System.Text.Encoding.UTF8.GetBytes("\nOBJDATA")) + 9;
            for (int i = 0; i < objNameHash.Count; i++)
            {
                ObjectHelper.CreateObject(project, content.Skip<byte>(curIndex)
                    .Take(IndexOf(curIndex, content, System.Text.Encoding.UTF8.GetBytes("\nOBJDATA")) - curIndex).ToArray(), objNameHash[i], objContentHash[i]);
                curIndex = IndexOf(curIndex, content, System.Text.Encoding.UTF8.GetBytes("\nOBJDATA")) + 9;
            }

            return commitTime;
        }
    }
}
