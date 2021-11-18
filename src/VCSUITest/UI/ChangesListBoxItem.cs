using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using LocalVersionControlSystem;
using LocalVersionControlSystem.ObjectSystem;

namespace VCSUITest.UI
{
    class ChangesListBoxItem
    {
        public string Information { get; set; }
        public string Type { get; set; }
        public string Indexing1 { get; set; }
        public string Indexing2 { get; set; }
        public string ObjectPath1 { get; set; }
        public string ObjectPath2 { get; set; }

        public ChangesListBoxItem(Project _project, string indexing1, string indexing2, bool tempMode1 = false, bool tempMode2 = false)
        {
            Indexing1 = indexing1;
            Indexing2 = indexing2;
            if (indexing1 == string.Empty)
            {
                Type = "Del";
                Information = Type + ":";
                ObjectPath2 = ObjectHelper.FindObjectPath(_project, indexing2.Substring(indexing2.Length - 130, 64), indexing2.Substring(indexing2.Length - 64), tempMode2);
                for (int i = 0; i < indexing2.Length / 131; i++)
                {
                    Information += "/" + ObjectHelper.GetName(
                        ObjectHelper.FindObjectPath(_project, indexing2.Substring(1 + i * 131, 64), indexing2.Substring(67 + i * 131, 64), tempMode2));
                }
                return;
            }
            if (indexing2 == string.Empty)
            {
                Type = "Add";
                Information = Type + ":";
                ObjectPath1 = ObjectHelper.FindObjectPath(_project, indexing1.Substring(indexing1.Length - 130, 64), indexing1.Substring(indexing1.Length - 64), tempMode1);
                for (int i = 0; i < indexing1.Length / 131; i++)
                {
                    Information += "/" + ObjectHelper.GetName(
                        ObjectHelper.FindObjectPath(_project, indexing1.Substring(1 + i * 131, 64), indexing1.Substring(67 + i * 131, 64), tempMode1));
                }
                return;
            }
            else
            {
                Type = "Mod";
                Information = Type + ":";
                ObjectPath1 = ObjectHelper.FindObjectPath(_project, indexing1.Substring(indexing1.Length - 130, 64), indexing1.Substring(indexing1.Length - 64), tempMode1);
                ObjectPath2 = ObjectHelper.FindObjectPath(_project, indexing2.Substring(indexing2.Length - 130, 64), indexing2.Substring(indexing2.Length - 64), tempMode2);
                for (int i = 0; i < indexing1.Length / 131; i++)
                {
                    Information += "/" + ObjectHelper.GetName(
                        ObjectHelper.FindObjectPath(_project, indexing1.Substring(1 + i * 131, 64), indexing1.Substring(67 + i * 131, 64), tempMode1));
                }
                return;
            }
        }

        public override string ToString()
        {
            if (Information.Length > 80)
                return Information.Substring(0, 17) + "..." + Information.Substring(Information.Length - 60);
            return Information;
        }
    }
}
