using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace LocalVersionControlSystem.ObjectSystem
{
    class ObjectCreator
    {
        private static readonly int nameSHA256PartLength = 6;
        private static readonly int contentSHA256PartLength = 6;

        public static void CreateObject(FileInfo f, string nameSHA256, string contentSHA256, string savePath)
        {
            byte[] tempHead = System.Text.Encoding.UTF8.GetBytes(nameSHA256 + "\n" + contentSHA256 + "\n");
            byte[] tempName = System.Text.Encoding.UTF8.GetBytes(f.Name + "\n");
            byte[] tempContent = File.ReadAllBytes(f.FullName);
            byte[] tempAll = tempHead.Concat(tempName).ToArray().Concat(tempContent).ToArray();
            File.WriteAllBytes(savePath + "/" + nameSHA256.Substring(0, nameSHA256PartLength) + contentSHA256.Substring(0, contentSHA256PartLength) + ".objdata",
                tempAll);
        }

        public static void CreateObject(DirectoryInfo d, string nameSHA256, string savePath)
        {
            byte[] tempHead = System.Text.Encoding.UTF8.GetBytes(nameSHA256 + "\n0000000000000000000000000000000000000000000000000000000000000000\n");
            byte[] tempName = System.Text.Encoding.UTF8.GetBytes(d.Name);
            byte[] tempAll = tempHead.Concat(tempName).ToArray();
            File.WriteAllBytes(savePath + "/" + nameSHA256.Substring(0, nameSHA256PartLength) + "000000.objdata", tempAll);
        }

        public static void CreateFile(string targetPath, string objectPath)
        {
            byte[] tempAll = File.ReadAllBytes(objectPath);
            string tempNameSHA256 = System.Text.Encoding.UTF8.GetString(tempAll.Skip(0).Take(64).ToArray());
            string tempContentSHA256 = System.Text.Encoding.UTF8.GetString(tempAll.Skip(65).Take(64).ToArray());

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

            //string tempNameSHA256 = temp.Substring(0, 64);
            //string tempContentSHA256 = temp.Substring(65, 64);
            //string tempName = temp.Substring(130, temp.IndexOf("\n", 130) - 130);
            //string tempContent = temp.Substring(temp.IndexOf("\n", 130) + 1);
            File.WriteAllBytes(targetPath + "/" + tempName, tempContent);
        }
    }
}
