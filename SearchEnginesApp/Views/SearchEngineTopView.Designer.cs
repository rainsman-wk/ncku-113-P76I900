namespace SearchEnginesApp.Views
{
    partial class SearchEngineTopView
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
            this.gbLoadFile = new System.Windows.Forms.GroupBox();
            this.buttonFileAnalysis = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbInSelection = new System.Windows.Forms.CheckBox();
            this.cbMatchCase = new System.Windows.Forms.CheckBox();
            this.gbSearchMode = new System.Windows.Forms.GroupBox();
            this.rbPhrase = new System.Windows.Forms.RadioButton();
            this.rbOthers = new System.Windows.Forms.RadioButton();
            this.rbWord = new System.Windows.Forms.RadioButton();
            this.cbSerachContent = new System.Windows.Forms.ComboBox();
            this.labelQueryTitle = new System.Windows.Forms.Label();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.gbInputFiles = new System.Windows.Forms.GroupBox();
            this.buttonLoadFile = new System.Windows.Forms.Button();
            this.gbXmlLoader.SuspendLayout();
            this.gbLoadFile.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.gbSearchMode.SuspendLayout();
            this.gbInputFiles.SuspendLayout();
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
            this.gbXmlLoader.Location = new System.Drawing.Point(820, 16);
            this.gbXmlLoader.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbXmlLoader.Name = "gbXmlLoader";
            this.gbXmlLoader.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbXmlLoader.Size = new System.Drawing.Size(363, 128);
            this.gbXmlLoader.TabIndex = 11;
            this.gbXmlLoader.TabStop = false;
            this.gbXmlLoader.Text = "Pubmed Xml Loader";
            // 
            // linkLabelPubmed
            // 
            this.linkLabelPubmed.AutoSize = true;
            this.linkLabelPubmed.Location = new System.Drawing.Point(26, 27);
            this.linkLabelPubmed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkLabelPubmed.Name = "linkLabelPubmed";
            this.linkLabelPubmed.Size = new System.Drawing.Size(230, 18);
            this.linkLabelPubmed.TabIndex = 11;
            this.linkLabelPubmed.TabStop = true;
            this.linkLabelPubmed.Text = "https://pubmed.ncbi.nlm.nih.gov/";
            // 
            // labelPMID
            // 
            this.labelPMID.AutoSize = true;
            this.labelPMID.Location = new System.Drawing.Point(26, 69);
            this.labelPMID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelPMID.Name = "labelPMID";
            this.labelPMID.Size = new System.Drawing.Size(50, 18);
            this.labelPMID.TabIndex = 10;
            this.labelPMID.Text = "PMID";
            // 
            // buttonGetXmlFile
            // 
            this.buttonGetXmlFile.Location = new System.Drawing.Point(243, 64);
            this.buttonGetXmlFile.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonGetXmlFile.Name = "buttonGetXmlFile";
            this.buttonGetXmlFile.Size = new System.Drawing.Size(112, 34);
            this.buttonGetXmlFile.TabIndex = 9;
            this.buttonGetXmlFile.Text = "Get ";
            this.buttonGetXmlFile.UseVisualStyleBackColor = true;
            this.buttonGetXmlFile.Click += new System.EventHandler(this.buttonGetXmlFile_Click);
            // 
            // pmidTextBox
            // 
            this.pmidTextBox.Location = new System.Drawing.Point(84, 64);
            this.pmidTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pmidTextBox.Name = "pmidTextBox";
            this.pmidTextBox.Size = new System.Drawing.Size(148, 29);
            this.pmidTextBox.TabIndex = 8;
            // 
            // gbLoadFile
            // 
            this.gbLoadFile.Controls.Add(this.buttonFileAnalysis);
            this.gbLoadFile.Controls.Add(this.groupBox1);
            this.gbLoadFile.Controls.Add(this.gbInputFiles);
            this.gbLoadFile.Location = new System.Drawing.Point(14, 14);
            this.gbLoadFile.Name = "gbLoadFile";
            this.gbLoadFile.Size = new System.Drawing.Size(800, 144);
            this.gbLoadFile.TabIndex = 12;
            this.gbLoadFile.TabStop = false;
            this.gbLoadFile.Text = "Search Engine";
            // 
            // buttonFileAnalysis
            // 
            this.buttonFileAnalysis.Location = new System.Drawing.Point(8, 105);
            this.buttonFileAnalysis.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonFileAnalysis.Name = "buttonFileAnalysis";
            this.buttonFileAnalysis.Size = new System.Drawing.Size(142, 34);
            this.buttonFileAnalysis.TabIndex = 5;
            this.buttonFileAnalysis.Text = "Analysis";
            this.buttonFileAnalysis.UseVisualStyleBackColor = true;
            this.buttonFileAnalysis.Click += new System.EventHandler(this.buttonFileAnalysis_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbInSelection);
            this.groupBox1.Controls.Add(this.cbMatchCase);
            this.groupBox1.Controls.Add(this.gbSearchMode);
            this.groupBox1.Controls.Add(this.cbSerachContent);
            this.groupBox1.Controls.Add(this.labelQueryTitle);
            this.groupBox1.Controls.Add(this.buttonSearch);
            this.groupBox1.Location = new System.Drawing.Point(159, 20);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(620, 117);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search ";
            // 
            // cbInSelection
            // 
            this.cbInSelection.AutoSize = true;
            this.cbInSelection.Location = new System.Drawing.Point(442, 84);
            this.cbInSelection.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbInSelection.Name = "cbInSelection";
            this.cbInSelection.Size = new System.Drawing.Size(117, 22);
            this.cbInSelection.TabIndex = 17;
            this.cbInSelection.Text = "In Selection";
            this.cbInSelection.UseVisualStyleBackColor = true;
            // 
            // cbMatchCase
            // 
            this.cbMatchCase.AutoSize = true;
            this.cbMatchCase.Location = new System.Drawing.Point(298, 84);
            this.cbMatchCase.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbMatchCase.Name = "cbMatchCase";
            this.cbMatchCase.Size = new System.Drawing.Size(117, 22);
            this.cbMatchCase.TabIndex = 3;
            this.cbMatchCase.Text = "Match Case";
            this.cbMatchCase.UseVisualStyleBackColor = true;
            // 
            // gbSearchMode
            // 
            this.gbSearchMode.Controls.Add(this.rbPhrase);
            this.gbSearchMode.Controls.Add(this.rbOthers);
            this.gbSearchMode.Controls.Add(this.rbWord);
            this.gbSearchMode.Location = new System.Drawing.Point(298, 16);
            this.gbSearchMode.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbSearchMode.Name = "gbSearchMode";
            this.gbSearchMode.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbSearchMode.Size = new System.Drawing.Size(282, 64);
            this.gbSearchMode.TabIndex = 16;
            this.gbSearchMode.TabStop = false;
            this.gbSearchMode.Text = "Search Mode";
            // 
            // rbPhrase
            // 
            this.rbPhrase.AutoSize = true;
            this.rbPhrase.Location = new System.Drawing.Point(93, 30);
            this.rbPhrase.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbPhrase.Name = "rbPhrase";
            this.rbPhrase.Size = new System.Drawing.Size(79, 22);
            this.rbPhrase.TabIndex = 2;
            this.rbPhrase.TabStop = true;
            this.rbPhrase.Text = "Phrase";
            this.rbPhrase.UseVisualStyleBackColor = true;
            this.rbPhrase.CheckedChanged += new System.EventHandler(this.SearchMode_CheckedChanged);
            // 
            // rbOthers
            // 
            this.rbOthers.AutoSize = true;
            this.rbOthers.Checked = true;
            this.rbOthers.Location = new System.Drawing.Point(182, 30);
            this.rbOthers.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbOthers.Name = "rbOthers";
            this.rbOthers.Size = new System.Drawing.Size(79, 22);
            this.rbOthers.TabIndex = 1;
            this.rbOthers.TabStop = true;
            this.rbOthers.Text = "Others";
            this.rbOthers.UseVisualStyleBackColor = true;
            this.rbOthers.CheckedChanged += new System.EventHandler(this.SearchMode_CheckedChanged);
            // 
            // rbWord
            // 
            this.rbWord.AutoSize = true;
            this.rbWord.Location = new System.Drawing.Point(9, 30);
            this.rbWord.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbWord.Name = "rbWord";
            this.rbWord.Size = new System.Drawing.Size(71, 22);
            this.rbWord.TabIndex = 0;
            this.rbWord.TabStop = true;
            this.rbWord.Text = "Word";
            this.rbWord.UseVisualStyleBackColor = true;
            this.rbWord.CheckedChanged += new System.EventHandler(this.SearchMode_CheckedChanged);
            // 
            // cbSerachContent
            // 
            this.cbSerachContent.FormattingEnabled = true;
            this.cbSerachContent.Location = new System.Drawing.Point(108, 32);
            this.cbSerachContent.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbSerachContent.Name = "cbSerachContent";
            this.cbSerachContent.Size = new System.Drawing.Size(180, 26);
            this.cbSerachContent.TabIndex = 15;
            this.cbSerachContent.TextUpdate += new System.EventHandler(this.cbSerachContent_TextUpdate);
            // 
            // labelQueryTitle
            // 
            this.labelQueryTitle.AutoSize = true;
            this.labelQueryTitle.Location = new System.Drawing.Point(18, 39);
            this.labelQueryTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelQueryTitle.Name = "labelQueryTitle";
            this.labelQueryTitle.Size = new System.Drawing.Size(82, 18);
            this.labelQueryTitle.TabIndex = 13;
            this.labelQueryTitle.Text = "File Query";
            // 
            // buttonSearch
            // 
            this.buttonSearch.Location = new System.Drawing.Point(18, 70);
            this.buttonSearch.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(112, 34);
            this.buttonSearch.TabIndex = 5;
            this.buttonSearch.Text = "Seach";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // gbInputFiles
            // 
            this.gbInputFiles.Controls.Add(this.buttonLoadFile);
            this.gbInputFiles.Location = new System.Drawing.Point(8, 20);
            this.gbInputFiles.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbInputFiles.Name = "gbInputFiles";
            this.gbInputFiles.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbInputFiles.Size = new System.Drawing.Size(142, 82);
            this.gbInputFiles.TabIndex = 6;
            this.gbInputFiles.TabStop = false;
            this.gbInputFiles.Text = "Input File (*.xml , *.json)";
            // 
            // buttonLoadFile
            // 
            this.buttonLoadFile.Location = new System.Drawing.Point(9, 42);
            this.buttonLoadFile.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonLoadFile.Name = "buttonLoadFile";
            this.buttonLoadFile.Size = new System.Drawing.Size(112, 34);
            this.buttonLoadFile.TabIndex = 4;
            this.buttonLoadFile.Text = "Load";
            this.buttonLoadFile.UseVisualStyleBackColor = true;
            this.buttonLoadFile.Click += new System.EventHandler(this.buttonLoadFile_Click);
            // 
            // SearchEngineTopView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbLoadFile);
            this.Controls.Add(this.gbXmlLoader);
            this.Name = "SearchEngineTopView";
            this.Size = new System.Drawing.Size(1200, 160);
            this.gbXmlLoader.ResumeLayout(false);
            this.gbXmlLoader.PerformLayout();
            this.gbLoadFile.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbSearchMode.ResumeLayout(false);
            this.gbSearchMode.PerformLayout();
            this.gbInputFiles.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbXmlLoader;
        private System.Windows.Forms.LinkLabel linkLabelPubmed;
        private System.Windows.Forms.Label labelPMID;
        private System.Windows.Forms.Button buttonGetXmlFile;
        private System.Windows.Forms.TextBox pmidTextBox;
        private System.Windows.Forms.GroupBox gbLoadFile;
        private System.Windows.Forms.Button buttonLoadFile;
        private System.Windows.Forms.GroupBox gbInputFiles;
        private System.Windows.Forms.Button buttonSearch;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelQueryTitle;
        private System.Windows.Forms.ComboBox cbSerachContent;
        private System.Windows.Forms.GroupBox gbSearchMode;
        private System.Windows.Forms.RadioButton rbPhrase;
        private System.Windows.Forms.RadioButton rbOthers;
        private System.Windows.Forms.RadioButton rbWord;
        private System.Windows.Forms.CheckBox cbMatchCase;
        private System.Windows.Forms.CheckBox cbInSelection;
        private System.Windows.Forms.Button buttonFileAnalysis;
    }
}
