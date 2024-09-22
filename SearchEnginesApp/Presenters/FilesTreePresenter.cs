using SearchEnginesApp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SearchEnginesApp.ToolModel;

namespace SearchEnginesApp.Presenters
{
    public class FilesTreePresenter
    {
        private readonly ToolModel _toolModel;
        private FilesTreeView View { get; set; }

        /// <summary>
        /// Files Tree Presenter
        /// </summary>
        /// <param name="toolModel">The Tool Model</param>
        public FilesTreePresenter(ToolModel toolModel)
        {
            _toolModel = toolModel;
            _toolModel.FilesLoaded += ToolModel_FilesLoadedEventReceived;

        }

        #region Handler
        private void ToolModel_FilesLoadedEventReceived(object sender, FileEventArgs e)
        {
            View.DisplayFileTree(e.XmlFiles, e.JsonFiles);
        }
        #endregion Handler


        #region View realted... 
        /// <summary>
        /// Method to show the view
        /// </summary>
        /// <returns>UserControl, the view connected to this presenter</returns>
        public UserControl ShowView()
        {
            if (View == null || View.IsDisposed)
            {
                View = new FilesTreeView(this);
                /* Register the required events */
            }

            return View;
        }

        /// <summary>
        /// Method called when view is closed
        /// </summary>
        public void CloseView()
        {
            if (View != null && !View.IsDisposed) View.Dispose();
        }
        #endregion View realted...

        public void SaveSelectFileState(string node, bool check)
        {
            _toolModel.UpdateFileState(node, check);
        }


    }
}
