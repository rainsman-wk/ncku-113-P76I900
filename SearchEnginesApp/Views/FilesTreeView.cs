using SearchEnginesApp.Presenters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SearchEnginesApp.Views
{
    public partial class FilesTreeView : UserControl
    {

        private readonly FilesTreePresenter _presenter;
        public FilesTreeView(FilesTreePresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
        }

        public void DisplayFileTree(List<string> xmlFiles, List<string> jsonFiles)
        {
            treeViewFiles.Nodes.Clear();

            // Load XML files into Node
            TreeNode xmlNode = new TreeNode("XML Files");
            if (xmlFiles.Count > 0)
            { 
                foreach (string file in xmlFiles)
                {
                    xmlNode.Nodes.Add(new TreeNode(file));
                }
                treeViewFiles.Nodes.Add(xmlNode);
            }

            // Load Json files into Node
            TreeNode jsonNode = new TreeNode("Json Files");
            if(jsonFiles.Count>0)
            {
                foreach (string file in jsonFiles)
                {
                    jsonNode.Nodes.Add(new TreeNode(file));
                }
                treeViewFiles.Nodes.Add(jsonNode);
            }
 
            treeViewFiles.ExpandAll();
            SelectAllRootNodes();
        }


        #region Control..
        private void SelectAllRootNodes()
        {
            foreach (TreeNode node in treeViewFiles.Nodes)
            {
                if (string.IsNullOrEmpty(node.Text) == false)
                {
                    node.Checked = true;
                }
            }

        }
        private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                if (node.Nodes.Count > 0)
                {
                    CheckAllChildNodes(node, nodeChecked);
                }
                _presenter.SaveSelectFileState(node.Text, nodeChecked);
            }
        }
        private void FilesTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            // Check cheange from Root
            if (e.Action != TreeViewAction.Unknown)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    // Select all Child Nodes
                    CheckAllChildNodes(e.Node, e.Node.Checked);
                }
                _presenter.SaveSelectFileState(e.Node.Text, e.Node.Checked);
            }
            
        }
        #endregion Control..


    }
}
