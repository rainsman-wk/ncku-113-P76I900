namespace SearchEnginesApp
{
    partial class MainForm
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tbSeachView = new System.Windows.Forms.TabControl();
            this.tpSearchResult = new System.Windows.Forms.TabPage();
            this.MainTopView = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tbSeachView.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Location = new System.Drawing.Point(8, 119);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tbSeachView);
            this.splitContainer1.Size = new System.Drawing.Size(1245, 519);
            this.splitContainer1.SplitterDistance = 212;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 1;
            // 
            // tbSeachView
            // 
            this.tbSeachView.Controls.Add(this.tpSearchResult);
            this.tbSeachView.Location = new System.Drawing.Point(2, 2);
            this.tbSeachView.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbSeachView.Name = "tbSeachView";
            this.tbSeachView.SelectedIndex = 0;
            this.tbSeachView.Size = new System.Drawing.Size(1024, 512);
            this.tbSeachView.TabIndex = 0;
            // 
            // tpSearchResult
            // 
            this.tpSearchResult.AutoScroll = true;
            this.tpSearchResult.Location = new System.Drawing.Point(4, 22);
            this.tpSearchResult.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tpSearchResult.Name = "tpSearchResult";
            this.tpSearchResult.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tpSearchResult.Size = new System.Drawing.Size(1016, 486);
            this.tpSearchResult.TabIndex = 0;
            this.tpSearchResult.Text = "SearchResult";
            this.tpSearchResult.UseVisualStyleBackColor = true;
            // 
            // MainTopView
            // 
            this.MainTopView.Location = new System.Drawing.Point(8, 8);
            this.MainTopView.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MainTopView.Name = "MainTopView";
            this.MainTopView.Size = new System.Drawing.Size(1245, 107);
            this.MainTopView.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.MainTopView);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MainForm";
            this.Text = "Search Engines";
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tbSeachView.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tbSeachView;
        private System.Windows.Forms.TabPage tpSearchResult;
        private System.Windows.Forms.Panel MainTopView;
    }
}

