namespace SearchEnginesApp.Views
{
    partial class SerchEngineView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gbXmlLoader = new System.Windows.Forms.GroupBox();
            this.linkLabelPubmed = new System.Windows.Forms.LinkLabel();
            this.labelPMID = new System.Windows.Forms.Label();
            this.buttonGetXmlFile = new System.Windows.Forms.Button();
            this.pmidTextBox = new System.Windows.Forms.TextBox();
            this.GroupFileParser = new System.Windows.Forms.GroupBox();
            this.labelQueryStatus = new System.Windows.Forms.Label();
            this.labelQueryTitle = new System.Windows.Forms.Label();
            this.textBoxQueryKeywords = new System.Windows.Forms.TextBox();
            this.BooksDataGridView = new System.Windows.Forms.DataGridView();
            this.FileIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FileType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NumOfCharacter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NumberOfWord = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NumberOfSentence = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonLoadFile = new System.Windows.Forms.Button();
            this.gbXmlLoader.SuspendLayout();
            this.GroupFileParser.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BooksDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // gbXmlLoader
            // 
            this.gbXmlLoader.AutoSize = true;
            this.gbXmlLoader.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gbXmlLoader.Controls.Add(this.linkLabelPubmed);
            this.gbXmlLoader.Controls.Add(this.labelPMID);
            this.gbXmlLoader.Controls.Add(this.buttonGetXmlFile);
            this.gbXmlLoader.Controls.Add(this.pmidTextBox);
            this.gbXmlLoader.Location = new System.Drawing.Point(3, 3);
            this.gbXmlLoader.Name = "gbXmlLoader";
            this.gbXmlLoader.Size = new System.Drawing.Size(243, 87);
            this.gbXmlLoader.TabIndex = 10;
            this.gbXmlLoader.TabStop = false;
            this.gbXmlLoader.Text = "Pubmed Xml Loader";
            // 
            // linkLabelPubmed
            // 
            this.linkLabelPubmed.AutoSize = true;
            this.linkLabelPubmed.Location = new System.Drawing.Point(17, 18);
            this.linkLabelPubmed.Name = "linkLabelPubmed";
            this.linkLabelPubmed.Size = new System.Drawing.Size(160, 12);
            this.linkLabelPubmed.TabIndex = 11;
            this.linkLabelPubmed.TabStop = true;
            this.linkLabelPubmed.Text = "https://pubmed.ncbi.nlm.nih.gov/";
            // 
            // labelPMID
            // 
            this.labelPMID.AutoSize = true;
            this.labelPMID.Location = new System.Drawing.Point(17, 46);
            this.labelPMID.Name = "labelPMID";
            this.labelPMID.Size = new System.Drawing.Size(33, 12);
            this.labelPMID.TabIndex = 10;
            this.labelPMID.Text = "PMID";
            // 
            // buttonGetXmlFile
            // 
            this.buttonGetXmlFile.Location = new System.Drawing.Point(162, 43);
            this.buttonGetXmlFile.Name = "buttonGetXmlFile";
            this.buttonGetXmlFile.Size = new System.Drawing.Size(75, 23);
            this.buttonGetXmlFile.TabIndex = 9;
            this.buttonGetXmlFile.Text = "Get ";
            this.buttonGetXmlFile.UseVisualStyleBackColor = true;
            this.buttonGetXmlFile.Click += new System.EventHandler(this.buttonGetXmlFile_Click);
            // 
            // pmidTextBox
            // 
            this.pmidTextBox.Location = new System.Drawing.Point(56, 43);
            this.pmidTextBox.Name = "pmidTextBox";
            this.pmidTextBox.Size = new System.Drawing.Size(100, 22);
            this.pmidTextBox.TabIndex = 8;
            // 
            // GroupFileParser
            // 
            this.GroupFileParser.AutoSize = true;
            this.GroupFileParser.Controls.Add(this.labelQueryStatus);
            this.GroupFileParser.Controls.Add(this.labelQueryTitle);
            this.GroupFileParser.Controls.Add(this.textBoxQueryKeywords);
            this.GroupFileParser.Controls.Add(this.BooksDataGridView);
            this.GroupFileParser.Controls.Add(this.buttonLoadFile);
            this.GroupFileParser.Location = new System.Drawing.Point(3, 96);
            this.GroupFileParser.Name = "GroupFileParser";
            this.GroupFileParser.Size = new System.Drawing.Size(678, 527);
            this.GroupFileParser.TabIndex = 11;
            this.GroupFileParser.TabStop = false;
            this.GroupFileParser.Text = "Files Parser";
            // 
            // labelQueryStatus
            // 
            this.labelQueryStatus.AutoSize = true;
            this.labelQueryStatus.Location = new System.Drawing.Point(301, 46);
            this.labelQueryStatus.Name = "labelQueryStatus";
            this.labelQueryStatus.Size = new System.Drawing.Size(90, 12);
            this.labelQueryStatus.TabIndex = 11;
            this.labelQueryStatus.Text = "Serach Result: Init";
            // 
            // labelQueryTitle
            // 
            this.labelQueryTitle.AutoSize = true;
            this.labelQueryTitle.Location = new System.Drawing.Point(263, 24);
            this.labelQueryTitle.Name = "labelQueryTitle";
            this.labelQueryTitle.Size = new System.Drawing.Size(34, 12);
            this.labelQueryTitle.TabIndex = 10;
            this.labelQueryTitle.Text = "Query";
            // 
            // textBoxQueryKeywords
            // 
            this.textBoxQueryKeywords.Location = new System.Drawing.Point(303, 21);
            this.textBoxQueryKeywords.Name = "textBoxQueryKeywords";
            this.textBoxQueryKeywords.Size = new System.Drawing.Size(156, 22);
            this.textBoxQueryKeywords.TabIndex = 9;
            this.textBoxQueryKeywords.Click += new System.EventHandler(this.textBoxQueryKeywords_TextChanged);
            // 
            // BooksDataGridView
            // 
            this.BooksDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.BooksDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.BooksDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FileIndex,
            this.FileName,
            this.FileType,
            this.NumOfCharacter,
            this.NumberOfWord,
            this.NumberOfSentence});
            this.BooksDataGridView.Location = new System.Drawing.Point(15, 73);
            this.BooksDataGridView.Name = "BooksDataGridView";
            this.BooksDataGridView.RowTemplate.Height = 24;
            this.BooksDataGridView.Size = new System.Drawing.Size(641, 433);
            this.BooksDataGridView.TabIndex = 8;
            // 
            // FileIndex
            // 
            this.FileIndex.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.FileIndex.FillWeight = 15F;
            this.FileIndex.HeaderText = "Index";
            this.FileIndex.MinimumWidth = 15;
            this.FileIndex.Name = "FileIndex";
            // 
            // FileName
            // 
            this.FileName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.FileName.FillWeight = 50F;
            this.FileName.HeaderText = "File Name";
            this.FileName.MinimumWidth = 70;
            this.FileName.Name = "FileName";
            // 
            // FileType
            // 
            this.FileType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.FileType.FillWeight = 15F;
            this.FileType.HeaderText = "File Type";
            this.FileType.MinimumWidth = 15;
            this.FileType.Name = "FileType";
            this.FileType.Width = 69;
            // 
            // NumOfCharacter
            // 
            this.NumOfCharacter.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.NumOfCharacter.FillWeight = 20F;
            this.NumOfCharacter.HeaderText = "Number of Character";
            this.NumOfCharacter.MinimumWidth = 30;
            this.NumOfCharacter.Name = "NumOfCharacter";
            // 
            // NumberOfWord
            // 
            this.NumberOfWord.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.NumberOfWord.FillWeight = 20F;
            this.NumberOfWord.HeaderText = "Number Of Word";
            this.NumberOfWord.MinimumWidth = 30;
            this.NumberOfWord.Name = "NumberOfWord";
            // 
            // NumberOfSentence
            // 
            this.NumberOfSentence.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.NumberOfSentence.FillWeight = 20F;
            this.NumberOfSentence.HeaderText = "Number Of Sentence";
            this.NumberOfSentence.Name = "NumberOfSentence";
            // 
            // buttonLoadFile
            // 
            this.buttonLoadFile.Location = new System.Drawing.Point(15, 24);
            this.buttonLoadFile.Name = "buttonLoadFile";
            this.buttonLoadFile.Size = new System.Drawing.Size(75, 23);
            this.buttonLoadFile.TabIndex = 3;
            this.buttonLoadFile.Text = "Load";
            this.buttonLoadFile.UseVisualStyleBackColor = true;
            this.buttonLoadFile.Click += new System.EventHandler(this.buttonLoadFile_Click);
            // 
            // SerchEngineView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.Controls.Add(this.GroupFileParser);
            this.Controls.Add(this.gbXmlLoader);
            this.Name = "SerchEngineView";
            this.Size = new System.Drawing.Size(856, 642);
            this.gbXmlLoader.ResumeLayout(false);
            this.gbXmlLoader.PerformLayout();
            this.GroupFileParser.ResumeLayout(false);
            this.GroupFileParser.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BooksDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbXmlLoader;
        private System.Windows.Forms.LinkLabel linkLabelPubmed;
        private System.Windows.Forms.Label labelPMID;
        private System.Windows.Forms.Button buttonGetXmlFile;
        private System.Windows.Forms.TextBox pmidTextBox;
        private System.Windows.Forms.GroupBox GroupFileParser;
        private System.Windows.Forms.Label labelQueryStatus;
        private System.Windows.Forms.Label labelQueryTitle;
        private System.Windows.Forms.TextBox textBoxQueryKeywords;
        private System.Windows.Forms.DataGridView BooksDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileType;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumOfCharacter;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumberOfWord;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumberOfSentence;
        private System.Windows.Forms.Button buttonLoadFile;
    }
}
