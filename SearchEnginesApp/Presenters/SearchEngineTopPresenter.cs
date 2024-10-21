using SearchEnginesApp.Utils;
using SearchEnginesApp.Views;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using static SearchEnginesApp.ToolModel;
using static SearchEnginesApp.Views.SearchEngineResultView;

namespace SearchEnginesApp.Presenters
{
    public class SearchEngineTopPresenter
    {
        private readonly ToolModel _toolModel;
        private SearchEngineTopView View { get; set; }
        private FilesTreeView FileView { get; set; }

        /// <summary>
        /// SerchEngineTop Presenter
        /// </summary>
        /// <param name="toolModel">The Tool Model</param>
        public SearchEngineTopPresenter(ToolModel toolModel)
        {
            _toolModel = toolModel;
        }


        /// <summary>
        /// Method to show the view
        /// </summary>
        /// <returns>UserControl, the view connected to this presenter</returns>
        public UserControl ShowView()
        {
            if (View == null || View.IsDisposed)
            {
                View = new SearchEngineTopView(this);
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

        public FileContent GetXmlContent(string path)
        {
            FileContent context = new FileContent();
            if (File.Exists(path))
            {
                XmlDocument file = new XmlDocument();
                file.Load(path);
 
                XmlNodeList articleNodes = file.SelectNodes("//PubmedArticle");
                foreach (XmlNode articleNode in articleNodes)
                {
                    context.Title = articleNode.SelectSingleNode("MedlineCitation/Article/ArticleTitle")?.InnerText;
                    XmlNodeList abstractNodes = articleNode.SelectNodes("MedlineCitation/Article/Abstract/AbstractText");
                    context.Abstract = abstractNodes.Cast<XmlNode>().Select(node => node.InnerText).ToList();
                    context.Journal = articleNode.SelectSingleNode("MedlineCitation/Article/Journal/Title")?.InnerText;
                    context.Pmid = articleNode.SelectSingleNode("MedlineCitation/PMID")?.InnerText;
                }
            }
            return context;
        }
 
        public void ResetBookDatabase()
        {
            _toolModel.ClearFileList();
            _toolModel.CleanSearchBooks();
        }

        public void GetFileLoadfromDatabase()
        {
            _toolModel.SetEventLoadFiles();
        }
        public void InitialzeSearchBookDB()
        {
            _toolModel.DatabaseInitialize();
        }
        public void LoadSearchBookDB()
        {
            _toolModel.GetSearchBookDB();
        }

        public void AddFileListToSearchEngine(string file)
        {
            _toolModel.AddFileList(file);
        }

        #region Search Features...
        public bool CheckFileSelected()
        {
            return _toolModel.IsSerachFileVailded();
        }

        public void FileAnalysis()
        {
            // Reset Search DataBase
            _toolModel.CleanSearchBooks();

            List<string> searchlist = new List<string>();
            _toolModel.GetSearchFileList(out searchlist);

            List<SearchBooks> books = new List<SearchBooks>();

            DatabaseHelper dbHelper = new DatabaseHelper();

            for (int i = 0; i< searchlist.Count; i++)
            {
                var book = new SearchBooks(searchlist[i], GetXmlContent(searchlist[i]));
                books.Add(book);
                dbHelper.InsertBook(book);
            }
            // Trigger Event to other view to update Serach book
            _toolModel.SetEventUpdateSerchBook(books);
        }
        public void SetSearchKeyWord(SearchWordArg keyword)
        {
            _toolModel.SetSearchWords(keyword);
            _toolModel.SetKeywordSearchEvent();
        }
        #endregion Search Features...



    }




}
