using System;

namespace LocalVersionControlSystem.IndexingSystem
{
    class Indexing
    {
        public string IndexingID { get; set; }
        public string IndexingName { get; set; }
        public string IndexingDescribe { get; set; }
        public DateTime SubmitTime { get; set; }
        public string LastIndexingID { get; set; }
        public string[] ProjectContents { get; set; }

        public Indexing(string indexingID, string indexingName, string indexingDescribe,
            DateTime submitTime, string lastIndexingID, string[] projectContents)
        {
            IndexingID = indexingID;
            IndexingName = indexingName;
            IndexingDescribe = indexingDescribe;
            SubmitTime = submitTime;
            LastIndexingID = lastIndexingID;
            ProjectContents = projectContents;
        }
    }
}
