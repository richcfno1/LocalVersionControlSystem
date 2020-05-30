using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace LocalVersionControlSystem.ObjectSystem
{
    class ObjectCreator
    {
        private static readonly int nameSHA256PartLength = 6;
        private static readonly int contentSHA256PartLength = 6;
        public static void CreateObject(FileInfo f, string nameSHA256, string contentSHA256, string savePath)
        {
            string tempContent = nameSHA256 + "\n" + contentSHA256 + "\n" + f.Name + "\n" + File.ReadAllText(f.FullName);
            File.WriteAllText(savePath + "/" + nameSHA256.Substring(0, nameSHA256PartLength) + contentSHA256.Substring(0, contentSHA256PartLength) + ".objdata", tempContent);
        }
        public static void CreateObject(DirectoryInfo d, string nameSHA256, string savePath)
        {
            string tempContent = nameSHA256 + "\n0000000000000000000000000000000000000000000000000000000000000000\n" + d.Name + "\n" + "";
            File.WriteAllText(savePath + "/" + nameSHA256.Substring(0, nameSHA256PartLength) + "000000.objdata", tempContent);
        }
    }
}
