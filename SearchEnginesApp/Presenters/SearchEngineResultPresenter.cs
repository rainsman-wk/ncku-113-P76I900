using SearchEnginesApp.Utils;
using SearchEnginesApp.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static SearchEnginesApp.ToolModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace SearchEnginesApp.Presenters
{
    public class SearchEngineResultPresenter
    {
        private readonly ToolModel _toolModel;
        private bool cbStopWord = false;
        private bool cbPortersAlgorithm = false;
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
            if (e.SearchData.Count > 0)
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
                var wordsDict = Utils.KeywordExtractor.ExtractTokenToDict(fileswordList,cbPortersAlgorithm,cbStopWord);
                List<string> keywords = wordsDict.Keys.ToList();
                View.UpdateFilesTopKeywords(keywords, 10);
            }
            else
            {
                View.UpdateLabelSearchResult("Wrong File Input, No content found", Color.Red);
            }
        }
        private void ToolModel_SearchDataEventReceived(object sender, KeywordEventArgs e)
        {
            if (e != null)
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
        public Dictionary<string, Tuple<int, double>> GetKeywordsDict()
        {
            Dictionary<string, Tuple<int, double>> keywords = new Dictionary<string, Tuple<int, double>>();

            List<SearchBooks> books = _toolModel.GetSerachBooks();
            List<string> fileWords = new List<string>();
            foreach (var book in books)
            {
                fileWords.AddRange(book.Content.Word);
            }
            keywords = Utils.KeywordExtractor.ExtractTokenToDict(fileWords,cbPortersAlgorithm,cbStopWord);
            return keywords;
        }
        public void GetSearchBookTokens()
        {
            List<string> tokens = new List<string>();
            bool KeywordSet = (_toolModel.GetSearchKeyword().SearchWords.Count > 0);
            int count = 0;
            List <SearchBooks> books = _toolModel.GetSerachBooks();
            foreach (var book in books)
            {
                if(KeywordSet)
                {
                    List<string> keywordfile = GetSearchWords(book.Content.Word);
                    tokens.AddRange(keywordfile);
                    if(keywordfile.Count >0) { count++; }
                }
                else
                {
                    tokens.AddRange(book.Content.Word);
                }
            }
            string Text = (KeywordSet) ? $"Find Keyword in {count.ToString()} files" : "All DataBase";
            ZipfChartForm zipfChartForm = new ZipfChartForm(new Point(960, 240), Text, tokens);
            zipfChartForm.Show();
        }

        public List<string> GetSearchWords(List<string> words)
        {
            SearchWordArg keywordarg = _toolModel.GetSearchKeyword();
            List<string> findfileword = new List<string>();

            string pattern;
            Regex regex;
            RegexOptions regexoption = (keywordarg.MatchCase) ? RegexOptions.None : RegexOptions.IgnoreCase;
            StringComparison checkoption = (keywordarg.MatchCase) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            foreach (string keyword in keywordarg.SearchWords)
            {
                switch (keywordarg.Mode)
                {
                    case SearchMode.Word:
                        pattern = @"\b(" + string.Join("|", keywordarg.SearchWords.Select(Regex.Escape)) + @")\b";
                        regex = new Regex(pattern, regexoption);
                        break;
                    case SearchMode.Phrase:
                        pattern = @"\b(" + string.Join("|", keywordarg.SearchWords.Select(Regex.Escape)) + @")\b";
                        regex = new Regex(pattern, regexoption);
                        break;
                    case SearchMode.Others:
                    default:
                        // No limited Serch Mode
                        regex = new Regex(keyword, regexoption);
                        break;
                }
                MatchCollection matches;

                foreach (string word in words)
                {
                    if ((keywordarg.Mode == SearchMode.Word) || (keywordarg.Mode == SearchMode.Phrase))
                    {
                        bool allKeywordsFound = true;
                        foreach (string kw in keywordarg.SearchWords)
                        {
                            Regex kwRegex = new Regex(@"\b" + Regex.Escape(kw) + @"\b", regexoption);
                            if (!kwRegex.IsMatch(word))
                            {
                                allKeywordsFound = false;
                                break;
                            }
                        }
                        if (allKeywordsFound)
                        {
                            findfileword = words;
                        }
                    }
                    else
                    {
                        matches = regex.Matches(word);
                        foreach (Match match in matches)
                        {
                            findfileword = words;
                        }
                    }

                }
            }
            return findfileword;
        }
        public void UpdateSortOption(bool portersAlgorithm, bool stopword)
        {
            cbStopWord = stopword;
            cbPortersAlgorithm = portersAlgorithm;
        }
    }
}
