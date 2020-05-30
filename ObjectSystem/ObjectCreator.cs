using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace LocalVersionControlSystem.ObjectSystem
{
    class ObjectCreator
    {
        //Object file name includes the first 6 digit of SHA256 of file and first 6 digit of SHA256 of file content.
        private static readonly int nameSHA256PartLength = 6;
        private static readonly int contentSHA256PartLength = 6;

        //Use fileinfo to find a file, record nameSHA256 and contentSHA256, create a object file.
        public static void CreateObject(FileInfo f, string nameSHA256, string contentSHA256, string savePath)
        {
            byte[] tempHead = System.Text.Encoding.UTF8.GetBytes(nameSHA256 + "\n" + contentSHA256 + "\n");
            byte[] tempName = System.Text.Encoding.UTF8.GetBytes(f.Name + "\n");
            byte[] tempContent = File.ReadAllBytes(f.FullName);
            byte[] tempAll = tempHead.Concat(tempName).ToArray().Concat(tempContent).ToArray();

            File.WriteAllBytes(savePath + "/" + nameSHA256.Substring(0, nameSHA256PartLength) + 
                contentSHA256.Substring(0, contentSHA256PartLength) + ".objdata", tempAll);
        }

        //Use directoryinfo to find a directory, record nameSHA256 and contentSHA256(0), create a object file.
        public static void CreateObject(DirectoryInfo d, string nameSHA256, string savePath)
        {
            byte[] tempHead = System.Text.Encoding.UTF8.GetBytes(nameSHA256 + "\n0000000000000000000000000000000000000000000000000000000000000000\n");
            byte[] tempName = System.Text.Encoding.UTF8.GetBytes(d.Name);
            byte[] tempAll = tempHead.Concat(tempName).ToArray();

            File.WriteAllBytes(savePath + "/" + nameSHA256.Substring(0, nameSHA256PartLength) + "000000.objdata", tempAll);
        }

        //Get SHA256 of name of a object file.
        public static string GetNameSHA256(string objectPath)
        {
            byte[] tempAll = File.ReadAllBytes(objectPath);
            return System.Text.Encoding.UTF8.GetString(tempAll.Skip(0).Take(64).ToArray());
        }

        //Get SHA256 of content of a object file.
        public static string GetContentSHA256(string objectPath)
        {
            byte[] tempAll = File.ReadAllBytes(objectPath);
            return System.Text.Encoding.UTF8.GetString(tempAll.Skip(65).Take(64).ToArray());
        }

        //Find the path to a object with specific NameSHA256, ContentSHA256, and path to all objects
        public static string FindObjectPath(string nameSHA256, string contentSHA256, string objectsPath)
        {
            string result = "";

            DirectoryInfo temp = new DirectoryInfo(objectsPath);
            foreach (FileInfo f in temp.GetFiles())
            {
                if (GetNameSHA256(f.FullName).Equals(nameSHA256) && GetContentSHA256(f.FullName).Equals(contentSHA256))
                {
                    result = f.FullName;
                    break;
                }
            }

            foreach (DirectoryInfo d in temp.GetDirectories())
            {
                if (GetNameSHA256(d.FullName).Equals(nameSHA256) && GetContentSHA256(temp.FullName).Equals(contentSHA256))
                {
                    result = d.FullName;
                    break;
                }
            }

            return result;
        }

        //Create a file at targetPath form a given object at objectPath, return the path of file.
        public static string CreateFile(string targetPath, string objectPath)
        {
            byte[] tempAll = File.ReadAllBytes(objectPath);

            //Find index of \n between name and content.
            int index = 0;
            for (int i = 130; i < tempAll.Length; i++)
            {
                if (tempAll[i] == 10)
                {
                    index = i;
                    break;
                }
            }
            string tempName = System.Text.Encoding.UTF8.GetString(tempAll.Skip(130).Take(index - 130).ToArray());
            byte[] tempContent = tempAll.Skip(index + 1).ToArray();

            File.WriteAllBytes(targetPath + "/" + tempName, tempContent);
            return targetPath + "/" + tempName;
        }

        //Create a directory at targetPath form a given object at objectPath, return the path of directory.
        public static string CreateDirectory(string targetPath, string objectPath)
        {
            byte[] tempAll = File.ReadAllBytes(objectPath);

            string tempName = System.Text.Encoding.UTF8.GetString(tempAll.Skip(130).ToArray());

            Directory.CreateDirectory(targetPath + "/" + tempName);

            return targetPath + "/" + tempName;
        }
    }
}
