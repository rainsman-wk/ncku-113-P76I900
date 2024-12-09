using System;
using System.Drawing;
using System.Windows.Forms;

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
            this.components = new System.ComponentModel.Container();
            this.gbXmlLoader = new System.Windows.Forms.GroupBox();
            this.lblPmidLoadState = new System.Windows.Forms.Label();
            this.labelPMID = new System.Windows.Forms.Label();
            this.buttonGetXmlFile = new System.Windows.Forms.Button();
            this.pmidTextBox = new System.Windows.Forms.TextBox();
            this.cmbYearFilter = new System.Windows.Forms.ComboBox();
            this.nudMaxResults = new System.Windows.Forms.NumericUpDown();
            this.chkAllYears = new System.Windows.Forms.CheckBox();
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
            this.tipPhrase = new System.Windows.Forms.ToolTip(this.components);
            this.tipOthers = new System.Windows.Forms.ToolTip(this.components);
            this.tipWord = new System.Windows.Forms.ToolTip(this.components);
            this.gbWord2Vec = new System.Windows.Forms.GroupBox();
            this.btnAnylzeTfIDf = new System.Windows.Forms.Button();
            this.btnLoadPubmed = new System.Windows.Forms.Button();
            this.btnLoadXmlDataBase = new System.Windows.Forms.Button();
            this.gbXmlLoader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxResults)).BeginInit();
            this.gbLoadFile.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.gbSearchMode.SuspendLayout();
            this.gbInputFiles.SuspendLayout();
            this.gbWord2Vec.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbXmlLoader
            // 
            this.gbXmlLoader.AutoSize = true;
            this.gbXmlLoader.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gbXmlLoader.Controls.Add(this.lblPmidLoadState);
            this.gbXmlLoader.Controls.Add(this.labelPMID);
            this.gbXmlLoader.Controls.Add(this.buttonGetXmlFile);
            this.gbXmlLoader.Controls.Add(this.pmidTextBox);
            this.gbXmlLoader.Controls.Add(this.cmbYearFilter);
            this.gbXmlLoader.Controls.Add(this.nudMaxResults);
            this.gbXmlLoader.Controls.Add(this.chkAllYears);
            this.gbXmlLoader.Location = new System.Drawing.Point(547, 11);
            this.gbXmlLoader.Name = "gbXmlLoader";
            this.gbXmlLoader.Size = new System.Drawing.Size(243, 101);
            this.gbXmlLoader.TabIndex = 11;
            this.gbXmlLoader.TabStop = false;
            this.gbXmlLoader.Text = "Pubmed Xml Loader";
            // 
            // lblPmidLoadState
            // 
            this.lblPmidLoadState.AutoSize = true;
            this.lblPmidLoadState.Location = new System.Drawing.Point(17, 71);
            this.lblPmidLoadState.Name = "lblPmidLoadState";
            this.lblPmidLoadState.Size = new System.Drawing.Size(0, 12);
            this.lblPmidLoadState.TabIndex = 14;
            // 
            // labelPMID
            // 
            this.labelPMID.AutoSize = true;
            this.labelPMID.Location = new System.Drawing.Point(17, 18);
            this.labelPMID.Name = "labelPMID";
            this.labelPMID.Size = new System.Drawing.Size(36, 12);
            this.labelPMID.TabIndex = 10;
            this.labelPMID.Text = "Search";
            // 
            // buttonGetXmlFile
            // 
            this.buttonGetXmlFile.Location = new System.Drawing.Point(162, 15);
            this.buttonGetXmlFile.Name = "buttonGetXmlFile";
            this.buttonGetXmlFile.Size = new System.Drawing.Size(75, 23);
            this.buttonGetXmlFile.TabIndex = 9;
            this.buttonGetXmlFile.Text = "Get ";
            this.buttonGetXmlFile.UseVisualStyleBackColor = true;
            this.buttonGetXmlFile.Click += new System.EventHandler(this.buttonGetXmlFile_Click);
            // 
            // pmidTextBox
            // 
            this.pmidTextBox.Location = new System.Drawing.Point(56, 15);
            this.pmidTextBox.Name = "pmidTextBox";
            this.pmidTextBox.Size = new System.Drawing.Size(100, 22);
            this.pmidTextBox.TabIndex = 8;
            // 
            // cmbYearFilter
            // 
            this.cmbYearFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbYearFilter.Items.AddRange(new object[] {
            "2024",
            "2023",
            "2022",
            "2021",
            "2020",
            "2019",
            "2018",
            "2017",
            "2016",
            "2015"});
            this.cmbYearFilter.Location = new System.Drawing.Point(79, 44);
            this.cmbYearFilter.Name = "cmbYearFilter";
            this.cmbYearFilter.Size = new System.Drawing.Size(80, 20);
            this.cmbYearFilter.TabIndex = 15;
            // 
            // nudMaxResults
            // 
            this.nudMaxResults.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudMaxResults.Location = new System.Drawing.Point(180, 43);
            this.nudMaxResults.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudMaxResults.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudMaxResults.Name = "nudMaxResults";
            this.nudMaxResults.Size = new System.Drawing.Size(57, 22);
            this.nudMaxResults.TabIndex = 16;
            this.nudMaxResults.Value = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            // 
            // chkAllYears
            // 
            this.chkAllYears.AutoSize = true;
            this.chkAllYears.Location = new System.Drawing.Point(6, 46);
            this.chkAllYears.Name = "chkAllYears";
            this.chkAllYears.Size = new System.Drawing.Size(67, 16);
            this.chkAllYears.TabIndex = 17;
            this.chkAllYears.Text = "All Years";
            this.chkAllYears.CheckedChanged += new System.EventHandler(this.chkAllYears_CheckedChanged);
            // 
            // gbLoadFile
            // 
            this.gbLoadFile.Controls.Add(this.buttonFileAnalysis);
            this.gbLoadFile.Controls.Add(this.groupBox1);
            this.gbLoadFile.Controls.Add(this.gbInputFiles);
            this.gbLoadFile.Location = new System.Drawing.Point(9, 9);
            this.gbLoadFile.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gbLoadFile.Name = "gbLoadFile";
            this.gbLoadFile.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gbLoadFile.Size = new System.Drawing.Size(533, 103);
            this.gbLoadFile.TabIndex = 12;
            this.gbLoadFile.TabStop = false;
            this.gbLoadFile.Text = "Search Engine";
            // 
            // buttonFileAnalysis
            // 
            this.buttonFileAnalysis.Location = new System.Drawing.Point(5, 70);
            this.buttonFileAnalysis.Name = "buttonFileAnalysis";
            this.buttonFileAnalysis.Size = new System.Drawing.Size(95, 23);
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
            this.groupBox1.Location = new System.Drawing.Point(106, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(413, 78);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search ";
            // 
            // cbInSelection
            // 
            this.cbInSelection.AutoSize = true;
            this.cbInSelection.Enabled = false;
            this.cbInSelection.Location = new System.Drawing.Point(295, 56);
            this.cbInSelection.Name = "cbInSelection";
            this.cbInSelection.Size = new System.Drawing.Size(79, 16);
            this.cbInSelection.TabIndex = 17;
            this.cbInSelection.Text = "In Selection";
            this.cbInSelection.UseVisualStyleBackColor = true;
            this.cbInSelection.Visible = false;
            // 
            // cbMatchCase
            // 
            this.cbMatchCase.AutoSize = true;
            this.cbMatchCase.Location = new System.Drawing.Point(199, 56);
            this.cbMatchCase.Name = "cbMatchCase";
            this.cbMatchCase.Size = new System.Drawing.Size(78, 16);
            this.cbMatchCase.TabIndex = 3;
            this.cbMatchCase.Text = "Match Case";
            this.cbMatchCase.UseVisualStyleBackColor = true;
            // 
            // gbSearchMode
            // 
            this.gbSearchMode.Controls.Add(this.rbPhrase);
            this.gbSearchMode.Controls.Add(this.rbOthers);
            this.gbSearchMode.Controls.Add(this.rbWord);
            this.gbSearchMode.Location = new System.Drawing.Point(199, 11);
            this.gbSearchMode.Name = "gbSearchMode";
            this.gbSearchMode.Size = new System.Drawing.Size(188, 43);
            this.gbSearchMode.TabIndex = 16;
            this.gbSearchMode.TabStop = false;
            this.gbSearchMode.Text = "Search Mode";
            // 
            // rbPhrase
            // 
            this.rbPhrase.AutoSize = true;
            this.rbPhrase.Location = new System.Drawing.Point(62, 20);
            this.rbPhrase.Name = "rbPhrase";
            this.rbPhrase.Size = new System.Drawing.Size(53, 16);
            this.rbPhrase.TabIndex = 2;
            this.rbPhrase.TabStop = true;
            this.rbPhrase.Text = "Phrase";
            this.tipPhrase.SetToolTip(this.rbPhrase, "Spilt Text by comma");
            this.rbPhrase.UseVisualStyleBackColor = true;
            this.rbPhrase.CheckedChanged += new System.EventHandler(this.SearchMode_CheckedChanged);
            // 
            // rbOthers
            // 
            this.rbOthers.AutoSize = true;
            this.rbOthers.Checked = true;
            this.rbOthers.Location = new System.Drawing.Point(121, 20);
            this.rbOthers.Name = "rbOthers";
            this.rbOthers.Size = new System.Drawing.Size(53, 16);
            this.rbOthers.TabIndex = 1;
            this.rbOthers.TabStop = true;
            this.rbOthers.Text = "Others";
            this.tipOthers.SetToolTip(this.rbOthers, "Full Text Searching");
            this.rbOthers.UseVisualStyleBackColor = true;
            this.rbOthers.CheckedChanged += new System.EventHandler(this.SearchMode_CheckedChanged);
            // 
            // rbWord
            // 
            this.rbWord.AutoSize = true;
            this.rbWord.Location = new System.Drawing.Point(6, 20);
            this.rbWord.Name = "rbWord";
            this.rbWord.Size = new System.Drawing.Size(50, 16);
            this.rbWord.TabIndex = 0;
            this.rbWord.TabStop = true;
            this.rbWord.Text = "Word";
            this.tipWord.SetToolTip(this.rbWord, "Spilt Text by space or comma");
            this.rbWord.UseVisualStyleBackColor = true;
            this.rbWord.CheckedChanged += new System.EventHandler(this.SearchMode_CheckedChanged);
            // 
            // cbSerachContent
            // 
            this.cbSerachContent.FormattingEnabled = true;
            this.cbSerachContent.Location = new System.Drawing.Point(72, 21);
            this.cbSerachContent.Name = "cbSerachContent";
            this.cbSerachContent.Size = new System.Drawing.Size(121, 20);
            this.cbSerachContent.TabIndex = 15;
            this.cbSerachContent.TextUpdate += new System.EventHandler(this.cbSerachContent_TextUpdate);
            // 
            // labelQueryTitle
            // 
            this.labelQueryTitle.AutoSize = true;
            this.labelQueryTitle.Location = new System.Drawing.Point(12, 26);
            this.labelQueryTitle.Name = "labelQueryTitle";
            this.labelQueryTitle.Size = new System.Drawing.Size(54, 12);
            this.labelQueryTitle.TabIndex = 13;
            this.labelQueryTitle.Text = "File Query";
            // 
            // buttonSearch
            // 
            this.buttonSearch.Location = new System.Drawing.Point(12, 47);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(75, 23);
            this.buttonSearch.TabIndex = 5;
            this.buttonSearch.Text = "Seach";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // gbInputFiles
            // 
            this.gbInputFiles.Controls.Add(this.buttonLoadFile);
            this.gbInputFiles.Location = new System.Drawing.Point(5, 13);
            this.gbInputFiles.Name = "gbInputFiles";
            this.gbInputFiles.Size = new System.Drawing.Size(95, 55);
            this.gbInputFiles.TabIndex = 6;
            this.gbInputFiles.TabStop = false;
            this.gbInputFiles.Text = "Input File (*.xml , *.json)";
            // 
            // buttonLoadFile
            // 
            this.buttonLoadFile.Location = new System.Drawing.Point(6, 28);
            this.buttonLoadFile.Name = "buttonLoadFile";
            this.buttonLoadFile.Size = new System.Drawing.Size(75, 23);
            this.buttonLoadFile.TabIndex = 4;
            this.buttonLoadFile.Text = "Load";
            this.buttonLoadFile.UseVisualStyleBackColor = true;
            this.buttonLoadFile.Click += new System.EventHandler(this.buttonLoadFile_Click);
            // 
            // tipPhrase
            // 
            this.tipPhrase.BackColor = System.Drawing.Color.LightBlue;
            // 
            // tipOthers
            // 
            this.tipOthers.BackColor = System.Drawing.Color.LightBlue;
            // 
            // tipWord
            // 
            this.tipWord.BackColor = System.Drawing.Color.LightBlue;
            // 
            // gbWord2Vec
            // 
            this.gbWord2Vec.Controls.Add(this.btnAnylzeTfIDf);
            this.gbWord2Vec.Controls.Add(this.btnLoadPubmed);
            this.gbWord2Vec.Controls.Add(this.btnLoadXmlDataBase);
            this.gbWord2Vec.Location = new System.Drawing.Point(797, 11);
            this.gbWord2Vec.Name = "gbWord2Vec";
            this.gbWord2Vec.Size = new System.Drawing.Size(177, 100);
            this.gbWord2Vec.TabIndex = 13;
            this.gbWord2Vec.TabStop = false;
            this.gbWord2Vec.Text = "Word2Vec";
            // 
            // btnAnylzeTfIDf
            // 
            this.btnAnylzeTfIDf.Location = new System.Drawing.Point(7, 72);
            this.btnAnylzeTfIDf.Name = "btnAnylzeTfIDf";
            this.btnAnylzeTfIDf.Size = new System.Drawing.Size(107, 23);
            this.btnAnylzeTfIDf.TabIndex = 2;
            this.btnAnylzeTfIDf.Text = "Analyze TD-IDF";
            this.btnAnylzeTfIDf.UseVisualStyleBackColor = true;
            this.btnAnylzeTfIDf.Click += new System.EventHandler(this.btnAnylzeTfIDf_Click);
            // 
            // btnLoadPubmed
            // 
            this.btnLoadPubmed.Location = new System.Drawing.Point(6, 47);
            this.btnLoadPubmed.Name = "btnLoadPubmed";
            this.btnLoadPubmed.Size = new System.Drawing.Size(107, 23);
            this.btnLoadPubmed.TabIndex = 1;
            this.btnLoadPubmed.Text = "Load Pubmed";
            this.btnLoadPubmed.UseVisualStyleBackColor = true;
            this.btnLoadPubmed.Click += new System.EventHandler(this.btnLoadPubmed_Click);
            // 
            // btnLoadXmlDataBase
            // 
            this.btnLoadXmlDataBase.Location = new System.Drawing.Point(7, 18);
            this.btnLoadXmlDataBase.Name = "btnLoadXmlDataBase";
            this.btnLoadXmlDataBase.Size = new System.Drawing.Size(107, 23);
            this.btnLoadXmlDataBase.TabIndex = 0;
            this.btnLoadXmlDataBase.Text = "Load XML Db";
            this.btnLoadXmlDataBase.UseVisualStyleBackColor = true;
            this.btnLoadXmlDataBase.Click += new System.EventHandler(this.btnLoadWord2Vec_Click);
            // 
            // SearchEngineTopView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbWord2Vec);
            this.Controls.Add(this.gbLoadFile);
            this.Controls.Add(this.gbXmlLoader);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "SearchEngineTopView";
            this.Size = new System.Drawing.Size(977, 121);
            this.gbXmlLoader.ResumeLayout(false);
            this.gbXmlLoader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxResults)).EndInit();
            this.gbLoadFile.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbSearchMode.ResumeLayout(false);
            this.gbSearchMode.PerformLayout();
            this.gbInputFiles.ResumeLayout(false);
            this.gbWord2Vec.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbXmlLoader;
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
        private System.Windows.Forms.ToolTip tipPhrase;
        private System.Windows.Forms.RadioButton rbOthers;
        private System.Windows.Forms.ToolTip tipOthers;
        private System.Windows.Forms.RadioButton rbWord;
        private System.Windows.Forms.ToolTip tipWord;
        private System.Windows.Forms.CheckBox cbMatchCase;
        private System.Windows.Forms.CheckBox cbInSelection;
        private System.Windows.Forms.Button buttonFileAnalysis;
        private Label lblPmidLoadState;
        private GroupBox gbWord2Vec;
        private Button btnLoadXmlDataBase;
        private Button btnLoadPubmed;
        private ComboBox cmbYearFilter;
        private NumericUpDown nudMaxResults;
        private Button btnAnylzeTfIDf;
    }
}
