namespace SearchEnginesApp.Views
{
    partial class SearchEngineResultView
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
            this.GroupFileParser = new System.Windows.Forms.GroupBox();
            this.btnZipfDistribution = new System.Windows.Forms.Button();
            this.lblFileKeywords = new System.Windows.Forms.Label();
            this.lblKeywordsTitle = new System.Windows.Forms.Label();
            this.labelFileSearchResult = new System.Windows.Forms.Label();
            this.BooksDataGridView = new System.Windows.Forms.DataGridView();
            this.lblXmlInfo = new System.Windows.Forms.Label();
            this.gbXmlFileInfo = new System.Windows.Forms.GroupBox();
            this.GroupFileParser.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BooksDataGridView)).BeginInit();
            this.gbXmlFileInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupFileParser
            // 
            this.GroupFileParser.AutoSize = true;
            this.GroupFileParser.Controls.Add(this.btnZipfDistribution);
            this.GroupFileParser.Controls.Add(this.lblFileKeywords);
            this.GroupFileParser.Controls.Add(this.lblKeywordsTitle);
            this.GroupFileParser.Controls.Add(this.labelFileSearchResult);
            this.GroupFileParser.Controls.Add(this.BooksDataGridView);
            this.GroupFileParser.Location = new System.Drawing.Point(3, 3);
            this.GroupFileParser.Name = "GroupFileParser";
            this.GroupFileParser.Size = new System.Drawing.Size(520, 320);
            this.GroupFileParser.TabIndex = 11;
            this.GroupFileParser.TabStop = false;
            this.GroupFileParser.Text = "Files Parser";
            // 
            // btnZipfDistribution
            // 
            this.btnZipfDistribution.Location = new System.Drawing.Point(431, 267);
            this.btnZipfDistribution.Name = "btnZipfDistribution";
            this.btnZipfDistribution.Size = new System.Drawing.Size(75, 23);
            this.btnZipfDistribution.TabIndex = 14;
            this.btnZipfDistribution.Text = "Zipf Curve";
            this.btnZipfDistribution.UseVisualStyleBackColor = true;
            this.btnZipfDistribution.Click += new System.EventHandler(this.btnZipfDistribution_Click);
            // 
            // lblFileKeywords
            // 
            this.lblFileKeywords.AllowDrop = true;
            this.lblFileKeywords.AutoEllipsis = true;
            this.lblFileKeywords.AutoSize = true;
            this.lblFileKeywords.Location = new System.Drawing.Point(6, 267);
            this.lblFileKeywords.MaximumSize = new System.Drawing.Size(400, 30);
            this.lblFileKeywords.Name = "lblFileKeywords";
            this.lblFileKeywords.Size = new System.Drawing.Size(100, 12);
            this.lblFileKeywords.TabIndex = 13;
            this.lblFileKeywords.Text = "No Keywords found";
            this.lblFileKeywords.Visible = false;
            this.lblFileKeywords.DoubleClick += new System.EventHandler(this.lblFileKeywords_DoubleClick);
            // 
            // lblKeywordsTitle
            // 
            this.lblKeywordsTitle.AutoSize = true;
            this.lblKeywordsTitle.Location = new System.Drawing.Point(6, 247);
            this.lblKeywordsTitle.Name = "lblKeywordsTitle";
            this.lblKeywordsTitle.Size = new System.Drawing.Size(129, 12);
            this.lblKeywordsTitle.TabIndex = 12;
            this.lblKeywordsTitle.Text = "File List Top 10 Keywords";
            this.lblKeywordsTitle.Visible = false;
            // 
            // labelFileSearchResult
            // 
            this.labelFileSearchResult.AutoSize = true;
            this.labelFileSearchResult.Location = new System.Drawing.Point(6, 224);
            this.labelFileSearchResult.Name = "labelFileSearchResult";
            this.labelFileSearchResult.Size = new System.Drawing.Size(122, 12);
            this.labelFileSearchResult.TabIndex = 11;
            this.labelFileSearchResult.Text = "File Search Result : None";
            // 
            // BooksDataGridView
            // 
            this.BooksDataGridView.AllowUserToAddRows = false;
            this.BooksDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.BooksDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.BooksDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.BooksDataGridView.Location = new System.Drawing.Point(6, 21);
            this.BooksDataGridView.Name = "BooksDataGridView";
            this.BooksDataGridView.ReadOnly = true;
            this.BooksDataGridView.RowTemplate.Height = 24;
            this.BooksDataGridView.Size = new System.Drawing.Size(500, 200);
            this.BooksDataGridView.TabIndex = 8;
            this.BooksDataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.BooksDataGridView_CellClick);
            // 
            // lblXmlInfo
            // 
            this.lblXmlInfo.AutoSize = true;
            this.lblXmlInfo.Location = new System.Drawing.Point(15, 18);
            this.lblXmlInfo.Name = "lblXmlInfo";
            this.lblXmlInfo.Size = new System.Drawing.Size(83, 12);
            this.lblXmlInfo.TabIndex = 10;
            this.lblXmlInfo.Text = "File Not Selected";
            // 
            // gbXmlFileInfo
            // 
            this.gbXmlFileInfo.Controls.Add(this.lblXmlInfo);
            this.gbXmlFileInfo.Location = new System.Drawing.Point(529, 3);
            this.gbXmlFileInfo.Name = "gbXmlFileInfo";
            this.gbXmlFileInfo.Size = new System.Drawing.Size(217, 320);
            this.gbXmlFileInfo.TabIndex = 12;
            this.gbXmlFileInfo.TabStop = false;
            this.gbXmlFileInfo.Text = "Xml File Infomation";
            // 
            // SearchEngineResultView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.Controls.Add(this.gbXmlFileInfo);
            this.Controls.Add(this.GroupFileParser);
            this.Name = "SearchEngineResultView";
            this.Size = new System.Drawing.Size(756, 328);
            this.GroupFileParser.ResumeLayout(false);
            this.GroupFileParser.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BooksDataGridView)).EndInit();
            this.gbXmlFileInfo.ResumeLayout(false);
            this.gbXmlFileInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox GroupFileParser;
        private System.Windows.Forms.DataGridView BooksDataGridView;
        private System.Windows.Forms.Label lblXmlInfo;
        private System.Windows.Forms.GroupBox gbXmlFileInfo;
        private System.Windows.Forms.Label labelFileSearchResult;
        private System.Windows.Forms.Label lblKeywordsTitle;
        private System.Windows.Forms.Label lblFileKeywords;
        private System.Windows.Forms.Button btnZipfDistribution;
    }
}
