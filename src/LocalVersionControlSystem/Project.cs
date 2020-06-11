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
        /// The name of folder containing temporary data.
        /// </summary>
        public const string TemporaryName = "Temp";

        /// <summary>
        /// The name of folder containing brench data.
        /// </summary>
        public const string BrenchName = "Brench.bchdata";

        /// <summary>
        /// The indexing folder of current project.
        /// </summary>
        public string IndexingFolderPath => System.IO.Path.Combine(Path, PrivateFolderName, IndexingName);

        /// <summary>
        /// The objects folder of current project.
        /// </summary>
        public string ObjectsFolderPath => System.IO.Path.Combine(Path, PrivateFolderName, ObjectsName);

        /// <summary>
        /// The temporary folder of current project.
        /// </summary>
        public string TemporaryFolderPath => System.IO.Path.Combine(Path, PrivateFolderName, TemporaryName);

        /// <summary>
        /// The brench file of current project.
        /// </summary>
        public string BrenchFolderPath => System.IO.Path.Combine(Path, PrivateFolderName, BrenchName);

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
