using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Specialized;
using Newtonsoft.Json;

namespace LocalVersionControlSystem.ObjectSystem
{
    static class ObjectHelper
    {
        public static readonly string EmptyZeroes = new string('0', 64);
        //Object file name includes the first 6 digit of HASH of file and first 6 digit of HASH of file content.
        private const int NameSHA256PartLength = 6;
        private const int ContentSHA256PartLength = 6;

        //Use fileinfo to find a file, record nameSHA256 and contentSHA256, create a object file.
        public static void CreateObject(this Project project, FileInfo f, string nameSHA256, string contentSHA256)
        {
            var outputFilePath = Path.Combine(project.ObjectsFolderPath, nameSHA256.Substring(0, NameSHA256PartLength) +
                contentSHA256.Substring(0, ContentSHA256PartLength) + ".objdata");
            string resultJSON = JsonConvert.SerializeObject(new Object(nameSHA256, contentSHA256, f.Name, File.ReadAllBytes(f.FullName)));
            File.WriteAllText(outputFilePath, resultJSON);
        }

        //Use directoryinfo to find a directory, record nameSHA256 and contentSHA256(0), create a object file.
        public static void CreateObject(this Project project, DirectoryInfo d, string nameSHA256)
        {
            var outputFilePath = Path.Combine(project.ObjectsFolderPath, nameSHA256.Substring(0, NameSHA256PartLength) + "000000.objdata");
            string resultJSON = JsonConvert.SerializeObject(new Object(nameSHA256, EmptyZeroes, d.Name, Array.Empty<byte>()));
            File.WriteAllText(outputFilePath, resultJSON);
        }

        //Get HASH of name of a object file.
        public static string GetFileNameSHA256(string objectPath)
        {
            return JsonConvert.DeserializeObject<Object>(File.ReadAllText(objectPath)).NameSHA256;
        }

        //Get HASH of content of a object file.
        public static string GetContentSHA256(string objectPath)
        {
            return JsonConvert.DeserializeObject<Object>(File.ReadAllText(objectPath)).ContentSHA256;
        }

        public static string GetName(string objectPath)
        {
            return JsonConvert.DeserializeObject<Object>(File.ReadAllText(objectPath)).Name;
        }

        //Find the path to a object with specific nameSHA256, contentSHA256, and path to all objects
        public static string? FindObjectPath(this Project project, string nameSHA256, string contentSHA256)
        {
            var objectsFolder = new DirectoryInfo(project.ObjectsFolderPath);
            foreach (var file in objectsFolder.GetFiles())
            {
                if (GetFileNameSHA256(file.FullName) == nameSHA256 && GetContentSHA256(file.FullName) == contentSHA256)
                {
                    return file.FullName;
                }
            }

            foreach (var directory in objectsFolder.GetDirectories())
            {
                if (GetFileNameSHA256(directory.FullName) == nameSHA256 && GetContentSHA256(directory.FullName) == contentSHA256)
                {
                    return directory.FullName;
                }
            }

            return null;
        }

        //Create a file at targetPath form a given object at objectPath, return the path of file.
        public static string CreateFile(string targetPath, string objectPath)
        {
            Object temp = JsonConvert.DeserializeObject<Object>(File.ReadAllText(objectPath));
            var fileName = temp.Name;
            var fileContent = temp.Content;
            var outputPath = Path.Combine(targetPath, fileName);
            File.WriteAllBytes(outputPath, fileContent);
            return outputPath;
        }

        //Create a directory at targetPath form a given object at objectPath, return the path of directory.
        public static string CreateDirectory(string targetPath, string objectPath)
        {
            var outputPath = Path.Combine(targetPath, JsonConvert.DeserializeObject<Object>(File.ReadAllText(objectPath)).Name);
            Directory.CreateDirectory(outputPath);
            return outputPath;
        }
    }
}
