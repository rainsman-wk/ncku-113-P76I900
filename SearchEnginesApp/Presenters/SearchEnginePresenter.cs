using SearchEnginesApp.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SearchEnginesApp.ToolModel;

namespace SearchEnginesApp.Presenters
{
    public class SearchEnginePresenter
    {
        private readonly ToolModel _toolModel;
        private SearchEngineResultView View { get; set; }

        /// <summary>
        /// SerchEngine Presenter
        /// </summary>
        /// <param name="toolModel">The Tool Model</param>
        public SearchEnginePresenter(ToolModel toolModel)
        {
            _toolModel = toolModel;
            _toolModel.SearchResultLoaded += ToolModel_SearchDataLoadedEventReceived;
        }

        #region Handler
        private void ToolModel_SearchDataLoadedEventReceived(object sender, SearchDataEventArgs e)
        {
            View.ResetResultViewPage();

            if(e.SearchData.Count > 0 )
            {
                List<SearchBooks> serachdata = e.SearchData;
                KeywordArg keywordarg = e.Keywordarg;

                List<string> names = new List<string>();
                List<FileContent> contents = new List<FileContent>();
                foreach (var book in serachdata)
                {
                    // Get File Name
                    names.Add(Path.GetFileNameWithoutExtension(book.Path));
                    // Add File Contents
                    contents.Add(book.Content);
                }

                // Update View File List
                View.UpdateFileList(names, contents);

                // Upload Select File Content
                View.LoadSelectContent(names, contents, keywordarg);

                // Update Label Result
                View.UpdateSearchResult(names, contents, keywordarg);

            }
            else
            {
                View.UpdateLabelSearchResult("Wrong File Input, No content found", Color.Red);
            }
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
                View = new SearchEngineResultView(this);
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

    }




}
