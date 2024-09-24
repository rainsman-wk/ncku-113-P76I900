using SearchEnginesApp.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static SearchEnginesApp.ToolModel;

namespace SearchEnginesApp.Presenters
{
    public class SearchEngineResultPresenter
    {
        private readonly ToolModel _toolModel;
        private SearchEngineResultView View { get; set; }

        /// <summary>
        /// SerchEngine Presenter
        /// </summary>
        /// <param name="toolModel">The Tool Model</param>
        public SearchEngineResultPresenter(ToolModel toolModel)
        {
            _toolModel = toolModel;
            _toolModel.FileAnalysisLoaded += ToolModel_FileAnalysisEventReceived;
            _toolModel.SearchKeywordLoaded += ToolModel_SearchDataEventReceived;
        }

        #region Handler
        private void ToolModel_FileAnalysisEventReceived(object sender, FileAnalsisEventArgs e)
        {
            View.ResetResultViewPage();

            if(e.SearchData.Count > 0 )
            {
                List<SearchBooks> serachdata = e.SearchData;
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
                View.LoadSelectContent(names, contents);
            }
            else
            {
                View.UpdateLabelSearchResult("Wrong File Input, No content found", Color.Red);
            }
        }
        private void ToolModel_SearchDataEventReceived(object sender, KeywordEventArgs e)
        {
            if(e != null)
            {
                KeywordArg keywordarg = e.Keywordarg;
                List<SearchBooks> Searchbooks = _toolModel.GetSerachBooks();
                // TODO , remove unused code
                if (Searchbooks.Count > 0)
                {
                    List<SearchBooks> serachdata = Searchbooks;
                    List<string> names = new List<string>();
                    List<FileContent> contents = new List<FileContent>();
                    foreach (var book in serachdata)
                    {
                        // Get File Name
                        names.Add(Path.GetFileNameWithoutExtension(book.Path));
                        // Add File Contents
                        contents.Add(book.Content);
                    }
                    // Update Label Result

                    View.UpdateFileSearchResult(names, contents, keywordarg);

                    View.UpdateContentSearchResult(keywordarg);
                }
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

        public string GetFileContent(string name)
        {
            string page = string.Empty;
            FileContent file = _toolModel.GetSearchBookContent(name);

            for (int i = 0; i < file.Content.Count; i++)
            {
                page += file.Content[i] + Environment.NewLine;
            }
            return page;
        }
        public KeywordArg GetKeyword()
        {
            KeywordArg keywordarg = _toolModel.GetSearchKeyword();
            return keywordarg;
        }

    }




}
