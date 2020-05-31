using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace LocalVersionControlSystem.Helper
{
    static class HashHelper
    {
        public static string HashString(string str)
        {
            var data = Encoding.UTF8.GetBytes(str);
            using var sha256 = SHA256.Create();
            return BytesToHexString(sha256.ComputeHash(data));
        }

        public static string HashFile(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var sha256 = SHA256.Create();
            return BytesToHexString(sha256.ComputeHash(stream));
        }

        private static string BytesToHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", string.Empty, StringComparison.Ordinal);
        }
    }
}
