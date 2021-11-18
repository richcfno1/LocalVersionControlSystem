namespace VCSUI
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.brenchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newBrenchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadBrenchToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.commitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.changesListBox = new System.Windows.Forms.ListBox();
            this.historyListBox = new System.Windows.Forms.ListBox();
            this.titltTextBox = new System.Windows.Forms.TextBox();
            this.contentTextBox = new System.Windows.Forms.TextBox();
            this.commitButton = new System.Windows.Forms.Button();
            this.changesTitle = new System.Windows.Forms.Label();
            this.historyTitle = new System.Windows.Forms.Label();
            this.refreshButton = new System.Windows.Forms.Button();
            this.pbTitle = new System.Windows.Forms.Label();
            this.pbName = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.histroyItemMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1.SuspendLayout();
            this.histroyItemMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projectToolStripMenuItem,
            this.brenchToolStripMenuItem,
            this.commitToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1330, 30);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // projectToolStripMenuItem
            // 
            this.projectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openProjectToolStripMenuItem});
            this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
            this.projectToolStripMenuItem.Size = new System.Drawing.Size(75, 24);
            this.projectToolStripMenuItem.Text = "Project";
            // 
            // openProjectToolStripMenuItem
            // 
            this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
            this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(132, 26);
            this.openProjectToolStripMenuItem.Text = "Open";
            this.openProjectToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // brenchToolStripMenuItem
            // 
            this.brenchToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newBrenchToolStripMenuItem,
            this.loadBrenchToolStripMenuItem1});
            this.brenchToolStripMenuItem.Name = "brenchToolStripMenuItem";
            this.brenchToolStripMenuItem.Size = new System.Drawing.Size(73, 24);
            this.brenchToolStripMenuItem.Text = "Brench";
            this.brenchToolStripMenuItem.Click += new System.EventHandler(this.brenchToolStripMenuItem_Click);
            // 
            // newBrenchToolStripMenuItem
            // 
            this.newBrenchToolStripMenuItem.Name = "newBrenchToolStripMenuItem";
            this.newBrenchToolStripMenuItem.Size = new System.Drawing.Size(128, 26);
            this.newBrenchToolStripMenuItem.Text = "New";
            this.newBrenchToolStripMenuItem.Click += new System.EventHandler(this.newBrenchToolStripMenuItem_Click);
            // 
            // loadBrenchToolStripMenuItem1
            // 
            this.loadBrenchToolStripMenuItem1.Name = "loadBrenchToolStripMenuItem1";
            this.loadBrenchToolStripMenuItem1.Size = new System.Drawing.Size(128, 26);
            this.loadBrenchToolStripMenuItem1.Text = "Load";
            // 
            // commitToolStripMenuItem
            // 
            this.commitToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToolStripMenuItem,
            this.importToolStripMenuItem});
            this.commitToolStripMenuItem.Name = "commitToolStripMenuItem";
            this.commitToolStripMenuItem.Size = new System.Drawing.Size(81, 24);
            this.commitToolStripMenuItem.Text = "Commit";
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(142, 26);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(142, 26);
            this.importToolStripMenuItem.Text = "Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // changesListBox
            // 
            this.changesListBox.FormattingEnabled = true;
            this.changesListBox.ItemHeight = 20;
            this.changesListBox.Location = new System.Drawing.Point(14, 89);
            this.changesListBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.changesListBox.Name = "changesListBox";
            this.changesListBox.Size = new System.Drawing.Size(419, 224);
            this.changesListBox.TabIndex = 1;
            this.changesListBox.SelectedIndexChanged += new System.EventHandler(this.changesListBox_SelectedIndexChanged);
            // 
            // historyListBox
            // 
            this.historyListBox.FormattingEnabled = true;
            this.historyListBox.ItemHeight = 20;
            this.historyListBox.Location = new System.Drawing.Point(14, 343);
            this.historyListBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.historyListBox.Name = "historyListBox";
            this.historyListBox.Size = new System.Drawing.Size(419, 224);
            this.historyListBox.TabIndex = 2;
            this.historyListBox.SelectedIndexChanged += new System.EventHandler(this.historyListBox_SelectedIndexChanged);
            // 
            // titltTextBox
            // 
            this.titltTextBox.Location = new System.Drawing.Point(15, 576);
            this.titltTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.titltTextBox.Name = "titltTextBox";
            this.titltTextBox.Size = new System.Drawing.Size(277, 27);
            this.titltTextBox.TabIndex = 3;
            this.titltTextBox.Text = "Commit name";
            // 
            // contentTextBox
            // 
            this.contentTextBox.Location = new System.Drawing.Point(15, 617);
            this.contentTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.contentTextBox.Multiline = true;
            this.contentTextBox.Name = "contentTextBox";
            this.contentTextBox.Size = new System.Drawing.Size(277, 105);
            this.contentTextBox.TabIndex = 4;
            this.contentTextBox.Text = "Commit description";
            // 
            // commitButton
            // 
            this.commitButton.Location = new System.Drawing.Point(302, 659);
            this.commitButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.commitButton.Name = "commitButton";
            this.commitButton.Size = new System.Drawing.Size(133, 65);
            this.commitButton.TabIndex = 5;
            this.commitButton.Text = "Commit!";
            this.commitButton.UseVisualStyleBackColor = true;
            this.commitButton.Click += new System.EventHandler(this.commitButton_Click);
            // 
            // changesTitle
            // 
            this.changesTitle.AutoSize = true;
            this.changesTitle.Location = new System.Drawing.Point(14, 63);
            this.changesTitle.Name = "changesTitle";
            this.changesTitle.Size = new System.Drawing.Size(71, 20);
            this.changesTitle.TabIndex = 6;
            this.changesTitle.Text = "Changes";
            // 
            // historyTitle
            // 
            this.historyTitle.AutoSize = true;
            this.historyTitle.Location = new System.Drawing.Point(14, 319);
            this.historyTitle.Name = "historyTitle";
            this.historyTitle.Size = new System.Drawing.Size(62, 20);
            this.historyTitle.TabIndex = 7;
            this.historyTitle.Text = "History";
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(302, 576);
            this.refreshButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(133, 65);
            this.refreshButton.TabIndex = 8;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // pbTitle
            // 
            this.pbTitle.AutoSize = true;
            this.pbTitle.Location = new System.Drawing.Point(14, 37);
            this.pbTitle.Name = "pbTitle";
            this.pbTitle.Size = new System.Drawing.Size(149, 20);
            this.pbTitle.TabIndex = 11;
            this.pbTitle.Text = "Project And Brench";
            // 
            // pbName
            // 
            this.pbName.AutoSize = true;
            this.pbName.Location = new System.Drawing.Point(190, 37);
            this.pbName.Name = "pbName";
            this.pbName.Size = new System.Drawing.Size(49, 20);
            this.pbName.TabIndex = 12;
            this.pbName.Text = "None";
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(446, 63);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(871, 659);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Change";
            // 
            // histroyItemMenuStrip
            // 
            this.histroyItemMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.histroyItemMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.loadToolStripMenuItem});
            this.histroyItemMenuStrip.Name = "histroyItemMenuStrip";
            this.histroyItemMenuStrip.Size = new System.Drawing.Size(119, 52);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(118, 24);
            this.showToolStripMenuItem.Text = "Show";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showCommitToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(118, 24);
            this.loadToolStripMenuItem.Text = "Load";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "cmtdata";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1330, 737);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pbName);
            this.Controls.Add(this.pbTitle);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.historyTitle);
            this.Controls.Add(this.changesTitle);
            this.Controls.Add(this.commitButton);
            this.Controls.Add(this.contentTextBox);
            this.Controls.Add(this.titltTextBox);
            this.Controls.Add(this.historyListBox);
            this.Controls.Add(this.changesListBox);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.histroyItemMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem projectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem commitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ListBox changesListBox;
        private System.Windows.Forms.ListBox historyListBox;
        private System.Windows.Forms.TextBox titltTextBox;
        private System.Windows.Forms.TextBox contentTextBox;
        private System.Windows.Forms.Button commitButton;
        private System.Windows.Forms.ToolStripMenuItem brenchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newBrenchToolStripMenuItem;
        private System.Windows.Forms.Label changesTitle;
        private System.Windows.Forms.Label historyTitle;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Label pbTitle;
        private System.Windows.Forms.Label pbName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ContextMenuStrip histroyItemMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadBrenchToolStripMenuItem1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}

