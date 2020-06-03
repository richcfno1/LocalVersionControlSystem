using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Specialized;

namespace LocalVersionControlSystem.ObjectSystem
{
    static class ObjectHelper
    {
        public static readonly string EmptyZeroes = new string('0', 64);
        //Object file name includes the first 6 digit of HASH of file and first 6 digit of HASH of file content.
        private const int NameHASHPartLength = 6;
        private const int ContentHASHPartLength = 6;

        //Use fileinfo to find a file, record nameHASH and contentHASH, create a object file.
        public static void CreateObject(this Project project, FileInfo f, string nameHASH, string contentHASH)
        {
            var header = $"{nameHASH}\n{contentHASH}\n{f.Name}\n";
            var objectFileData = Encoding.UTF8.GetBytes(header).Concat(File.ReadAllBytes(f.FullName)).ToArray();
            var outputFilePath = Path.Combine(project.ObjectsFolderPath, nameHASH.Substring(0, NameHASHPartLength) +
                contentHASH.Substring(0, ContentHASHPartLength) + ".objdata");
            File.WriteAllBytes(outputFilePath, objectFileData);
        }

        //Use directoryinfo to find a directory, record nameHASH and contentHASH(0), create a object file.
        public static void CreateObject(this Project project, DirectoryInfo d, string nameHASH)
        {
            var objectFileText = $"{nameHASH}\n{EmptyZeroes}\n{d.Name}";
            var outputFilePath = Path.Combine(project.ObjectsFolderPath, nameHASH.Substring(0, NameHASHPartLength) + "000000.objdata");
            File.WriteAllText(outputFilePath, objectFileText);
        }

        public static void CreateObject(this Project project, byte[] data, string nameHASH, string contentHASH)
        {
            var outputFilePath = Path.Combine(project.ObjectsFolderPath, nameHASH.Substring(0, NameHASHPartLength) +
                contentHASH.Substring(0, ContentHASHPartLength) + ".objdata");
            File.WriteAllBytes(outputFilePath, data);
        }

        //Get HASH of name of a object file.
        public static string GetFileNameHash(string objectPath)
        {
            var objectFileContent = File.ReadAllBytes(objectPath);
            return Encoding.UTF8.GetString(objectFileContent.Skip(0).Take(64).ToArray());
        }

        //Get HASH of content of a object file.
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

        //Find the path to a object with specific NameHASH, ContentHASH, and path to all objects
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
