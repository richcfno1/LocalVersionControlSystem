using LocalVersionControlSystem;
using LocalVersionControlSystem.CommitSystem;
using LocalVersionControlSystem.IndexingSystem;
using LocalVersionControlSystem.ObjectSystem;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using VCSUITest.Helper;
using VCSUITest.UI;

namespace VCSUI
{
    public partial class Form1 : Form
    {
        private Project _project;
        private Brench _brench;
        private int _curBrenchIndex;
        private IndexingTreeList _indexingTreeList;

        public Form1()
        {
            InitializeComponent();
            foreach (Control control in this.Controls)
                control.Enabled = false;
            menuStrip1.Enabled = true;
            projectToolStripMenuItem.Enabled = true;
            brenchToolStripMenuItem.Enabled = false;
            commitToolStripMenuItem.Enabled = false;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "Choose Project Directory";
            folderBrowserDialog1.ShowNewFolderButton = true;
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Desktop;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                _project = new Project(folderBrowserDialog1.SelectedPath);
                _brench = new Brench(_project);
                _curBrenchIndex = 0;
                _indexingTreeList = new IndexingTreeList(_project);
                _indexingTreeList.LoadIndexingTrees();

                _brench.Load();

                foreach (Control control in this.Controls)
                    control.Enabled = true;
                menuStrip1.Enabled = true;
                projectToolStripMenuItem.Enabled = true;
                brenchToolStripMenuItem.Enabled = true;
                commitToolStripMenuItem.Enabled = true;

                refreshButton_Click(sender, e);
            }
        }

        private void changesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (changesListBox.SelectedIndex == -1)
                return;
            groupBox1.Controls.Clear();

