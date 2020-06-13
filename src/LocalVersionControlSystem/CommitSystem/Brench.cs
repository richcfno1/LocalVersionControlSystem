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
            _filePath = string.Empty;
            if (project != null)
            {
                _filePath = project.BrenchFolderPath;
                if (!Load())
                {
                    NameList.Add("Master");
                    IndexingTreeIDList.Add("000000000000");
                    Save();
                }
            }
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

        public bool Load()
        {
            if (!File.Exists(_filePath))
                return false;
            Brench temp = JsonConvert.DeserializeObject<Brench>(File.ReadAllText(_filePath));
            this.NameList = temp.NameList;
            this.IndexingTreeIDList = temp.IndexingTreeIDList;
            return true;
        }

        public void Save()
        {
            string resultJSON = JsonConvert.SerializeObject(this);
            File.WriteAllText(_filePath, resultJSON);
        }
    }
}
