using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace LocalVersionControlSystem.CommitSystem
{
    class Brench
    {
        public List<string> NameList { get; set; }
        public List<string> IndexingTreeIDList { get; set; }
        private string _filePath;

        public Brench(Project project)
        {
            NameList = new List<string>();
            IndexingTreeIDList = new List<string>();
            if (project != null)
                _filePath = project.BrenchFolderPath;
        }

        public void NewBrench(string name, string indexingTreeID)
        {
            NameList.Add(name);
            IndexingTreeIDList.Add(indexingTreeID);
        }

        public string GetBrench(string name)
        {
            return IndexingTreeIDList[NameList.IndexOf(name)];
        }

        public void Load()
        {
            Brench temp = JsonConvert.DeserializeObject<Brench>(File.ReadAllText(_filePath));
            this.NameList = temp.NameList;
            this.IndexingTreeIDList = temp.IndexingTreeIDList;
        }

        public void Save()
        {
            string resultJSON = JsonConvert.SerializeObject(this);
            File.WriteAllText(_filePath, resultJSON);
        }
    }
}
