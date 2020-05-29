using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace LocalVersionControlSystem.IndexingSystem
{
    class SHA256Helper
    {
        public static string GetStringSHA256(string str)
        {
            byte[] sHA256Data = Encoding.UTF8.GetBytes(str);

            SHA256Managed sHA256 = new SHA256Managed();
            byte[] by = sHA256.ComputeHash(sHA256Data);

            return BitConverter.ToString(by);
        }

        public static string GetFileSHA256(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Open);

            SHA256Managed sHA256 = new SHA256Managed();
            byte[] by = sHA256.ComputeHash(stream);

            return BitConverter.ToString(by);
        }
    }
}