            string[] textType = { "txt", "cpp", "cs", "hpp", "c", "h"};
            string[] imageType = { "jpeg", "jpg", "png" };
            if (textType.Contains<string>(((ChangesListBoxItem)changesListBox.SelectedItem).Information.
                Substring(((ChangesListBoxItem)changesListBox.SelectedItem).Information.LastIndexOf(".") + 1)))
            {
                Text1 text1 = new Text1();

                ChangesListBoxItem changesListBoxItem = (ChangesListBoxItem)changesListBox.SelectedItem;
                if (changesListBoxItem.Type == "Add")
                {
                    text1.textBox1.Text = Encoding.UTF8.GetString(ObjectHelper.GetContent(changesListBoxItem.ObjectPath1));
                    groupBox1.Controls.Add(text1);
                }
                if (changesListBoxItem.Type == "Del")
                {
                    text1.textBox1.Text = Encoding.UTF8.GetString(ObjectHelper.GetContent(changesListBoxItem.ObjectPath2));
                    groupBox1.Controls.Add(text1);
                }

                Text2 text2 = new Text2();
                if (changesListBoxItem.Type == "Mod")
                {
                    text2.textBox1.Text = Encoding.UTF8.GetString(ObjectHelper.GetContent(changesListBoxItem.ObjectPath1));
                    text2.textBox2.Text = Encoding.UTF8.GetString(ObjectHelper.GetContent(changesListBoxItem.ObjectPath2));
                    groupBox1.Controls.Add(text2);
                }
            }
            if (imageType.Contains<string>(((ChangesListBoxItem)changesListBox.SelectedItem).Information.
                Substring(((ChangesListBoxItem)changesListBox.SelectedItem).Information.LastIndexOf(".") + 1)))
            {
                Image1 image1 = new Image1();

                ChangesListBoxItem changesListBoxItem = (ChangesListBoxItem)changesListBox.SelectedItem;
                if (changesListBoxItem.Type == "Add")
                {
                    MemoryStream ms = new MemoryStream(ObjectHelper.GetContent(changesListBoxItem.ObjectPath1));
                    image1.pictureBox1.Image = ImageHelper.ResizeImage(Image.FromStream(ms), image1.pictureBox1.Size);
                    groupBox1.Controls.Add(image1);
                }
                if (changesListBoxItem.Type == "Del")
                {
                    MemoryStream ms = new MemoryStream(ObjectHelper.GetContent(changesListBoxItem.ObjectPath2));
                    image1.pictureBox1.Image = ImageHelper.ResizeImage(Image.FromStream(ms), image1.pictureBox1.Size);
                    groupBox1.Controls.Add(image1);
                }

                Image2 image2 = new Image2();
                if (changesListBoxItem.Type == "Mod")
                {
                    MemoryStream ms1 = new MemoryStream(ObjectHelper.GetContent(changesListBoxItem.ObjectPath1));
                    image2.pictureBox1.Image = ImageHelper.ResizeImage(Image.FromStream(ms1), image2.pictureBox1.Size);
                    MemoryStream ms2 = new MemoryStream(ObjectHelper.GetContent(changesListBoxItem.ObjectPath2));
                    image2.pictureBox2.Image = ImageHelper.ResizeImage(Image.FromStream(ms2), image2.pictureBox2.Size);
                    groupBox1.Controls.Add(image2);
                    //MessageBox.Show(image1.pictureBox1.Size.Width.ToString());
                }
            }
        }

        private void historyListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (historyListBox.SelectedIndex == -1)
                return;
            histroyItemMenuStrip.Show(MousePosition.X, MousePosition.Y);
        }

        private void showCommitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changesListBox.Items.Clear();
            IndexingTree currentIndexingTree = (IndexingTree)historyListBox.SelectedItem;
            IndexingTree compareIndexingTree = _indexingTreeList.GetIndexingTree(currentIndexingTree.LastIndexingID);

            if (currentIndexingTree != null)
                currentIndexingTree.ImportTreeFromIndexing();
            if (compareIndexingTree != null)
                compareIndexingTree.ImportTreeFromIndexing();
            List<string> add = IndexingTree.CompareTwoIndexing(currentIndexingTree, compareIndexingTree).ToList();
            List<string> del = IndexingTree.CompareTwoIndexing(compareIndexingTree, currentIndexingTree).ToList();

            //Modify Add Delete
            for (int i = 0; i < add.Count; i++)
            {
                for (int j = 0; j < del.Count; j++)
                {
                    if (add[i].Substring(0, add[i].Length - 64) == add[j].Substring(0, add[j].Length - 64))
                    {
                        changesListBox.Items.Add(new ChangesListBoxItem(_project, add[i], del[j]));
                        add.RemoveAt(i);
                        del.RemoveAt(j);
                        i--;
                        break;
                    }
                }
            }
            foreach (string i in add)
                changesListBox.Items.Add(new ChangesListBoxItem(_project, i, string.Empty));
            foreach (string i in del)
                changesListBox.Items.Add(new ChangesListBoxItem(_project, string.Empty, i));
        }

        private void loadCommitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IndexingTree temp = (IndexingTree)historyListBox.SelectedItem;

            foreach (FileInfo f in new DirectoryInfo(_project.Path).GetFiles())
                File.Delete(f.FullName);
            foreach (DirectoryInfo d in new DirectoryInfo(_project.Path).GetDirectories())
                if (d.Name != ".LocalVersionControlSystem")
                    Directory.Delete(d.FullName, true);

            temp.ImportTreeFromIndexing();
            temp.ExportTreeToDirectory();
            refreshButton_Click(sender, e);
        }

        private void brenchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadBrenchToolStripMenuItem1.DropDownItems.Clear();
            foreach (string name in _brench.NameList)
            {
                ToolStripMenuItem brenchItem = new ToolStripMenuItem();
                brenchItem.Text = name;
                ToolStripMenuItem openItem = new ToolStripMenuItem();
                openItem.Text = "Open";
                openItem.Click += new System.EventHandler(this.openItem_Click);
                brenchItem.DropDownItems.Add(openItem);
                ToolStripMenuItem mergeItem = new ToolStripMenuItem();
                mergeItem.Text = "Merge";
                mergeItem.Click += new System.EventHandler(this.mergeItem_Click);
                brenchItem.DropDownItems.Add(mergeItem);
                loadBrenchToolStripMenuItem1.DropDownItems.Add(brenchItem);
            }
        }

        private void openItem_Click(object sender, EventArgs e)
        {
            _curBrenchIndex = _brench.NameList.IndexOf(((ToolStripMenuItem)sender).OwnerItem.Text);
            refreshButton_Click(sender, e);
        }

        private void mergeItem_Click(object sender, EventArgs e)
        {
            ListChooseDialog lTemp = new ListChooseDialog();
            lTemp.Text = "Merge Brench";
            lTemp.label1.Text = "Merge " + ((ToolStripMenuItem)sender).OwnerItem.Text + " to:";
            lTemp.listBox1.Items.Clear();
            foreach (string name in _brench.NameList)
                lTemp.listBox1.Items.Add(name);
            if (lTemp.ShowDialog() == DialogResult.OK && lTemp.listBox1.SelectedIndex != -1)
            {
                string id1 = _brench.IndexingTreeIDList[_brench.NameList.IndexOf(((ToolStripMenuItem)sender).OwnerItem.Text)];
                string id2 = _brench.IndexingTreeIDList[_brench.NameList.IndexOf((string)lTemp.listBox1.SelectedItem)];
                IndexingTree result = IndexingMergeHelper.Merge(_indexingTreeList.GetIndexingTree(id1), _indexingTreeList.GetIndexingTree(id2));
                for (int i = 0; i < IndexingMergeHelper.UpdateNodeIn1.Count; i++)
                {
                    ConflictFileDialog cTemp = new ConflictFileDialog();

                    Text2 text2 = new Text2();
                    string file1ObjectPath = ObjectHelper.FindObjectPath(_project,
                        IndexingMergeHelper.UpdateNodeIn1[i].NameSHA256, IndexingMergeHelper.UpdateNodeIn1[i].ContentSHA256);
                    string file2ObjectPath = ObjectHelper.FindObjectPath(_project,
                        IndexingMergeHelper.UpdateNodeIn2[i].NameSHA256, IndexingMergeHelper.UpdateNodeIn2[i].ContentSHA256);
                    text2.label1.Text = ((ToolStripMenuItem)sender).OwnerItem.Text;
                    text2.label2.Text = (string)lTemp.listBox1.SelectedItem;
                    text2.textBox1.Height = 463;
                    text2.textBox2.Height = 463;
                    text2.textBox1.Text = Encoding.UTF8.GetString(ObjectHelper.GetContent(file1ObjectPath));
                    text2.textBox2.Text = Encoding.UTF8.GetString(ObjectHelper.GetContent(file2ObjectPath));

                    cTemp.groupBox1.Controls.Clear();
                    cTemp.groupBox1.Controls.Add(text2);
                    cTemp.comboBox1.Items.Add(text2.label1.Text);
                    cTemp.comboBox1.Items.Add(text2.label2.Text);
                    cTemp.comboBox1.SelectedIndex = 0;
                    if (cTemp.ShowDialog() == DialogResult.OK)
                    {
                        if (cTemp.comboBox1.SelectedIndex == 0)
                        {
                            IndexingMergeHelper.Insert(result.GetRoot(), IndexingMergeHelper.UpdateNodeIn1[i]);
                        }
                        else if (cTemp.comboBox1.SelectedIndex == 1)
                        {
                            IndexingMergeHelper.Insert(result.GetRoot(), IndexingMergeHelper.UpdateNodeIn2[i]);
                        }
                    }
                    else
                    {
                        IndexingMergeHelper.Clean();
                        return;
                    }
                }
                result.ExportTreeToIndexing();
                _indexingTreeList.LoadIndexingTrees();
                result.LastIndexingID = id2;
                _brench.IndexingTreeIDList[_brench.NameList.IndexOf((string)lTemp.listBox1.SelectedItem)] = result.ID;
                _curBrenchIndex = _brench.NameList.IndexOf((string)lTemp.listBox1.SelectedItem);
                _brench.Save();
            }
            refreshButton_Click(sender, e);
        }

        private void newBrenchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewBrenchDialog temp = new NewBrenchDialog();
            if (temp.ShowDialog() == DialogResult.OK)
            {
                if (_brench.NameList.Contains(temp.textBox1.Text))
                {
                    MessageBox.Show("Exist Brench Name!");
                    return;
                }
                _brench.NameList.Add(temp.textBox1.Text);
                _brench.IndexingTreeIDList.Add(_brench.IndexingTreeIDList[_curBrenchIndex]);
                _curBrenchIndex = _brench.NameList.IndexOf(temp.textBox1.Text);
            }
            refreshButton_Click(sender, e);
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(Path.Combine(_project.TemporaryFolderPath, "Indexing")))
                Directory.CreateDirectory(Path.Combine(_project.TemporaryFolderPath, "Indexing"));
            if (!Directory.Exists(Path.Combine(_project.TemporaryFolderPath, "Objects")))
                Directory.CreateDirectory(Path.Combine(_project.TemporaryFolderPath, "Objects"));

            changesListBox.Items.Clear();
            historyListBox.Items.Clear();

            //Changes
            IndexingTree currentIndexingTree = new IndexingTree(_project, _brench.IndexingTreeIDList[_curBrenchIndex], true);
            IndexingTree compareIndexingTree = _indexingTreeList.GetIndexingTree(_brench.IndexingTreeIDList[_curBrenchIndex]);

            if(currentIndexingTree != null)
                currentIndexingTree.ImportTreeFromDirectory();
            if (compareIndexingTree != null)
                compareIndexingTree.ImportTreeFromIndexing();
            List<string> add = IndexingTree.CompareTwoIndexing(currentIndexingTree, compareIndexingTree).ToList();
            List<string> del = IndexingTree.CompareTwoIndexing(compareIndexingTree, currentIndexingTree).ToList();

            //Modify Add Delete
            for (int i = 0; i < add.Count; i++)
            {
                for (int j = 0; j < del.Count; j++)
                {
                    if (add[i].Substring(0, add[i].Length - 64) == del[j].Substring(0, del[j].Length - 64))
                    {
                        changesListBox.Items.Add(new ChangesListBoxItem(_project, add[i], del[j], true, false));
                        add.RemoveAt(i);
                        del.RemoveAt(j);
                        i--;
                        break;
                    }
                }
            }
            foreach (string i in add)
                changesListBox.Items.Add(new ChangesListBoxItem(_project, i, string.Empty, true));
            foreach (string i in del)
                changesListBox.Items.Add(new ChangesListBoxItem(_project, string.Empty, i));

            //History
            string tempID = _brench.IndexingTreeIDList[_curBrenchIndex];
            while (true)
            {
                if (tempID == "000000000000")
                    break;
                historyListBox.Items.Add(_indexingTreeList.GetIndexingTree(tempID));
                tempID = _indexingTreeList.GetIndexingTree(tempID).LastIndexingID;
            }

            //Project and brench information
            pbName.Text = "Project: " + _project.ProjectName + " Brench: " + _brench.NameList[_curBrenchIndex];

            //Brench
            loadBrenchToolStripMenuItem1.DropDownItems.Clear();
            foreach (string name in _brench.NameList)
            {
                ToolStripMenuItem brenchItem = new ToolStripMenuItem();
                brenchItem.Text = name;
                ToolStripMenuItem openItem = new ToolStripMenuItem();
                openItem.Text = "Open";
                openItem.Click += new System.EventHandler(this.openItem_Click);
                brenchItem.DropDownItems.Add(openItem);
                ToolStripMenuItem mergeItem = new ToolStripMenuItem();
                mergeItem.Text = "Merge";
                mergeItem.Click += new System.EventHandler(this.mergeItem_Click);
                brenchItem.DropDownItems.Add(mergeItem);
                loadBrenchToolStripMenuItem1.DropDownItems.Add(brenchItem);
            }
        }

        private void commitButton_Click(object sender, EventArgs e)
        {
            IndexingTree curIndexingTree = new IndexingTree(_project, _brench.IndexingTreeIDList[_curBrenchIndex]);
            curIndexingTree.Name = titltTextBox.Text;
            curIndexingTree.Describe = contentTextBox.Text;
            curIndexingTree.ImportTreeFromDirectory();
            curIndexingTree.ExportTreeToIndexing();
            _indexingTreeList.AddIndexingTree(curIndexingTree);
            _brench.IndexingTreeIDList[_curBrenchIndex] = curIndexingTree.ID;
            _brench.Save();
            refreshButton_Click(sender, e);
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListChooseDialog temp = new ListChooseDialog();
            temp.Text = "Export Commit";
            temp.label1.Text = "Export commit with name:";
            temp.listBox1.Items.Clear();
            foreach (IndexingTree i in _indexingTreeList.GetAllIndexingTrees())
                temp.listBox1.Items.Add(i);
            if(temp.ShowDialog() == DialogResult.OK && temp.listBox1.SelectedIndex != -1)
            {
                saveFileDialog1.Filter = "Commit Data File(*.cmtdata)|*.cmtdata";
                saveFileDialog1.FilterIndex = 1;
                saveFileDialog1.FileName = temp.listBox1.SelectedItem.ToString();
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    CommitHelper.ExportCommit(_project, saveFileDialog1.FileName, (IndexingTree)temp.listBox1.SelectedItem);
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListChooseDialog temp = new ListChooseDialog();
            temp.Text = "Import Commit";
            temp.label1.Text = "Add the commit to:";
            temp.listBox1.Items.Clear();
            foreach (string name in _brench.NameList)
                temp.listBox1.Items.Add(name);
            openFileDialog1.Filter = "Commit Data File(*.cmtdata)|*.cmtdata";
            saveFileDialog1.FilterIndex = 1;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if(temp.ShowDialog() == DialogResult.OK && temp.listBox1.SelectedIndex != -1)
                {
                    _curBrenchIndex = temp.listBox1.SelectedIndex;
                    string lastID1 = _brench.IndexingTreeIDList[_curBrenchIndex];
                    string lastID2 = _indexingTreeList.GetIndexingTree(_brench.IndexingTreeIDList[_curBrenchIndex]).LastIndexingID;
                    _brench.IndexingTreeIDList[_curBrenchIndex] = CommitHelper.ImportCommit(_project, openFileDialog1.FileName, 
                        openFileDialog1.FileName.Substring(openFileDialog1.FileName.LastIndexOf("\\")));
                    _indexingTreeList.Clear();
                    _indexingTreeList.LoadIndexingTrees();
                    if (lastID1 == _brench.IndexingTreeIDList[_curBrenchIndex])
                        lastID1 = lastID2;
                    _indexingTreeList.GetIndexingTree(_brench.IndexingTreeIDList[_curBrenchIndex]).LastIndexingID = lastID1;
                    _indexingTreeList.GetIndexingTree(_brench.IndexingTreeIDList[_curBrenchIndex]).ExportTreeToIndexing();

                    _brench.Save();
                }
            }
            refreshButton_Click(sender, e);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_project == null)
                return;
            foreach (FileInfo f in new DirectoryInfo(_project.TemporaryFolderPath).GetFiles())
                File.Delete(f.FullName);
            foreach (DirectoryInfo d in new DirectoryInfo(_project.TemporaryFolderPath).GetDirectories())
                Directory.Delete(d.FullName, true);
        }
    }
}
