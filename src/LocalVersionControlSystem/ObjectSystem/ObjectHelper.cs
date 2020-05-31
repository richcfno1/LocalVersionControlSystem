using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace LocalVersionControlSystem.ObjectSystem
{
    static class ObjectHelper
    {
        public static readonly string EmptyZeroes = new string('0', 64);
        //Object file name includes the first 6 digit of SHA256 of file and first 6 digit of SHA256 of file content.
        private const int NameSHA256PartLength = 6;
        private const int ContentSHA256PartLength = 6;

        //Use fileinfo to find a file, record nameSHA256 and contentSHA256, create a object file.
        public static void CreateObject(this Project project, FileInfo f, string nameSHA256, string contentSHA256)
        {
            var header = $"{nameSHA256}\n{contentSHA256}\n{f.Name}\n";
            var objectFileData = Encoding.UTF8.GetBytes(header).Concat(File.ReadAllBytes(f.FullName)).ToArray();
            var outputFilePath = Path.Combine(project.ObjectsFolderPath, nameSHA256.Substring(0, NameSHA256PartLength) +
                contentSHA256.Substring(0, ContentSHA256PartLength) + ".objdata");
            File.WriteAllBytes(outputFilePath, objectFileData);
        }

        //Use directoryinfo to find a directory, record nameSHA256 and contentSHA256(0), create a object file.
        public static void CreateObject(this Project project, DirectoryInfo d, string nameSHA256)
        {
            var objectFileText = $"{nameSHA256}\n{EmptyZeroes}\n{d.Name}";
            var outputFilePath = Path.Combine(project.ObjectsFolderPath, nameSHA256.Substring(0, NameSHA256PartLength) + "000000.objdata");
            File.WriteAllText(outputFilePath, objectFileText);
        }

        //Get SHA256 of name of a object file.
        public static string GetFileNameHash(string objectPath)
        {
            var objectFileContent = File.ReadAllBytes(objectPath);
            return Encoding.UTF8.GetString(objectFileContent.Skip(0).Take(64).ToArray());
        }

        //Get SHA256 of content of a object file.
        public static string GetContentHash(string objectPath)
        {
            var objectFileContent = File.ReadAllBytes(objectPath);
            return Encoding.UTF8.GetString(objectFileContent.Skip(65).Take(64).ToArray());
        }

        public static string GetName(string objectPath)
        {
            var objectFileContent = File.ReadAllBytes(objectPath);
            if (GetContentHash(objectPath) != EmptyZeroes)
            {
                var index = 0;
                for (var i = 130; i < objectFileContent.Length; i++)
                {
                    if (objectFileContent[i] == 10)
                    {
                        index = i;
                        break;
                    }
                }
                return Encoding.UTF8.GetString(objectFileContent.Skip(130).Take(index - 130).ToArray());
            }
            return Encoding.UTF8.GetString(objectFileContent.Skip(130).ToArray());
        }

        //Find the path to a object with specific NameSHA256, ContentSHA256, and path to all objects
        public static string? FindObjectPath(this Project project, string nameHash, string contentHash)
        {
            var objectsFolder = new DirectoryInfo(project.ObjectsFolderPath);
            foreach (var file in objectsFolder.GetFiles())
            {
                if (GetFileNameHash(file.FullName) == nameHash && GetContentHash(file.FullName) == contentHash)
                {
                    return file.FullName;
                }
            }

            foreach (var directory in objectsFolder.GetDirectories())
            {
                if (GetFileNameHash(directory.FullName) == nameHash && GetContentHash(directory.FullName) == contentHash)
                {
                    return directory.FullName;
                }
            }

            return null;
        }

        //Create a file at targetPath form a given object at objectPath, return the path of file.
        public static string CreateFile(string targetPath, string objectPath)
        {
            var objectFileContent = File.ReadAllBytes(objectPath);

            //Find index of \n between name and content.
            var index = 0;
            for (var i = 130; i < objectFileContent.Length; i++)
            {
                if (objectFileContent[i] == 10)
                {
                    index = i;
                    break;
                }
            }
            var fileName = Encoding.UTF8.GetString(objectFileContent.Skip(130).Take(index - 130).ToArray());
            var fileContent = objectFileContent.Skip(index + 1).ToArray();

            var outputPath = Path.Combine(targetPath, fileName);
            File.WriteAllBytes(outputPath, fileContent);
            return outputPath;
        }

        //Create a directory at targetPath form a given object at objectPath, return the path of directory.
        public static string CreateDirectory(string targetPath, string objectPath)
        {
            var objectFileContent = File.ReadAllBytes(objectPath);

            var directoryName = Encoding.UTF8.GetString(objectFileContent.Skip(130).ToArray());

            var outputPath = Path.Combine(targetPath, directoryName);
            Directory.CreateDirectory(outputPath);
            return outputPath;
        }
    }
}
