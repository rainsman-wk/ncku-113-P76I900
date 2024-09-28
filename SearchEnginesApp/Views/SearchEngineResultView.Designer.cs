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
            this.labelFileSearchResult = new System.Windows.Forms.Label();
            this.BooksDataGridView = new System.Windows.Forms.DataGridView();
            this.FileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NrCharIncludingSpaces = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NrCharExcludingSpaces = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NrWords = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NrSentences = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NrNonAsciiChar = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NrNonAsciiWords = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.labelResult = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelPageSearchResult = new System.Windows.Forms.Label();
            this.richTextBoxFileContent = new System.Windows.Forms.RichTextBox();
            this.GroupFileParser.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BooksDataGridView)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupFileParser
            // 
            this.GroupFileParser.AutoSize = true;
            this.GroupFileParser.Controls.Add(this.labelFileSearchResult);
            this.GroupFileParser.Controls.Add(this.BooksDataGridView);
            this.GroupFileParser.Location = new System.Drawing.Point(3, 3);
            this.GroupFileParser.Name = "GroupFileParser";
            this.GroupFileParser.Size = new System.Drawing.Size(599, 199);
            this.GroupFileParser.TabIndex = 11;
            this.GroupFileParser.TabStop = false;
            this.GroupFileParser.Text = "Files Parser";
            // 
            // labelFileSearchResult
            // 
            this.labelFileSearchResult.AutoSize = true;
            this.labelFileSearchResult.Location = new System.Drawing.Point(7, 169);
            this.labelFileSearchResult.Name = "labelFileSearchResult";
            this.labelFileSearchResult.Size = new System.Drawing.Size(122, 12);
            this.labelFileSearchResult.TabIndex = 11;
            this.labelFileSearchResult.Text = "File Search Result : None";
            // 
            // BooksDataGridView
            // 
            this.BooksDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.BooksDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.BooksDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.BooksDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FileName,
            this.NrCharIncludingSpaces,
            this.NrCharExcludingSpaces,
            this.NrWords,
            this.NrSentences,
            this.NrNonAsciiChar,
            this.NrNonAsciiWords});
            this.BooksDataGridView.Location = new System.Drawing.Point(6, 21);
            this.BooksDataGridView.Name = "BooksDataGridView";
            this.BooksDataGridView.RowTemplate.Height = 24;
            this.BooksDataGridView.Size = new System.Drawing.Size(580, 137);
            this.BooksDataGridView.TabIndex = 8;
            this.BooksDataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.BooksDataGridView_CellClick);
            // 
            // FileName
            // 
            this.FileName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.FileName.FillWeight = 50F;
            this.FileName.HeaderText = "File Name";
            this.FileName.MinimumWidth = 70;
            this.FileName.Name = "FileName";
            // 
            // NrCharIncludingSpaces
            // 
            this.NrCharIncludingSpaces.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.NrCharIncludingSpaces.FillWeight = 20F;
            this.NrCharIncludingSpaces.HeaderText = "Char (Incl. Spaces)";
            this.NrCharIncludingSpaces.MinimumWidth = 30;
            this.NrCharIncludingSpaces.Name = "NrCharIncludingSpaces";
            // 
            // NrCharExcludingSpaces
            // 
            this.NrCharExcludingSpaces.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.NrCharExcludingSpaces.FillWeight = 20F;
            this.NrCharExcludingSpaces.HeaderText = "Char (Excl. Spaces)";
            this.NrCharExcludingSpaces.MinimumWidth = 30;
            this.NrCharExcludingSpaces.Name = "NrCharExcludingSpaces";
            // 
            // NrWords
            // 
            this.NrWords.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.NrWords.FillWeight = 20F;
            this.NrWords.HeaderText = "Words";
            this.NrWords.Name = "NrWords";
            // 
            // NrSentences
            // 
            this.NrSentences.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.NrSentences.FillWeight = 20F;
            this.NrSentences.HeaderText = "Sentences";
            this.NrSentences.Name = "NrSentences";
            // 
            // NrNonAsciiChar
            // 
            this.NrNonAsciiChar.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.NrNonAsciiChar.FillWeight = 20F;
            this.NrNonAsciiChar.HeaderText = "Non-ASCII Char";
            this.NrNonAsciiChar.Name = "NrNonAsciiChar";
            // 
            // NrNonAsciiWords
            // 
            this.NrNonAsciiWords.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.NrNonAsciiWords.FillWeight = 20F;
            this.NrNonAsciiWords.HeaderText = "Non-ASCII Words";
            this.NrNonAsciiWords.Name = "NrNonAsciiWords";
            // 
            // labelResult
            // 
            this.labelResult.AutoSize = true;
            this.labelResult.Location = new System.Drawing.Point(19, 20);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(111, 12);
            this.labelResult.TabIndex = 10;
            this.labelResult.Text = "Keyword(s) not Found";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelResult);
            this.groupBox2.Location = new System.Drawing.Point(608, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(331, 199);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Result";
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.labelPageSearchResult);
            this.groupBox1.Controls.Add(this.richTextBoxFileContent);
            this.groupBox1.Location = new System.Drawing.Point(4, 200);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(936, 287);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File Content";
            // 
            // labelPageSearchResult
            // 
            this.labelPageSearchResult.AutoSize = true;
            this.labelPageSearchResult.Location = new System.Drawing.Point(6, 257);
            this.labelPageSearchResult.Name = "labelPageSearchResult";
            this.labelPageSearchResult.Size = new System.Drawing.Size(127, 12);
            this.labelPageSearchResult.TabIndex = 10;
            this.labelPageSearchResult.Text = "Page Search Result : None";
            // 
            // richTextBoxFileContent
            // 
            this.richTextBoxFileContent.Location = new System.Drawing.Point(6, 21);
            this.richTextBoxFileContent.Name = "richTextBoxFileContent";
            this.richTextBoxFileContent.ReadOnly = true;
            this.richTextBoxFileContent.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.richTextBoxFileContent.Size = new System.Drawing.Size(917, 233);
            this.richTextBoxFileContent.TabIndex = 9;
            this.richTextBoxFileContent.Text = "";
            // 
            // SearchEngineResultView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.GroupFileParser);
            this.Name = "SearchEngineResultView";
            this.Size = new System.Drawing.Size(943, 490);
            this.GroupFileParser.ResumeLayout(false);
            this.GroupFileParser.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BooksDataGridView)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox GroupFileParser;
        private System.Windows.Forms.DataGridView BooksDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn NrCharIncludingSpaces;
        private System.Windows.Forms.DataGridViewTextBoxColumn NrCharExcludingSpaces;
        private System.Windows.Forms.DataGridViewTextBoxColumn NrWords;
        private System.Windows.Forms.DataGridViewTextBoxColumn NrSentences;
        private System.Windows.Forms.DataGridViewTextBoxColumn NrNonAsciiChar;
        private System.Windows.Forms.DataGridViewTextBoxColumn NrNonAsciiWords;
        private System.Windows.Forms.Label labelResult;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox richTextBoxFileContent;
        private System.Windows.Forms.Label labelPageSearchResult;
        private System.Windows.Forms.Label labelFileSearchResult;
    }
}
