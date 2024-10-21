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
        public ToolModel GetToolModel()
        {
            return _toolModel;
        }

        #region Handler
        private void ToolModel_FileAnalysisEventReceived(object sender, FileAnalsisEventArgs e)
        {
            if(e.SearchData.Count > 0 )
            {
                List<SearchBooks> serachdata = e.SearchData;
                List<string> names = new List<string>();
                List<FileContent> contents = new List<FileContent>();
                List<string> fileswordList = new List<string>();
                foreach (var book in serachdata)
                {
                    // Get File Name
                    names.Add(Path.GetFileNameWithoutExtension(book.Path));
                    // Add File Contents
                    contents.Add(book.Content);
                    fileswordList.AddRange(book.Content.Word);
                }
                // Update View File List
                View.UpdateFileList(names, contents);

                // Update Keywords from Loading File
                View.UpdateFilesTopKeywords(Utils.KeywordExtractor.ExtractKeywordsToList(fileswordList,10));
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
                SearchWordArg keywordarg = e.Keywordarg;
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
        public FileContent GetFileContent(string name)
        {
            FileContent file = _toolModel.GetSearchBookContent(name);
            return file;
        }
        public SearchWordArg GetSearchWords()
        {
            SearchWordArg searchwordArg = _toolModel.GetSearchKeyword();
            return searchwordArg;
        }
        public Dictionary<string, (int count, List<int> indices)> GetKeywordsDict(int rank)
        {
            Dictionary<string, (int count, List<int> indices)> keywords = new Dictionary<string, (int count, List<int> indices)>();

            List<SearchBooks> books = _toolModel.GetSerachBooks();
            List<string> fileWords = new List<string>();
            foreach (var book in books)
            {
                fileWords.AddRange(book.Content.Word);
            }
            keywords = Utils.KeywordExtractor.ExtractKeywordsToDict(fileWords, rank);
            return keywords;
        }
        public List<string> GetSearchBookTokens()
        {
            List<string> tokens = new List<string>();

            List<SearchBooks> books = _toolModel.GetSerachBooks();
            foreach (var book in books)
            {
                tokens.AddRange(book.Content.Word);
            }
            return tokens;
        }


    }




}
