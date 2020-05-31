using System.IO;

namespace LocalVersionControlSystem
{
    class Project
    {
        /// <summary>
        /// The name of folder containing indexing and object data.
        /// </summary>
        public const string PrivateFolderName = ".LocalVersionControlSystem";

        /// <summary>
        /// The name of folder containing indexing data.
        /// </summary>
        public const string IndexingName = "Indexing";

        /// <summary>
        /// The name of folder containing object data.
        /// </summary>
        public const string ObjectsName = "Objects";

        /// <summary>
        /// The indexing folder of current project.
        /// </summary>
        public string IndexingFolderPath => System.IO.Path.Combine(Path, PrivateFolderName, IndexingName);

        /// <summary>
        /// The objects folder of current project.
        /// </summary>
        public string ObjectsFolderPath => System.IO.Path.Combine(Path, PrivateFolderName, ObjectsName);

        /// <summary>
        /// The project path.
        /// </summary>
        public string Path { get; }

        public Project(string projectPath)
        {
            Path = projectPath;
        }
    }
}
