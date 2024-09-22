using System.Windows.Forms;

namespace SearchEnginesApp.Views
{
    partial class FilesTreeView
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
            this.treeViewFiles = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeViewFiles
            // 
            this.treeViewFiles.CheckBoxes = true;
            this.treeViewFiles.Location = new System.Drawing.Point(4, 4);
            this.treeViewFiles.Name = "treeViewFiles";
            this.treeViewFiles.Size = new System.Drawing.Size(247, 488);
            this.treeViewFiles.TabIndex = 0;
            this.treeViewFiles.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.FilesTreeView_AfterCheck);
            // 
            // FilesTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeViewFiles);
            this.Name = "FilesTreeView";
            this.Size = new System.Drawing.Size(254, 495);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewFiles;
    }
}
